using Game.Common;
using Game.Tools;
using Unity.Entities;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private ToolClearSystem m_ToolClearSystem;
        private EntityQuery m_PreExistingHighlightedQuery;

        private void InitializeToolStateCleanup()
        {
            m_ToolClearSystem =
                World.GetOrCreateSystemManaged<
                    ToolClearSystem>();

            m_PreExistingHighlightedQuery =
                GetEntityQuery(
                    new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            ComponentType.ReadOnly<
                                Highlighted>()
                        },

                        None = new ComponentType[]
                        {
                            ComponentType.ReadOnly<
                                Deleted>(),

                            ComponentType.ReadOnly<
                                Temp>(),

                            ComponentType.ReadOnly<
                                Overridden>()
                        }
                    });

            Mod.LogDiagnosticInfo(
                "Previous tool selection cleanup initialized.");
        }

        private void DisposeToolStateCleanup()
        {
            m_ToolClearSystem = null;
        }

        private void ClearPreviousToolSelection()
        {
            int highlightedCount =
                m_PreExistingHighlightedQuery
                    .CalculateEntityCount();

            m_ToolSystem.selected =
                Entity.Null;

            if (highlightedCount > 0)
            {
                EntityManager.AddComponent<
                    BatchesUpdated>(
                    m_PreExistingHighlightedQuery);

                EntityManager.RemoveComponent<
                    Highlighted>(
                    m_PreExistingHighlightedQuery);
            }

            m_ToolClearSystem?.Update();

            if (highlightedCount > 0)
            {
                Mod.LogDiagnosticInfo(
                    $"Cleared {highlightedCount} highlight(s) " +
                    $"from the previously active tool.");
            }
        }
    }
}
