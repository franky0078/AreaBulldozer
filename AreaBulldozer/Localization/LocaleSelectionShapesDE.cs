using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{

    public class LocaleSelectionShapesDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleSelectionShapesDE(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { "AreaBulldozer.UI.Triangle", "Dreieck" },
                { "AreaBulldozer.UI.TriangleTooltip", "Gleichseitige Auswahlfläche um den Mauszeiger. Die Größe entspricht dem Abstand vom Mittelpunkt zur Ecke. Mit gehaltener rechter Maustaste drehbar." },
                { "AreaBulldozer.UI.TriangleSize", "Größe Dreieck" },

                { "AreaBulldozer.UI.MultiPointLine", "Mehrpunktlinie" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "Korridor aus 2 bis 15 Punkten. Linksklick setzt Punkte. Doppelklick am letzten Punkt schließt die Mehrpunktlinie ab und löscht. Rechtsklick entfernt den letzten Punkt, Esc verwirft die komplette Auswahl." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "Breite Mehrpunktlinie" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "Korridor schmaler" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "Korridor breiter" },

                // Alte Spline-Schlüssel
                { "AreaBulldozer.UI.Spline", "Mehrpunktlinie" },
                { "AreaBulldozer.UI.SplineTooltip", "Korridor aus 2 bis 15 Punkten. Linksklick setzt Punkte. Doppelklick schließt ab und löscht. Rechtsklick entfernt den letzten Punkt, Esc bricht ab." },
                { "AreaBulldozer.UI.SplineWidth", "Breite Mehrpunktlinie" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Diagnose-Logging aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Schreibt zusätzliche Info- und Diagnosemeldungen in das Log. Standardmäßig deaktiviert. Warnungen und Fehler werden unabhängig von dieser Einstellung weiterhin protokolliert." }
            };
        }

        public void Unload()
        {
        }
    }
}
