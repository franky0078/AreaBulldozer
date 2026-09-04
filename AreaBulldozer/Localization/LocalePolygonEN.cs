using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocalePolygonEN : IDictionarySource
    {
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                {
                    "AreaBulldozer.UI.FreeAreaPolygon",
                    "Freeform area"
                },
                {
                    "AreaBulldozer.UI.FreeAreaPolygonActive",
                    "Freeform area"
                },
                {
                    "AreaBulldozer.UI.PolygonPoints",
                    "points"
                },
                {
                    "AreaBulldozer.UI.FreeAreaPolygonTooltip",
                    "Define a freeform deletion area using an unlimited number of corner points. Left-click to place points. Click the first point or double-click to close the polygon and delete everything inside it. Right-click removes the last point, and Esc cancels the current selection. Self-intersecting edges are prevented automatically."
                }
            };
        }

        public void Unload()
        {
        }
    }
}
