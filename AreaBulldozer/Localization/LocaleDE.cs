using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleDE(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Area Bulldozer" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Allgemein" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "Sicherheit" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "Darstellung und Startbutton" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "Tastenbelegung" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "Information" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "Bäume und Vegetation entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "Erfasst Bäume, Büsche und andere Vegetation innerhalb des Auswahlbereichs." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "Gebäude entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "Erfasst vollständige Hauptgebäude innerhalb des Auswahlbereichs. Gebäudeeigene Props, Marker und Lanes werden nicht zusätzlich als einzelne Gebäude gezählt." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "Straßen entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "Erfasst vollständige Hauptstraßensegmente, deren Verlauf den Auswahlbereich berührt. Fußwege, Bahnstrecken, Marker-Netzwerke und gebäudeeigene Netze bleiben ausgeschlossen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "Fußwege entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "Erfasst vollständige eigenständige Fußwegsegmente, deren Verlauf den Auswahlbereich berührt. Straßen, Bahnstrecken, Marker-Netzwerke und gebäudeeigene SubNets bleiben ausgeschlossen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "Bahnstrecken entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "Erfasst vollständige eigenständige Zug-, Tram-, U-Bahn- und Metro-Gleissegmente, deren Verlauf den Auswahlbereich berührt. Straßen, Fußwege, Marker-Netzwerke und gebäudeeigene SubNets bleiben ausgeschlossen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "Flächen und Oberflächen entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "Erfasst vollständige polygonale Oberflächen und Spaces, sobald der Auswahlbereich den Rand berührt oder sein Mittelpunkt innerhalb der Fläche liegt. Zugeordnete Asset-Flächen benötigen zusätzlich den passenden Unterobjektfilter und können nach einem Asset-Update erneut erzeugt werden." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "Statische Objekte entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "Aktiviert die darunter ausgewählten Prop- und Markerarten. Sichtbare Props werden direkt über ihre Kategorie erfasst; sensible Aktivitäts-, Spawn- und Lane-Marker beachten weiterhin die Unterobjekt-Sicherheitsoptionen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "Allgemeine Props und Dekorationen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "Erfasst statische Props, die keiner der spezielleren Kategorien zugeordnet werden." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "Straßenleuchten" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "Erfasst Objekte mit der Straßenleuchten-Komponente, auch wenn sie zu einem Gebäude oder Netzwerk gehören." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "Mülleimer und Mengenobjekte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "Erfasst Mengenobjekte, zu denen unter anderem Mülleimer und vergleichbare Props gehören können." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "Werbung und Branding" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "Erfasst Markenobjekte, Werbeschilder und ähnliche Dekorationen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "Aktivitätspunkte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "Erfasst Aktivitätspunkte von Assets. Diese können die Nutzung eines Gebäudes beeinflussen und sind deshalb standardmäßig deaktiviert." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "Spawnpunkte – blau" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "Erfasst die blauen SpawnLocation-Marker, an denen Fahrzeuge oder andere Objekte eines Assets erscheinen können. Zugeordnete Spawnpunkte benötigen zusätzlich den passenden Unterobjektfilter." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Asset-Lanes / SubLanes – grün und blau" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Erfasst die grünen und blauen gestrichelten Fahr-, Fuß- und Parkspuren innerhalb von Gebäude-Assets. Technisch werden nur SubLanes mit NetLanePrefab oder NetLaneGeometryPrefab ausgewählt; normale Straßen-Edges bleiben unberührt." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "Marker-Hintergrund abdunkeln" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Dunkelt die Spielansicht ab, solange Aktivitätspunkte, Spawnpunkte oder Asset-Lanes aktiviert sind. Menüs bleiben bedienbar." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Abdunkelungsstärke" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Legt die Abdunkelung der Spielansicht in Prozent fest." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "Linienstärke der Auswahl" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "Legt eine feste Linienstärke für die kreisförmige und quadratische Auswahl fest. Die Linienstärke ändert sich nicht mehr mit der Auswahlgröße." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Gebäude-Unterobjekte einschließen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Erlaubt die ausgewählten Objektarten auch dann, wenn sie zu einem Gebäude gehören. Der passende Objektfilter muss ebenfalls aktiviert sein. Unterobjekte können nach Gebäudeaktualisierungen erneut erscheinen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Netzwerk-Unterobjekte einschließen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Erlaubt die ausgewählten Objektarten auch dann, wenn sie zu einer Straße oder einem anderen Netzwerk gehören. Der passende Objektfilter muss ebenfalls aktiviert sein. Unterobjekte können später erneut erzeugt werden." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "Zugeordnete Objekte schützen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "Schützt zugeordnete Objekte, deren Besitzer weder als Gebäude noch als Netzwerk erkannt wird. Diese Sicherheitseinstellung sollte aktiviert bleiben." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "Quadratische Auswahl verwenden" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "Deaktiviert: kreisförmige Auswahl. Aktiviert: frei drehbares Quadrat. Zum Drehen die rechte Maustaste gedrückt halten und die Maus horizontal bewegen. Die eingestellte Größe ist beim Kreis der Radius und beim Quadrat die halbe Kantenlänge." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "Standard-Auswahlgröße" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "Legt die Größe in Metern fest: Kreisradius beziehungsweise halbe Kantenlänge des Quadrats." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "Skalierung des Werkzeugfensters" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Skaliert das Area-Bulldozer-Fenster von 75 bis 125 Prozent. Kleinere Werte schaffen mehr Platz; größere Werte verbessern die Lesbarkeit auf hochauflösenden Monitoren." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseUniversalModMenu)), "Button im neuen Mod-Menü anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseUniversalModMenu)), "Verschiebt den Area-Bulldozer-Startbutton in das universelle Mod-Menü des Spiels. Der separate schwebende Button wird dann ausgeblendet." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "Schwebenden Button verschiebbar machen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Erlaubt das Verschieben des separaten Startbuttons mit Strg + gedrückter linker Maustaste. Beim Loslassen wird die Position gespeichert. Ein normaler Linksklick aktiviert oder deaktiviert weiterhin nur das Werkzeug. Diese Einstellung wirkt nicht bei Verwendung des neuen Mod-Menüs." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "Buttonposition zurücksetzen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "Setzt den verschiebbaren Startbutton wieder an seine Standardposition oben links." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "Große Auswahl bestätigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "Verlangt ab der eingestellten Objektanzahl einen zweiten Klick innerhalb von fünf Sekunden. Die Auswahlform wird während der Bestätigung gelb dargestellt." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "Bestätigungsgrenze" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "Legt fest, ab wie vielen ausgewählten Objekten eine zweite Bestätigung erforderlich ist." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "Area Bulldozer aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "Aktiviert oder deaktiviert das Werkzeug." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Tastenbelegung zurücksetzen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Setzt die Tastenbelegung auf den Standardwert zurück." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "Zeigt die aktuell installierte Version von Area Bulldozer an." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "Area Bulldozer aktivieren" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.ToggleTool", "Area Bulldozer umschalten" },
                { "AreaBulldozer.UI.LauncherDragHint", "Strg + Linksklick halten und ziehen; loslassen zum Speichern" },
                { "AreaBulldozer.UI.ModMenuOpen", "Werkzeug öffnen" },
                { "AreaBulldozer.UI.Close", "Werkzeug schlie\u00DFen" },
                { "AreaBulldozer.UI.Active", "Aktiv" },
                { "AreaBulldozer.UI.Subtitle", "Mehrere Objekte in einem Bereich entfernen" },
                { "AreaBulldozer.UI.Selection", "Auswahl" },
                { "AreaBulldozer.UI.Circle", "Kreis" },
                { "AreaBulldozer.UI.Square", "Quadrat" },
                { "AreaBulldozer.UI.Radius", "Radius" },
                { "AreaBulldozer.UI.HalfSide", "Gr\u00F6\u00DFe Quadrat" },
                { "AreaBulldozer.UI.Rotation", "Drehung" },
                { "AreaBulldozer.UI.Degrees", "Grad" },
                { "AreaBulldozer.UI.MainFilters", "Objektfilter" },
                { "AreaBulldozer.UI.ChooseFilters", "Auswahl festlegen" },
                { "AreaBulldozer.UI.Vegetation", "B\u00E4ume und Vegetation" },
                { "AreaBulldozer.UI.VegetationShort", "Vegetation" },
                { "AreaBulldozer.UI.Buildings", "Geb\u00E4ude" },
                { "AreaBulldozer.UI.Roads", "Stra\u00DFen" },
                { "AreaBulldozer.UI.Paths", "Fu\u00DFwege" },
                { "AreaBulldozer.UI.Railways", "Gleise" },
                { "AreaBulldozer.UI.Surfaces", "Fl\u00E4chen und Bereiche" },
                { "AreaBulldozer.UI.SurfacesShort", "Fl\u00E4chen" },
                { "AreaBulldozer.UI.StaticObjects", "Props und Marker" },
                { "AreaBulldozer.UI.StaticObjectsShort", "Props / Marker" },
                { "AreaBulldozer.UI.AdvancedFilters", "Erweiterte Prop- und Markerfilter" },
                { "AreaBulldozer.UI.StaticMasterNotice", "Aktiviere oben Props und Marker, damit diese Kategorien verwendet werden." },
                { "AreaBulldozer.UI.GeneralProps", "Allgemeine Props" },
                { "AreaBulldozer.UI.StreetLights", "Stra\u00DFenlaternen" },
                { "AreaBulldozer.UI.QuantityObjects", "M\u00FClleimer und Mengenobjekte" },
                { "AreaBulldozer.UI.Branding", "Werbung und Branding" },
                { "AreaBulldozer.UI.ActivityLocations", "Aktivit\u00E4tspunkte" },
                { "AreaBulldozer.UI.SpawnLocations", "Spawnpunkte" },
                { "AreaBulldozer.UI.AssetLanes", "Asset-Lanes / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "Marker-Hintergrund abdunkeln" },
                { "AreaBulldozer.UI.BackgroundDarkness", "Abdunkelungsst\u00E4rke" },
                { "AreaBulldozer.UI.Safety", "Sicherheit" },
                { "AreaBulldozer.UI.BuildingSubObjects", "Geb\u00E4ude-Unterobjekte" },
                { "AreaBulldozer.UI.NetworkSubObjects", "Netzwerk-Unterobjekte" },
                { "AreaBulldozer.UI.ProtectOwned", "Zugeordnete Objekte sch\u00FCtzen" },
                { "AreaBulldozer.UI.ConfirmLarge", "Gro\u00DFe Auswahl best\u00E4tigen" },
                { "AreaBulldozer.UI.Threshold", "Best\u00E4tigungsgrenze" },
                { "AreaBulldozer.UI.Objects", "Objekte" },
                { "AreaBulldozer.UI.ApplyHelp", "Ausgew\u00E4hlte Objekte l\u00F6schen" },
                { "AreaBulldozer.UI.RotateHelp", "Gedr\u00FCckt halten und drehen" },
                { "AreaBulldozer.UI.ShortcutHelp", "Werkzeug umschalten" }
            };
        }

        public void Unload()
        {
        }
    }
}