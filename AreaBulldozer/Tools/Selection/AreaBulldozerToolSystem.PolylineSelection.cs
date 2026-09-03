using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const int kPolylineMaximumPoints = 15;
        private const float kPolylineMinimumSegmentLength = 0.5f;
        private const float kPolylineDoubleClickSeconds = 0.35f;
        private const float kPolylineDoubleClickDistance = 2.5f;

        private readonly List<float3> m_PolylinePoints =
            new();

        private readonly List<float3>
            m_LargeSelectionConfirmationPolylinePoints =
                new();

        private bool m_PolylineSelectionLocked;
        private float m_PolylineLastClickTime = -1000f;
        private float3 m_PolylineLastClickPosition;
        private int m_LargeSelectionConfirmationPolylineWidth;

        public int MaximumPolylinePoints =>
            kPolylineMaximumPoints;

        public int CurrentPolylinePointCount =>
            m_PolylinePoints.Count;

        private bool HasPolylineStart =>
            m_PolylinePoints.Count > 0;

        private bool IncludePolylineCursorPoint =>
            UsePolylineBrush &&
            HasValidPosition &&
            !m_PolylineSelectionLocked &&
            m_PolylinePoints.Count > 0 &&
            m_PolylinePoints.Count <
                kPolylineMaximumPoints;

        private int CurrentPolylineGeometryPointCount =>
            m_PolylinePoints.Count +
            (IncludePolylineCursorPoint ? 1 : 0);

        private float3 GetPolylineGeometryPoint(
            int index)
        {
            if (index < m_PolylinePoints.Count)
            {
                return m_PolylinePoints[index];
            }

            return CurrentPosition;
        }

        private float2 GetPolylineGeometryPoint2D(
            int index)
        {
            float3 point =
                GetPolylineGeometryPoint(index);

            return new float2(
                point.x,
                point.z);
        }

        private bool TryGetPolylineQueryCircle(
            out float2 center,
            out float radius)
        {
            center = float2.zero;
            radius = 0f;

            int pointCount =
                CurrentPolylineGeometryPointCount;

            if (pointCount == 0)
            {
                return false;
            }

            float2 first =
                GetPolylineGeometryPoint2D(0);

            float2 minimum = first;
            float2 maximum = first;

            for (int index = 1;
                 index < pointCount;
                 index++)
            {
                float2 point =
                    GetPolylineGeometryPoint2D(index);

                minimum =
                    math.min(minimum, point);

                maximum =
                    math.max(maximum, point);
            }

            center =
                (minimum + maximum) * 0.5f;

            float maximumDistance = 0f;

            for (int index = 0;
                 index < pointCount;
                 index++)
            {
                maximumDistance =
                    math.max(
                        maximumDistance,
                        math.distance(
                            center,
                            GetPolylineGeometryPoint2D(index)));
            }

            radius =
                maximumDistance +
                CurrentLineHalfWidth +
                2f;

            return true;
        }

        private void UpdatePolylineSelectionInput()
        {
            if (!UsePolylineBrush)
            {
                ResetPolylineSelection();
                return;
            }

            if (Keyboard.current != null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelLargeSelectionConfirmation();
                ResetPolylineSelection();
                ClearSelectionPreview();
                return;
            }

            if (m_RotateSquareHoldAction != null &&
                m_RotateSquareHoldAction.WasPressedThisFrame())
            {
                CancelLargeSelectionConfirmation();

                if (m_PolylineSelectionLocked)
                {
                    m_PolylineSelectionLocked = false;
                }

                if (m_PolylinePoints.Count > 0)
                {
                    m_PolylinePoints.RemoveAt(
                        m_PolylinePoints.Count - 1);
                }

                ResetPolylineDoubleClickState();
                InvalidateSelectionGeometry();

                if (m_PolylinePoints.Count == 0)
                {
                    ClearSelectionPreview();
                }

                return;
            }

            if (m_IsPointerOverUI ||
                !HasValidPosition ||
                m_ApplyAction == null ||
                !m_ApplyAction.WasPressedThisFrame())
            {
                return;
            }

            if (m_LargeSelectionConfirmationPending &&
                m_PolylineSelectionLocked)
            {
                ExecutePolylineDeletion();
                return;
            }

            if (m_PolylineSelectionLocked)
            {

                ExecutePolylineDeletion();
                return;
            }

            float currentTime =
                UnityEngine.Time.unscaledTime;

            bool isDoubleClick =
                m_PolylinePoints.Count >= 2 &&
                currentTime -
                    m_PolylineLastClickTime <=
                    kPolylineDoubleClickSeconds &&
                math.distancesq(
                    CurrentPosition,
                    m_PolylineLastClickPosition) <=
                kPolylineDoubleClickDistance *
                kPolylineDoubleClickDistance;

            if (isDoubleClick)
            {
                m_PolylineSelectionLocked = true;
                ResetPolylineDoubleClickState();
                InvalidateSelectionGeometry();
                ExecutePolylineDeletion();
                return;
            }

            if (m_PolylinePoints.Count <
                kPolylineMaximumPoints)
            {
                bool canAddPoint =
                    m_PolylinePoints.Count == 0 ||
                    math.distance(
                        m_PolylinePoints[
                            m_PolylinePoints.Count - 1],
                        CurrentPosition) >=
                    kPolylineMinimumSegmentLength;

                if (canAddPoint)
                {
                    m_PolylinePoints.Add(
                        CurrentPosition);
                }
            }

            m_PolylineLastClickTime =
                currentTime;

            m_PolylineLastClickPosition =
                CurrentPosition;

            CancelLargeSelectionConfirmation();
            InvalidateSelectionGeometry();
        }

        private void ExecutePolylineDeletion()
        {
            if (m_PolylinePoints.Count < 2)
            {
                return;
            }

            m_PolylineSelectionLocked = true;

            if (!TryGetPolylineQueryCircle(
                    out float2 selectionCenter,
                    out float _))
            {
                return;
            }

            float3 pointerPosition =
                CurrentPosition;

            CurrentPosition =
                new float3(
                    selectionCenter.x,
                    pointerPosition.y,
                    selectionCenter.y);

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
                return;
            }

            StartDeleteVisualFeedback();
            ResetPolylineSelection();
            ClearSelectionPreview();
        }

        private void ResetPolylineDoubleClickState()
        {
            m_PolylineLastClickTime = -1000f;
            m_PolylineLastClickPosition = float3.zero;
        }

        private void ResetPolylineSelection()
        {
            m_PolylinePoints.Clear();
            m_PolylineSelectionLocked = false;
            ResetPolylineDoubleClickState();
        }

        private bool PolylineConfirmationGeometryMatches()
        {
            if (!UsePolylineBrush ||
                !m_PolylineSelectionLocked ||
                CurrentLineWidth !=
                    m_LargeSelectionConfirmationPolylineWidth ||
                m_PolylinePoints.Count !=
                    m_LargeSelectionConfirmationPolylinePoints.Count)
            {
                return false;
            }

            for (int index = 0;
                 index < m_PolylinePoints.Count;
                 index++)
            {
                if (math.distancesq(
                        m_PolylinePoints[index],
                        m_LargeSelectionConfirmationPolylinePoints[index]) >
                    0.01f)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
