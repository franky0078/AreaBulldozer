using Game.Simulation;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private SimulationSystem m_CarFilterSimulationSystem;
        private bool m_CarFilterOwnsPause;
        private float m_PreviousCarFilterSpeed;

        public void OnCarFilterChanged()
        {
            MarkSpatialIndexStale();
            InvalidateSelectionGeometry();
            UpdateCarFilterPause();
        }

        private void UpdateCarFilterPause()
        {
            if (!IsToolActive || !(Mod.Settings?.DeleteCars ?? false))
            {
                ReleaseCarFilterPause();
                return;
            }

            m_CarFilterSimulationSystem ??=
                World.GetOrCreateSystemManaged<SimulationSystem>();

            if (m_CarFilterOwnsPause ||
                m_CarFilterSimulationSystem.selectedSpeed <= 0f)
            {
                return;
            }

            m_PreviousCarFilterSpeed =
                m_CarFilterSimulationSystem.selectedSpeed;
            m_CarFilterSimulationSystem.selectedSpeed = 0f;
            m_CarFilterOwnsPause = true;
        }

        private void ObserveCarFilterPauseOverride()
        {
            // If the user (or another mod) resumes, do not restore stale speed.
            if (m_CarFilterOwnsPause &&
                m_CarFilterSimulationSystem != null &&
                m_CarFilterSimulationSystem.selectedSpeed > 0f)
            {
                m_CarFilterOwnsPause = false;
                MarkSpatialIndexStale();
            }
        }

        private void ReleaseCarFilterPause()
        {
            if (!m_CarFilterOwnsPause)
            {
                return;
            }

            m_CarFilterOwnsPause = false;

            if (m_CarFilterSimulationSystem != null &&
                m_CarFilterSimulationSystem.selectedSpeed <= 0f &&
                m_PreviousCarFilterSpeed > 0f)
            {
                m_CarFilterSimulationSystem.selectedSpeed =
                    m_PreviousCarFilterSpeed;
            }

            m_PreviousCarFilterSpeed = 0f;
        }
    }
}
