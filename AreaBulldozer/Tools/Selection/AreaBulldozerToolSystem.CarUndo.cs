using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Vehicles;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Bicycle = Game.Vehicles.Bicycle;
using CarLane = Game.Net.CarLane;
using CarLaneFlags = Game.Vehicles.CarLaneFlags;
using PersonalCar = Game.Vehicles.PersonalCar;
using Transform = Game.Objects.Transform;
using Unspawned = Game.Objects.Unspawned;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private struct CarSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public Entity Owner;
            public Transform Transform;
            public PseudoRandomSeed? Seed;
            public PersonalCarFlags State;
            public Car? DrivingState;
            public CarCurrentLane? CurrentLane;
            public Entity Target;
            public List<TrailerSnapshot> Trailers;
        }

        private struct TrailerSnapshot
        {
            public Entity Original;
            public Entity Prefab;
            public Transform Transform;
            public PseudoRandomSeed? Seed;
            public CarTrailerLane Lane;
        }

        private readonly List<CarSnapshot> m_CarUndo = new();

        private Entity GetSelectedCarRoot(Entity entity)
        {
            if (EntityManager.HasComponent<CarTrailer>(entity) &&
                EntityManager.HasComponent<Controller>(entity))
            {
                Entity tractor = EntityManager.GetComponentData<Controller>(entity).m_Controller;
                if (IsEntityUsable(tractor) && EntityManager.HasBuffer<LayoutElement>(tractor))
                {
                    return tractor;
                }
            }

            return entity;
        }

        private void RecordCarForUndo(Entity entity)
        {
            if (!EntityManager.HasComponent<PersonalCar>(entity) ||
                EntityManager.HasComponent<Bicycle>(entity) ||
                EntityManager.HasComponent<CarTrailer>(entity) ||
                !EntityManager.HasComponent<PrefabRef>(entity) ||
                !EntityManager.HasComponent<Transform>(entity) ||
                EntityManager.HasComponent<Unspawned>(entity))
            {
                return;
            }

            PersonalCar car = EntityManager.GetComponentData<PersonalCar>(entity);
            Entity owner = ReadUndoComponent<Owner>(entity)?.m_Owner ?? Entity.Null;
            Entity prefab = EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab;
            bool dummyTraffic = (car.m_State & PersonalCarFlags.DummyTraffic) != 0;
            CarCurrentLane? currentLane = ReadUndoComponent<CarCurrentLane>(entity);
            Entity target = ReadUndoComponent<Target>(entity)?.m_Target ?? Entity.Null;
            if (!IsCarUndoPrefab(prefab) ||
                (dummyTraffic ? !currentLane.HasValue : !IsCarUndoHousehold(owner)))
            {
                return;
            }

            Transform transform = EntityManager.GetComponentData<Transform>(entity);
            if (EntityManager.HasComponent<ParkedCar>(entity))
            {
                if (!TryGetVehicleSelectionPosition(entity, out float3 position))
                {
                    return;
                }

                transform.m_Position = position;
            }

            List<TrailerSnapshot> trailers = null;
            if (EntityManager.HasBuffer<LayoutElement>(entity))
            {
                DynamicBuffer<LayoutElement> layout = EntityManager.GetBuffer<LayoutElement>(entity, true);
                for (int i = 0; i < layout.Length; i++)
                {
                    Entity member = layout[i].m_Vehicle;
                    if (member == entity || !IsEntityUsable(member) ||
                        !EntityManager.HasComponent<CarTrailer>(member) ||
                        !EntityManager.HasComponent<PrefabRef>(member) ||
                        !EntityManager.HasComponent<Transform>(member) ||
                        !EntityManager.HasComponent<CarTrailerLane>(member))
                    {
                        continue;
                    }

                    (trailers ??= new()).Add(new TrailerSnapshot
                    {
                        Original = member,
                        Prefab = EntityManager.GetComponentData<PrefabRef>(member).m_Prefab,
                        Transform = EntityManager.GetComponentData<Transform>(member),
                        Seed = ReadUndoComponent<PseudoRandomSeed>(member),
                        Lane = EntityManager.GetComponentData<CarTrailerLane>(member)
                    });
                }
            }

            m_CarUndo.Add(new CarSnapshot
            {
                Original = entity,
                Prefab = prefab,
                Owner = owner,
                Transform = transform,
                Seed = ReadUndoComponent<PseudoRandomSeed>(entity),
                State = car.m_State,
                DrivingState = ReadUndoComponent<Car>(entity),
                CurrentLane = currentLane,
                Target = target,
                Trailers = trailers
            });
        }

        private bool IsCarUndoPrefab(Entity prefab) =>
            IsEntityUsable(prefab, allowOverridden: true) &&
            EntityManager.HasComponent<MovingObjectData>(prefab) &&
            EntityManager.HasComponent<ObjectGeometryData>(prefab) &&
            EntityManager.HasComponent<ObjectData>(prefab);

        private bool IsCarUndoHousehold(Entity household) =>
            IsEntityUsable(household, allowOverridden: true) &&
            EntityManager.HasComponent<Game.Citizens.Household>(household) &&
            EntityManager.HasBuffer<OwnedVehicle>(household);

        private bool CanUseUndoLane(Entity lane, float position)
        {
            if (!IsEntityUsable(lane) || !EntityManager.HasComponent<CarLane>(lane) ||
                !EntityManager.HasComponent<Curve>(lane) ||
                !math.isfinite(position) || position < 0f || position > 1f)
            {
                return false;
            }

            float length = EntityManager.GetComponentData<Curve>(lane).m_Length;
            if (!math.isfinite(length) || length <= 0f)
            {
                return false;
            }

            if (EntityManager.HasBuffer<LaneObject>(lane))
            {
                DynamicBuffer<LaneObject> objects = EntityManager.GetBuffer<LaneObject>(lane, true);
                for (int i = 0; i < objects.Length; i++)
                {
                    LaneObject other = objects[i];
                    float start = math.min(other.m_CurvePosition.x, other.m_CurvePosition.y);
                    float end = math.max(other.m_CurvePosition.x, other.m_CurvePosition.y);
                    float gap = math.max(start - position, position - end) * length;
                    if (gap < 3f && IsEntityUsable(other.m_LaneObject) &&
                        (EntityManager.HasComponent<CarCurrentLane>(other.m_LaneObject) ||
                         EntityManager.HasComponent<CarTrailerLane>(other.m_LaneObject)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CanRestoreMovingCar(CarSnapshot snapshot, CarCurrentLane lane)
        {
            if (!snapshot.DrivingState.HasValue ||
                !math.all(math.isfinite(snapshot.Transform.m_Position)) ||
                !IsEntityUsable(snapshot.Target, allowOverridden: true) ||
                lane.m_ChangeLane != Entity.Null ||
                !CanUseUndoLane(lane.m_Lane, lane.m_CurvePosition.x))
            {
                return false;
            }

            if (snapshot.Trailers == null)
            {
                return true;
            }

            if (!EntityManager.HasComponent<CarTractorData>(snapshot.Prefab))
            {
                return false;
            }

            foreach (TrailerSnapshot trailer in snapshot.Trailers)
            {
                if (IsEntityUsable(trailer.Original, allowOverridden: true) ||
                    !IsCarUndoPrefab(trailer.Prefab) ||
                    !EntityManager.HasComponent<CarTrailerData>(trailer.Prefab) ||
                    !math.all(math.isfinite(trailer.Transform.m_Position)) ||
                    !CanUseUndoLane(trailer.Lane.m_Lane, trailer.Lane.m_CurvePosition.x) ||
                    (trailer.Lane.m_NextLane != Entity.Null &&
                     !CanUseUndoLane(trailer.Lane.m_NextLane, trailer.Lane.m_NextPosition.x)))
                {
                    return false;
                }
            }

            return true;
        }

        private void InitializeUndoVehicle(Entity vehicle, Entity prefab, Transform transform, PseudoRandomSeed? seed)
        {
            EntityManager.SetComponentData(vehicle, new PrefabRef(prefab));
            EntityManager.SetComponentData(vehicle, transform);
            if (seed.HasValue && EntityManager.HasComponent<PseudoRandomSeed>(vehicle))
            {
                EntityManager.SetComponentData(vehicle, seed.Value);
            }

            EntityManager.AddComponent<Created>(vehicle);
            EntityManager.AddComponent<Updated>(vehicle);
        }

        private bool RestoreMovingCar(CarSnapshot snapshot, CarCurrentLane lane, bool dummyTraffic)
        {
            EntityArchetype archetype = EntityManager.GetComponentData<ObjectData>(snapshot.Prefab).m_Archetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return false;
            }

            Entity car = EntityManager.CreateEntity(archetype);
            List<Entity> trailers = null;
            bool restored = false;
            try
            {
                if (!EntityManager.HasComponent<PersonalCar>(car) ||
                    !EntityManager.HasComponent<Car>(car) ||
                    !EntityManager.HasComponent<CarCurrentLane>(car) ||
                    !EntityManager.HasComponent<CarNavigation>(car) ||
                    !EntityManager.HasComponent<Target>(car) ||
                    !EntityManager.HasComponent<PathOwner>(car) ||
                    !EntityManager.HasBuffer<PathElement>(car))
                {
                    return false;
                }

                InitializeUndoVehicle(car, snapshot.Prefab, snapshot.Transform, snapshot.Seed);
                if (!dummyTraffic || IsEntityUsable(snapshot.Owner, allowOverridden: true))
                {
                    SetUndoComponent(car, new Owner { m_Owner = snapshot.Owner });
                }

                // Pathfinding starts fresh. Boarding, passenger and lane reservation state belongs to the deleted car.
                PersonalCarFlags state = snapshot.State & (PersonalCarFlags.DummyTraffic | PersonalCarFlags.Transporting);
                EntityManager.SetComponentData(car, new PersonalCar(Entity.Null, state));
                EntityManager.SetComponentData(car, snapshot.DrivingState.Value);
                float position = lane.m_CurvePosition.x;
                EntityManager.SetComponentData(car, new CarCurrentLane(
                    new ParkedCar(lane.m_Lane, position),
                    CarLaneFlags.FixedLane | CarLaneFlags.ResetSpeed));
                EntityManager.SetComponentData(car, new CarNavigation
                {
                    m_TargetPosition = snapshot.Transform.m_Position,
                    m_TargetRotation = snapshot.Transform.m_Rotation
                });
                PathOwner pathOwner = default;
                Target target = default;
                VehicleUtils.SetTarget(ref pathOwner, ref target, snapshot.Target);
                EntityManager.SetComponentData(car, target);
                EntityManager.SetComponentData(car, pathOwner);

                if (snapshot.Trailers != null)
                {
                    trailers = new List<Entity>(snapshot.Trailers.Count);
                    foreach (TrailerSnapshot saved in snapshot.Trailers)
                    {
                        EntityArchetype trailerArchetype =
                            EntityManager.GetComponentData<ObjectData>(saved.Prefab).m_Archetype;
                        if (trailerArchetype.Equals(default(EntityArchetype)))
                        {
                            return false;
                        }

                        Entity trailer = EntityManager.CreateEntity(trailerArchetype);
                        trailers.Add(trailer);
                        if (!EntityManager.HasComponent<CarTrailer>(trailer) ||
                            !EntityManager.HasComponent<CarTrailerLane>(trailer) ||
                            !EntityManager.HasComponent<Controller>(trailer))
                        {
                            return false;
                        }

                        InitializeUndoVehicle(trailer, saved.Prefab, saved.Transform, saved.Seed);
                        EntityManager.SetComponentData(trailer, new Controller(car));
                        EntityManager.SetComponentData(trailer, saved.Lane);
                    }

                    DynamicBuffer<LayoutElement> layout = EntityManager.HasBuffer<LayoutElement>(car)
                        ? EntityManager.GetBuffer<LayoutElement>(car)
                        : EntityManager.AddBuffer<LayoutElement>(car);
                    layout.Add(new LayoutElement(car));
                    foreach (Entity trailer in trailers)
                    {
                        layout.Add(new LayoutElement(trailer));
                    }
                }

                restored = true;
                return true;
            }
            finally
            {
                if (!restored)
                {
                    if (trailers != null)
                    {
                        foreach (Entity trailer in trailers)
                        {
                            EntityManager.DestroyEntity(trailer);
                        }
                    }

                    EntityManager.DestroyEntity(car);
                }
            }
        }

        private bool RestoreCar(CarSnapshot snapshot)
        {
            bool dummyTraffic = (snapshot.State & PersonalCarFlags.DummyTraffic) != 0;
            if (IsEntityUsable(snapshot.Original, allowOverridden: true) ||
                !IsCarUndoPrefab(snapshot.Prefab) ||
                (!dummyTraffic && !IsCarUndoHousehold(snapshot.Owner)))
            {
                return false;
            }

            if (snapshot.CurrentLane is CarCurrentLane lane &&
                CanRestoreMovingCar(snapshot, lane) &&
                RestoreMovingCar(snapshot, lane, dummyTraffic))
            {
                return true;
            }

            if (dummyTraffic || snapshot.Trailers != null)
            {
                return false;
            }

            EntityArchetype archetype =
                EntityManager.GetComponentData<MovingObjectData>(snapshot.Prefab).m_StoppedArchetype;
            if (archetype.Equals(default(EntityArchetype)))
            {
                return false;
            }

            Entity car = EntityManager.CreateEntity(archetype);
            if (!EntityManager.HasComponent<PersonalCar>(car) ||
                !EntityManager.HasComponent<ParkedCar>(car) ||
                !EntityManager.HasComponent<PrefabRef>(car) ||
                !EntityManager.HasComponent<Transform>(car))
            {
                EntityManager.DestroyEntity(car);
                return false;
            }

            try
            {
                InitializeUndoVehicle(car, snapshot.Prefab, snapshot.Transform, snapshot.Seed);
                EntityManager.SetComponentData(car, new PersonalCar(Entity.Null, default));
                EntityManager.SetComponentData(car, new ParkedCar(Entity.Null, 0f));
                SetUndoComponent(car, new Owner { m_Owner = snapshot.Owner });
                SetUndoComponent(car, new FixParkingLocation(Entity.Null, car));
                EntityManager.AddComponent<Unspawned>(car);
                return true;
            }
            catch
            {
                EntityManager.DestroyEntity(car);
                throw;
            }
        }
    }
}
