using Unity.Entities;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{

    internal static class SelectionGeometry
    {
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
                0.0001f)
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
            float2 segment =
                end - start;

            float segmentLengthSquared =
                math.lengthsq(segment);

            float interpolation =
                segmentLengthSquared > 0.0001f
                    ? math.saturate(
                        math.dot(
                            circleCenter - start,
                            segment) /
                        segmentLengthSquared)
                    : 0f;

            float2 nearestPoint =
                start +
                segment * interpolation;

            return math.distancesq(
                nearestPoint,
                circleCenter) <= radiusSquared;
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
                    isInside =
                        !isInside;
                }
            }

            return isInside;
        }
    }
}
