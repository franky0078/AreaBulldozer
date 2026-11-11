using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleZHHANT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleZHHANT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "主要" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "安全" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "顯示器和發射器" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "按鍵設定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "資訊" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "移除樹木和植物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "針對選擇區內的樹木,灌木和其他植物." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "包含 Anarchy植物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "允許過關或 Anarchy預設無法使用這個選項." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "拆除建築物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "目標完成選區內的根部建築物。 建築物擁有的道具、標記和通道不算作單獨的建築物 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "拆除車輛" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "移除選取區內的移動及停放路面車輛。 遊戲啟動時會自動暫停 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "移除道路" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "目標完成主路邊緣, 其曲線與選擇區相交。 路徑、鐵路、標記網、建築網路仍然被排除在外 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "移除行人路徑" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "目標是完成獨立的步行者路徑,其路徑交叉選拔區域.道路,鐵路,標記網路和建築物所有 SubNets仍然沒有." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "移除鐵路軌跡" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "這項計畫將完成獨立的火車,電車,地鐵和地鐵軌跡,其路線交叉選項區域.道路,步行者路徑,標記網路和建築物所有 SubNets仍然沒有." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "移除表面和空間" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "目標在選項區域交叉他們的邊界或其中心在區域內時完成多角表面和空間. 資產擁有的區域也需要相匹配的子物件範圍,並且可能在資產更新後再生." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "移除靜止物體" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "可選取下面的道具和標記類別.可見的道具直接由其類別控制;敏感的活動,生產率和車道標記仍然尊重子物體安全選項." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "其他裝置:" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "針對不屬於更特定類別的靜電道具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "路燈" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "針對有街道燈具的物體,包括建築物或網路所擁有的燈具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "垃圾桶和數量物件" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "針對數量物體, 可能包括垃圾桶和類似道具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "品牌及廣告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "針對品牌物品,廣告及類似裝飾." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "活動地點" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "目標是資產活動位置. 這些可能影響建築物行為," },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "垃圾桶位置 - 藍色" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "針對車輛或其它資產物體可能出現的藍色 SpawnLocation標記." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "資源通道 / SubLanes — 綠色和藍色" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "標籤是綠色和藍色的車輛, 步行者和停車場. SubLanes 使用 NetLanePrefab 或是 NetLaneGeometryPrefab 選擇;正常的道路邊緣仍不適用." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "暗淡的背景" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "在啟動標籤過錯或漏失資產時, 縮小遊戲大綱前的背景。 大綱和選單保持視窗 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "背景黑暗" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "顯示遊戲視覺減光強度為百分比." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "選擇線的厚度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "設定圓形和方形選取的固定圖框厚度。 其厚度不再隨選取大小而變更 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "保護資產植物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "在植物濾鏡下, 儲存屬於建築物或其他資產的植物." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "包含建築子設施" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "允許在屬於建築物時選擇的物件型別. 必須啟用相匹配的物件濾鏡. 建構更新後,子物件可能會再生." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "包含網路子物件" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "允許選定的物件型別,當他們屬於道路或其他網路. 匹配物件濾鏡也必須啟用. 副物件可能稍後再生." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "保護所有物品" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "保護所有物件,其所有者並不被認為是建築物或網路." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "使用方形選擇" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "禁用 : 圓形選擇 。 啟用 : 自由旋轉的方形。 按住滑鼠右鍵, 水平移動滑鼠以旋轉它。 所配置的大小是圓形半徑或方形半邊長 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "預設選擇尺寸" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "設定尺寸以米:圓半徑或方形半邊長度." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "工具視窗尺度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "在 Area Bulldozer 視窗中,可從 75% 到 125% 的尺寸.較低的值提供更多空間;較高的值提高高解析度顯示器的可讀性." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer 發射器位置" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "選擇是否 Area Bulldozer 從獨立發射器開啟,從正常的Bulldozer工具選項,或從通用模組選單." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "獨立按鍵" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "常見的牛排機" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "全球模組選單" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "做浮按鍵可動性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "允許用 Ctrl + 滑鼠左鍵移動獨立的發射器 。 重放滑鼠按鍵會儲存位置 。 此選項只在發射器位置設定為「 單獨按鍵 」 時才適用 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "重置按鈕位置" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "移動可拖放的發射機回到左上角的預設位置." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "確認大選" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "選取的物件達到設定數量時，須在5秒內再次點擊。等待確認期間，選取範圍會顯示為黃色。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "確認閾值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "設定需要第二次確認的選項物件數量." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "啟動 Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "啟動或禁用工具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "重置關鍵結合" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "預設鍵的重置為預設值" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "顯示目前安裝的 Area Bulldozer版本." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "啟動 Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "在地下背景下" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "只有地下視野:白色 = 原始,深色 =淡化/色彩." },
                { "AreaBulldozer.UI.HeightFilter", "高度過濾器" },
                { "AreaBulldozer.UI.HeightAll", "所有的高度" },
                { "AreaBulldozer.UI.HeightAbove", "在地形上 / 地形上" },
                { "AreaBulldozer.UI.HeightBelow", "在地形下" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "適用於預覽和刪除.物體使用其起源;網路沿其曲線取樣.地形以下最多的0.5 m 數為表面.混合的過渡段只可選取所有高度.建築物及其子物仍然是一個單位." },
                { "AreaBulldozer.UI.UndergroundView", "在地下觀光" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "使用遊戲地下視野顯示地下物體. 僅改變視野,而不是高度過濾器. 地形下方自動啟動此視野." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "色彩" },
                { "AreaBulldozer.UI.OpenColorEditor", "開啟顏色編輯器" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "選項" },
                { "AreaBulldozer.UI.ToolOptionsSize", "介面尺寸" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "一般" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "精簡" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "安全" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "包含建築子設施" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "包含網路子物件" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "保護所有物品" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "確認閾值" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "確認大量的刪除" },
                { "AreaBulldozer.UI.ToolOptionsOn", "開啟" },
                { "AreaBulldozer.UI.ToolOptionsOff", "關閉" },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "按鈕、圖示和標題使用標準大小 。" },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "使按鍵、 圖示和標題更小, 所以工具面板佔用的空間更少 。" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "允許在屬於建築物時選擇的物件型別. 必須啟用相匹配的物件濾鏡. 建構更新後,子物件可能會再生." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "允許選定的物件型別,當他們屬於道路或其他網路. 匹配物件濾鏡也必須啟用. 副物件可能稍後再生." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "保護所有物件,其所有者並不被認為是建築物或網路." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "選取的物件達到設定數量時，須在5秒內再次點擊。等待確認期間，選取範圍會顯示為黃色。" },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "設定需要第二次確認的選項物件數量." },
                { "AreaBulldozer.UI.ToolOptionsClose", "關閉選項" },
                { "AreaBulldozer.UI.CloseColorEditor", "密切的顏色編輯器" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "調整選擇、 確認、 刪除回應、 選取的表面、 以及使用實用預覽的無效多邊形顏色 。" },
                { "AreaBulldozer.UI.ColorEditor", "色彩編輯器" },
                { "AreaBulldozer.UI.ColorSelection", "通常的選擇" },
                { "AreaBulldozer.UI.ColorConfirmation", "確認" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "不有效的多角形" },
                { "AreaBulldozer.UI.ColorDeletion", "已被刪除" },
                { "AreaBulldozer.UI.ColorSurfaces", "選擇的物件" },
                { "AreaBulldozer.UI.ColorPresets", "預設顏色" },
                { "AreaBulldozer.UI.ChooseColor", "選擇色彩" },
                { "AreaBulldozer.UI.ColorRed", "紅色" },
                { "AreaBulldozer.UI.ColorGreen", "綠色" },
                { "AreaBulldozer.UI.ColorBlue", "藍色" },
                { "AreaBulldozer.UI.ColorHex", "十六進位色碼" },
                { "AreaBulldozer.UI.ResetCurrentColor", "請重新設定這個顏色" },
                { "AreaBulldozer.UI.ResetAllColors", "重新設定所有顏色" },
                { "AreaBulldozer.UI.ToggleTool", "轉換 Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "按住 Ctrl + 滑鼠左鍵移動; 釋放以儲存" },
                { "AreaBulldozer.UI.ModMenuOpen", "開啟工具" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "開啟 Area Bulldozer,並以圓形或方形選項移除多個物件." },
                { "AreaBulldozer.UI.Navigation", "航行方式" },
                { "AreaBulldozer.UI.BackToBulldozer", "還是回到了正常的牛排機" },
                { "AreaBulldozer.UI.Close", "關閉工具" },
                { "AreaBulldozer.UI.Active", "活動" },
                { "AreaBulldozer.UI.Subtitle", "在一個區域內移除多個物件" },
                { "AreaBulldozer.UI.Selection", "選擇" },
                { "AreaBulldozer.UI.Circle", "圓形" },
                { "AreaBulldozer.UI.Square", "正方形" },
                { "AreaBulldozer.UI.CircleTooltip", "在導覽器周圍的圓形選項區域" },
                { "AreaBulldozer.UI.SquareTooltip", "平方選擇區域, 按住滑鼠右鍵可旋轉 。" },
                { "AreaBulldozer.UI.DecreaseSize", "縮小選項" },
                { "AreaBulldozer.UI.IncreaseSize", "種植選擇" },
                { "AreaBulldozer.UI.RotateLeft", "轉向反時鐘方向" },
                { "AreaBulldozer.UI.RotateRight", "按時方向旋轉" },
                { "AreaBulldozer.UI.MissingAssets", "缺失的資產" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "刪除已確認失蹤的車輛、植物和物件的白色佔位符, 包括子物件。 不包括建築物和網路 。" },
                { "AreaBulldozer.UI.VegetationTooltip", "抹去樹木,灌木和植物." },
                { "AreaBulldozer.UI.BuildingsTooltip", "清除該區域的建築物." },
                { "AreaBulldozer.UI.Cars", "汽車" },
                { "AreaBulldozer.UI.CarsTooltip", "移除移動和停放的路面車。 遊戲會自動暫停, 只要mod暫停, 就會恢復先前的速度 。" },
                { "AreaBulldozer.UI.RoadsTooltip", "關閉該區域的道路." },
                { "AreaBulldozer.UI.PathsTooltip", "取消行人與腳踏車路線." },
                { "AreaBulldozer.UI.RailwaysTooltip", "取消鐵路軌跡." },
                { "AreaBulldozer.UI.SurfacesTooltip", "抹去表面和區域." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "主切換道具和標記。 開啟下面的細節過濾器 。" },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "道具 / 標記" },
                { "AreaBulldozer.UI.Radius", "半徑" },
                { "AreaBulldozer.UI.HalfSide", "方形尺寸" },
                { "AreaBulldozer.UI.Rotation", "轉動" },
                { "AreaBulldozer.UI.Degrees", "排行榜" },
                { "AreaBulldozer.UI.MainFilters", "實體過濾器" },
                { "AreaBulldozer.UI.ChooseFilters", "選擇過濾器" },
                { "AreaBulldozer.UI.Vegetation", "樹木和植物" },
                { "AreaBulldozer.UI.VegetationShort", "植物" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "保護資產植物" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "在植物濾鏡下, 儲存屬於建築物或其他資產的植物." },
                { "AreaBulldozer.UI.TreeAgeFilter", "樹齡" },
                { "AreaBulldozer.UI.TreeAgeAll", "所有植物" },
                { "AreaBulldozer.UI.TreeAgeSapling", "幼苗" },
                { "AreaBulldozer.UI.TreeAgeYoung", "幼樹" },
                { "AreaBulldozer.UI.TreeAgeMature", "成熟樹木" },
                { "AreaBulldozer.UI.TreeAgeElderly", "老樹" },
                { "AreaBulldozer.UI.TreeAgeDead", "枯樹" },
                { "AreaBulldozer.UI.TreeAgePrevious", "之前的年齡階段" },
                { "AreaBulldozer.UI.TreeAgeNext", "年齡下一步" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "加上植被濾波器,可以移動覆蓋的或Anarchy-放置的樹。" },
                { "AreaBulldozer.UI.Buildings", "建築物" },
                { "AreaBulldozer.UI.Roads", "道路" },
                { "AreaBulldozer.UI.Paths", "步行者之路" },
                { "AreaBulldozer.UI.Railways", "鐵路線" },
                { "AreaBulldozer.UI.Surfaces", "表面和空間" },
                { "AreaBulldozer.UI.SurfacesShort", "表面" },
                { "AreaBulldozer.UI.StaticObjects", "道具與標記" },
                { "AreaBulldozer.UI.StaticObjectsShort", "道具 / 標記" },
                { "AreaBulldozer.UI.AdvancedFilters", "高階的道具和標記濾鏡" },
                { "AreaBulldozer.UI.StaticMasterNotice", "啟用上述類別的道具和標記." },
                { "AreaBulldozer.UI.GeneralProps", "一般道具" },
                { "AreaBulldozer.UI.StreetLights", "路燈" },
                { "AreaBulldozer.UI.QuantityObjects", "桶和數量物件" },
                { "AreaBulldozer.UI.Branding", "品牌及廣告" },
                { "AreaBulldozer.UI.ActivityLocations", "活動地點" },
                { "AreaBulldozer.UI.SpawnLocations", "即時位置" },
                { "AreaBulldozer.UI.AssetLanes", "資產行徑 / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "暗淡的背景" },
                { "AreaBulldozer.UI.BackgroundDarkness", "背景黑暗" },
                { "AreaBulldozer.UI.Safety", "安全" },
                { "AreaBulldozer.UI.BuildingSubObjects", "建築子專案" },
                { "AreaBulldozer.UI.NetworkSubObjects", "網路子物件" },
                { "AreaBulldozer.UI.ProtectOwned", "保護所有物品" },
                { "AreaBulldozer.UI.ConfirmLarge", "確認大選" },
                { "AreaBulldozer.UI.Threshold", "確認閾值" },
                { "AreaBulldozer.UI.Objects", "其他物體" },
                { "AreaBulldozer.UI.ApplyHelp", "刪除已選定的物件" },
                { "AreaBulldozer.UI.RotateHelp", "保持和拖動以旋轉" },
                { "AreaBulldozer.UI.ShortcutHelp", "轉換工具" },

                { "AreaBulldozer.UI.Triangle", "三角形" },
                { "AreaBulldozer.UI.TriangleTooltip", "遊標周圍的等效選擇區。 大小從中央到角落來測量 。 按住滑鼠右鍵, 水平移動以旋轉它 。" },
                { "AreaBulldozer.UI.TriangleSize", "三角形尺寸" },
                { "AreaBulldozer.UI.MultiPointLine", "多點線" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "走廊使用 2 到 25 個點,直或圓的過渡. 左點選增加點選. 雙點選最後點選完成和刪除.右點選移除最後點選; Esc 取消整個選擇." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "多點線寬度" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "讓走廊更窄" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "拓寬走廊" },
                { "AreaBulldozer.UI.PolylineStyle", "線條風格" },
                { "AreaBulldozer.UI.PolylineStraight", "直接使用" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "聯絡所有放置的點與直線段." },
                { "AreaBulldozer.UI.PolylineCurved", "曲線" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "繞過放置點之間的轉移。 曲線會在相鄰區段之間保持控制, 不會過度射擊 。" },
                { "AreaBulldozer.UI.PolylineRounding", "曲線圓化" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "降低圓化" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "增加圓化" },
                { "AreaBulldozer.UI.Spline", "多點線" },
                { "AreaBulldozer.UI.SplineTooltip", "走廊使用2至25點.左點選增加點選.雙點選完成和刪除.右點選移除最後一點; Esc取消." },
                { "AreaBulldozer.UI.SplineWidth", "多點線寬度" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "自由形狀區域" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "自由形狀區域" },
                { "AreaBulldozer.UI.PolygonPoints", "點數" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "透過無限數個角點,定義一個自由形式的刪除區域. 按左點來放置點. 點選第一點或雙點來關閉多角形,並擦除它內的一切. 右點選會移除最後一點, Esc 取消當前的選擇. 自行交叉邊緣被自動阻止." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "啟用診斷記錄" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "將其他資訊與診斷訊息寫入紀錄。 預設已禁用 。 警告與錯誤仍會被登入, 無論此設定如何 。" }
            };
        }

        public void Unload()
        {
        }
    }
}
