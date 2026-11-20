using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.InputSystem;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        internal event Action AssetFilterChanged;
        internal bool AssetPickerActive { get; private set; }
        private bool m_AssetPickerWaitForRelease;
        private bool m_AssetDeleteWaitForRelease;
        private Entity m_AssetPickerHover;
        private string m_AssetFilterStorage;
        private readonly HashSet<Entity> m_AssetFilterPrefabs = new();
        private string m_AssetFilterItems = "[]";

        internal void SetAssetPicker(bool active)
        {
            AssetPickerActive = active && IsToolActive;
            m_AssetPickerHover = Entity.Null;
            requireAreas = AssetPickerActive ? Game.Areas.AreaTypeMask.Surfaces | Game.Areas.AreaTypeMask.Spaces :
                Game.Areas.AreaTypeMask.None;
            m_AssetPickerWaitForRelease = AssetPickerActive;
            m_AssetDeleteWaitForRelease = true;
            ResetContinuousDeleteState();
            ResetPolylineSelection();
            CancelLargeSelectionConfirmation();
            ClearSelectionPreview();
            m_NextPreviewUpdateTime = 0f;
        }

        internal void NotifyAssetFilterChanged()
        {
            m_AssetFilterStorage = null;
            m_AssetPickerHover = Entity.Null;
            ResetContinuousDeleteState();
            CancelLargeSelectionConfirmation();
            ClearSelectionPreview();
            m_NextPreviewUpdateTime = 0f;
        }

        private void ResetAssetFilterRuntime()
        {
            AssetPickerActive = false;
            requireAreas = Game.Areas.AreaTypeMask.None;
            m_AssetPickerHover = Entity.Null;
            m_AssetFilterStorage = null;
            m_AssetFilterPrefabs.Clear();
            m_AssetFilterItems = "[]";
        }

        private bool MatchesAssetFilter(Entity entity)
        {
            if (!(Mod.Settings?.AssetFilterEnabled ?? false)) return true;
            EnsureAssetFilter();
            entity = GetSelectedCarRoot(entity);
            // An active empty or unresolved list deliberately selects nothing.
            return EntityManager.HasComponent<PrefabRef>(entity) &&
                m_AssetFilterPrefabs.Contains(EntityManager.GetComponentData<PrefabRef>(entity).m_Prefab);
        }

        internal string GetAssetFilterItems()
        {
            EnsureAssetFilter();
            return m_AssetFilterItems;
        }

        private void EnsureAssetFilter()
        {
            string stored = Mod.Settings?.AssetFilterPrefabs ?? "";
            if (stored == m_AssetFilterStorage) return;
            m_AssetFilterStorage = stored;
            m_AssetFilterPrefabs.Clear();
            var json = new StringBuilder("[");
            foreach (string id in AssetFilterSettings.Read(stored))
            {
                string[] parts = id.Split('/');
                string name = Uri.UnescapeDataString(parts[1]);
                string icon = "";
                bool available = TryResolveAssetFilterPrefab(id, out PrefabBase prefab, out Entity entity);
                if (available)
                {
                    m_AssetFilterPrefabs.Add(entity);
                    name = prefab.name;
                    icon = ImageSystem.GetThumbnail(prefab);
                    if (string.IsNullOrEmpty(icon)) icon = ImageSystem.GetIcon(prefab);
                }
                if (json.Length > 1) json.Append(',');
                json.Append("{\"id\":"); AppendAssetJsonString(json, id);
                json.Append(",\"name\":"); AppendAssetJsonString(json, name);
                json.Append(",\"icon\":"); AppendAssetJsonString(json, icon ?? "");
                json.Append(",\"available\":").Append(available ? "true}" : "false}");
            }
            m_AssetFilterItems = json.Append(']').ToString();
        }

        private bool TryResolveAssetFilterPrefab(string id, out PrefabBase prefab, out Entity entity)
        {
            prefab = null;
            entity = Entity.Null;
            string[] parts = id.Split('/');
            if (parts.Length != 2 && parts.Length != 3) return false;
            Colossal.Hash128 hash = default;
            if (parts.Length == 3 && !Colossal.Hash128.TryParse(parts[2], out hash)) return false;
            var prefabId = new PrefabID(Uri.UnescapeDataString(parts[0]), Uri.UnescapeDataString(parts[1]), hash);
            return m_PrefabSystem.TryGetPrefab(prefabId, out prefab) && prefab != null &&
                m_PrefabSystem.TryGetEntity(prefab, out entity) && IsEntityUsable(entity);
        }

        private static void AppendAssetJsonString(StringBuilder json, string value)
        {
            json.Append('"');
            foreach (char c in value)
            {
                if (c == '"' || c == '\\') json.Append('\\');
                if (c < 32) json.Append("\\u").Append(((int)c).ToString("x4"));
                else json.Append(c);
            }
            json.Append('"');
        }

        private bool TryGetAssetPickerTarget(Entity entity, out Entity target, out PrefabBase prefab)
        {
            target = GetSelectedCarRoot(entity);
            prefab = null;
            if (IsEntityUsable(target, allowOverridden: true) && TryGetPrefabBase(target, out prefab))
            {
                // Network junctions can have a special composition prefab. Pick
                // an incident segment so the list matches removable segments.
                if (EntityManager.HasComponent<Game.Net.Node>(target) &&
                    EntityManager.HasBuffer<Game.Net.ConnectedEdge>(target))
                {
                    var edges = EntityManager.GetBuffer<Game.Net.ConnectedEdge>(target, true);
                    foreach (Game.Net.ConnectedEdge edge in edges)
                    {
                        if (IsEntityUsable(edge.m_Edge) && TryGetPrefabBase(edge.m_Edge, out prefab))
                        {
                            target = edge.m_Edge;
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        private void EnablePickedAssetCategory(Entity target)
        {
            Setting setting = Mod.Settings;
            if (setting == null || !IsEntityUsable(target, allowOverridden: true)) return;
            bool previouslyCars = setting.DeleteCars;

            // Follow the same category boundaries as the spatial index. Keep
            // existing categories and all height/ownership protection settings.
            if (EntityManager.HasComponent<Game.Objects.Plant>(target))
                setting.DeleteTrees = true;
            else if (IsBuildingEntity(target))
                setting.DeleteBuildings = true;
            else if (EntityManager.HasComponent<Game.Vehicles.Car>(target) ||
                     EntityManager.HasComponent<Game.Vehicles.ParkedCar>(target) ||
                     EntityManager.HasComponent<Game.Vehicles.CarTrailer>(target))
                setting.DeleteCars = true;
            else if (IsMainRoadEdgeEntity(target))
                setting.DeleteRoads = true;
            else if (IsMainPedestrianPathEdgeEntity(target))
                setting.DeletePaths = true;
            else if (IsMainRailwayEdgeEntity(target))
                setting.DeleteRailways = true;
            else if (EntityManager.HasComponent<Game.Areas.Surface>(target) ||
                     EntityManager.HasComponent<Game.Areas.Space>(target))
                setting.DeleteSurfaces = true;
            else if (IsAssetMarkerNetwork(target) || IsStandaloneAssetLane(target) ||
                     (EntityManager.HasComponent<Game.Net.Lane>(target) && IsAssetLaneMarker(target)))
            {
                setting.DeleteStaticObjects = true;
                setting.DeleteMarkerNetworks = true;
            }
            else if (IsSpawnPointMarker(target) || IsActivityLocationTarget(target) || IsStaticObjectEntity(target))
            {
                setting.DeleteStaticObjects = true;
                if (IsSpawnPointMarker(target)) setting.DeleteSpawnLocations = true;
                switch (GetStaticObjectCategory(target))
                {
                    case StaticObjectCategory.StreetLight: setting.DeleteStreetLights = true; break;
                    case StaticObjectCategory.QuantityObject: setting.DeleteQuantityObjects = true; break;
                    case StaticObjectCategory.BrandingObject: setting.DeleteBrandingObjects = true; break;
                    case StaticObjectCategory.ActivityLocation: setting.DeleteActivityLocations = true; break;
                    case StaticObjectCategory.GeneralProp:
                        if (IsStaticObjectEntity(target)) setting.DeleteGeneralProps = true;
                        break;
                }
            }

            MarkSpatialIndexStale();
            if (setting.DeleteCars != previouslyCars) OnCarFilterChanged();
        }

        private JobHandle UpdateAssetPicker(JobHandle inputDeps)
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true ||
                Mouse.current?.rightButton.wasPressedThisFrame == true)
            {
                SetAssetPicker(false);
                return inputDeps;
            }
            bool held = m_ApplyAction?.ReadValue<float>() >= 0.5f || Mouse.current?.leftButton.isPressed == true;
            if (!held) m_AssetPickerWaitForRelease = false;
            Entity target = Entity.Null;
            PrefabBase prefab = null;
            if (!m_IsPointerOverUI && GetRaycastResult(out Entity rayEntity, out _))
            {
                if (!TryGetAssetPickerTarget(rayEntity, out target, out prefab))
                {
                    target = Entity.Null;
                    prefab = null;
                }
            }
            if (prefab == null) target = Entity.Null;
            if (target != m_AssetPickerHover)
            {
                ClearSelectionPreview();
                m_AssetPickerHover = target;
                if (target != Entity.Null)
                {
                    m_NextHighlightedEntities.Clear();
                    AddEntityToNextPreview(target);
                    ApplyPreviewHighlightDiff();
                }
            }
            HasValidPosition = false;
            if (!m_AssetPickerWaitForRelease && !m_IsPointerOverUI && prefab != null &&
                (m_ApplyAction?.WasPressedThisFrame() == true || Mouse.current?.leftButton.wasPressedThisFrame == true))
            {
                var ids = AssetFilterSettings.Read(Mod.Settings?.AssetFilterPrefabs);
                string id = prefab.GetPrefabID().ToUrlSegment();
                if (Mod.Settings != null &&
                    (ids.Contains(id) || ids.Count < AssetFilterSettings.MaximumAssets))
                {
                    var before = new AssetSelectionState(Mod.Settings);
                    if (!ids.Contains(id)) ids.Add(id);
                    Mod.Settings.AssetFilterPrefabs = string.Join("\n", ids);
                    Mod.Settings.AssetFilterEnabled = true;
                    RecordAssetSelectionUndo(before);
                    EnablePickedAssetCategory(target);
                    NotifyAssetFilterChanged();
                    AssetFilterChanged?.Invoke();
                }
                // Stay in pick mode for multiple assets; each click is consumed.
                m_AssetPickerWaitForRelease = true;
            }
            return inputDeps;
        }
    }
}
