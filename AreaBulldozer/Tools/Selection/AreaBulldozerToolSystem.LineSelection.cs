using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private bool m_LineSelectionHasStart;
        private bool m_LineSelectionEndLocked;
        private float3 m_LineStartPosition;
        private float3 m_LineEndPosition;

        private float3 CurrentLineEndPosition =>
            m_LineSelectionEndLocked
                ? m_LineEndPosition
                : CurrentPosition;

        private void UpdateLineSelectionInput()
        {
            if (!UseLineBrush)
            {
                ResetLineSelection();
                return;
            }

            if (m_IsPointerOverUI ||
                !HasValidPosition ||
                m_ApplyAction == null)
            {
                return;
            }

            if (m_RotateSquareHoldAction != null &&
                m_RotateSquareHoldAction.WasPressedThisFrame())
            {
                CancelLargeSelectionConfirmation();
                ResetLineSelection();
                ClearSelectionPreview();
                return;
            }

            if (!m_ApplyAction.WasPressedThisFrame())
            {
                return;
            }

            if (!m_LineSelectionHasStart)
            {
                m_LineStartPosition =
                    CurrentPosition;

                m_LineEndPosition =
                    CurrentPosition;

                m_LineSelectionHasStart = true;
                m_LineSelectionEndLocked = false;

                CancelLargeSelectionConfirmation();
                InvalidateSelectionGeometry();
                return;
            }

            if (!m_LineSelectionEndLocked)
            {
                m_LineEndPosition =
                    CurrentPosition;

                float2 start =
                    new(
                        m_LineStartPosition.x,
                        m_LineStartPosition.z);

                float2 end =
                    new(
                        m_LineEndPosition.x,
                        m_LineEndPosition.z);

                if (math.distance(start, end) < 0.5f)
                {
                    // Treat a nearly zero-length line as a new start point.
                    m_LineStartPosition =
                        CurrentPosition;

                    m_LineEndPosition =
                        CurrentPosition;

                    InvalidateSelectionGeometry();
                    return;
                }

                m_LineSelectionEndLocked = true;
            }

            ExecuteLineDeletion();
        }

        private void ExecuteLineDeletion()
        {
            float3 pointerPosition =
                CurrentPosition;

            float3 lineEnd =
                CurrentLineEndPosition;

            CurrentPosition =
                new float3(
                    (m_LineStartPosition.x + lineEnd.x) * 0.5f,
                    (m_LineStartPosition.y + lineEnd.y) * 0.5f,
                    (m_LineStartPosition.z + lineEnd.z) * 0.5f);

            try
            {
                DeleteSelectedObjects();
            }
            finally
            {
                CurrentPosition =
                    pointerPosition;
            }

            if (m_LargeSelectionConfirmationPending)
            {
                m_LineSelectionEndLocked = true;
                return;
            }

            StartDeleteVisualFeedback();
            ResetLineSelection();
            ClearSelectionPreview();
        }

        private void ResetLineSelection()
        {
            m_LineSelectionHasStart = false;
            m_LineSelectionEndLocked = false;
            m_LineStartPosition = float3.zero;
            m_LineEndPosition = float3.zero;
        }
    }
}
