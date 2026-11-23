using System.Diagnostics;
using System.Text;

namespace AreaBulldozer.Diagnostics
{
    // Development-only timing of the tool's hot paths.
    //
    // To measure: set Enabled to true, rebuild, use the tool in game. Every
    // ReportIntervalSeconds a summary per measuring point is written to the
    // mod log (calls, average/maximum milliseconds, average item count).
    // Always keep Enabled false for published builds: with the constant off
    // the compiler removes the measuring code.
    internal static class PerfTrace
    {
        internal const bool Enabled = false;

        private const float ReportIntervalSeconds = 10f;

        internal const int SpatialRebuild = 0;
        internal const int SpatialQuery = 1;
        internal const int CarScan = 2;
        internal const int PreviewUpdate = 3;
        internal const int HighlightApply = 4;
        internal const int SurfaceOutline = 5;
        internal const int DeleteSelection = 6;
        internal const int SpatialSync = 7;
        internal const int SpatialMainRebuild = 8;

        private const int PointCount = 9;

        private static readonly long[] Calls = new long[PointCount];
        private static readonly long[] TotalTicks = new long[PointCount];
        private static readonly long[] MaxTicks = new long[PointCount];
        private static readonly long[] Items = new long[PointCount];
        private static long s_NextReportTimestamp;
        private static bool s_Disabled;

#pragma warning disable CS0162 // Unreachable while Enabled is false.
        internal static long Begin()
        {
            if (!Enabled || s_Disabled)
            {
                return 0;
            }

            return Stopwatch.GetTimestamp();
        }

        // Measuring must never break the game: anything that goes wrong in
        // here is caught, reported once and switches the measuring off.
        internal static void End(int point, long start, int items = 0)
        {
            if (!Enabled || s_Disabled || start == 0)
            {
                return;
            }

            try
            {
                if (point < 0 || point >= Calls.Length)
                {
                    return;
                }

                long now = Stopwatch.GetTimestamp();
                long elapsed = now - start;
                Calls[point]++;
                TotalTicks[point] += elapsed;
                Items[point] += items;
                if (elapsed > MaxTicks[point])
                {
                    MaxTicks[point] = elapsed;
                }

                ReportIfDue(now);
            }
            catch (System.Exception exception)
            {
                s_Disabled = true;
                TryLog("[PerfTrace] disabled after an error: " + exception);
            }
        }

        private static void ReportIfDue(long now)
        {
            // Stopwatch only, no Unity API: End can be reached from code paths
            // where Unity's main-thread-only properties are not allowed.
            if (s_NextReportTimestamp == 0)
            {
                s_NextReportTimestamp = now + (long)(ReportIntervalSeconds * Stopwatch.Frequency);
                return;
            }

            if (now < s_NextReportTimestamp)
            {
                return;
            }

            s_NextReportTimestamp = now + (long)(ReportIntervalSeconds * Stopwatch.Frequency);

            double ticksToMs = 1000.0 / Stopwatch.Frequency;
            var report = new StringBuilder("[PerfTrace] last ")
                .Append(ReportIntervalSeconds.ToString("0"))
                .Append(" s:");

            for (int index = 0; index < Calls.Length; index++)
            {
                long calls = Calls[index];
                if (calls == 0)
                {
                    continue;
                }

                report.Append(" | ").Append(GetLabel(index)).Append(": ")
                    .Append(calls).Append(" calls, avg ")
                    .Append((TotalTicks[index] * ticksToMs / calls).ToString("0.000"))
                    .Append(" ms, max ")
                    .Append((MaxTicks[index] * ticksToMs).ToString("0.000"))
                    .Append(" ms, avg items ")
                    .Append((Items[index] / (double)calls).ToString("0.0"));

                Calls[index] = 0;
                TotalTicks[index] = 0;
                MaxTicks[index] = 0;
                Items[index] = 0;
            }

            TryLog(report.ToString());
        }

        private static string GetLabel(int point)
        {
            switch (point)
            {
                case SpatialRebuild: return "Spatial index rebuild";
                case SpatialQuery: return "Spatial query (incl. cars)";
                case CarScan: return "Car scan";
                case PreviewUpdate: return "Preview update";
                case HighlightApply: return "Highlight apply";
                case SurfaceOutline: return "Surface outline (per frame)";
                case DeleteSelection: return "Delete selection";
                case SpatialSync: return "Spatial index sync (items = chunks)";
                case SpatialMainRebuild: return "Main index rebuild (roads/paths/areas)";
                default: return "Point " + point;
            }
        }

        private static void TryLog(string message)
        {
            try
            {
                Mod.Log?.Info(message);
            }
            catch
            {
                // Logging must never affect gameplay either.
            }
        }
#pragma warning restore CS0162
    }
}
