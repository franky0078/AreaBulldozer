using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocalePL : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePL(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Główny" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "Bezpieczeństwo" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "Wyświetlacz i wyrzutnik" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "Skróty klawiszowe" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "Informacje" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "Usunięcie drzew i roślinności" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "Wystarczają drzewa, krzewy i inne roślinności w obszarze wyboru." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Włączyć roślinność Anarchy" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Pozwala usuwać drzewa i roślinność, które zostały przeniesione lub umieszczone w Anarchy." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "Zdejmowanie budynków" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "W budynku znajdują się rekwizyty, markery i pasy, które nie są uważane za oddzielne." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "Wyprowadź samochody" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "Wyjmuje ruchome i zaparkowane pojazdy drogowe w obszarze wyboru. Gra automatycznie zatrzymuje się, gdy jest włączona." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "Wyciąć drogi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "Celem jest uzupełnienie głównych krawędzi dróg, których krzywa przecina obszar wyboru, a ścieżki, drogi kolejowe, sieci markerów i sieci należące do budynków pozostają wykluczone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "Zdejmij ścieżki piesze" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "Celem jest kompletne autonomiczne krawędzi szlaków pieszych, których trasa przecina obszar wyboru. Drogi, kolei, sieci markerów i budynków SubNets pozostają wykluczone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "Wyprowadzenie torów kolejowych" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "Celem jest kompletne samodzielne pociągi, tramwaje, metra i tory metra, których trasa przecina obszar wyboru. Drogi, ścieżki piesze, sieci markerów i budynki SubNets pozostają wykluczone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "Usunięcie powierzchni i przestrzeni" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "Obszary posiadające aktywa wymagają również dopasowania obszaru podobiektu i mogą regenerację po aktualizacji aktywów." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "Usunięcie obiektów statycznych" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "Wykorzystuje wybrane kategorie rekwizycji i markerów poniżej. Widoczne rekwizycje są sterowane bezpośrednio przez ich kategorię; czułe aktywności, spawn i markery pasów nadal przestrzegają opcji bezpieczeństwa podobiektu." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "Główne rekwizyty i dekoracje" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "Celują statyczne rekwizyty, które nie należą do jednej z bardziej szczegółowych kategorii." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "Światła uliczne" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "Celuje się na obiekty z elementem światła ulicznego, w tym na światła należące do budynków lub sieci." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "Wysyłki i obiekty ilościowe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "Celuje na obiekty wielkości, które mogą obejmować śmietnik i podobne rekwizyty." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "Marka i reklamy" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "Celuje się na przedmioty marki, reklamy i podobne dekoracje." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "Miejsca działalności" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "Celuje lokalizacje aktywów, które mogą wpływać na zachowanie budynku i są domyślnie wyłączone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "Miejsce lombardu sinum blue" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "Celem są niebieskie SpawnLocation markery, w których pojazdy lub inne obiekty aktywów mogą pojawić się." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Pasy startowe / SubLanes Zielony i niebieski" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Celem jest zielone i niebieskie pasy pojazdów, pieszych i parkingowych w środku budynków. SubLanes wykorzystując NetLanePrefab lub NetLaneGeometryPrefab są wybrane; normalne krawędzie dróg pozostają wykluczone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "Zmłodny tło" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Zmniejsza tło przed wytyczką gry, a filtry markerów lub brakujące aktywa są włączone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Ciemność na tle" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Ustawia siłę osłabania w %." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "Gęstość linii wyboru" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "Ustawia stałą grubość konturów dla kružnych i kwadratowych kształtów wyboru." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "Ochrona roślinności aktywów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "Pozostawia rośliny należące do budynków lub innych aktywów podczas usuwania filtra roślinności." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Włączyć podobiekty budowlane" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Wybrane typy obiektów, gdy należą do budynku. Filtr obiektów pasujących musi być również włączony. Podobjekty mogą regenerować po aktualizacjach budynku." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Włączyć podobiekty sieci" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Pozwala na wybranie typów obiektów, gdy należą do drogi lub innej sieci. Filtr obiektów pasujących musi być również włączony." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "Ochrona własnych obiektów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "Ochrona obiektów własnych, których właściciel nie jest uznany za budynek lub sieć." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "Użyj wyboru kwadratowego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "Włączony: okrągły wybór. Umożliwiony: swobodnie obracający się kwadrat. Trzymaj prawą przyciskę myszy i ruszaj myszą horyzontalnie, aby ją obracać." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "Wielkość wyboru domyślnego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "Ustawia wielkość w metrach: promienie okręgu lub długość półstron kwadratowych." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "Skala okna narzędzia" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Wskalewanie okna Area Bulldozer z 75 do 125 procent. niższe wartości zapewniają więcej miejsca; wyższe wartości poprawią czytelność na wyświetlaczach o wysokiej rozdzielczości." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer lokalizacja wyrzucenia" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "Wybiera, czy Area Bulldozer jest uruchomiony z oddzielnego uruchamiacza, z normalnych opcji narzędzi Bulldozer lub z uniwersalnego menu mod." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "Przycisk samodzielny" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "Zwykły buldozer" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "Uniwersalne menu mod" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "Wykorzystaj pływający przycisk" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Pozwala przenosić oddzielny uruchamiacz przy użyciu Ctrl + lewego przycisku myszy. Uwolnienie przycisku myszy oszczędza pozycję. Opcja ta ma zastosowanie tylko wtedy, gdy lokalizacja uruchamiacza jest ustawiona na przycisk 'Standalone'." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "Pozycja przycisku resetowania" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "Przenosi odciągający wyrzutek do domyślnej pozycji w lewym górnym rogu." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "Potwierdź duże wybory" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "Po wybraniu ustawionej liczby obiektów wymagane jest powtórne kliknięcie w ciągu pięciu sekund." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "Praga potwierdzenia" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "Ustawia liczbę wybranych obiektów, w których wymagane jest drugie potwierdzenie." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "Aktywowanie Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "Aktywuje lub wyłącza narzędzie." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Przywiązania klucza do resetowania" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Przywiązanie klucza do wartości domyślnej." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "Wyświetla obecnie zainstalowaną wersję Area Bulldozer." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "Aktywowanie Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "Podgórny tło" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "Wystarczy wyświetlanie pod ziemią: białe = oryginalne, ciemniejsze kolory = osłabiające / odcienie." },
                { "AreaBulldozer.UI.HeightFilter", "Filtr wysokości" },
                { "AreaBulldozer.UI.HeightAll", "Wszystkie wysokości" },
                { "AreaBulldozer.UI.HeightAbove", "W górę / na terenie" },
                { "AreaBulldozer.UI.HeightBelow", "Pod terenem" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "Obejrzenie i usunięcie. Obiekty używają ich pochodzenia; sieci są próbkowane wzdłuż ich krzywej. Do 0,5 m poniżej terenu liczy się jako powierzchnia. Segmenty przejściowe mieszane są wybierane tylko z Wszystkimi wysokościami. Budynki i ich podobiekty pozostają jedną jednostką." },
                { "AreaBulldozer.UI.UndergroundView", "Widok podziemny" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "Pokazuje obiekty podziemne za pomocą widoku pod ziemią gry. Zmienia widok tylko, a nie filtr wysokości." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "Kolory" },
                { "AreaBulldozer.UI.OpenColorEditor", "Otwarty edytor kolorów" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "Opcje" },
                { "AreaBulldozer.UI.ToolOptionsSize", "Wielkość użytkownika" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "Normalny" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "Kompaktowy" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "Bezpieczeństwo" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "Włączyć podobiekty budowlane" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "Włączyć podobiekty sieci" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "Ochrona własnych obiektów" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "Praga potwierdzenia" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "Potwierdzenie dużych usunięć" },
                { "AreaBulldozer.UI.ToolOptionsOn", "Wł." },
                { "AreaBulldozer.UI.ToolOptionsOff", "Wył." },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "Używa standardowych rozmiarów dla przycisków, ikon i nagłówków." },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "To zmniejsza przyciski, ikony i nagłówki, więc panel narzędzi zajmuje mniej miejsca." },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "Wybrane typy obiektów, gdy należą do budynku. Filtr obiektów pasujących musi być również włączony. Podobjekty mogą regenerować po aktualizacjach budynku." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "Pozwala na wybranie typów obiektów, gdy należą do drogi lub innej sieci. Filtr obiektów pasujących musi być również włączony." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "Ochrona obiektów własnych, których właściciel nie jest uznany za budynek lub sieć." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "Po wybraniu ustawionej liczby obiektów wymagane jest powtórne kliknięcie w ciągu pięciu sekund." },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "Ustawia liczbę wybranych obiektów, w których wymagane jest drugie potwierdzenie." },
                { "AreaBulldozer.UI.UndoVegetation", "Przywróć roślinność" },
                { "AreaBulldozer.UI.UndoVegetationTooltip", "Przywraca roślinność usuniętą w ostatniej operacji, także rośliny należące do istniejących budynków i parków ({count})." },
                { "AreaBulldozer.UI.ToolOptionsClose", "Opcje zamknięte" },
                { "AreaBulldozer.UI.CloseColorEditor", "Edytora kolorowego" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "Dostosowuje wybór, potwierdzenie, odbiór usunięcia, wybrane powierzchnie i nieprawidłowe kolory wielokątów z przeglądem na żywo." },
                { "AreaBulldozer.UI.ColorEditor", "Edytor kolorów" },
                { "AreaBulldozer.UI.ColorSelection", "Normalny wybór" },
                { "AreaBulldozer.UI.ColorConfirmation", "Potwierdzenie" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "Nieprawidłowy wielokąt" },
                { "AreaBulldozer.UI.ColorDeletion", "Aktywne usunięcie" },
                { "AreaBulldozer.UI.ColorSurfaces", "Wybrane obiekty" },
                { "AreaBulldozer.UI.ColorPresets", "Ustawienia kolorów" },
                { "AreaBulldozer.UI.ChooseColor", "Wybierz kolor" },
                { "AreaBulldozer.UI.ColorRed", "Czerwony" },
                { "AreaBulldozer.UI.ColorGreen", "Zielony" },
                { "AreaBulldozer.UI.ColorBlue", "Niebieski" },
                { "AreaBulldozer.UI.ColorHex", "Kod HEX" },
                { "AreaBulldozer.UI.ResetCurrentColor", "/ Zastosowanie koloru" },
                { "AreaBulldozer.UI.ResetAllColors", "Odresetowanie wszystkich kolorów" },
                { "AreaBulldozer.UI.ToggleTool", "Wykrycie Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "Trzymaj Ctrl + lewy przycisk myszy do przesuwania; uwolnić do zapisywania" },
                { "AreaBulldozer.UI.ModMenuOpen", "Narzędzie otwarte" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "Otwórz Area Bulldozer i usuwaj wiele obiektów poprzez okrągły lub kwadratowy wybór." },
                { "AreaBulldozer.UI.Navigation", "Nawigacja" },
                { "AreaBulldozer.UI.BackToBulldozer", "Wracamy do normalnego bulldozera" },
                { "AreaBulldozer.UI.Close", "Narzędzie zamknięte" },
                { "AreaBulldozer.UI.Active", "Aktywny" },
                { "AreaBulldozer.UI.Subtitle", "Usunięcie kilku obiektów w jednym obszarze" },
                { "AreaBulldozer.UI.Selection", "Wybór" },
                { "AreaBulldozer.UI.Circle", "Koło" },
                { "AreaBulldozer.UI.Square", "Kwadrat" },
                { "AreaBulldozer.UI.CircleTooltip", "Okrągły obszar wyboru wokół kursoru." },
                { "AreaBulldozer.UI.SquareTooltip", "Obszar wyboru kwadratowy, można go obracać przyciskając prawą przyciskę myszy." },
                { "AreaBulldozer.UI.DecreaseSize", "Wybór skrzyżowania" },
                { "AreaBulldozer.UI.IncreaseSize", "Wybór roślin" },
                { "AreaBulldozer.UI.RotateLeft", "Obrót w kierunku przeciwnego kierunku zegara" },
                { "AreaBulldozer.UI.RotateRight", "Zwrot w kierunku zegarka" },
                { "AreaBulldozer.UI.MissingAssets", "Brakujące aktywa" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "Wyłączenie białych miejsc pojazdów, zakładów i obiektów, w tym podobiektów, które zostały potwierdzone jako zaginione." },
                { "AreaBulldozer.UI.VegetationTooltip", "Wymazać drzewa, krzewy i rośliny." },
                { "AreaBulldozer.UI.BuildingsTooltip", "Wymazać budynki w okolicy." },
                { "AreaBulldozer.UI.Cars", "Samochody" },
                { "AreaBulldozer.UI.CarsTooltip", "Wyklucza ruchliwe i zaparkowane pojazdy drogowe. gra automatycznie zatrzymuje się i przywraca swoją wcześniejszą prędkość tylko wtedy, gdy mod zatrzyma go." },
                { "AreaBulldozer.UI.RoadsTooltip", "Wymazać drogi w okolicy." },
                { "AreaBulldozer.UI.PathsTooltip", "Usunięcie ścieżek pieszych i rowerowych." },
                { "AreaBulldozer.UI.RailwaysTooltip", "Wymazać ścieżki." },
                { "AreaBulldozer.UI.SurfacesTooltip", "Usuń powierzchnie i obszary." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "Przełącznik główny do rekwizytów i markerów." },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "Rekwizyty / markery" },
                { "AreaBulldozer.UI.Radius", "Promień" },
                { "AreaBulldozer.UI.HalfSide", "Rozmiar kwadratowy" },
                { "AreaBulldozer.UI.Rotation", "Rotacja" },
                { "AreaBulldozer.UI.Degrees", "°" },
                { "AreaBulldozer.UI.MainFilters", "Filtry obiektów" },
                { "AreaBulldozer.UI.ChooseFilters", "Wybierz filtry" },
                { "AreaBulldozer.UI.Vegetation", "Drzewa i roślinność" },
                { "AreaBulldozer.UI.VegetationShort", "Roślinność" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "Ochrona roślinności aktywów" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "Pozostawia rośliny należące do budynków lub innych aktywów podczas usuwania filtra roślinności." },
                { "AreaBulldozer.UI.TreeAgeFilter", "Wiek drzew" },
                { "AreaBulldozer.UI.TreeAgeAll", "Wszystkie rośliny" },
                { "AreaBulldozer.UI.TreeAgeSapling", "Sadzonki" },
                { "AreaBulldozer.UI.TreeAgeYoung", "Młode" },
                { "AreaBulldozer.UI.TreeAgeMature", "Dojrzałe" },
                { "AreaBulldozer.UI.TreeAgeElderly", "Stare" },
                { "AreaBulldozer.UI.TreeAgeDead", "Martwe" },
                { "AreaBulldozer.UI.TreeAgePrevious", "Poprzedni etap wieku" },
                { "AreaBulldozer.UI.TreeAgeNext", "Następny etap wieku" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "Wraz z filtrem roślinności, pozwala usuwać drzewa, które zostały zaniedbone lub Anarchy umieszczone." },
                { "AreaBulldozer.UI.Buildings", "Budynki" },
                { "AreaBulldozer.UI.Roads", "Drogi" },
                { "AreaBulldozer.UI.Paths", "Ścieżki piesze" },
                { "AreaBulldozer.UI.Railways", "Szlaków kolejowych" },
                { "AreaBulldozer.UI.Surfaces", "Powierzchnie i przestrzenie" },
                { "AreaBulldozer.UI.SurfacesShort", "Powierzchnie" },
                { "AreaBulldozer.UI.StaticObjects", "Rekwizyty i markery" },
                { "AreaBulldozer.UI.StaticObjectsShort", "Rekwizyty / markery" },
                { "AreaBulldozer.UI.AdvancedFilters", "Zaawansowane filtry rekwizytowe i markery" },
                { "AreaBulldozer.UI.StaticMasterNotice", "Aktywuj rekwizytory i markery powyżej do użycia tych kategorii." },
                { "AreaBulldozer.UI.GeneralProps", "Główne rekwizyty" },
                { "AreaBulldozer.UI.StreetLights", "Światła uliczne" },
                { "AreaBulldozer.UI.QuantityObjects", "Zbiorniki i obiekty ilościowe" },
                { "AreaBulldozer.UI.Branding", "Marka i reklamy" },
                { "AreaBulldozer.UI.ActivityLocations", "Miejsca działalności" },
                { "AreaBulldozer.UI.SpawnLocations", "Lokalizacje spawnów" },
                { "AreaBulldozer.UI.AssetLanes", "Ścieżki aktywów / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "Zmłodny tło" },
                { "AreaBulldozer.UI.BackgroundDarkness", "Ciemność na tle" },
                { "AreaBulldozer.UI.Safety", "Bezpieczeństwo" },
                { "AreaBulldozer.UI.BuildingSubObjects", "Podobjekty budowlane" },
                { "AreaBulldozer.UI.NetworkSubObjects", "Podobjekty sieci" },
                { "AreaBulldozer.UI.ProtectOwned", "Ochrona własnych obiektów" },
                { "AreaBulldozer.UI.ConfirmLarge", "Potwierdź duże wybory" },
                { "AreaBulldozer.UI.Threshold", "Praga potwierdzenia" },
                { "AreaBulldozer.UI.Objects", "obiekty" },
                { "AreaBulldozer.UI.ApplyHelp", "Usuwanie wybranych obiektów" },
                { "AreaBulldozer.UI.RotateHelp", "Trzymać i ciągnąć do obrotu" },
                { "AreaBulldozer.UI.ShortcutHelp", "Narzędzie przełączania" },

                { "AreaBulldozer.UI.Triangle", "Trójkąt" },
                { "AreaBulldozer.UI.TriangleTooltip", "Obszar wyboru w równej stronie wokół kurzora. Rozmiar mierzy się od środka do rogu." },
                { "AreaBulldozer.UI.TriangleSize", "Wielkość trójkąta" },
                { "AreaBulldozer.UI.MultiPointLine", "Linia wielopunktowa" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "Korytarz wykorzystujący od 2 do 25 punktów z prostymi lub zaokrąglonymi przejściami. Esc odwołuje cały wybór." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "Szerokość linii wielokrotnych punktów" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "Użnij korytarz" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "Rozszerzyć korytarz" },
                { "AreaBulldozer.UI.PolylineStyle", "Styl linii" },
                { "AreaBulldozer.UI.PolylineStraight", "Wprost" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "Łączy wszystkie umieszczone punkty z segmentami prostymi." },
                { "AreaBulldozer.UI.PolylineCurved", "Kręgosłup" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "Zaokrągla przejścia między punktami umieszczonymi. Krzywa pozostaje kontrolowana pomiędzy sąsiadującymi segmentami i nie przewyższa prędkości." },
                { "AreaBulldozer.UI.PolylineRounding", "Zaokrąglony zakręt" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "Zmniejszenie zaokrąglania" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "Zwiększenie zaokrąglania" },
                { "AreaBulldozer.UI.Spline", "Linia wielopunktowa" },
                { "AreaBulldozer.UI.SplineTooltip", "Korytarz wykorzystujący 2 do 25 punktów. Lewy kliknij dodaje punkty. Podwójne kliknięcie kończy i usuwa. Prawy kliknij usuwa ostatni punkt; Esc anuluje." },
                { "AreaBulldozer.UI.SplineWidth", "Szerokość linii wielokrotnych punktów" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "Obszar wolnej formy" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "Obszar wolnej formy" },
                { "AreaBulldozer.UI.PolygonPoints", "punkty" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "Definiuj obszar usunięcia wolnego kształtu przy użyciu nieograniczonej liczby punktów narożnych. Kliknij w lewo, aby umieścić punkty. Kliknij w pierwszy punkt lub kliknij dwukrotnie, aby zamknąć wielokąt i usunąć wszystko w nim. Kliknięcie w prawo usuwa ostatni punkt, a Esc anuluje bieżący wybór." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Włącz dziennik diagnostyczny" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Napisa dodatkowe wiadomości informacyjne i diagnostyczne do dziennika. Wyłączone domyślnie." }
            };
        }

        public void Unload()
        {
        }
    }
}
