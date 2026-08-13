namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {

        private bool m_ContinuousDeleteLogPending;
        private int m_ContinuousDeleteBatchCount;
        private int m_ContinuousDeleteTotalObjects;
        private int m_ContinuousDeleteVegetation;
        private int m_ContinuousDeleteBuildings;
        private int m_ContinuousDeleteRoads;
        private int m_ContinuousDeletePaths;
        private int m_ContinuousDeleteRailways;
        private int m_ContinuousDeleteSurfaces;
        private int m_ContinuousDeleteStaticObjects;
        private int m_ContinuousDeleteSpawnLocations;
        private int m_ContinuousDeleteMarkerNetworks;
        private int m_ContinuousDeleteNodesDeleted;
        private int m_ContinuousDeleteNodesUpdated;
        private int m_ContinuousDeleteEdgesUpdated;
        private int m_ContinuousDeleteProtectedIgnored;

        private void AccumulateContinuousDeleteStats(
            int totalObjects,
            int vegetation,
            int buildings,
            int roads,
            int paths,
            int railways,
            int surfaces,
            int staticObjects,
            int spawnLocations,
            int markerNetworks,
            int nodesDeleted,
            int nodesUpdated,
            int edgesUpdated,
            int protectedIgnored)
        {
            m_ContinuousDeleteLogPending = true;
            m_ContinuousDeleteBatchCount++;

            m_ContinuousDeleteTotalObjects += totalObjects;
            m_ContinuousDeleteVegetation += vegetation;
            m_ContinuousDeleteBuildings += buildings;
            m_ContinuousDeleteRoads += roads;
            m_ContinuousDeletePaths += paths;
            m_ContinuousDeleteRailways += railways;
            m_ContinuousDeleteSurfaces += surfaces;
            m_ContinuousDeleteStaticObjects += staticObjects;
            m_ContinuousDeleteSpawnLocations += spawnLocations;
            m_ContinuousDeleteMarkerNetworks += markerNetworks;
            m_ContinuousDeleteNodesDeleted += nodesDeleted;
            m_ContinuousDeleteNodesUpdated += nodesUpdated;
            m_ContinuousDeleteEdgesUpdated += edgesUpdated;
            m_ContinuousDeleteProtectedIgnored += protectedIgnored;
        }

        private void FlushContinuousDeleteLog()
        {
            if (!m_ContinuousDeleteLogPending)
            {
                return;
            }

            SafeLogInfo(
                $"Area Bulldozer: continuous delete finished. " +
                $"{m_ContinuousDeleteBatchCount} batches, " +
                $"{m_ContinuousDeleteTotalObjects} objects total. " +
                $"Vegetation: {m_ContinuousDeleteVegetation}, " +
                $"buildings: {m_ContinuousDeleteBuildings}, " +
                $"roads: {m_ContinuousDeleteRoads}, " +
                $"pedestrian paths: {m_ContinuousDeletePaths}, " +
                $"railway tracks: {m_ContinuousDeleteRailways}, " +
                $"surfaces and spaces: {m_ContinuousDeleteSurfaces}, " +
                $"static objects: {m_ContinuousDeleteStaticObjects}, " +
                $"spawn locations: " +
                $"{m_ContinuousDeleteSpawnLocations}, " +
                $"asset lanes: {m_ContinuousDeleteMarkerNetworks}, " +
                $"network endpoint nodes deleted: " +
                $"{m_ContinuousDeleteNodesDeleted}, " +
                $"network nodes updated: " +
                $"{m_ContinuousDeleteNodesUpdated}, " +
                $"connected network edges updated: " +
                $"{m_ContinuousDeleteEdgesUpdated}, " +
                $"protected sub-objects ignored: " +
                $"{m_ContinuousDeleteProtectedIgnored}.");

            ResetContinuousDeleteLog();
        }

        private void ResetContinuousDeleteLog()
        {
            m_ContinuousDeleteLogPending = false;
            m_ContinuousDeleteBatchCount = 0;
            m_ContinuousDeleteTotalObjects = 0;
            m_ContinuousDeleteVegetation = 0;
            m_ContinuousDeleteBuildings = 0;
            m_ContinuousDeleteRoads = 0;
            m_ContinuousDeletePaths = 0;
            m_ContinuousDeleteRailways = 0;
            m_ContinuousDeleteSurfaces = 0;
            m_ContinuousDeleteStaticObjects = 0;
            m_ContinuousDeleteSpawnLocations = 0;
            m_ContinuousDeleteMarkerNetworks = 0;
            m_ContinuousDeleteNodesDeleted = 0;
            m_ContinuousDeleteNodesUpdated = 0;
            m_ContinuousDeleteEdgesUpdated = 0;
            m_ContinuousDeleteProtectedIgnored = 0;
        }
    }
}
