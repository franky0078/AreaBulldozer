using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private bool IsCandidateInsidePolyline(
            in SpatialCandidate candidate,
            float halfWidth)
        {
            if (UseFreeAreaPolygon)
            {
                return IsCandidateInsideFreeAreaPolygon(
                    candidate);
            }

            int pointCount =
                CurrentPolylineGeometryPointCount;

            if (pointCount < 2)
            {
                return false;
            }

            if (candidate.IsSurfaceArea)
            {
                return IsSurfaceAreaInsidePolyline(
                    candidate.Entity,
                    halfWidth);
            }

            for (int index = 0;
                 index + 1 < pointCount;
                 index++)
            {
                float2 corridorStart =
                    GetPolylineGeometryPoint2D(index);

                float2 corridorEnd =
                    GetPolylineGeometryPoint2D(index + 1);

                if (!candidate.IsSegment)
                {
                    if (SelectionGeometry.IsPointInsideLineCorridor(
                            candidate.Position,
                            corridorStart,
                            corridorEnd,
                            halfWidth))
                    {
                        return true;
                    }

                    continue;
                }

                if (SelectionGeometry.IsSegmentInsideLineCorridor(
                        candidate.Position,
                        candidate.EndPosition,
                        corridorStart,
                        corridorEnd,
                        halfWidth))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSurfaceAreaInsidePolyline(
            Entity areaEntity,
            float halfWidth)
        {
            if (!TryGetAreaNodes(
                    areaEntity,
                    out DynamicBuffer<Game.Areas.Node> nodes))
            {
                return false;
            }

            int polylinePointCount =
                CurrentPolylineGeometryPointCount;

            if (polylinePointCount < 2)
            {
                return false;
            }

            for (int nodeIndex = 0;
                 nodeIndex < nodes.Length;
                 nodeIndex++)
            {
                int nextNodeIndex =
                    nodeIndex + 1 < nodes.Length
                        ? nodeIndex + 1
                        : 0;

                float2 areaStart =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[nodeIndex]);

                float2 areaEnd =
                    SelectionGeometry.GetAreaNodePosition(
                        nodes[nextNodeIndex]);

                if (!math.all(math.isfinite(areaStart)) ||
                    !math.all(math.isfinite(areaEnd)))
                {
                    return false;
                }

                for (int lineIndex = 0;
                     lineIndex + 1 < polylinePointCount;
                     lineIndex++)
                {
                    float2 corridorStart =
                        GetPolylineGeometryPoint2D(lineIndex);

                    float2 corridorEnd =
                        GetPolylineGeometryPoint2D(lineIndex + 1);

                    if (SelectionGeometry.IsPointInsideLineCorridor(
                            areaStart,
                            corridorStart,
                            corridorEnd,
                            halfWidth) ||
                        SelectionGeometry.IsSegmentInsideLineCorridor(
                            areaStart,
                            areaEnd,
                            corridorStart,
                            corridorEnd,
                            halfWidth))
                    {
                        return true;
                    }
                }
            }

            for (int index = 0;
                 index < polylinePointCount;
                 index++)
            {
                if (SelectionGeometry.IsPointInsidePolygon(
                        nodes,
                        GetPolylineGeometryPoint2D(index)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
