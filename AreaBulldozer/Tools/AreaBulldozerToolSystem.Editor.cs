using Game;
using Game.SceneFlow;
using Game.UI.Editor;
using System;
using System.Linq;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const string kEditorIcon =
            "Media/Game/Icons/Bulldozer.svg";

        private EditorToolUISystem m_EditorToolUISystem;
        private IEditorTool m_EditorTool;
        private IEditorTool m_PreviousEditorTool;

        private bool IsEditorMode =>
            GameManager.instance?.gameMode == GameMode.Editor;

        private void InitializeEditorTool()
        {
            m_EditorToolUISystem =
                World.GetExistingSystemManaged<EditorToolUISystem>();

            if (m_EditorToolUISystem == null)
            {
                SafeLogWarn(
                    "Area Bulldozer editor integration is not available.");

                return;
            }

            IEditorTool[] editorTools =
                m_EditorToolUISystem.tools ??
                Array.Empty<IEditorTool>();

            IEditorTool existingTool =
                editorTools.FirstOrDefault(
                    editorTool => editorTool?.id == toolID);

            if (existingTool != null)
            {
                SafeLogWarn(
                    $"An editor tool with id '{toolID}' is already registered.");

                return;
            }

            m_EditorTool =
                new AreaBulldozerEditorTool(
                    World,
                    this,
                    kEditorIcon);

            Array.Resize(
                ref editorTools,
                editorTools.Length + 1);

            editorTools[editorTools.Length - 1] =
                m_EditorTool;

            m_EditorToolUISystem.tools = editorTools;

            TryLogInfo(
                "Area Bulldozer registered in the editor toolbar.");
        }

        private void SelectEditorTool()
        {
            if (!IsEditorMode ||
                m_EditorToolUISystem == null ||
                m_EditorTool == null ||
                m_EditorToolUISystem.activeTool == m_EditorTool)
            {
                return;
            }

            m_PreviousEditorTool =
                m_EditorToolUISystem.activeTool;

            m_EditorToolUISystem.activeTool =
                m_EditorTool;
        }

        private void RestorePreviousEditorTool()
        {
            if (!IsEditorMode ||
                m_EditorToolUISystem == null ||
                m_EditorToolUISystem.activeTool != m_EditorTool)
            {
                return;
            }

            IEditorTool previousTool =
                m_PreviousEditorTool;

            m_PreviousEditorTool = null;

            m_EditorToolUISystem.activeTool =
                previousTool;
        }

        private void DisposeEditorTool()
        {
            if (m_EditorToolUISystem?.tools != null &&
                m_EditorTool != null)
            {
                IEditorTool[] editorTools =
                    m_EditorToolUISystem.tools;

                int index =
                    Array.IndexOf(
                        editorTools,
                        m_EditorTool);

                if (index >= 0)
                {
                    IEditorTool[] remainingTools =
                        new IEditorTool[editorTools.Length - 1];

                    if (index > 0)
                    {
                        Array.Copy(
                            editorTools,
                            0,
                            remainingTools,
                            0,
                            index);
                    }

                    if (index < editorTools.Length - 1)
                    {
                        Array.Copy(
                            editorTools,
                            index + 1,
                            remainingTools,
                            index,
                            editorTools.Length - index - 1);
                    }

                    m_EditorToolUISystem.tools =
                        remainingTools;
                }
            }

            m_PreviousEditorTool = null;
            m_EditorTool = null;
            m_EditorToolUISystem = null;
        }
    }

    internal sealed class AreaBulldozerEditorTool : EditorTool
    {
        public AreaBulldozerEditorTool(
            Unity.Entities.World world,
            AreaBulldozerToolSystem toolSystem,
            string iconPath)
            : base(world)
        {
            id = toolSystem.toolID;
            icon = iconPath;
            panel = world.GetOrCreateSystemManaged<BulldozeToolPanel>();
            tool = toolSystem;
        }
    }
}
