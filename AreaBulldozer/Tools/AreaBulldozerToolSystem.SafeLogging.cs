using System;

namespace AreaBulldozer.Tools
{
    public partial class AreaBulldozerToolSystem
    {


        private static void SafeLogInfo(
            string message)
        {
            try
            {
                Mod.Log?.Info(message);
            }
            catch (Exception)
            {
                // Framework-internes Logging-Problem. (nichts tun)

            }
        }

        private static void SafeLogWarn(
            string message)
        {
            try
            {
                Mod.Log?.Warn(message);
            }
            catch (Exception)
            {
                // Siehe SafeLogInfo.
            }
        }

        private static void SafeLogError(
            string message)
        {
            try
            {
                Mod.Log?.Error(message);
            }
            catch (Exception)
            {
                // Siehe SafeLogInfo.
            }
        }
    }
}
