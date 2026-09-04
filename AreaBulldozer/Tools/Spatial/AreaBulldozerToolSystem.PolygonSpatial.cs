using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private const float kFreeAreaPolygonBoundaryToleranceSquared =
            0.0004f;

        private bool IsCandidateInsideFreeAreaPolygon(
            in SpatialCandidate candidate)
        {
            int polygonPointCount =
                GetFreeAreaPolygonSelectionVertexCount();

            if (polygonPointCount < 3)
            {
                return false;
            }

            if (candidate.IsSurfaceArea)
            {
                return IsSurfaceAreaInsideFreeAreaPolygon(
                    candidate.Entity,
                    polygonPointCount);
            }

            if (!candidate.IsSegment)
            {
                return IsPointInsideFreeAreaPolygon(
                    candidate.Position,
                    polygonPointCount);
            }

            return IsSegmentInsideFreeAreaPolygon(
                candidate.Position,
                candidate.EndPosition,
                polygonPointCount);
        }

        private bool IsPointInsideFreeAreaPolygon(
            float2 point,
            int polygonPointCount)
        {
            bool inside = false;

            for (int index = 0;
                 index < polygonPointCount;
                 index++)
            {
                int nextIndex =
                    index + 1 < polygonPointCount
                        ? index + 1
                        : 0;

                float2 start =
                    GetFreeAreaPolygonSelectionVertex2D(
                        index);

                float2 end =
                    GetFreeAreaPolygonSelectionVertex2D(
                        nextIndex);

                if (!math.all(math.isfinite(start)) ||
                    !math.all(math.isfinite(end)))
                {
                    return false;
                }

                if (SelectionGeometry.DistancePointToSegmentSquared(
                        point,
                        start,
                        end) <=
                    kFreeAreaPolygonBoundaryToleranceSquared)
                {
                    return true;
                }

                bool crossesHorizontalRay =
                    (start.y > point.y) !=
                    (end.y > point.y);

                if (!crossesHorizontalRay)
                {
                    continue;
                }

                float verticalDifference =
                    end.y - start.y;

                if (math.abs(verticalDifference) <
                    0.0001f)
                {
                    continue;
                }

                float intersectionX =
                    start.x +
                    (point.y - start.y) *
                    (end.x - start.x) /
                    verticalDifference;

                if (point.x < intersectionX)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private bool IsSegmentInsideFreeAreaPolygon(
            float2 segmentStart,
            float2 segmentEnd,
            int polygonPointCount)
        {
            if (IsPointInsideFreeAreaPolygon(
                    segmentStart,
                    polygonPointCount) ||
                IsPointInsideFreeAreaPolygon(
                    segmentEnd,
                    polygonPointCount))
            {
                return true;
            }

            for (int index = 0;
                 index < polygonPointCount;
                 index++)
            {
                int nextIndex =
                    index + 1 < polygonPointCount
                        ? index + 1
                        : 0;

                float2 polygonStart =
                    GetFreeAreaPolygonSelectionVertex2D(
                        index);

                float2 polygonEnd =
                    GetFreeAreaPolygonSelectionVertex2D(
                        nextIndex);

                if (SelectionGeometry.SegmentsIntersect(
                        segmentStart,
                        segmentEnd,
                        polygonStart,
                        polygonEnd))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSurfaceAreaInsideFreeAreaPolygon(
            Entity areaEntity,
            int polygonPointCount)
        {
            if (!TryGetAreaNodes(
                    areaEntity,
                    out DynamicBuffer<Game.Areas.Node> nodes))
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

                if (IsPointInsideFreeAreaPolygon(
                        areaStart,
                        polygonPointCount))
                {
                    return true;
                }

                for (int polygonIndex = 0;
                     polygonIndex < polygonPointCount;
                     polygonIndex++)
                {
                    int nextPolygonIndex =
                        polygonIndex + 1 <
                            polygonPointCount
                            ? polygonIndex + 1
                            : 0;

                    float2 polygonStart =
                        GetFreeAreaPolygonSelectionVertex2D(
                            polygonIndex);

                    float2 polygonEnd =
                        GetFreeAreaPolygonSelectionVertex2D(
                            nextPolygonIndex);

                    if (SelectionGeometry.SegmentsIntersect(
                            areaStart,
                            areaEnd,
                            polygonStart,
                            polygonEnd))
                    {
                        return true;
                    }
                }
            }

            for (int polygonIndex = 0;
                 polygonIndex < polygonPointCount;
                 polygonIndex++)
            {
                if (SelectionGeometry.IsPointInsidePolygon(
                        nodes,
                        GetFreeAreaPolygonSelectionVertex2D(
                            polygonIndex)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
