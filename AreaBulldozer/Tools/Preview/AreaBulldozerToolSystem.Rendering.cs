using Colossal.Mathematics;
using Game.Rendering;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        // ------------------------------------------------------------
        // Auswahlform
        // ------------------------------------------------------------

        private JobHandle DrawToolShape(
            JobHandle inputDeps)
        {
            OverlayRenderSystem.Buffer overlayBuffer =
                m_OverlayRenderSystem.GetBuffer(
                    out JobHandle overlayDependencies);

            NativeArray<float3> surfaceOutlineSegmentPoints =
                BuildSurfaceOutlineSegmentPoints();

            NativeArray<float3> squareBrushCorners =
                BuildSquareBrushPreviewCorners();

            float selectionLineWidth =
                math.clamp(
                    Mod.Settings?.SelectionLineThickness ?? 65,
                    25,
                    150) /
                100f;

            ToolRadiusJob radiusJob =
                new()
                {
                    OverlayBuffer = overlayBuffer,
                    Position = CurrentPosition,
                    Radius = CurrentRadius,
                    SelectionLineWidth =
                        selectionLineWidth,
                    UseSquareBrush =
                        UseSquareBrush,
                    ConfirmationPending =
                        m_LargeSelectionConfirmationPending,
                    SurfaceOutlineSegmentPoints =
                        surfaceOutlineSegmentPoints,
                    SquareBrushCorners =
                        squareBrushCorners
                };

            JobHandle jobHandle =
                radiusJob.Schedule(
                    JobHandle.CombineDependencies(
                        inputDeps,
                        overlayDependencies));

            m_OverlayRenderSystem.AddBufferWriter(
                jobHandle);

            return jobHandle;
        }

        // ------------------------------------------------------------
        // Eckpunkte der quadratischen Auswahl
        // ------------------------------------------------------------

        private NativeArray<float3>
            BuildSquareBrushPreviewCorners()
        {
            if (!UseSquareBrush ||
                !HasValidPosition)
            {
                return new NativeArray<float3>(
                    0,
                    Allocator.TempJob);
            }

            const float verticalOffset = 0.12f;

            float halfSize =
                CurrentRadius;

            float3 center =
                CurrentPosition;

            float2[] localCorners =
            {
                new(-halfSize, -halfSize),
                new( halfSize, -halfSize),
                new( halfSize,  halfSize),
                new(-halfSize,  halfSize)
            };

            NativeArray<float3> corners =
                new(
                    4,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            for (int index = 0;
                 index < localCorners.Length;
                 index++)
            {
                float2 rotatedOffset =
                    RotateSquareLocalToWorld(
                        localCorners[index],
                        SquareRotationRadians);

                corners[index] =
                    new float3(
                        center.x + rotatedOffset.x,
                        center.y + verticalOffset,
                        center.z + rotatedOffset.y);
            }

            return corners;
        }

        // ------------------------------------------------------------
        // Liniensegmente für die Umrandung ausgewählter Flächen..
        // ------------------------------------------------------------

        private NativeArray<float3>
            BuildSurfaceOutlineSegmentPoints()
        {
            const int maximumOutlineSegments = 12288;
            const float verticalOffset = 0.18f;

            List<float3> segmentPoints =
                new();

            if (m_HighlightedEntities is not null)
            {
                foreach (Entity entity in
                         m_HighlightedEntities)
                {
                    if (segmentPoints.Count >=
                        maximumOutlineSegments * 2)
                    {
                        break;
                    }

                    if (entity == Entity.Null ||
                        !EntityManager.Exists(entity) ||
                        !EntityManager.HasComponent<Game.Areas.Area>(
                            entity) ||
                        !EntityManager.HasBuffer<Game.Areas.Node>(
                            entity))
                    {
                        continue;
                    }

                    DynamicBuffer<Game.Areas.Node> nodes =
                        EntityManager.GetBuffer<Game.Areas.Node>(
                            entity,
                            true);

                    if (nodes.Length < 3)
                    {
                        continue;
                    }

                    for (int index = 0;
                         index < nodes.Length &&
                         segmentPoints.Count <
                             maximumOutlineSegments * 2;
                         index++)
                    {
                        int nextIndex =
                            index + 1 < nodes.Length
                                ? index + 1
                                : 0;

                        float3 start =
                            nodes[index].m_Position;

                        float3 end =
                            nodes[nextIndex].m_Position;

                        if (!math.all(math.isfinite(start)) ||
                            !math.all(math.isfinite(end)))
                        {
                            continue;
                        }

                        start.y += verticalOffset;
                        end.y += verticalOffset;

                        segmentPoints.Add(start);
                        segmentPoints.Add(end);
                    }
                }
            }

            NativeArray<float3> result =
                new(
                    segmentPoints.Count,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            for (int index = 0;
                 index < segmentPoints.Count;
                 index++)
            {
                result[index] =
                    segmentPoints[index];
            }

            return result;
        }

        // ------------------------------------------------------------
        // Verlängert ein Liniensegment an beiden Enden.
        // ------------------------------------------------------------

        private static Line3.Segment CreateExtendedSegment(
            float3 start,
            float3 end,
            float extension)
        {
            float3 direction =
                math.normalizesafe(
                    end - start);

            return new Line3.Segment(
                start - direction * extension,
                end + direction * extension);
        }

        // ------------------------------------------------------------
        // Rendering-Job
        // ------------------------------------------------------------

        private struct ToolRadiusJob : IJob
        {
            public OverlayRenderSystem.Buffer OverlayBuffer;

            public float3 Position;
            public float Radius;
            public float SelectionLineWidth;

            public bool UseSquareBrush;
            public bool ConfirmationPending;

            [DeallocateOnJobCompletion]
            public NativeArray<float3>
                SurfaceOutlineSegmentPoints;

            [DeallocateOnJobCompletion]
            public NativeArray<float3>
                SquareBrushCorners;

            public void Execute()
            {
                float radius =
                    math.max(
                        5f,
                        Radius);

                float lineWidth =
                    math.clamp(
                        SelectionLineWidth,
                        0.25f,
                        1.5f);

                UnityEngine.Color selectionColor =
                    ConfirmationPending
                        ? new UnityEngine.Color(
                            1f,
                            0.85f,
                            0.1f,
                            1f)
                        : new UnityEngine.Color(
                            1f,
                            0.25f,
                            0.1f,
                            1f);

                UnityEngine.Color surfaceOutlineColor =
                    new(
                        0.08f,
                        0.86f,
                        1f,
                        1f);

                DrawSurfaceOutlines(
                    surfaceOutlineColor);

                if (UseSquareBrush)
                {
                    DrawSquare(
                        selectionColor,
                        lineWidth);

                    return;
                }

                OverlayBuffer.DrawCircle(
                    selectionColor,
                    default,
                    lineWidth,
                    0f,
                    new float2(
                        0f,
                        1f),
                    Position,
                    radius * 2f);
            }

            // --------------------------------------------------------
            // Umrandungen ausgewählter Flächen
            // --------------------------------------------------------

            private void DrawSurfaceOutlines(
                UnityEngine.Color outlineColor)
            {
                const float surfaceLineWidth = 0.45f;
                const float cornerDiameter = 0.58f;
                const float cornerBorderWidth = 0.05f;

                for (int index = 0;
                     index + 1 <
                         SurfaceOutlineSegmentPoints.Length;
                     index += 2)
                {
                    float3 start =
                        SurfaceOutlineSegmentPoints[index];

                    float3 end =
                        SurfaceOutlineSegmentPoints[index + 1];

                    OverlayBuffer.DrawLine(
                        outlineColor,
                        new Line3.Segment(
                            start,
                            end),
                        surfaceLineWidth);

                    OverlayBuffer.DrawCircle(
                        outlineColor,
                        outlineColor,
                        cornerBorderWidth,
                        0f,
                        new float2(
                            0f,
                            1f),
                        start,
                        cornerDiameter);
                }
            }

            // --------------------------------------------------------
            // Quadrat aus vier verlängerten Linien
            // --------------------------------------------------------

            private void DrawSquare(
                UnityEngine.Color selectionColor,
                float lineWidth)
            {
                if (SquareBrushCorners.Length != 4)
                {
                    return;
                }

                float cornerOverlap =
                    math.clamp(
                        lineWidth * 0.5f,
                        0.15f,
                        1f);

                OverlayBuffer.DrawLine(
                    selectionColor,
                    CreateExtendedSegment(
                        SquareBrushCorners[0],
                        SquareBrushCorners[1],
                        cornerOverlap),
                    lineWidth);

                OverlayBuffer.DrawLine(
                    selectionColor,
                    CreateExtendedSegment(
                        SquareBrushCorners[1],
                        SquareBrushCorners[2],
                        cornerOverlap),
                    lineWidth);

                OverlayBuffer.DrawLine(
                    selectionColor,
                    CreateExtendedSegment(
                        SquareBrushCorners[2],
                        SquareBrushCorners[3],
                        cornerOverlap),
                    lineWidth);

                OverlayBuffer.DrawLine(
                    selectionColor,
                    CreateExtendedSegment(
                        SquareBrushCorners[3],
                        SquareBrushCorners[0],
                        cornerOverlap),
                    lineWidth);
            }
        }
    }
}
