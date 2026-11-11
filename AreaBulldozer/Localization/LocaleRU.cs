using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleRU : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleRU(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Основное" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "Безопасность" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "Интерфейс и запуск" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "Клавиши" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "Информация" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "Удалить деревья и растительность" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "Нацелена на деревья, кустарники и другую растительность в зоне отбора." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Включить Anarchy растительность" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Допускает удаление перечеркнутых или помещенных Anarchy деревьев и растительности." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "Удалять здания" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "Застройки, маркировки и проезжие полосы, принадлежащие зданиям, не считаются отдельными зданиями." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "Удалять машины" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "Убирает движущиеся и стоявшие в зоне отбора автомобильные транспортные средства." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "Удалять дороги" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "Цели завершают основные дорожные края, кривая которых пересекает избирательную зону." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "Удалить пешеходные тропы" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "Целью являются полные самостоятельные тропы пешеходов, маршрут которых пересекается с зоной отбора. SubNets остаются исключенными." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "Удаление железнодорожных путей" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "Целью полных автономных железнодорожных, трамвайных, метрополитенных и метрополитенных рельсов, маршрут которых пересекается с выборочной зоной. SubNets остаются исключенными." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "Удалить поверхности и пространства" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "Цели полны полигональные поверхности и пространства, когда зона отбора пересекает их границу или ее центр находится внутри области." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "Удалить статические объекты" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "Включает выбранные категории реквизитов и маркеров ниже. Видимые реквизиты контролируются непосредственно по их категории; чувствительная активность, реквизиты и маркеры полосы по-прежнему уважают варианты безопасности подобъекта." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "Реквизит и декор" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "Цель статических реквизитов, которые не относятся к одной из более конкретных категорий." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "Уличные огни" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "Цель объекта с компонентом уличного освещения, включая лампы, принадлежащие зданиям или сетям." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "Мусорные контейнеры и количественные предметы" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "Целью объектов количества, которые могут быть мусорные контейнеры и аналогичные приспособления." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "Брендинг и реклама" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "Нацелены на предметы бренда, рекламу и подобные украшения." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "Точки активности" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "Целевая деятельность активов. Они могут повлиять на поведение здания и отключаются по умолчанию." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "Точки появления (синие)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "Нацелена на синие маркеры SpawnLocation, где могут появляться транспортные средства или другие объекты активов." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Линии активов / SubLanes - зеленый и синий" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Нацелены на зеленые и синие полосы транспортных средств, пешеходов и парковочных полос внутри зданий. SubLanes использование NetLanePrefab или NetLaneGeometryPrefab выбираются; обычные дороги остаются исключенными." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "Затемнить фон" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Загружает фона перед очертанием игры, пока включены фильтры маркеров или отсутствующие активы." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Затемнение фона" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Устанавливает силу оттуманивания в процентах." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "Толщина контура" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "Устанавливает фиксированную толщину очертания для круговых и квадратных форм выбора." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "Защита растений объектов" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "Сохраняет растения, принадлежащие к зданиям или другим активам при удалении с помощью фильтра растений." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Включать подобъекты зданий" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Позволяет выбирать типы объектов, когда они принадлежат зданию. Также должен быть включен соответствующий объектный фильтр. Субобъекты могут регенерировать после обновления здания." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Включать подобъекты сетей" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Допускает выбранные типы объектов, когда они принадлежат к дороге или другой сети." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "Защищать привязанные объекты" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "Защищает принадлежащие объекты, владелец которых не признан зданием или сетью." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "Квадратная область" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "Запрещается: круглый выбор. Запрещается: свободно вращающийся квадрат. Задержите правое кнопку мыши и переместите мышь горизонтально, чтобы вращаться." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "Размер области по умолчанию" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "Устанавливает размеры в метрах: радиус круга или квадратная длина половины стороны." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "Масштаб окна" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Масштабирует окно Area Bulldozer от 75 до 125 процентов. Более низкие значения обеспечивают больше места; более высокие значения улучшают читаемость на дисплеях с высоким разрешением." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer местоположение пускового устройства" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "Выбирает, запускается ли Area Bulldozer с отдельного пускового устройства, с обычных вариантов инструмента для бульдозера или с универсального меню модулей." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "Отдельная кнопка" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "Обычный бульдозер" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "Общее меню модов" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "Сделайте плавучую кнопку подвижной" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Разрешает перемещать отдельный пусковый аппарат с помощью кнопки мыши Ctrl + левый. Освобождение кнопки мыши сохраняет положение." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "Сбросить положение кнопки" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "Перемещает переносный пусковый аппарат обратно в его дефолтное положение в левом верхнем углу." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "Подтверждать большую выборку" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "Требуется второй клик в течение пяти секунд, когда выбран конфигурированный номер объектов. Форма выбора становится желтой, пока ожидается подтверждение." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "Порог подтверждения" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "Устанавливает количество выбранных объектов, для которых требуется второе подтверждение." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "Активировать Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "Активация или деактивация инструмента." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Сбросить клавиши" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Реставрация клавиша-связки на его дефолтовое значение." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "Версия" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "Показывает в настоящее время установленную версию Area Bulldozer." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "Активировать Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "Подземный фон" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "Только на подземном изображении: белый = оригинальный, более темные цвета = затуманение / оттенок." },
                { "AreaBulldozer.UI.HeightFilter", "Фильтр по высоте" },
                { "AreaBulldozer.UI.HeightAll", "Все высоты" },
                { "AreaBulldozer.UI.HeightAbove", "На земле и выше" },
                { "AreaBulldozer.UI.HeightBelow", "Под землёй" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "Применяется для предварительного просмотра и удаления. Объекты используют свое происхождение; сети пробиваются вдоль их кривой. До 0,5 м ниже местности считается поверхностью. Смешанные переходные сегменты выбираются только с Все высоты. Здания и их подобъекты остаются одной единицей." },
                { "AreaBulldozer.UI.UndergroundView", "Подземный вид" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "Показывает подземные объекты с помощью игрового подземного вида. изменяет вид только, а не высота фильтра." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "Цвета" },
                { "AreaBulldozer.UI.OpenColorEditor", "Открыть редактор цветов" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "Настройки" },
                { "AreaBulldozer.UI.ToolOptionsSize", "Размер интерфейса" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "Обычный" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "Компактный" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "Безопасность" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "Включать подобъекты зданий" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "Включать подобъекты сетей" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "Защищать привязанные объекты" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "Порог подтверждения" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "Подтверждать массовый снос" },
                { "AreaBulldozer.UI.ToolOptionsOn", "Вкл." },
                { "AreaBulldozer.UI.ToolOptionsOff", "Выкл." },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "Использует стандартный размер для кнопок, икон и заголовков." },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "Уменьшает количество кнопок, икон и заголовков, так что панель инструментов занимает меньше места." },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "Позволяет выбирать типы объектов, когда они принадлежат зданию. Также должен быть включен соответствующий объектный фильтр. Субобъекты могут регенерировать после обновления здания." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "Допускает выбранные типы объектов, когда они принадлежат к дороге или другой сети." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "Защищает принадлежащие объекты, владелец которых не признан зданием или сетью." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "Требуется второй клик в течение пяти секунд, когда выбран конфигурированный номер объектов. Форма выбора становится желтой, пока ожидается подтверждение." },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "Устанавливает количество выбранных объектов, для которых требуется второе подтверждение." },
                { "AreaBulldozer.UI.ToolOptionsClose", "Закрыть настройки" },
                { "AreaBulldozer.UI.CloseColorEditor", "Закрыть редактор цветов" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "Регулирует отбор, подтверждение, обратную связь с удалением, выбранную поверхность и недействительные цвета многоугольника с живым просмотром." },
                { "AreaBulldozer.UI.ColorEditor", "Редактор цветов" },
                { "AreaBulldozer.UI.ColorSelection", "Обычное выделение" },
                { "AreaBulldozer.UI.ColorConfirmation", "Подтверждение" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "Недопустимый контур" },
                { "AreaBulldozer.UI.ColorDeletion", "Снос" },
                { "AreaBulldozer.UI.ColorSurfaces", "Выбранные объекты" },
                { "AreaBulldozer.UI.ColorPresets", "Готовые цвета" },
                { "AreaBulldozer.UI.ChooseColor", "Выбрать цвет" },
                { "AreaBulldozer.UI.ColorRed", "Красный" },
                { "AreaBulldozer.UI.ColorGreen", "Зелёный" },
                { "AreaBulldozer.UI.ColorBlue", "Синий" },
                { "AreaBulldozer.UI.ColorHex", "Код HEX" },
                { "AreaBulldozer.UI.ResetCurrentColor", "Сбросить этот цвет" },
                { "AreaBulldozer.UI.ResetAllColors", "Сбросить все цвета" },
                { "AreaBulldozer.UI.ToggleTool", "Вкл. / выкл. Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "Держите Ctrl + левую кнопку мыши, чтобы двигаться; отпустите, чтобы сохранить" },
                { "AreaBulldozer.UI.ModMenuOpen", "Открыть инструмент" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "Откройте Area Bulldozer и удалите несколько объектов с помощью кругового или квадратного отбора." },
                { "AreaBulldozer.UI.Navigation", "Навигация" },
                { "AreaBulldozer.UI.BackToBulldozer", "Вернемся к обычному бульдозеру" },
                { "AreaBulldozer.UI.Close", "Закрыть инструмент" },
                { "AreaBulldozer.UI.Active", "Активный" },
                { "AreaBulldozer.UI.Subtitle", "Удалить несколько объектов в одной области" },
                { "AreaBulldozer.UI.Selection", "Выделение" },
                { "AreaBulldozer.UI.Circle", "Круг" },
                { "AreaBulldozer.UI.Square", "Квадрат" },
                { "AreaBulldozer.UI.CircleTooltip", "Круглое избирательное пространство вокруг курсора." },
                { "AreaBulldozer.UI.SquareTooltip", "Площадь выбора квадратной, вращаемая при помощи правой кнопки мыши." },
                { "AreaBulldozer.UI.DecreaseSize", "Уменьшить область" },
                { "AreaBulldozer.UI.IncreaseSize", "Увеличить область" },
                { "AreaBulldozer.UI.RotateLeft", "Поворачивать в обратном направлении" },
                { "AreaBulldozer.UI.RotateRight", "Поворачивайте в направлении часа" },
                { "AreaBulldozer.UI.MissingAssets", "Отсутствующие объекты" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "Удалить белые помещения подтвержденных отсутствий транспортных средств, растений и объектов, включая подобъекты." },
                { "AreaBulldozer.UI.VegetationTooltip", "Удалить деревья, кустарники и растения." },
                { "AreaBulldozer.UI.BuildingsTooltip", "Удалить здания в этом районе." },
                { "AreaBulldozer.UI.Cars", "Автомобили" },
                { "AreaBulldozer.UI.CarsTooltip", "Игра автоматически останавливается и восстанавливает свою прежнюю скорость только в том случае, если модуль остановил ее." },
                { "AreaBulldozer.UI.RoadsTooltip", "Удалить дороги в этом районе." },
                { "AreaBulldozer.UI.PathsTooltip", "Удалить пешеходные и велосипедные трассы." },
                { "AreaBulldozer.UI.RailwaysTooltip", "Удалить железнодорожные пути." },
                { "AreaBulldozer.UI.SurfacesTooltip", "Удалить поверхности и зоны." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "Главный переключатель для реквизитов и маркеров." },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "Реквизит / маркеры" },
                { "AreaBulldozer.UI.Radius", "Радий" },
                { "AreaBulldozer.UI.HalfSide", "Сторона квадрата" },
                { "AreaBulldozer.UI.Rotation", "Поворот" },
                { "AreaBulldozer.UI.Degrees", "дег" },
                { "AreaBulldozer.UI.MainFilters", "Фильтры объектов" },
                { "AreaBulldozer.UI.ChooseFilters", "Выберите фильтры" },
                { "AreaBulldozer.UI.Vegetation", "Деревья и растительность" },
                { "AreaBulldozer.UI.VegetationShort", "Растительность" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "Защита растений объектов" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "Сохраняет растения, принадлежащие к зданиям или другим активам при удалении с помощью фильтра растений." },
                { "AreaBulldozer.UI.TreeAgeFilter", "Возраст деревьев" },
                { "AreaBulldozer.UI.TreeAgeAll", "Все растения" },
                { "AreaBulldozer.UI.TreeAgeSapling", "Саженцы" },
                { "AreaBulldozer.UI.TreeAgeYoung", "Молодые" },
                { "AreaBulldozer.UI.TreeAgeMature", "Взрослые" },
                { "AreaBulldozer.UI.TreeAgeElderly", "Старые" },
                { "AreaBulldozer.UI.TreeAgeDead", "Погибшие" },
                { "AreaBulldozer.UI.TreeAgePrevious", "Предыдущий возрастный этап" },
                { "AreaBulldozer.UI.TreeAgeNext", "Следующий возрастный этап" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "Вместе с фильтром растительности, позволяет удалить перевернутые или Anarchy - размещенные деревья." },
                { "AreaBulldozer.UI.Buildings", "Здания" },
                { "AreaBulldozer.UI.Roads", "Дороги" },
                { "AreaBulldozer.UI.Paths", "Пешеходные пути" },
                { "AreaBulldozer.UI.Railways", "Железнодорожные пути" },
                { "AreaBulldozer.UI.Surfaces", "Поверхности и пространства" },
                { "AreaBulldozer.UI.SurfacesShort", "Поверхности" },
                { "AreaBulldozer.UI.StaticObjects", "Реквизит и маркеры" },
                { "AreaBulldozer.UI.StaticObjectsShort", "Реквизит / маркеры" },
                { "AreaBulldozer.UI.AdvancedFilters", "Усовершенствованные фильтры и маркеры" },
                { "AreaBulldozer.UI.StaticMasterNotice", "Определить использование указанных категорий приборов и маркеров." },
                { "AreaBulldozer.UI.GeneralProps", "Общий реквизит" },
                { "AreaBulldozer.UI.StreetLights", "Уличные огни" },
                { "AreaBulldozer.UI.QuantityObjects", "Контейнеры и количественные объекты" },
                { "AreaBulldozer.UI.Branding", "Брендинг и реклама" },
                { "AreaBulldozer.UI.ActivityLocations", "Точки активности" },
                { "AreaBulldozer.UI.SpawnLocations", "Точки появления" },
                { "AreaBulldozer.UI.AssetLanes", "Прокладки активов / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "Затемнить фон" },
                { "AreaBulldozer.UI.BackgroundDarkness", "Затемнение фона" },
                { "AreaBulldozer.UI.Safety", "Безопасность" },
                { "AreaBulldozer.UI.BuildingSubObjects", "Подобъекты зданий" },
                { "AreaBulldozer.UI.NetworkSubObjects", "Подобъекты сетей" },
                { "AreaBulldozer.UI.ProtectOwned", "Защищать привязанные объекты" },
                { "AreaBulldozer.UI.ConfirmLarge", "Подтверждать большую выборку" },
                { "AreaBulldozer.UI.Threshold", "Порог подтверждения" },
                { "AreaBulldozer.UI.Objects", "объекты" },
                { "AreaBulldozer.UI.ApplyHelp", "Удалить выбранные объекты" },
                { "AreaBulldozer.UI.RotateHelp", "Держи и тяни, чтобы вращаться" },
                { "AreaBulldozer.UI.ShortcutHelp", "Вкл. / выкл. инструмент" },

                { "AreaBulldozer.UI.Triangle", "Треугольник" },
                { "AreaBulldozer.UI.TriangleTooltip", "Равносторонняя область отбора вокруг курсора. Размер измеряется от центра до угла. Держите правую кнопку мыши и двигайтесь горизонтально, чтобы вратить ее." },
                { "AreaBulldozer.UI.TriangleSize", "Размер треугольника" },
                { "AreaBulldozer.UI.MultiPointLine", "Многоточная линия" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "Коридор с использованием от 2 до 25 точек с прямыми или закругленными переходами. Левый клик добавляет точки. Двойной клик на окончательную точку, чтобы закончить и удалить. Правый клик удаляет последнюю точку; Esc отменяет весь выбор." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "Ширина линии в нескольких точках" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "Сделайте коридор узким" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "Расширить коридор" },
                { "AreaBulldozer.UI.PolylineStyle", "Вид линии" },
                { "AreaBulldozer.UI.PolylineStraight", "Прямой" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "Соединяет все помещенные точки с прямыми сегментами." },
                { "AreaBulldozer.UI.PolylineCurved", "Кривая" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "Окружает переходы между размещенными точками. Кривая остается контролируемой между смежными сегментами и не переходит." },
                { "AreaBulldozer.UI.PolylineRounding", "Скругление" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "Уменьшение закругления" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "Увеличение закругления" },
                { "AreaBulldozer.UI.Spline", "Многоточная линия" },
                { "AreaBulldozer.UI.SplineTooltip", "Коридор с использованием от 2 до 25 точек. Левый клик добавляет точки. Двойной клик заканчивает и удаляет. Правый клик удаляет последнюю точку; Esc отменяет." },
                { "AreaBulldozer.UI.SplineWidth", "Ширина линии в нескольких точках" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "Свободная форма" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "Свободная форма" },
                { "AreaBulldozer.UI.PolygonPoints", "точек" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "Определить зону удаления свободной формы с использованием неограниченного количества угловых точек. Нажмите слева, чтобы разместить точки. Нажмите на первую точку или дважды нажмите, чтобы закрыть многоугольник и удалить все внутри него. Правый клик удаляет последнюю точку, и Esc отменяет текущий выбор." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Включить журнал диагностики" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Пишет дополнительные информационные и диагностические сообщения в журнале. Отключен по умолчанию. Предупреждения и ошибки все еще записываются независимо от этой настройки." }
            };
        }

        public void Unload()
        {
        }
    }
}
