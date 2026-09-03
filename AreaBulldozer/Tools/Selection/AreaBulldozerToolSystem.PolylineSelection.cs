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

        // Cached, sampled corridor center line. 
        private readonly List<float3> m_PolylineGeometryPoints =
            new();

        private int m_PolylineGeometryVersion;
        private int m_PolylineGeometryCacheVersion = -1;
        private bool m_PolylineGeometryCacheIncludesCursor;
        private float3 m_PolylineGeometryCacheCursor;
        private bool m_PolylineGeometryCacheCurved;
        private int m_PolylineGeometryCacheRounding = -1;

        private readonly List<float3>
            m_LargeSelectionConfirmationPolylinePoints =
                new();

        private bool m_PolylineSelectionLocked;
        private float m_PolylineLastClickTime = -1000f;
        private float3 m_PolylineLastClickPosition;
        private int m_LargeSelectionConfirmationPolylineWidth;
        private bool m_LargeSelectionConfirmationPolylineCurved;
        private int m_LargeSelectionConfirmationPolylineRounding;

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

        private int CurrentPolylineGeometryPointCount
        {
            get
            {
                EnsurePolylineGeometryCache();
                return m_PolylineGeometryPoints.Count;
            }
        }

        private float3 GetPolylineGeometryPoint(
            int index)
        {
            EnsurePolylineGeometryCache();
            return m_PolylineGeometryPoints[index];
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

        private void EnsurePolylineGeometryCache()
        {
            bool includeCursor =
                IncludePolylineCursorPoint;

            bool curved =
                UseCurvedPolyline;

            int rounding =
                CurrentPolylineRounding;

            float3 cursor =
                includeCursor
                    ? CurrentPosition
                    : float3.zero;

            bool cursorMatches =
                !includeCursor ||
                (m_PolylineGeometryCacheIncludesCursor &&
                 math.distancesq(
                     cursor,
                     m_PolylineGeometryCacheCursor) <= 0.000001f);

            if (m_PolylineGeometryCacheVersion ==
                    m_PolylineGeometryVersion &&
                m_PolylineGeometryCacheIncludesCursor ==
                    includeCursor &&
                cursorMatches &&
                m_PolylineGeometryCacheCurved == curved &&
                m_PolylineGeometryCacheRounding == rounding)
            {
                return;
            }

            RebuildPolylineGeometryCache(
                includeCursor,
                curved,
                rounding);

            m_PolylineGeometryCacheVersion =
                m_PolylineGeometryVersion;

            m_PolylineGeometryCacheIncludesCursor =
                includeCursor;

            m_PolylineGeometryCacheCursor =
                cursor;

            m_PolylineGeometryCacheCurved =
                curved;

            m_PolylineGeometryCacheRounding =
                rounding;
        }

        private void RebuildPolylineGeometryCache(
            bool includeCursor,
            bool curved,
            int rounding)
        {
            m_PolylineGeometryPoints.Clear();

            int sourcePointCount =
                m_PolylinePoints.Count +
                (includeCursor ? 1 : 0);

            if (sourcePointCount == 0)
            {
                return;
            }

            if (!curved ||
                sourcePointCount < 3)
            {
                for (int index = 0;
                     index < sourcePointCount;
                     index++)
                {
                    AppendPolylineGeometryPoint(
                        GetPolylineSourcePoint(
                            index,
                            includeCursor));
                }

                return;
            }

            AppendPolylineGeometryPoint(
                GetPolylineSourcePoint(
                    0,
                    includeCursor));

            float roundingFraction =
                math.clamp(rounding, 10, 80) /
                200f;

            float targetSpacing =
                math.clamp(
                    CurrentLineWidth * 0.25f,
                    1f,
                    4f);

            for (int index = 1;
                 index + 1 < sourcePointCount;
                 index++)
            {
                float3 previous =
                    GetPolylineSourcePoint(
                        index - 1,
                        includeCursor);

                float3 corner =
                    GetPolylineSourcePoint(
                        index,
                        includeCursor);

                float3 next =
                    GetPolylineSourcePoint(
                        index + 1,
                        includeCursor);

                float previousLength =
                    math.distance(
                        new float2(previous.x, previous.z),
                        new float2(corner.x, corner.z));

                float nextLength =
                    math.distance(
                        new float2(corner.x, corner.z),
                        new float2(next.x, next.z));

                if (previousLength <
                        kPolylineMinimumSegmentLength ||
                    nextLength <
                        kPolylineMinimumSegmentLength)
                {
                    AppendPolylineGeometryPoint(corner);
                    continue;
                }

                float cutDistance =
                    math.min(
                        previousLength,
                        nextLength) *
                    roundingFraction;

                float3 curveStart =
                    math.lerp(
                        corner,
                        previous,
                        cutDistance /
                        previousLength);

                float3 curveEnd =
                    math.lerp(
                        corner,
                        next,
                        cutDistance /
                        nextLength);

                AppendPolylineGeometryPoint(
                    curveStart);

                float approximateCurveLength =
                    math.distance(
                        new float2(curveStart.x, curveStart.z),
                        new float2(corner.x, corner.z)) +
                    math.distance(
                        new float2(corner.x, corner.z),
                        new float2(curveEnd.x, curveEnd.z));

                int sampleCount =
                    math.clamp(
                        (int)math.ceil(
                            approximateCurveLength /
                            targetSpacing),
                        3,
                        32);

                for (int sample = 1;
                     sample <= sampleCount;
                     sample++)
                {
                    float t =
                        sample /
                        (float)sampleCount;

                    float inverseT =
                        1f - t;

                    float3 point =
                        inverseT * inverseT * curveStart +
                        2f * inverseT * t * corner +
                        t * t * curveEnd;

                    AppendPolylineGeometryPoint(
                        point);
                }
            }

            AppendPolylineGeometryPoint(
                GetPolylineSourcePoint(
                    sourcePointCount - 1,
                    includeCursor));
        }

        private float3 GetPolylineSourcePoint(
            int index,
            bool includeCursor)
        {
            if (index < m_PolylinePoints.Count)
            {
                return m_PolylinePoints[index];
            }

            return includeCursor
                ? CurrentPosition
                : m_PolylinePoints[
                    m_PolylinePoints.Count - 1];
        }

        private void AppendPolylineGeometryPoint(
            float3 point)
        {
            if (m_PolylineGeometryPoints.Count > 0 &&
                math.distancesq(
                    m_PolylineGeometryPoints[
                        m_PolylineGeometryPoints.Count - 1],
                    point) <= 0.000001f)
            {
                return;
            }

            m_PolylineGeometryPoints.Add(point);
        }

        private void MarkPolylineGeometryDirty()
        {
            m_PolylineGeometryVersion++;
            m_PolylineGeometryCacheVersion = -1;
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

                    MarkPolylineGeometryDirty();
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
                MarkPolylineGeometryDirty();
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

                    MarkPolylineGeometryDirty();
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
            m_PolylineGeometryPoints.Clear();
            m_PolylineSelectionLocked = false;
            MarkPolylineGeometryDirty();
            ResetPolylineDoubleClickState();
        }

        private bool PolylineConfirmationGeometryMatches()
        {
            if (!UsePolylineBrush ||
                !m_PolylineSelectionLocked ||
                CurrentLineWidth !=
                    m_LargeSelectionConfirmationPolylineWidth ||
                UseCurvedPolyline !=
                    m_LargeSelectionConfirmationPolylineCurved ||
                CurrentPolylineRounding !=
                    m_LargeSelectionConfirmationPolylineRounding ||
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
