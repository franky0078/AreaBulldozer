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

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Anarchy-Vegetation einschließen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Erlaubt das Löschen von überschriebenen beziehungsweise mit Anarchy platzierten Bäumen und Pflanzen. Diese Option ist standardmäßig deaktiviert." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "Gebäude entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "Erfasst vollständige Hauptgebäude innerhalb des Auswahlbereichs. Gebäudeeigene Props, Marker und Lanes werden nicht zusätzlich als einzelne Gebäude gezählt." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "Autos entfernen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "Entfernt fahrende und geparkte Straßenfahrzeuge im Auswahlbereich. Die Simulation pausiert bei Aktivierung automatisch." },

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

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "Hintergrund abdunkeln" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Dunkelt den Hintergrund vor den Spiel-Umrandungen ab, wenn Markerfilter oder fehlende Assets aktiviert sind. Umrandungen und Menüs bleiben sichtbar." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Abdunkelungsstärke" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Legt die Abdunkelung der Spielansicht in Prozent fest." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "Linienstärke der Auswahl" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "Legt eine feste Linienstärke für die kreisförmige und quadratische Auswahl fest. Die Linienstärke ändert sich nicht mehr mit der Auswahlgröße." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "Asset-Vegetation schützen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "Schützt Pflanzen, die zu Gebäuden oder anderen Assets gehören, beim Löschen mit dem Vegetationsfilter." },
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

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Startort des Area Bulldozers" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "Legt fest, ob Area Bulldozer über einen separaten Startbutton, im Werkzeugfenster des normalen Bulldozers oder über das universelle Mod-Menü gestartet wird." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "Separater Startbutton" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "Beim normalen Bulldozer" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "Universelles Mod-Menü" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "Schwebenden Button verschiebbar machen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Erlaubt das Verschieben des separaten Startbuttons mit Strg + gedrückter linker Maustaste. Beim Loslassen wird die Position gespeichert. Diese Einstellung wirkt nur beim Startort 'Separater Startbutton'." },

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

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "Untergrund" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "Nur in der Untergrundansicht: Weiß = Original, dunklere Farben = Abdunklung / Tönung. UI und Auswahlumrisse bleiben unverändert." },
                { "AreaBulldozer.UI.HeightFilter", "Höhenfilter" },
                { "AreaBulldozer.UI.HeightAll", "Alle Höhen" },
                { "AreaBulldozer.UI.HeightAbove", "Oberhalb / auf Gelände" },
                { "AreaBulldozer.UI.HeightBelow", "Unterhalb Gelände" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "Gilt für Vorschau und Löschen. Objekte werden nach ihrem Ursprung eingeordnet, Netzwerke entlang ihrer Kurve. Bis 0,5 m unter Gelände zählt als Oberfläche. Gemischte Übergangssegmente sind nur bei Alle Höhen auswählbar. Gebäude samt Unterobjekten bleiben eine Einheit." },
                { "AreaBulldozer.UI.UndergroundView", "Untergrundansicht" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "Zeigt unterirdische Objekte mit der Untergrundansicht des Spiels. Ändert nur die Ansicht, nicht den Höhenfilter. Unterhalb schaltet diese Ansicht automatisch ein." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "Farben" },
                { "AreaBulldozer.UI.OpenColorEditor", "Farbeditor öffnen" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "Optionen" },
                { "AreaBulldozer.UI.ToolOptionsSize", "UI-Größe" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "Normal" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "Kompakt" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "Sicherheit" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "Gebäude-Unterobjekte einschließen" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "Netzwerk-Unterobjekte einschließen" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "Zugeordnete Objekte schützen" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "Bestätigungsgrenze" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "Große Auswahl bestätigen" },
                { "AreaBulldozer.UI.ToolOptionsOn", "Ein" },
                { "AreaBulldozer.UI.ToolOptionsOff", "Aus" },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "Verwendet die Standardgröße für Schaltflächen, Symbole und Überschriften." },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "Verkleinert Schaltflächen, Symbole und Überschriften, damit das Werkzeugpanel weniger Platz braucht." },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "Erlaubt die ausgewählten Objektarten auch dann, wenn sie zu einem Gebäude gehören. Der passende Objektfilter muss ebenfalls aktiviert sein. Unterobjekte können nach Gebäudeaktualisierungen erneut erscheinen." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "Erlaubt die ausgewählten Objektarten auch dann, wenn sie zu einer Straße oder einem anderen Netzwerk gehören. Der passende Objektfilter muss ebenfalls aktiviert sein. Unterobjekte können später erneut erzeugt werden." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "Schützt zugeordnete Objekte, deren Besitzer weder als Gebäude noch als Netzwerk erkannt wird. Diese Sicherheitseinstellung sollte aktiviert bleiben." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "Verlangt ab der eingestellten Objektanzahl einen zweiten Klick innerhalb von fünf Sekunden. Die Auswahlform wird während der Bestätigung gelb dargestellt." },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "Legt fest, ab wie vielen ausgewählten Objekten eine zweite Bestätigung erforderlich ist." },
                { "AreaBulldozer.UI.UndoVegetation", "Vegetation wiederherstellen" },
                { "AreaBulldozer.UI.UndoVegetationTooltip", "Stellt Vegetation aus der letzten Löschaktion wieder her, einschließlich Pflanzen an noch vorhandenen Gebäuden und Parks ({count})." },
                { "AreaBulldozer.UI.ToolOptionsClose", "Optionen schließen" },
                { "AreaBulldozer.UI.CloseColorEditor", "Farbeditor schließen" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "Passt die Farben der Auswahl, Bestätigung, Löschrückmeldung, ausgewählten Flächen und ungültigen Polygone mit direkter Vorschau an." },
                { "AreaBulldozer.UI.ColorEditor", "Farbeditor" },
                { "AreaBulldozer.UI.ColorSelection", "Normale Auswahl" },
                { "AreaBulldozer.UI.ColorConfirmation", "Bestätigung" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "Ungültiges Polygon" },
                { "AreaBulldozer.UI.ColorDeletion", "Aktiver Löschvorgang" },
                { "AreaBulldozer.UI.ColorSurfaces", "Ausgewählte Objekte" },
                { "AreaBulldozer.UI.ColorPresets", "Farbvorgaben" },
                { "AreaBulldozer.UI.ChooseColor", "Farbe auswählen" },
                { "AreaBulldozer.UI.ColorRed", "Rot" },
                { "AreaBulldozer.UI.ColorGreen", "Grün" },
                { "AreaBulldozer.UI.ColorBlue", "Blau" },
                { "AreaBulldozer.UI.ColorHex", "Hex-Wert" },
                { "AreaBulldozer.UI.ResetCurrentColor", "Diese Farbe zurücksetzen" },
                { "AreaBulldozer.UI.ResetAllColors", "Alle Farben zurücksetzen" },
                { "AreaBulldozer.UI.ToggleTool", "Area Bulldozer umschalten" },
                { "AreaBulldozer.UI.LauncherDragHint", "Strg + Linksklick halten und ziehen; loslassen zum Speichern" },
                { "AreaBulldozer.UI.ModMenuOpen", "Werkzeug öffnen" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "Area Bulldozer öffnen und mehrere Objekte mit einer Kreis- oder Quadratauswahl entfernen." },
                { "AreaBulldozer.UI.Navigation", "Navigation" },
                { "AreaBulldozer.UI.BackToBulldozer", "Zurück zum normalen Bulldozer" },
                { "AreaBulldozer.UI.Close", "Werkzeug schlie\u00DFen" },
                { "AreaBulldozer.UI.Active", "Aktiv" },
                { "AreaBulldozer.UI.Subtitle", "Mehrere Objekte in einem Bereich entfernen" },
                { "AreaBulldozer.UI.Selection", "Auswahl" },
                { "AreaBulldozer.UI.Circle", "Kreis" },
                { "AreaBulldozer.UI.Square", "Quadrat" },
                { "AreaBulldozer.UI.CircleTooltip", "Runde Auswahlfläche um den Mauszeiger." },
                { "AreaBulldozer.UI.SquareTooltip", "Quadratische Auswahlfläche, drehbar mit gehaltener rechter Maustaste." },
                { "AreaBulldozer.UI.DecreaseSize", "Auswahl verkleinern" },
                { "AreaBulldozer.UI.IncreaseSize", "Auswahl vergrößern" },
                { "AreaBulldozer.UI.RotateLeft", "Gegen den Uhrzeigersinn drehen" },
                { "AreaBulldozer.UI.RotateRight", "Im Uhrzeigersinn drehen" },
                { "AreaBulldozer.UI.MissingAssets", "Fehlende Assets" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "Weiße Platzhalter bestätigter fehlender Fahrzeuge, Pflanzen und Objekte löschen, auch als Unterobjekte. Gebäude und Netzwerke sind nicht enthalten." },
                { "AreaBulldozer.UI.VegetationTooltip", "Bäume, Büsche und Pflanzen löschen." },
                { "AreaBulldozer.UI.BuildingsTooltip", "Gebäude im Bereich löschen." },
                { "AreaBulldozer.UI.Cars", "Autos" },
                { "AreaBulldozer.UI.CarsTooltip", "Fahrende und geparkte Straßenfahrzeuge entfernen. Pausiert automatisch und stellt die vorherige Geschwindigkeit nur wieder her, wenn die Mod selbst pausiert hat." },
                { "AreaBulldozer.UI.RoadsTooltip", "Straßen im Bereich löschen." },
                { "AreaBulldozer.UI.PathsTooltip", "Fuß- und Radwege löschen." },
                { "AreaBulldozer.UI.RailwaysTooltip", "Schienenwege löschen." },
                { "AreaBulldozer.UI.SurfacesTooltip", "Oberflächen und Bereiche löschen." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "Hauptschalter für Props und Marker. Aktiviert die Detailfilter darunter." },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "Props / Marker" },
                { "AreaBulldozer.UI.Radius", "Radius" },
                { "AreaBulldozer.UI.HalfSide", "Gr\u00F6\u00DFe Quadrat" },
                { "AreaBulldozer.UI.Rotation", "Drehung" },
                { "AreaBulldozer.UI.Degrees", "Grad" },
                { "AreaBulldozer.UI.MainFilters", "Objektfilter" },
                { "AreaBulldozer.UI.ChooseFilters", "Auswahl festlegen" },
                { "AreaBulldozer.UI.Vegetation", "B\u00E4ume und Vegetation" },
                { "AreaBulldozer.UI.VegetationShort", "Vegetation" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "Asset-Vegetation schützen" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "Schützt Pflanzen, die zu Gebäuden oder anderen Assets gehören, beim Löschen mit dem Vegetationsfilter." },
                { "AreaBulldozer.UI.TreeAgeFilter", "Baumalter" },
                { "AreaBulldozer.UI.TreeAgeAll", "Alle Pflanzen" },
                { "AreaBulldozer.UI.TreeAgeSapling", "Setzlinge" },
                { "AreaBulldozer.UI.TreeAgeYoung", "Jung" },
                { "AreaBulldozer.UI.TreeAgeMature", "Ausgewachsen" },
                { "AreaBulldozer.UI.TreeAgeElderly", "Alt" },
                { "AreaBulldozer.UI.TreeAgeDead", "Tot" },
                { "AreaBulldozer.UI.TreeAgePrevious", "Vorherige Altersstufe" },
                { "AreaBulldozer.UI.TreeAgeNext", "Nächste Altersstufe" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "Erlaubt zusammen mit dem Vegetationsfilter das Löschen von überschriebenen beziehungsweise mit Anarchy platzierten Bäumen." },
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
                { "AreaBulldozer.UI.DimBackground", "Hintergrund abdunkeln" },
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
                { "AreaBulldozer.UI.ShortcutHelp", "Werkzeug umschalten" },

                { "AreaBulldozer.UI.Triangle", "Dreieck" },
                { "AreaBulldozer.UI.TriangleTooltip", "Gleichseitige Auswahlfläche um den Mauszeiger. Die Größe entspricht dem Abstand vom Mittelpunkt zur Ecke. Mit gehaltener rechter Maustaste drehbar." },
                { "AreaBulldozer.UI.TriangleSize", "Größe Dreieck" },
                { "AreaBulldozer.UI.MultiPointLine", "Mehrpunktlinie" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "Korridor aus 2 bis 25 Punkten mit geraden oder abgerundeten Übergängen. Linksklick setzt Punkte. Doppelklick am letzten Punkt schließt die Mehrpunktlinie ab und löscht. Rechtsklick entfernt den letzten Punkt, Esc verwirft die komplette Auswahl." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "Breite Mehrpunktlinie" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "Korridor schmaler" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "Korridor breiter" },
                { "AreaBulldozer.UI.PolylineStyle", "Linienform" },
                { "AreaBulldozer.UI.PolylineStraight", "Gerade" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "Verbindet alle gesetzten Punkte mit geraden Segmenten." },
                { "AreaBulldozer.UI.PolylineCurved", "Kurve" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "Rundet die Übergänge zwischen den gesetzten Punkten weich ab. Die Kurve bleibt kontrolliert zwischen den angrenzenden Segmenten und überschwingt nicht." },
                { "AreaBulldozer.UI.PolylineRounding", "Kurvenrundung" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "Rundung verringern" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "Rundung erhöhen" },
                { "AreaBulldozer.UI.Spline", "Mehrpunktlinie" },
                { "AreaBulldozer.UI.SplineTooltip", "Korridor aus 2 bis 25 Punkten. Linksklick setzt Punkte. Doppelklick schließt ab und löscht. Rechtsklick entfernt den letzten Punkt, Esc bricht ab." },
                { "AreaBulldozer.UI.SplineWidth", "Breite Mehrpunktlinie" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "Freie Fläche" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "Freie Fläche" },
                { "AreaBulldozer.UI.PolygonPoints", "Punkte" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "Beliebig viele Eckpunkte definieren eine freie Löschfläche. Linksklick setzt Punkte. Klick auf den ersten Punkt oder Doppelklick schließt das Polygon und löscht den Inhalt. Rechtsklick entfernt den letzten Punkt, Esc verwirft die Auswahl. Selbstüberschneidende Kanten werden blockiert." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Diagnose-Logging aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Schreibt zusätzliche Info- und Diagnosemeldungen in das Log. Standardmäßig deaktiviert. Warnungen und Fehler werden unabhängig von dieser Einstellung weiterhin protokolliert." }
            };
        }

        public void Unload()
        {
        }
    }
}
