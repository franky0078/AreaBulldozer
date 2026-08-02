using Game;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.InputSystem;

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

            NativeArray<float3> surfacePreviewPoints =
                BuildSurfacePreviewPoints();

            NativeArray<float3> squareBrushPreviewPoints =
                BuildSquareBrushPreviewPoints();

            ToolRadiusJob radiusJob =
                new ToolRadiusJob
                {
                    OverlayBuffer = overlayBuffer,
                    Position = CurrentPosition,
                    Radius = CurrentRadius,
                    UseSquareBrush =
                        this.UseSquareBrush,
                    ConfirmationPending =
                        m_LargeSelectionConfirmationPending,
                    SurfacePreviewPoints =
                        surfacePreviewPoints,
                    SquareBrushPreviewPoints =
                        squareBrushPreviewPoints
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
            BuildSquareBrushPreviewPoints()
        {
            if (!UseSquareBrush ||
                !HasValidPosition)
            {
                return new NativeArray<float3>(
                    0,
                    Allocator.TempJob);
            }

            const int maximumPreviewPoints = 4096;
            const float verticalOffset = 0.12f;
            const float pointSpacing = 0.55f;

            float halfSize =
                CurrentRadius;

            float3 center =
                CurrentPosition;

            float2[] localCorners =
            {
                new float2(-halfSize, -halfSize),
                new float2( halfSize, -halfSize),
                new float2( halfSize,  halfSize),
                new float2(-halfSize,  halfSize)
            };

            float3[] corners =
                new float3[4];

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

            List<float3> points =
                new();

            for (int edgeIndex = 0;
                 edgeIndex < corners.Length &&
                 points.Count < maximumPreviewPoints;
                 edgeIndex++)
            {
                int nextEdgeIndex =
                    edgeIndex + 1 <
                    corners.Length
                        ? edgeIndex + 1
                        : 0;

                float3 start =
                    corners[edgeIndex];

                float3 end =
                    corners[nextEdgeIndex];

                float edgeLength =
                    math.distance(
                        new float2(
                            start.x,
                            start.z),
                        new float2(
                            end.x,
                            end.z));

                int stepCount =
                    math.clamp(
                        (int)math.ceil(
                            edgeLength /
                            pointSpacing),
                        1,
                        1024);

                for (int step = 0;
                     step < stepCount &&
                     points.Count <
                         maximumPreviewPoints;
                     step++)
                {
                    float interpolation =
                        step /
                        (float)stepCount;

                    points.Add(
                        math.lerp(
                            start,
                            end,
                            interpolation));
                }
            }

            NativeArray<float3> result =
                new(
                    points.Count,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            for (int index = 0;
                 index < points.Count;
                 index++)
            {
                result[index] =
                    points[index];
            }

            return result;
        }

        private NativeArray<float3> BuildSurfacePreviewPoints()
        {
            const int maximumPreviewPoints = 12288;
            const float pointSpacing = 0.55f;
            const float verticalOffset = 0.18f;

            List<float3> points =
                new();

            if (m_HighlightedEntities != null)
            {
                foreach (Entity entity in
                         m_HighlightedEntities)
                {
                    if (points.Count >= maximumPreviewPoints)
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
                         points.Count < maximumPreviewPoints;
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

                        float edgeLength =
                            math.distance(
                                new float2(start.x, start.z),
                                new float2(end.x, end.z));

                        int stepCount =
                            math.clamp(
                                (int)math.ceil(
                                    edgeLength / pointSpacing),
                                1,
                                512);

                        for (int step = 0;
                             step < stepCount &&
                             points.Count < maximumPreviewPoints;
                             step++)
                        {
                            float interpolation =
                                step / (float)stepCount;

                            float3 point =
                                math.lerp(
                                    start,
                                    end,
                                    interpolation);

                            point.y += verticalOffset;
                            points.Add(point);
                        }
                    }
                }
            }

            NativeArray<float3> result =
                new(
                    points.Count,
                    Allocator.TempJob,
                    NativeArrayOptions.UninitializedMemory);

            for (int index = 0;
                 index < points.Count;
                 index++)
            {
                result[index] =
                    points[index];
            }

            return result;
        }

        private struct ToolRadiusJob : IJob
        {
            public OverlayRenderSystem.Buffer OverlayBuffer;

            public float3 Position;
            public float Radius;
            public bool UseSquareBrush;
            public bool ConfirmationPending;

            [DeallocateOnJobCompletion]
            public NativeArray<float3> SurfacePreviewPoints;

            [DeallocateOnJobCompletion]
            public NativeArray<float3> SquareBrushPreviewPoints;

            public void Execute()
            {
                float radius =
                    math.max(
                        5f,
                        Radius);

                float lineWidth =
                    math.max(
                        0.25f,
                        radius / 20f);

                UnityEngine.Color circleColor =
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

                if (ConfirmationPending)
                {
                    lineWidth *= 1.5f;
                }

                UnityEngine.Color surfaceOutlineColor =
                    new UnityEngine.Color(
                        0.08f,
                        0.86f,
                        1f,
                        1f);

                UnityEngine.Color surfaceFillColor =
                    new UnityEngine.Color(
                        0.18f,
                        0.78f,
                        1f,
                        0.42f);

                for (int index = 0;
                     index < SurfacePreviewPoints.Length;
                     index++)
                {
                    OverlayBuffer.DrawCircle(
                        surfaceOutlineColor,
                        surfaceFillColor,
                        0.12f,
                        0f,
                        new float2(
                            0f,
                            1f),
                        SurfacePreviewPoints[index],
                        0.75f);
                }

                if (UseSquareBrush)
                {
                    const float squarePointDiameter = 0.75f;

                    for (int index = 0;
                         index <
                            SquareBrushPreviewPoints.Length;
                         index++)
                    {
                        OverlayBuffer.DrawCircle(
                            circleColor,
                            circleColor,
                            0.08f,
                            0f,
                            new float2(
                                0f,
                                1f),
                            SquareBrushPreviewPoints[index],
                            squarePointDiameter);
                    }

                    return;
                }

                OverlayBuffer.DrawCircle(
                    circleColor,

                    default,

                    lineWidth,

                    0f,

                    new float2(
                        0f,
                        1f),

                    Position,

                    radius * 2f);
            }
        }
    }
}