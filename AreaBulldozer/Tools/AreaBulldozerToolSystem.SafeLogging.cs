using System;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {
        private static void SafeLogInfo(
            string message)
        {

            if (!Mod.DiagnosticLoggingEnabled)
            {
                return;
            }

            Mod.LogDiagnosticInfo(message);
        }

        private static void SafeLogWarn(
            string message)
        {
            try
            {
                // Warnings are never hidden by the diagnostic switch.
                Mod.Log?.Warn(message);
            }
            catch (Exception)
            {
                // Logging must never affect gameplay.
            }
        }

        private static void SafeLogError(
            string message)
        {
            try
            {
                // Errors are never hidden by the diagnostic switch.
                Mod.Log?.Error(message);
            }
            catch (Exception)
            {
                // Logging must never affect gameplay.
            }
        }
    }
}
