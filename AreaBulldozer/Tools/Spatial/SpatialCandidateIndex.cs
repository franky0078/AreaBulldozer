using System.Collections.Generic;
using Unity.Mathematics;

namespace AreaBulldozer.Tools
{

    internal sealed class SpatialCandidateIndex
    {
        private readonly float m_CellSize;

        private readonly Dictionary<
            long,
            List<SpatialCandidate>> m_Buckets =
            new();

        private readonly List<SpatialCandidate> m_QueryBuffer =
            new();

        private readonly List<List<SpatialCandidate>> m_BucketPool =
            new();

        public SpatialCandidateIndex(
            float cellSize)
        {
            m_CellSize =
                math.max(
                    1f,
                    cellSize);
        }

        public int CellCount =>
            m_Buckets.Count;

        public void Clear()
        {
            foreach (
                List<SpatialCandidate> bucket
                in m_Buckets.Values)
            {
                bucket.Clear();

                m_BucketPool.Add(
                    bucket);
            }

            m_Buckets.Clear();
            m_QueryBuffer.Clear();
        }

        public void Add(
            in SpatialCandidate candidate)
        {
            float2 minimum =
                math.min(
                    candidate.Position,
                    candidate.EndPosition);

            float2 maximum =
                math.max(
                    candidate.Position,
                    candidate.EndPosition);

            int minCellX =
                GetCellCoordinate(
                    minimum.x);

            int maxCellX =
                GetCellCoordinate(
                    maximum.x);

            int minCellZ =
                GetCellCoordinate(
                    minimum.y);

            int maxCellZ =
                GetCellCoordinate(
                    maximum.y);

            for (
                int cellZ = minCellZ;
                cellZ <= maxCellZ;
                cellZ++)
            {
                for (
                    int cellX = minCellX;
                    cellX <= maxCellX;
                    cellX++)
                {
                    long key =
                        GetCellKey(
                            cellX,
                            cellZ);

                    if (!m_Buckets.TryGetValue(
                            key,
                            out List<SpatialCandidate> bucket))
                    {
                        int poolIndex =
                            m_BucketPool.Count - 1;

                        if (poolIndex >= 0)
                        {
                            bucket =
                                m_BucketPool[poolIndex];

                            m_BucketPool.RemoveAt(
                                poolIndex);
                        }
                        else
                        {
                            bucket =
                                new();
                        }

                        m_Buckets.Add(
                            key,
                            bucket);
                    }

                    bucket.Add(candidate);
                }
            }
        }

        public List<SpatialCandidate> Query(
            float2 center,
            float radius)
        {
            m_QueryBuffer.Clear();

            int minCellX =
                GetCellCoordinate(
                    center.x - radius);

            int maxCellX =
                GetCellCoordinate(
                    center.x + radius);

            int minCellZ =
                GetCellCoordinate(
                    center.y - radius);

            int maxCellZ =
                GetCellCoordinate(
                    center.y + radius);

            for (
                int cellZ = minCellZ;
                cellZ <= maxCellZ;
                cellZ++)
            {
                for (
                    int cellX = minCellX;
                    cellX <= maxCellX;
                    cellX++)
                {
                    long key =
                        GetCellKey(
                            cellX,
                            cellZ);

                    if (!m_Buckets.TryGetValue(
                            key,
                            out List<SpatialCandidate> bucket))
                    {
                        continue;
                    }

                    m_QueryBuffer.AddRange(
                        bucket);
                }
            }

            return m_QueryBuffer;
        }

        private int GetCellCoordinate(
            float worldCoordinate)
        {
            return (int)math.floor(
                worldCoordinate /
                m_CellSize);
        }

        private static long GetCellKey(
            int cellX,
            int cellZ)
        {
            return
                ((long)cellX << 32) ^
                (uint)cellZ;
        }
    }
}
