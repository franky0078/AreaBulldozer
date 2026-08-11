using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    internal enum SpatialCandidateKind
    {
        Vegetation,
        Building,
        Road,
        PedestrianPath,
        Railway,
        SurfaceArea,
        StaticObject,
        SpawnLocation,
        AssetLane
    }

    internal readonly struct SpatialCandidate
    {
        public SpatialCandidate(
            Entity entity,
            float2 position,
            SpatialCandidateKind kind,
            StaticObjectCategory staticCategory)
            : this(
                entity,
                position,
                position,
                kind,
                staticCategory,
                false)
        {
        }

        public SpatialCandidate(
            Entity entity,
            float2 position,
            float2 endPosition,
            SpatialCandidateKind kind,
            StaticObjectCategory staticCategory,
            bool isSegment)
        {
            Entity = entity;
            Position = position;
            EndPosition = endPosition;
            Kind = kind;
            StaticCategory = staticCategory;
            IsSegment = isSegment;
        }

        public Entity Entity
        {
            get;
        }

        public float2 Position
        {
            get;
        }

        public float2 EndPosition
        {
            get;
        }

        public SpatialCandidateKind Kind
        {
            get;
        }

        public StaticObjectCategory StaticCategory
        {
            get;
        }

        public bool IsSegment
        {
            get;
        }

        public bool IsVegetation =>
            Kind == SpatialCandidateKind.Vegetation;

        public bool IsBuilding =>
            Kind == SpatialCandidateKind.Building;

        public bool IsRoad =>
            Kind == SpatialCandidateKind.Road;

        public bool IsPedestrianPath =>
            Kind == SpatialCandidateKind.PedestrianPath;

        public bool IsRailway =>
            Kind == SpatialCandidateKind.Railway;

        public bool IsSurfaceArea =>
            Kind == SpatialCandidateKind.SurfaceArea;

        public bool IsStaticObject =>
            Kind == SpatialCandidateKind.StaticObject;

        public bool IsSpawnLocation =>
            Kind == SpatialCandidateKind.SpawnLocation;

        public bool IsAssetLane =>
            Kind == SpatialCandidateKind.AssetLane;
    }
}
