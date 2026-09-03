using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{
    internal static class SelectionGeometry
    {
        private const float kEpsilon = 0.0001f;

        public static float2 RotateLocalToWorld(
            float2 localOffset,
            float rotationRadians)
        {
            float sine =
                math.sin(rotationRadians);

            float cosine =
                math.cos(rotationRadians);

            return new float2(
                cosine * localOffset.x -
                sine * localOffset.y,
                sine * localOffset.x +
                cosine * localOffset.y);
        }

        public static float2 RotateWorldToLocal(
            float2 worldOffset,
            float rotationRadians)
        {
            float sine =
                math.sin(rotationRadians);

            float cosine =
                math.cos(rotationRadians);

            return new float2(
                cosine * worldOffset.x +
                sine * worldOffset.y,
                -sine * worldOffset.x +
                cosine * worldOffset.y);
        }

        public static bool IsPointInsideSquare(
            float2 point,
            float2 squareCenter,
            float halfSize,
            float rotationRadians)
        {
            float2 localPoint =
                RotateWorldToLocal(
                    point - squareCenter,
                    rotationRadians);

            float2 offset =
                math.abs(localPoint);

            return
                offset.x <= halfSize &&
                offset.y <= halfSize;
        }

        public static bool IsSegmentInsideSquare(
            float2 start,
            float2 end,
            float2 squareCenter,
            float halfSize,
            float rotationRadians)
        {
            float2 localStart =
                RotateWorldToLocal(
                    start - squareCenter,
                    rotationRadians);

            float2 localEnd =
                RotateWorldToLocal(
                    end - squareCenter,
                    rotationRadians);

            float2 minimum =
                new float2(-halfSize, -halfSize);

            float2 maximum =
                new float2(halfSize, halfSize);

            if (math.all(localStart >= minimum) &&
                math.all(localStart <= maximum) ||
                math.all(localEnd >= minimum) &&
                math.all(localEnd <= maximum))
            {
                return true;
            }

            float2 direction =
                localEnd -
                localStart;

            float minimumInterpolation = 0f;
            float maximumInterpolation = 1f;

            return
                ClipSegmentAxis(
                    localStart.x,
                    direction.x,
                    minimum.x,
                    maximum.x,
                    ref minimumInterpolation,
                    ref maximumInterpolation) &&
                ClipSegmentAxis(
                    localStart.y,
                    direction.y,
                    minimum.y,
                    maximum.y,
                    ref minimumInterpolation,
                    ref maximumInterpolation);
        }

        private static bool ClipSegmentAxis(
            float start,
            float direction,
            float minimum,
            float maximum,
            ref float minimumInterpolation,
            ref float maximumInterpolation)
        {
            if (math.abs(direction) <
                kEpsilon)
            {
                return
                    start >= minimum &&
                    start <= maximum;
            }

            float inverseDirection =
                1f /
                direction;

            float first =
                (minimum - start) *
                inverseDirection;

            float second =
                (maximum - start) *
                inverseDirection;

            if (first > second)
            {
                float temporary =
                    first;

                first =
                    second;

                second =
                    temporary;
            }

            minimumInterpolation =
                math.max(
                    minimumInterpolation,
                    first);

            maximumInterpolation =
                math.min(
                    maximumInterpolation,
                    second);

            return
                minimumInterpolation <=
                maximumInterpolation;
        }

        public static bool IsSegmentInsideCircle(
            float2 start,
            float2 end,
            float2 circleCenter,
            float radiusSquared)
        {
            return DistancePointToSegmentSquared(
                       circleCenter,
                       start,
                       end) <=
                   radiusSquared;
        }

        public static void GetEquilateralTriangleCorners(
            float2 center,
            float cornerRadius,
            float rotationRadians,
            out float2 cornerA,
            out float2 cornerB,
            out float2 cornerC)
        {
            float safeRadius =
                math.max(0.01f, cornerRadius);

            const float sqrtThreeOverTwo =
                0.8660254037844386f;

            float2 localA =
                new(0f, safeRadius);

            float2 localB =
                new(
                    -sqrtThreeOverTwo * safeRadius,
                    -0.5f * safeRadius);

            float2 localC =
                new(
                    sqrtThreeOverTwo * safeRadius,
                    -0.5f * safeRadius);

            cornerA =
                center +
                RotateLocalToWorld(
                    localA,
                    rotationRadians);

            cornerB =
                center +
                RotateLocalToWorld(
                    localB,
                    rotationRadians);

            cornerC =
                center +
                RotateLocalToWorld(
                    localC,
                    rotationRadians);
        }

        public static bool IsPointInsideTriangle(
            float2 point,
            float2 a,
            float2 b,
            float2 c)
        {
            float d1 =
                Cross(
                    b - a,
                    point - a);

            float d2 =
                Cross(
                    c - b,
                    point - b);

            float d3 =
                Cross(
                    a - c,
                    point - c);

            bool hasNegative =
                d1 < -kEpsilon ||
                d2 < -kEpsilon ||
                d3 < -kEpsilon;

            bool hasPositive =
                d1 > kEpsilon ||
                d2 > kEpsilon ||
                d3 > kEpsilon;

            return !(hasNegative && hasPositive);
        }

        public static bool IsSegmentInsideTriangle(
            float2 start,
            float2 end,
            float2 a,
            float2 b,
            float2 c)
        {
            if (IsPointInsideTriangle(start, a, b, c) ||
                IsPointInsideTriangle(end, a, b, c))
            {
                return true;
            }

            return
                SegmentsIntersect(start, end, a, b) ||
                SegmentsIntersect(start, end, b, c) ||
                SegmentsIntersect(start, end, c, a);
        }

        public static bool IsPointInsideLineCorridor(
            float2 point,
            float2 lineStart,
            float2 lineEnd,
            float halfWidth)
        {
            float safeHalfWidth =
                math.max(0.01f, halfWidth);

            return DistancePointToSegmentSquared(
                       point,
                       lineStart,
                       lineEnd) <=
                   safeHalfWidth * safeHalfWidth;
        }

        public static bool IsSegmentInsideLineCorridor(
            float2 segmentStart,
            float2 segmentEnd,
            float2 lineStart,
            float2 lineEnd,
            float halfWidth)
        {
            float safeHalfWidth =
                math.max(0.01f, halfWidth);

            return DistanceSegmentToSegmentSquared(
                       segmentStart,
                       segmentEnd,
                       lineStart,
                       lineEnd) <=
                   safeHalfWidth * safeHalfWidth;
        }

        public static float DistancePointToSegmentSquared(
            float2 point,
            float2 segmentStart,
            float2 segmentEnd)
        {
            float2 segment =
                segmentEnd - segmentStart;

            float segmentLengthSquared =
                math.lengthsq(segment);

            float interpolation =
                segmentLengthSquared > kEpsilon
                    ? math.saturate(
                        math.dot(
                            point - segmentStart,
                            segment) /
                        segmentLengthSquared)
                    : 0f;

            float2 nearestPoint =
                segmentStart +
                segment * interpolation;

            return math.distancesq(
                point,
                nearestPoint);
        }

        public static float DistanceSegmentToSegmentSquared(
            float2 firstStart,
            float2 firstEnd,
            float2 secondStart,
            float2 secondEnd)
        {
            if (SegmentsIntersect(
                    firstStart,
                    firstEnd,
                    secondStart,
                    secondEnd))
            {
                return 0f;
            }

            float distance0 =
                DistancePointToSegmentSquared(
                    firstStart,
                    secondStart,
                    secondEnd);

            float distance1 =
                DistancePointToSegmentSquared(
                    firstEnd,
                    secondStart,
                    secondEnd);

            float distance2 =
                DistancePointToSegmentSquared(
                    secondStart,
                    firstStart,
                    firstEnd);

            float distance3 =
                DistancePointToSegmentSquared(
                    secondEnd,
                    firstStart,
                    firstEnd);

            return math.min(
                math.min(distance0, distance1),
                math.min(distance2, distance3));
        }

        public static bool SegmentsIntersect(
            float2 a0,
            float2 a1,
            float2 b0,
            float2 b1)
        {
            float o1 =
                Cross(
                    a1 - a0,
                    b0 - a0);

            float o2 =
                Cross(
                    a1 - a0,
                    b1 - a0);

            float o3 =
                Cross(
                    b1 - b0,
                    a0 - b0);

            float o4 =
                Cross(
                    b1 - b0,
                    a1 - b0);

            if (((o1 > kEpsilon && o2 < -kEpsilon) ||
                 (o1 < -kEpsilon && o2 > kEpsilon)) &&
                ((o3 > kEpsilon && o4 < -kEpsilon) ||
                 (o3 < -kEpsilon && o4 > kEpsilon)))
            {
                return true;
            }

            if (math.abs(o1) <= kEpsilon &&
                IsPointOnSegment(b0, a0, a1))
            {
                return true;
            }

            if (math.abs(o2) <= kEpsilon &&
                IsPointOnSegment(b1, a0, a1))
            {
                return true;
            }

            if (math.abs(o3) <= kEpsilon &&
                IsPointOnSegment(a0, b0, b1))
            {
                return true;
            }

            return math.abs(o4) <= kEpsilon &&
                   IsPointOnSegment(a1, b0, b1);
        }

        private static bool IsPointOnSegment(
            float2 point,
            float2 segmentStart,
            float2 segmentEnd)
        {
            float2 minimum =
                math.min(
                    segmentStart,
                    segmentEnd) -
                new float2(kEpsilon);

            float2 maximum =
                math.max(
                    segmentStart,
                    segmentEnd) +
                new float2(kEpsilon);

            return
                math.all(point >= minimum) &&
                math.all(point <= maximum);
        }

        private static float Cross(
            float2 first,
            float2 second)
        {
            return
                first.x * second.y -
                first.y * second.x;
        }

        public static float2 GetAreaNodePosition(
            Game.Areas.Node node)
        {
            return new float2(
                node.m_Position.x,
                node.m_Position.z);
        }

        public static bool IsPointInsidePolygon(
            DynamicBuffer<Game.Areas.Node> nodes,
            float2 point)
        {
            bool isInside = false;

            for (int index = 0;
                 index < nodes.Length;
                 index++)
            {
                int nextIndex =
                    index + 1 < nodes.Length
                        ? index + 1
                        : 0;

                float2 start =
                    GetAreaNodePosition(
                        nodes[index]);

                float2 end =
                    GetAreaNodePosition(
                        nodes[nextIndex]);

                if (!math.all(math.isfinite(start)) ||
                    !math.all(math.isfinite(end)))
                {
                    return false;
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
                    kEpsilon)
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
                    isInside =
                        !isInside;
                }
            }

            return isInside;
        }
    }
}
