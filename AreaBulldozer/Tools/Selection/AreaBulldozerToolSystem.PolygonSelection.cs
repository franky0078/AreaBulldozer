using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace AreaBulldozer.Tools
{
    internal static class FreeAreaPolygonModeState
    {
        public static bool Active { get; set; }
    }

    public partial class AreaBulldozerToolSystem
    {
        private const float kFreeAreaPolygonMinimumSegmentLength = 0.5f;
        private const float kFreeAreaPolygonDoubleClickSeconds = 0.35f;
        private const float kFreeAreaPolygonDoubleClickDistance = 2.5f;
        private const float kFreeAreaPolygonCloseDistance = 2.5f;
        private const float kFreeAreaPolygonMinimumAreaTwice = 0.25f;

        private bool m_FreeAreaPolygonPreviewInvalid;
        private bool m_FreeAreaPolygonCloseCandidate;

        public bool UseFreeAreaPolygon =>
            FreeAreaPolygonModeState.Active &&
            CurrentSelectionShape ==
                AreaBulldozerSelectionShape.Polyline;

        public int CurrentFreeAreaPolygonPointCount =>
            UseFreeAreaPolygon
                ? m_PolylinePoints.Count
                : 0;

        internal bool FreeAreaPolygonPreviewInvalid =>
            m_FreeAreaPolygonPreviewInvalid;

        internal bool FreeAreaPolygonCloseCandidate =>
            m_FreeAreaPolygonCloseCandidate;

        internal bool FreeAreaPolygonConfirmationPending =>
            m_LargeSelectionConfirmationPending;

        internal bool FreeAreaPolygonSelectionLocked =>
            m_PolylineSelectionLocked;

        internal void ResetFreeAreaPolygonRuntimeState()
        {
            m_FreeAreaPolygonPreviewInvalid = false;
            m_FreeAreaPolygonCloseCandidate = false;

            ResetPolylineSelection();
        }

        private void UpdateFreeAreaPolygonInput()
        {
            if (!UseFreeAreaPolygon)
            {
                return;
            }

            UpdateFreeAreaPolygonPreviewState();

            if (Keyboard.current is not null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelLargeSelectionConfirmation();
                ResetFreeAreaPolygonRuntimeState();
                ClearSelectionPreview();

                return;
            }

            if (m_RotateSquareHoldAction is not null &&
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

                    MarkPolylineGeometryDirty();
                }

                m_FreeAreaPolygonPreviewInvalid = false;
                m_FreeAreaPolygonCloseCandidate = false;

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
                m_ApplyAction is null ||
                !m_ApplyAction.WasPressedThisFrame())
            {
                return;
            }

            if (m_LargeSelectionConfirmationPending &&
                m_PolylineSelectionLocked)
            {
                ExecuteFreeAreaPolygonDeletion();
                return;
            }

            if (m_PolylineSelectionLocked)
            {
                ExecuteFreeAreaPolygonDeletion();
                return;
            }

            if (m_FreeAreaPolygonCloseCandidate)
            {
                if (CanCloseFreeAreaPolygon())
                {
                    LockAndExecuteFreeAreaPolygon();
                }
                else
                {
                    m_FreeAreaPolygonPreviewInvalid = true;
                }

                return;
            }

            float currentTime =
                UnityEngine.Time.unscaledTime;

            bool isDoubleClick =
                m_PolylinePoints.Count >= 3 &&
                currentTime -
                    m_PolylineLastClickTime <=
                    kFreeAreaPolygonDoubleClickSeconds &&
                math.distancesq(
                    CurrentPosition,
                    m_PolylineLastClickPosition) <=
                kFreeAreaPolygonDoubleClickDistance *
                kFreeAreaPolygonDoubleClickDistance;

            if (isDoubleClick)
            {
                if (CanCloseFreeAreaPolygon())
                {
                    LockAndExecuteFreeAreaPolygon();
                }
                else
                {
                    m_FreeAreaPolygonPreviewInvalid = true;

                    ResetPolylineDoubleClickState();
                }

                return;
            }

            bool canAddPoint =
                m_PolylinePoints.Count == 0 ||
                math.distance(
                    m_PolylinePoints[^1],
                    CurrentPosition) >=
                kFreeAreaPolygonMinimumSegmentLength;

            if (canAddPoint &&
                CanAppendFreeAreaPolygonPoint(
                    CurrentPosition))
            {
                // No hard maximum: polygon points are unlimited.
                m_PolylinePoints.Add(
                    CurrentPosition);

                MarkPolylineGeometryDirty();

                m_FreeAreaPolygonPreviewInvalid = false;
            }
            else if (canAddPoint)
            {
                m_FreeAreaPolygonPreviewInvalid = true;

                SafeLogInfo(
                    "Area Bulldozer: polygon point ignored because " +
                    "the new edge would intersect the polygon or " +
                    "would close it before three vertices exist.");
            }

            m_PolylineLastClickTime =
                currentTime;

            m_PolylineLastClickPosition =
                CurrentPosition;

            CancelLargeSelectionConfirmation();
            InvalidateSelectionGeometry();
            UpdateFreeAreaPolygonPreviewState();
        }

        private void LockAndExecuteFreeAreaPolygon()
        {
            m_PolylineSelectionLocked = true;

            m_FreeAreaPolygonPreviewInvalid = false;
            m_FreeAreaPolygonCloseCandidate = false;

            ResetPolylineDoubleClickState();
            MarkPolylineGeometryDirty();
            InvalidateSelectionGeometry();

            ExecuteFreeAreaPolygonDeletion();
        }

        private void ExecuteFreeAreaPolygonDeletion()
        {
            if (m_PolylinePoints.Count < 3 ||
                !CanCloseFreeAreaPolygon())
            {
                m_FreeAreaPolygonPreviewInvalid = true;

                return;
            }

            m_PolylineSelectionLocked = true;

            MarkPolylineGeometryDirty();

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

            ResetFreeAreaPolygonRuntimeState();
            ClearSelectionPreview();
        }

        private void UpdateFreeAreaPolygonPreviewState()
        {
            m_FreeAreaPolygonPreviewInvalid = false;
            m_FreeAreaPolygonCloseCandidate = false;

            if (!UseFreeAreaPolygon ||
                !HasValidPosition ||
                m_PolylineSelectionLocked ||
                m_PolylinePoints.Count == 0)
            {
                return;
            }

            float2 cursor =
                new(
                    CurrentPosition.x,
                    CurrentPosition.z);

            float2 first =
                GetFreeAreaPolygonStoredPoint2D(0);

            if (m_PolylinePoints.Count >= 3 &&
                math.distance(
                    cursor,
                    first) <=
                kFreeAreaPolygonCloseDistance)
            {
                m_FreeAreaPolygonCloseCandidate = true;

                m_FreeAreaPolygonPreviewInvalid =
                    !CanCloseFreeAreaPolygon();

                return;
            }

            float2 last =
                GetFreeAreaPolygonStoredPoint2D(
                    m_PolylinePoints.Count - 1);

            if (math.distance(
                    cursor,
                    last) <
                kFreeAreaPolygonMinimumSegmentLength)
            {
                return;
            }

            m_FreeAreaPolygonPreviewInvalid =
                !CanAppendFreeAreaPolygonPoint(
                    CurrentPosition);
        }

        private bool CanAppendFreeAreaPolygonPoint(
            float3 point)
        {
            int pointCount =
                m_PolylinePoints.Count;

            if (pointCount == 0)
            {
                return true;
            }

            float2 start =
                GetFreeAreaPolygonStoredPoint2D(
                    pointCount - 1);

            float2 end =
                new(
                    point.x,
                    point.z);

            if (math.distance(
                    start,
                    end) <
                kFreeAreaPolygonMinimumSegmentLength)
            {
                return false;
            }

            if (pointCount < 3 &&
                pointCount >= 2 &&
                math.distance(
                    end,
                    GetFreeAreaPolygonStoredPoint2D(0)) <=
                kFreeAreaPolygonCloseDistance)
            {
                return false;
            }

            for (int index = 0;
                 index + 1 < pointCount - 1;
                 index++)
            {
                float2 edgeStart =
                    GetFreeAreaPolygonStoredPoint2D(
                        index);

                float2 edgeEnd =
                    GetFreeAreaPolygonStoredPoint2D(
                        index + 1);

                if (SelectionGeometry.SegmentsIntersect(
                        start,
                        end,
                        edgeStart,
                        edgeEnd))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanCloseFreeAreaPolygon()
        {
            int pointCount =
                m_PolylinePoints.Count;

            if (pointCount < 3)
            {
                return false;
            }

            float2 first =
                GetFreeAreaPolygonStoredPoint2D(0);

            float2 last =
                GetFreeAreaPolygonStoredPoint2D(
                    pointCount - 1);

            if (math.distance(
                    first,
                    last) <
                kFreeAreaPolygonMinimumSegmentLength)
            {
                return false;
            }

            for (int index = 1;
                 index + 1 < pointCount - 1;
                 index++)
            {
                float2 edgeStart =
                    GetFreeAreaPolygonStoredPoint2D(
                        index);

                float2 edgeEnd =
                    GetFreeAreaPolygonStoredPoint2D(
                        index + 1);

                if (SelectionGeometry.SegmentsIntersect(
                        last,
                        first,
                        edgeStart,
                        edgeEnd))
                {
                    return false;
                }
            }

            float twiceArea = 0f;

            for (int index = 0;
                 index < pointCount;
                 index++)
            {
                int nextIndex =
                    index + 1 < pointCount
                        ? index + 1
                        : 0;

                float2 current =
                    GetFreeAreaPolygonStoredPoint2D(
                        index);

                float2 next =
                    GetFreeAreaPolygonStoredPoint2D(
                        nextIndex);

                twiceArea +=
                    current.x * next.y -
                    current.y * next.x;
            }

            return math.abs(twiceArea) >=
                   kFreeAreaPolygonMinimumAreaTwice;
        }

        private float2 GetFreeAreaPolygonStoredPoint2D(
            int index)
        {
            float3 point =
                m_PolylinePoints[index];

            return new float2(
                point.x,
                point.z);
        }

        internal int GetFreeAreaPolygonSelectionVertexCount()
        {
            if (!UseFreeAreaPolygon)
            {
                return 0;
            }

            int count =
                m_PolylinePoints.Count;

            if (!m_PolylineSelectionLocked &&
                HasValidPosition &&
                count > 0 &&
                !m_FreeAreaPolygonPreviewInvalid)
            {
                count++;
            }

            return count;
        }

        internal float2 GetFreeAreaPolygonSelectionVertex2D(
            int index)
        {
            if (index < m_PolylinePoints.Count)
            {
                return
                    GetFreeAreaPolygonStoredPoint2D(
                        index);
            }

            return new float2(
                CurrentPosition.x,
                CurrentPosition.z);
        }

        internal void CopyFreeAreaPolygonPreviewPoints(
            List<float3> target)
        {
            target.Clear();

            if (!UseFreeAreaPolygon ||
                !HasValidPosition)
            {
                return;
            }

            foreach (float3 point in
                     m_PolylinePoints)
            {
                target.Add(point);
            }

            if (!m_PolylineSelectionLocked)
            {
                target.Add(
                    CurrentPosition);
            }
        }
    }
}
