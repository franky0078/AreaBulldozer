using Colossal.Mathematics;
using Game;
using Game.Rendering;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerPolygonOverlaySystem :
        GameSystemBase
    {
        private OverlayRenderSystem m_OverlayRenderSystem;
        private AreaBulldozerToolSystem m_Tool;

        private readonly List<float3> m_PreviewPoints =
            new();

        protected override void OnCreate()
        {
            base.OnCreate();

            m_OverlayRenderSystem =
                World.GetOrCreateSystemManaged<
                    OverlayRenderSystem>();

            m_Tool =
                World.GetOrCreateSystemManaged<
                    AreaBulldozerToolSystem>();
        }

        protected override void OnUpdate()
        {
            if (m_Tool is null ||
                !m_Tool.IsToolActive ||
                !m_Tool.UseFreeAreaPolygon)
            {
                return;
            }

            m_Tool.CopyFreeAreaPolygonPreviewPoints(
                m_PreviewPoints);

            if (m_PreviewPoints.Count == 0)
            {
                return;
            }

            NativeArray<float3> points =
                new(
                    m_PreviewPoints.Count,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            const float verticalOffset = 0.24f;

            for (int index = 0;
                 index < m_PreviewPoints.Count;
                 index++)
            {
                points[index] =
                    m_PreviewPoints[index] +
                    new float3(
                        0f,
                        verticalOffset,
                        0f);
            }

            OverlayRenderSystem.Buffer overlayBuffer =
                m_OverlayRenderSystem.GetBuffer(
                    out JobHandle overlayDependencies);

            PolygonOverlayJob job =
                new()
                {
                    OverlayBuffer = overlayBuffer,
                    Points = points,
                    StoredPointCount =
                        m_Tool.CurrentFreeAreaPolygonPointCount,
                    InvalidPreview =
                        m_Tool.FreeAreaPolygonPreviewInvalid,
                    CloseCandidate =
                        m_Tool.FreeAreaPolygonCloseCandidate,
                    ConfirmationPending =
                        m_Tool.FreeAreaPolygonConfirmationPending,
                    Locked =
                        m_Tool.FreeAreaPolygonSelectionLocked
                };

            JobHandle jobHandle =
                job.Schedule(
                    JobHandle.CombineDependencies(
                        Dependency,
                        overlayDependencies));

            m_OverlayRenderSystem.AddBufferWriter(
                jobHandle);

            Dependency =
                jobHandle;
        }

        protected override void OnDestroy()
        {
            m_PreviewPoints.Clear();

            m_Tool = null;
            m_OverlayRenderSystem = null;

            base.OnDestroy();
        }

        private struct PolygonOverlayJob : IJob
        {
            public OverlayRenderSystem.Buffer
                OverlayBuffer;

            [DeallocateOnJobCompletion]
            public NativeArray<float3> Points;

            public int StoredPointCount;
            public bool InvalidPreview;
            public bool CloseCandidate;
            public bool ConfirmationPending;
            public bool Locked;

            public void Execute()
            {
                UnityEngine.Color normalColor =
                    new(
                        1f,
                        0.25f,
                        0.1f,
                        1f);

                UnityEngine.Color invalidColor =
                    new(
                        1f,
                        0.05f,
                        0.05f,
                        1f);

                UnityEngine.Color closeColor =
                    new(
                        0.2f,
                        0.95f,
                        0.35f,
                        1f);

                UnityEngine.Color confirmationColor =
                    new(
                        1f,
                        0.85f,
                        0.1f,
                        1f);

                UnityEngine.Color lineColor =
                    ConfirmationPending
                        ? confirmationColor
                        : InvalidPreview
                            ? invalidColor
                            : CloseCandidate
                                ? closeColor
                                : normalColor;

                const float lineWidth = 0.72f;
                const float markerDiameter = 1.45f;
                const float markerBorderWidth = 0.11f;

                if (Points.Length == 1)
                {
                    DrawMarker(
                        Points[0],
                        lineColor,
                        markerDiameter,
                        markerBorderWidth);

                    return;
                }

                for (int index = 0;
                     index + 1 < Points.Length;
                     index++)
                {
                    OverlayBuffer.DrawLine(
                        lineColor,
                        new Line3.Segment(
                            Points[index],
                            Points[index + 1]),
                        lineWidth);
                }

                if (StoredPointCount >= 3)
                {
                    int lastIndex;

                    if (Locked)
                    {
                        lastIndex =
                            math.min(
                                Points.Length - 1,
                                StoredPointCount - 1);
                    }
                    else
                    {
                        lastIndex =
                            Points.Length - 1;
                    }

                    OverlayBuffer.DrawLine(
                        lineColor,
                        new Line3.Segment(
                            Points[lastIndex],
                            Points[0]),
                        lineWidth);
                }

                int markerCount =
                    math.min(
                        StoredPointCount,
                        Points.Length);

                for (int index = 0;
                     index < markerCount;
                     index++)
                {
                    UnityEngine.Color markerColor =
                        index == 0 &&
                        CloseCandidate
                            ? closeColor
                            : lineColor;

                    DrawMarker(
                        Points[index],
                        markerColor,
                        markerDiameter,
                        markerBorderWidth);
                }
            }

            private void DrawMarker(
                float3 position,
                UnityEngine.Color color,
                float diameter,
                float borderWidth)
            {
                OverlayBuffer.DrawCircle(
                    color,
                    color,
                    borderWidth,
                    0f,
                    new float2(
                        0f,
                        1f),
                    position,
                    diameter);
            }
        }
    }
}
