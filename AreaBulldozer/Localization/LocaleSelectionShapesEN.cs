using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{

    public class LocaleSelectionShapesEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleSelectionShapesEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { "AreaBulldozer.UI.Triangle", "Triangle" },
                { "AreaBulldozer.UI.TriangleTooltip", "Equilateral selection area around the cursor. Size is measured from the center to a corner. Hold the right mouse button and move horizontally to rotate it." },
                { "AreaBulldozer.UI.TriangleSize", "Triangle size" },

                { "AreaBulldozer.UI.MultiPointLine", "Multi-point line" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "Corridor using 2 to 15 points. Left click adds points. Double-click the final point to finish and delete. Right click removes the last point; Esc cancels the whole selection." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "Multi-point line width" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "Make corridor narrower" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "Make corridor wider" },

                // Keep old spline keys
                { "AreaBulldozer.UI.Spline", "Multi-point line" },
                { "AreaBulldozer.UI.SplineTooltip", "Corridor using 2 to 15 points. Left click adds points. Double-click finishes and deletes. Right click removes the last point; Esc cancels." },
                { "AreaBulldozer.UI.SplineWidth", "Multi-point line width" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Enable diagnostic logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Writes additional informational and diagnostic messages to the log. Disabled by default. Warnings and errors are still logged regardless of this setting." }
            };
        }

        public void Unload()
        {
        }
    }
}
