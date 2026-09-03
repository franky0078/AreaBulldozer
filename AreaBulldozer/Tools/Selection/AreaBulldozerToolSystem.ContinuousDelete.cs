using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // Update interval for continuous deletion (seconds)
        private const float kContinuousDeleteUpdateInterval = 0.05f;

        // Green visual feedback duration
        private const float kDeleteVisualFeedbackDuration = 0.20f;

        // Required brush movement before the next continuous delete
        private const float kContinuousDeleteMoveFactor = 0.15f;
        private const float kContinuousDeleteMinMoveThreshold = 1.5f;
        private const float kContinuousDeleteMaxMoveThreshold = 8f;

        private bool m_ContinuousDeleteActive;
        private float3 m_LastContinuousDeletePosition;
        private float m_NextContinuousDeleteTime;
        private float m_DeleteVisualFeedbackUntil;

        private float CurrentContinuousDeleteMoveThreshold =>
            math.clamp(
                CurrentRadius * kContinuousDeleteMoveFactor,
                kContinuousDeleteMinMoveThreshold,
                kContinuousDeleteMaxMoveThreshold);

        private bool IsDeleteVisualFeedbackActive
        {
            get
            {
                if (m_LargeSelectionConfirmationPending ||
                    m_IsPointerOverUI)
                {
                    return false;
                }

                bool held =
                    m_ApplyAction != null &&
                    m_ApplyAction.ReadValue<float>() >= 0.5f;

                if (m_ContinuousDeleteActive && held)
                {
                    return true;
                }

                return UnityEngine.Time.unscaledTime <
                    m_DeleteVisualFeedbackUntil;
            }
        }

        private void UpdateContinuousDeleteInput()
        {
            // The multi-point line 
            if (UsePolylineBrush)
            {
                ResetContinuousDeleteState();
                UpdatePolylineSelectionInput();
                return;
            }

            if (m_ApplyAction == null)
            {
                ResetContinuousDeleteState();
                return;
            }

            bool pressed =
                m_ApplyAction.WasPressedThisFrame();

            bool released =
                m_ApplyAction.WasReleasedThisFrame();

            bool held =
                m_ApplyAction.ReadValue<float>() >= 0.5f;

            if (m_IsPointerOverUI)
            {
                ResetContinuousDeleteState();
                return;
            }

            if (released || !held)
            {
                ResetContinuousDeleteState();
                return;
            }

            if (pressed)
            {
                DeleteSelectedObjects();

                m_LastContinuousDeletePosition =
                    CurrentPosition;

                m_NextContinuousDeleteTime =
                    UnityEngine.Time.unscaledTime +
                    kContinuousDeleteUpdateInterval;

                if (!m_LargeSelectionConfirmationPending)
                {
                    StartDeleteVisualFeedback();
                }

                m_ContinuousDeleteActive =
                    !m_LargeSelectionConfirmationPending;

                return;
            }

            if (!m_ContinuousDeleteActive ||
                m_LargeSelectionConfirmationPending)
            {
                return;
            }

            float2 currentPosition =
                new float2(
                    CurrentPosition.x,
                    CurrentPosition.z);

            float2 lastDeletePosition =
                new float2(
                    m_LastContinuousDeletePosition.x,
                    m_LastContinuousDeletePosition.z);

            float movedDistance =
                math.distance(
                    currentPosition,
                    lastDeletePosition);

            if (movedDistance <
                CurrentContinuousDeleteMoveThreshold)
            {
                return;
            }

            if (UnityEngine.Time.unscaledTime <
                m_NextContinuousDeleteTime)
            {
                return;
            }

            DeleteSelectedObjects();

            m_LastContinuousDeletePosition =
                CurrentPosition;

            m_NextContinuousDeleteTime =
                UnityEngine.Time.unscaledTime +
                kContinuousDeleteUpdateInterval;

            if (!m_LargeSelectionConfirmationPending)
            {
                StartDeleteVisualFeedback();
            }

            if (m_LargeSelectionConfirmationPending)
            {
                m_ContinuousDeleteActive = false;
            }
        }

        private void StartDeleteVisualFeedback()
        {
            m_DeleteVisualFeedbackUntil =
                UnityEngine.Time.unscaledTime +
                kDeleteVisualFeedbackDuration;
        }

        private void ResetContinuousDeleteState()
        {
            FlushContinuousDeleteLog();

            m_ContinuousDeleteActive = false;

            m_LastContinuousDeletePosition =
                CurrentPosition;

            m_NextContinuousDeleteTime = 0f;
        }
    }
}
