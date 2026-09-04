using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocalePolygonDE : IDictionarySource
    {
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                {
                    "AreaBulldozer.UI.FreeAreaPolygon",
                    "Freie Fläche"
                },
                {
                    "AreaBulldozer.UI.FreeAreaPolygonActive",
                    "Freie Fläche"
                },
                {
                    "AreaBulldozer.UI.FreeAreaPolygonTooltip",
                    "Beliebig viele Eckpunkte definieren eine freie Löschfläche. Linksklick setzt Punkte. Klick auf den ersten Punkt oder Doppelklick schließt das Polygon und löscht den Inhalt. Rechtsklick entfernt den letzten Punkt, Esc verwirft die Auswahl. Selbstüberschneidende Kanten werden blockiert."
                }
            };
        }

        public void Unload()
        {
        }
    }
}
