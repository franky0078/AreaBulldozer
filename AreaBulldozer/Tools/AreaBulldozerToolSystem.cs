using Game;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem : ToolBaseSystem
    {
        private const float kPreviewMoveThreshold = 1.5f;
        private const float kPreviewUpdateInterval = 0.075f;


        private const int kDefaultLargeSelectionThreshold = 250;
        private const float kLargeSelectionConfirmationTimeout = 5f;
        private const float kLargeSelectionMoveTolerance = 2f;


        private OverlayRenderSystem m_OverlayRenderSystem;
        private ToolOutputBarrier m_ToolOutputBarrier;


        private EntityQuery m_PlantQuery;
        private EntityQuery m_BuildingQuery;
        private EntityQuery m_RoadEdgeQuery;
        private EntityQuery m_NetEdgeQuery;
        private EntityQuery m_SurfaceAreaQuery;
        private EntityQuery m_StaticObjectQuery;
        private EntityQuery m_SpawnLocationQuery;
        private EntityQuery m_SubLaneOwnerQuery;


        private InputAction m_ApplyAction;
        private bool m_IsPointerOverUI;
        private InputAction m_RotateSquareHoldAction;
        private InputAction m_RotateSquareDeltaAction;


        private HashSet<Entity> m_HighlightedEntities;
        private HashSet<Entity> m_NextHighlightedEntities;
        private HashSet<Entity> m_PendingDeletion;


        private float3 m_LastPreviewPosition;
        private float m_LastPreviewRadius;
        private float m_NextPreviewUpdateTime;
        private bool m_LastUseSquareBrush;
        private float m_LastPreviewSquareRotationRadians;


        private FilterSnapshot m_LastFilterSnapshot;

        private bool m_LargeSelectionConfirmationPending;
        private float3 m_LargeSelectionConfirmationPosition;
        private float m_LargeSelectionConfirmationRadius;
        private bool m_LargeSelectionConfirmationUseSquareBrush;
        private float m_LargeSelectionConfirmationSquareRotationRadians;
        private float m_LargeSelectionConfirmationExpiresAt;


        private FilterSnapshot m_ConfirmationFilterSnapshot;

        private int m_ConfirmationObjectCount;
        private int m_ConfirmationVegetationCount;
        private int m_ConfirmationBuildingCount;
        private int m_ConfirmationRoadCount;
        private int m_ConfirmationPathCount;
        private int m_ConfirmationRailwayCount;
        private int m_ConfirmationSurfaceCount;
        private int m_ConfirmationStaticObjectCount;
        private int m_ConfirmationSpawnLocationCount;
        private int m_ConfirmationMarkerNetworkCount;
        private int m_ConfirmationThreshold;


        private HashSet<Entity> m_ConfirmationEntities;

        private enum OwnerScope
        {
            None,
            Building,
            Network,
            Other
        }

        public static AreaBulldozerToolSystem Instance
        {
            get;
            private set;
        }

        public override string toolID =>
            "AreaBulldozerTool";

        public float3 CurrentPosition
        {
            get;
            private set;
        }

        public bool HasValidPosition
        {
            get;
            private set;
        }

        public bool IsToolActive =>
            m_ToolSystem.activeTool == this;

        public float CurrentRadius
        {
            get
            {
                if (Mod.Settings == null)
                {
                    return 30f;
                }

                return math.max(
                    5f,
                    Mod.Settings.BrushRadius);
            }
        }

        public bool UseSquareBrush =>
            Mod.Settings != null &&
            Mod.Settings.UseSquareBrush;

        public float SquareRotationRadians
        {
            get;
            private set;
        }

        public float SquareRotationDegrees =>
            math.degrees(SquareRotationRadians);

        private float CurrentSpatialQueryRadius
        {
            get
            {
                return UseSquareBrush
                    ? CurrentRadius * 1.41421356237f
                    : CurrentRadius;
            }
        }


        public int CurrentLargeSelectionThreshold
        {
            get
            {
                if (Mod.Settings == null)
                {
                    return kDefaultLargeSelectionThreshold;
                }

                return math.clamp(
                    Mod.Settings.LargeSelectionThreshold,
                    50,
                    2000);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            Instance = this;

            m_OverlayRenderSystem =
                World.GetOrCreateSystemManaged<
                    OverlayRenderSystem>();

            m_ToolOutputBarrier =
                World.GetOrCreateSystemManaged<
                    ToolOutputBarrier>();

            m_HighlightedEntities =
                new();

            m_NextHighlightedEntities =
                new();

            m_PendingDeletion =
                new();

            m_ConfirmationEntities =
                new();

            InitializeSpatialIndex();
            InitializeMarkerVisibility();
            InitializeToolStateCleanup();

            m_PlantQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Transform>(),
                        ComponentType.ReadOnly<Plant>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>()
                    }
                });

            m_BuildingQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Transform>(),
                        ComponentType.ReadOnly<
                            Game.Buildings.Building>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>()
                    }
                });

            m_RoadEdgeQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Net.Edge>(),
                        ComponentType.ReadOnly<Game.Net.Road>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>()
                    }
                });

            m_NetEdgeQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Net.Edge>(),
                        ComponentType.ReadOnly<PrefabRef>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>()
                    }
                });

            m_SurfaceAreaQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Areas.Area>(),
                        ComponentType.ReadOnly<Game.Areas.Node>()
                    },

                    Any = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Areas.Surface>(),
                        ComponentType.ReadOnly<Game.Areas.Space>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>()
                    }
                });

            m_StaticObjectQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Transform>(),
                        ComponentType.ReadOnly<PrefabRef>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>(),
                        ComponentType.ReadOnly<Plant>(),
                        ComponentType.ReadOnly<Tree>(),

                        // Never treat actual building entities as
                        // removable static props.
                        ComponentType.ReadOnly<
                            Game.Buildings.Building>()
                    }
                });

            m_SpawnLocationQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Transform>(),
                        ComponentType.ReadOnly<
                            Game.Objects.SpawnLocation>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>(),
                        ComponentType.ReadOnly<Overridden>()
                    }
                });

            m_SubLaneOwnerQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Game.Net.SubLane>()
                    },

                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Deleted>(),
                        ComponentType.ReadOnly<Temp>()
                    }
                });

            m_ApplyAction = new(
                "AreaBulldozerApply",
                InputActionType.Button,
                "<Mouse>/leftButton");

            m_RotateSquareHoldAction = new(
                "AreaBulldozerRotateSquareHold",
                InputActionType.Button,
                "<Mouse>/rightButton");

            m_RotateSquareDeltaAction = new(
                "AreaBulldozerRotateSquareDelta",
                InputActionType.Value,
                "<Mouse>/delta");

            SquareRotationRadians = 0f;

            ResetPreviewState();

            CurrentPosition = float3.zero;
            HasValidPosition = false;
            Enabled = false;

            TryLogInfo(
                $"{nameof(AreaBulldozerToolSystem)} created.");

            TryLogInfo(
                "Building filter support initialized.");

            TryLogInfo(
                "Road network filter support initialized.");

            TryLogInfo(
                "Pedestrian-path network filter support initialized.");

            TryLogInfo(
                "Railway-track network filter support initialized.");

            TryLogInfo(
                "Surface/space area filter support initialized.");

            TryLogInfo(
                "Static object filter support initialized.");

            TryLogInfo(
                "Static object category filters initialized.");

            TryLogInfo(
                "Spawn-location marker filter initialized.");

            TryLogInfo(
                "Asset-lane/SubLane filter initialized.");

            TryLogInfo(
                "Building protection for static-object selection initialized.");

            TryLogInfo(
                "Owned sub-object scope filters initialized.");

            TryLogInfo(
                $"Large selection confirmation initialized at " +
                $"{CurrentLargeSelectionThreshold} objects.");

            TryLogInfo(
                $"Spatial preview index configured with " +
                $"{kSpatialCellSize:0} m cells.");
        }


        private static void TryLogInfo(string message)
        {
            try
            {
                if (Mod.Log != null)
                {
                    Mod.Log.Info(message);
                }
            }
            catch
            {
                // Logging failed.
            }
        }

        protected override void OnDestroy()
        {
            m_IsPointerOverUI = false;

            RestoreMarkerVisibility();
            DisposeMarkerVisibility();
            DisposeToolStateCleanup();

            ClearSelectionPreview();

            if (m_ApplyAction != null)
            {
                m_ApplyAction.Disable();
                m_ApplyAction.Dispose();
                m_ApplyAction = null;
            }

            if (m_RotateSquareHoldAction != null)
            {
                m_RotateSquareHoldAction.Disable();
                m_RotateSquareHoldAction.Dispose();
                m_RotateSquareHoldAction = null;
            }

            if (m_RotateSquareDeltaAction != null)
            {
                m_RotateSquareDeltaAction.Disable();
                m_RotateSquareDeltaAction.Dispose();
                m_RotateSquareDeltaAction = null;
            }

            m_HighlightedEntities?.Clear();
            m_HighlightedEntities = null;

            m_NextHighlightedEntities?.Clear();
            m_NextHighlightedEntities = null;

            m_PendingDeletion?.Clear();
            m_PendingDeletion = null;

            m_ConfirmationEntities?.Clear();
            m_ConfirmationEntities = null;

            DisposeSpatialIndex();

            if (Instance == this)
            {
                Instance = null;
            }

            Mod.Log.Info(
                $"{nameof(AreaBulldozerToolSystem)} destroyed.");

            base.OnDestroy();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            CurrentPosition = float3.zero;
            HasValidPosition = false;

            m_PendingDeletion ??=
                new();

            ClearPreviousToolSelection();
            ResetPreviewState();

            RebuildSpatialIndex();
            UpdateMarkerVisibility();

            m_ApplyAction?.Enable();
            m_RotateSquareHoldAction?.Enable();
            m_RotateSquareDeltaAction?.Enable();

            Mod.Log.Info(
                "Area Bulldozer tool activated.");

            Mod.Log.Info(
                $"Active selection - shape: {(UseSquareBrush ? "square" : "circle")}, " +
                $"size: {CurrentRadius:0} m " +
                $"{(UseSquareBrush ? "half-side" : "radius")}" +
                $"{(UseSquareBrush ? $", rotation: {SquareRotationDegrees:0} degrees" : string.Empty)}. " +
                $"Active filters - vegetation: {Mod.Settings?.DeleteTrees}, " +
                $"buildings: {Mod.Settings?.DeleteBuildings}, " +
                $"roads: {Mod.Settings?.DeleteRoads}, " +
                $"pedestrian paths: {Mod.Settings?.DeletePaths}, " +
                $"railway tracks: {Mod.Settings?.DeleteRailways}, " +
                $"surfaces and spaces: {Mod.Settings?.DeleteSurfaces}, " +
                $"static objects: {Mod.Settings?.DeleteStaticObjects}, " +
                $"general props: {Mod.Settings?.DeleteGeneralProps}, " +
                $"street lights: {Mod.Settings?.DeleteStreetLights}, " +
                $"quantity objects: {Mod.Settings?.DeleteQuantityObjects}, " +
                $"branding: {Mod.Settings?.DeleteBrandingObjects}, " +
                $"activity locations: {Mod.Settings?.DeleteActivityLocations}, " +
                $"spawn locations: {Mod.Settings?.DeleteSpawnLocations}, " +
                $"asset lanes: {Mod.Settings?.DeleteMarkerNetworks}, " +
                $"dim marker background: {Mod.Settings?.DimMarkerBackground}, " +
                $"background darkness: {Mod.Settings?.MarkerBackgroundDarkness}%, " +
                $"building sub-objects: {Mod.Settings?.DeleteBuildingSubObjects}, " +
                $"network sub-objects: {Mod.Settings?.DeleteNetworkSubObjects}, " +
                $"protect other owned: {Mod.Settings?.ProtectOwnedObjects}, " +
                $"confirmation threshold: {CurrentLargeSelectionThreshold}.");
        }

        protected override void OnStopRunning()
        {
            m_ApplyAction?.Disable();
            m_RotateSquareHoldAction?.Disable();
            m_RotateSquareDeltaAction?.Disable();

            RestoreMarkerVisibility();

            CancelLargeSelectionConfirmation();

            FlushContinuousDeleteLog();
            m_ContinuousDeleteActive = false;

            ClearSelectionPreview();

            CurrentPosition = float3.zero;
            HasValidPosition = false;
            m_IsPointerOverUI = false;

            MarkSpatialIndexStale();

            SafeLogInfo(
                "Area Bulldozer tool deactivated.");

            base.OnStopRunning();
        }

        public void ToggleTool()
        {
            if (m_ToolSystem.activeTool == this)
            {
                DeactivateTool();
            }
            else
            {
                ActivateTool();
            }
        }

        public void ActivateTool()
        {
            if (m_ToolSystem.activeTool == this)
            {
                return;
            }

            Enabled = true;

            m_ToolSystem.selected = Entity.Null;
            m_ToolSystem.activeTool = this;
        }

        public void DeactivateTool()
        {
            if (m_ToolSystem.activeTool != this)
            {
                Enabled = false;
                return;
            }

            CancelLargeSelectionConfirmation();

            ClearSelectionPreview();

            m_ToolSystem.selected = Entity.Null;
            m_ToolSystem.activeTool =
                m_DefaultToolSystem;

            Enabled = false;
        }

        public void SetPointerOverUI(bool isPointerOverUI)
        {
            m_IsPointerOverUI = isPointerOverUI;

            if (isPointerOverUI &&
                m_LargeSelectionConfirmationPending)
            {
                CancelLargeSelectionConfirmation();
            }
        }

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();

            m_ToolRaycastSystem.typeMask =
                TypeMask.Terrain;

            m_ToolRaycastSystem.collisionMask =
                CollisionMask.OnGround |
                CollisionMask.Overground;
        }

        public override PrefabBase GetPrefab()
        {
            return null;
        }

        public override bool TrySetPrefab(
            PrefabBase prefab)
        {
            return false;
        }

        private void UpdateSquareRotationInput()
        {
            if (m_IsPointerOverUI ||
                !UseSquareBrush ||
                m_RotateSquareHoldAction == null ||
                m_RotateSquareDeltaAction == null ||
                m_RotateSquareHoldAction.ReadValue<float>() < 0.5f)
            {
                return;
            }

            UnityEngine.Vector2 pointerDelta =
                m_RotateSquareDeltaAction.ReadValue<
                    UnityEngine.Vector2>();

            if (math.abs(pointerDelta.x) < 0.01f)
            {
                return;
            }

            const float degreesPerPixel = 0.35f;

            float nextDegrees =
                math.fmod(
                    SquareRotationDegrees +
                    pointerDelta.x * degreesPerPixel +
                    360f,
                    360f);

            SquareRotationRadians =
                math.radians(nextDegrees);

            m_NextPreviewUpdateTime = 0f;

            if (m_LargeSelectionConfirmationPending)
            {
                CancelLargeSelectionConfirmation(
                    logReason:
                    "Large selection confirmation cancelled because the square rotated.");
            }
        }

        public void SetSquareRotationDegrees(float degrees)
        {
            float normalizedDegrees =
                math.fmod(
                    degrees + 360f,
                    360f);

            SquareRotationRadians =
                math.radians(normalizedDegrees);

            m_NextPreviewUpdateTime = 0f;
            CancelLargeSelectionConfirmation();
        }

        protected override JobHandle OnUpdate(
            JobHandle inputDeps)
        {
            try
            {
                return OnToolUpdate(inputDeps);
            }
            catch (System.Exception exception)
            {
                SafeLogError(
                    $"Area Bulldozer: recovered from an error in " +
                    $"OnUpdate: {exception}");

                return inputDeps;
            }
        }

        private JobHandle OnToolUpdate(
            JobHandle inputDeps)
        {
            CleanupPendingDeletions();
            RefreshSpatialIndexIfNeeded();
            UpdateLargeSelectionConfirmationState();
            UpdateMarkerVisibility();

            if (!GetRaycastResult(
                    out Entity _,
                    out RaycastHit hit))
            {
                HasValidPosition = false;
                CurrentPosition = float3.zero;

                CancelLargeSelectionConfirmation();
                ClearSelectionPreview();

                return inputDeps;
            }

            CurrentPosition = hit.m_HitPosition;
            HasValidPosition = true;

            UpdateSquareRotationInput();
            UpdateSelectionPreviewIfNeeded();

            UpdateContinuousDeleteInput();

            return DrawToolShape(inputDeps);
        }
    }
}
