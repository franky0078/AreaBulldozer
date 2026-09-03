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

            NativeArray<float3> triangleBrushCorners =
                BuildTriangleBrushPreviewCorners();

            NativeArray<float3> polylineBrushPoints =
                BuildPolylineBrushPreviewPoints();

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
                    LineHalfWidth = CurrentLineHalfWidth,
                    SelectionLineWidth =
                        selectionLineWidth,
                    SelectionShape =
                        (int)CurrentSelectionShape,
                    ConfirmationPending =
                        m_LargeSelectionConfirmationPending,
                    DeleteActive =
                        IsDeleteVisualFeedbackActive,
                    SurfaceOutlineSegmentPoints =
                        surfaceOutlineSegmentPoints,
                    SquareBrushCorners =
                        squareBrushCorners,
                    TriangleBrushCorners =
                        triangleBrushCorners,
                    PolylineBrushPoints =
                        polylineBrushPoints
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
                    SelectionGeometry.RotateLocalToWorld(
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

        private NativeArray<float3>
            BuildTriangleBrushPreviewCorners()
        {
            if (!UseTriangleBrush ||
                !HasValidPosition)
            {
                return new NativeArray<float3>(
                    0,
                    Allocator.TempJob);
            }

            const float verticalOffset = 0.12f;

            float2 center =
                new(
                    CurrentPosition.x,
                    CurrentPosition.z);

            SelectionGeometry.GetEquilateralTriangleCorners(
                center,
                CurrentRadius,
                SquareRotationRadians,
                out float2 a,
                out float2 b,
                out float2 c);

            NativeArray<float3> corners =
                new(
                    3,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            corners[0] =
                new float3(
                    a.x,
                    CurrentPosition.y + verticalOffset,
                    a.y);

            corners[1] =
                new float3(
                    b.x,
                    CurrentPosition.y + verticalOffset,
                    b.y);

            corners[2] =
                new float3(
                    c.x,
                    CurrentPosition.y + verticalOffset,
                    c.y);

            return corners;
        }

        private NativeArray<float3>
            BuildPolylineBrushPreviewPoints()
        {
            if (!UsePolylineBrush ||
                !HasValidPosition)
            {
                return new NativeArray<float3>(
                    0,
                    Allocator.TempJob);
            }

            const float verticalOffset = 0.12f;

            int pointCount =
                CurrentPolylineGeometryPointCount;

            if (pointCount == 0)
            {
                NativeArray<float3> cursorPoint =
                    new(
                        1,
                        Allocator.TempJob,
                        NativeArrayOptions.UninitializedMemory);

                cursorPoint[0] =
                    CurrentPosition +
                    new float3(0f, verticalOffset, 0f);

                return cursorPoint;
            }

            NativeArray<float3> points =
                new(
                    pointCount,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            for (int index = 0;
                 index < pointCount;
                 index++)
            {
                points[index] =
                    GetPolylineGeometryPoint(index) +
                    new float3(0f, verticalOffset, 0f);
            }

            return points;
        }

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

        private struct ToolRadiusJob : IJob
        {
            public OverlayRenderSystem.Buffer OverlayBuffer;

            public float3 Position;
            public float Radius;
            public float LineHalfWidth;
            public float SelectionLineWidth;

            public int SelectionShape;
            public bool ConfirmationPending;
            public bool DeleteActive;

            [DeallocateOnJobCompletion]
            public NativeArray<float3>
                SurfaceOutlineSegmentPoints;

            [DeallocateOnJobCompletion]
            public NativeArray<float3>
                SquareBrushCorners;

            [DeallocateOnJobCompletion]
            public NativeArray<float3>
                TriangleBrushCorners;

            [DeallocateOnJobCompletion]
            public NativeArray<float3>
                PolylineBrushPoints;

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

                UnityEngine.Color selectionColor;

                if (ConfirmationPending)
                {
                    selectionColor =
                        new UnityEngine.Color(
                            1f,
                            0.85f,
                            0.1f,
                            1f);
                }
                else if (DeleteActive)
                {
                    selectionColor =
                        new UnityEngine.Color(
                            0.12f,
                            0.95f,
                            0.22f,
                            1f);
                }
                else
                {
                    selectionColor =
                        new UnityEngine.Color(
                            1f,
                            0.25f,
                            0.1f,
                            1f);
                }

                UnityEngine.Color surfaceOutlineColor =
                    new(
                        0.08f,
                        0.86f,
                        1f,
                        1f);

                DrawSurfaceOutlines(
                    surfaceOutlineColor);

                switch ((AreaBulldozerSelectionShape)SelectionShape)
                {
                    case AreaBulldozerSelectionShape.Square:
                        DrawSquare(
                            selectionColor,
                            lineWidth);
                        return;

                    case AreaBulldozerSelectionShape.Triangle:
                        DrawTriangle(
                            selectionColor,
                            lineWidth);
                        return;

                    case AreaBulldozerSelectionShape.Polyline:
                        DrawPolylineSelection(
                            selectionColor,
                            lineWidth);
                        return;

                    default:
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
                        return;
                }
            }

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

            private void DrawSquare(
                UnityEngine.Color selectionColor,
                float lineWidth)
            {
                if (SquareBrushCorners.Length != 4)
                {
                    return;
                }

                DrawClosedPolygon(
                    SquareBrushCorners,
                    selectionColor,
                    lineWidth);
            }

            private void DrawTriangle(
                UnityEngine.Color selectionColor,
                float lineWidth)
            {
                if (TriangleBrushCorners.Length != 3)
                {
                    return;
                }

                DrawClosedPolygon(
                    TriangleBrushCorners,
                    selectionColor,
                    lineWidth);
            }

            private void DrawClosedPolygon(
                NativeArray<float3> points,
                UnityEngine.Color selectionColor,
                float lineWidth)
            {
                float cornerOverlap =
                    math.clamp(
                        lineWidth * 0.5f,
                        0.15f,
                        1f);

                for (int index = 0;
                     index < points.Length;
                     index++)
                {
                    int nextIndex =
                        index + 1 < points.Length
                            ? index + 1
                            : 0;

                    OverlayBuffer.DrawLine(
                        selectionColor,
                        CreateExtendedSegment(
                            points[index],
                            points[nextIndex],
                            cornerOverlap),
                        lineWidth);
                }
            }

            private void DrawPolylineSelection(
                UnityEngine.Color selectionColor,
                float lineWidth)
            {
                float corridorDiameter =
                    math.max(
                        2f,
                        LineHalfWidth * 2f);

                if (PolylineBrushPoints.Length == 1)
                {
                    OverlayBuffer.DrawCircle(
                        selectionColor,
                        default,
                        lineWidth,
                        0f,
                        new float2(0f, 1f),
                        PolylineBrushPoints[0],
                        corridorDiameter);
                    return;
                }

                if (PolylineBrushPoints.Length < 2)
                {
                    return;
                }

                float overlap =
                    math.clamp(
                        lineWidth * 0.5f,
                        0.15f,
                        1f);

                for (int index = 0;
                     index + 1 < PolylineBrushPoints.Length;
                     index++)
                {
                    float3 start =
                        PolylineBrushPoints[index];

                    float3 end =
                        PolylineBrushPoints[index + 1];

                    float2 direction =
                        new float2(
                            end.x - start.x,
                            end.z - start.z);

                    float2 normalizedDirection =
                        math.normalizesafe(
                            direction,
                            new float2(1f, 0f));

                    float2 perpendicular =
                        new float2(
                            -normalizedDirection.y,
                            normalizedDirection.x) *
                        LineHalfWidth;

                    float3 sideAStart =
                        new(
                            start.x + perpendicular.x,
                            start.y,
                            start.z + perpendicular.y);

                    float3 sideAEnd =
                        new(
                            end.x + perpendicular.x,
                            end.y,
                            end.z + perpendicular.y);

                    float3 sideBStart =
                        new(
                            start.x - perpendicular.x,
                            start.y,
                            start.z - perpendicular.y);

                    float3 sideBEnd =
                        new(
                            end.x - perpendicular.x,
                            end.y,
                            end.z - perpendicular.y);

                    OverlayBuffer.DrawLine(
                        selectionColor,
                        CreateExtendedSegment(
                            sideAStart,
                            sideAEnd,
                            overlap),
                        lineWidth);

                    OverlayBuffer.DrawLine(
                        selectionColor,
                        CreateExtendedSegment(
                            sideBStart,
                            sideBEnd,
                            overlap),
                        lineWidth);

                    OverlayBuffer.DrawLine(
                        selectionColor,
                        new Line3.Segment(
                            start,
                            end),
                        math.max(0.2f, lineWidth * 0.45f));
                }

                float controlPointDiameter =
                    math.clamp(
                        lineWidth * 2.2f,
                        0.8f,
                        2.4f);

                for (int index = 0;
                     index < PolylineBrushPoints.Length;
                     index++)
                {
                    OverlayBuffer.DrawCircle(
                        selectionColor,
                        default,
                        lineWidth,
                        0f,
                        new float2(0f, 1f),
                        PolylineBrushPoints[index],
                        corridorDiameter);

                    OverlayBuffer.DrawCircle(
                        selectionColor,
                        selectionColor,
                        0.05f,
                        0f,
                        new float2(0f, 1f),
                        PolylineBrushPoints[index],
                        controlPointDiameter);
                }
            }
        }
    }
}
