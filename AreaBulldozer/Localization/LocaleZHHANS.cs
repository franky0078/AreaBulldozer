using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleZHHANS : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleZHHANS(Setting setting)
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
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "显示器和发射器" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "按键绑定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "信息" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "移除树木和植物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "目标是选区内的树木,灌木和其他植物." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "包括 Anarchy植物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "允许重置或 Anarchy- 放置的树木和植被要移除." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "拆除建筑物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "目标是选择区内的完整根基建筑.建筑物所有的道具,标记和车道不被视为单独的建筑." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "移除汽车" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "移动和停车的道路车辆在选择区移动. 游戏在启动时自动停止." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "清除道路" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "目标是完善主要道路边缘,其曲线交叉选区.道路,铁路,标记网络和建筑所有网络仍然被排除在外." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "移除行人路径" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "目标是完成独立的行人路径边缘,其路线交叉选区.道路,铁路,标记网络和建筑物所有 SubNets仍然除外." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "移除铁路轨道" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "目标是完成单独的火车,电车,地铁和地铁轨道边缘,其路线交叉选区.道路,步行者路径,标记网络和建筑物所有 SubNets仍然除外." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "删除表面和空间" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "当选区交叉其边界或其中心位于该区域内时,目标完成多边形表面和空间.资产所有区域也需要匹配的子物体范围,并且在资产更新后可能会再生." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "删除静态对象" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "可启用下面选定的道具和标记类别.可见的道具由其类别直接控制;敏感活动,产卵和车道标记仍然尊重子对象安全选项." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "一般道具和装饰" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "针对不属于更具体类别的静态道具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "路灯" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "针对有路灯组件的物体,包括建筑物或网络拥有的灯光." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "垃圾桶和数量物品" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "目标数量物体,包括垃圾桶和类似的道具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "品牌和广告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "针对品牌物品,广告和类似的装饰." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "活动地点" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "目标是资产活动位置.这些可能影响建筑行为,并且默认禁用." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "分包地点 - 蓝色" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "针对车辆或其他资产物体出现的蓝色 SpawnLocation标记. 拥有产品的产品地点也需要匹配的子物体范围." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "资产道 / SubLanes — 绿色和蓝色" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "目标是绿色和蓝色的车辆,步行者和停车场. SubLanes 使用 NetLanePrefab 或 NetLaneGeometryPrefab 选择;常规的路边仍被排除在外." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "暗淡的背景" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "缩小游戏大纲前的背景,同时启用标记过滤器或缺失资产。大纲和菜单保持可见。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "背景黑暗" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "设定游戏视图的强度以百分比表示 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "选择线厚度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "设置圆形和方形选择形状的固定轮廓厚度。厚度不再随选择大小而变化。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "保护资产植物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "在用植被过滤器删除时保留属于建筑物或其他资产的植物。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "包括建筑子物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "允许选定对象类型在属于建筑物时。匹配对象过滤器也必须启用。子对象可以在建筑物更新后再生。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "包含网络子对象" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "允许选定对象类型在属于一个路段或另一个网络时。匹配对象过滤器也必须启用。子对象可能会在以后再生。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "保护所有物品" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "保护所有者不被确认为建筑物或网络的自有物体。 此安全选项应该继续启用 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "使用方形选择" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "禁用 : 循环选择 。 启用了 : 自由旋转的方块。 请按住鼠标右键, 并水平移动鼠标旋转。 配置的大小是圆形半径或方形半边长 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "默认选择大小" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "设置尺寸为米:圆半径或方形半边长度." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "工具窗口规模" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Area Bulldozer窗口从75%到125%.较低的值提供了更多空间;较高的值提高了高分辨率显示器的可读性." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer发射器位置" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "选择Area Bulldozer是否从单独的发射器启动,从正常的布尔多泽工具选项,或从通用模特菜单." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "独立按钮" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "普通推土机" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "全球模块菜单" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "使浮动按钮可移动" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "允许用 Ctrl + 鼠标左键移动单独的启动器。 释放鼠标按钮可以保存位置。 此选项仅在启动器位置设置为“ 自动按钮 ” 时适用 。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "重置按钮位置" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "移动可拖放的发射器回到左上角的默认位置" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "确认大选" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "选中的对象达到设定数量时，须在5秒内再次点击。等待确认期间，选区会显示为黄色。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "确认阈值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "设定需要第二次确认的选定的对象数量." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "激活 Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "激活或禁用工具." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "重新设置键绑定" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "将键绑定重置到默认值." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "显示目前安装的Area Bulldozer版本." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "激活 Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "地下背景" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "地下视图仅: 白 = 原色,较暗的颜色 = 暗淡 / 淡淡. UI和选择大纲保持不变." },
                { "AreaBulldozer.UI.HeightFilter", "高度过滤器" },
                { "AreaBulldozer.UI.HeightAll", "所有的高度" },
                { "AreaBulldozer.UI.HeightAbove", "在地形上" },
                { "AreaBulldozer.UI.HeightBelow", "在地形下面" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "适用于预览和删除.物体使用其起源;网络沿其曲线进行样本.地形以下最多的0.5米被视为表面.混合过渡段只可选择所有高度.建筑物及其子物仍然是一个单元." },
                { "AreaBulldozer.UI.UndergroundView", "地下景观" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "使用游戏的地下视图显示地下对象。仅更改视图,而不是高度过滤器。在地形下面可以自动实现此视图。" },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "颜色" },
                { "AreaBulldozer.UI.OpenColorEditor", "开放的颜色编辑器" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "选项" },
                { "AreaBulldozer.UI.ToolOptionsSize", "用户界面大小" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "普通" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "紧凑" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "安全" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "包括建筑子物" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "包含网络子对象" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "保护所有物品" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "确认阈值" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "确认大量的删除" },
                { "AreaBulldozer.UI.ToolOptionsOn", "开启" },
                { "AreaBulldozer.UI.ToolOptionsOff", "关闭" },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "使用按钮、图标和标题的标准大小。" },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "使按钮、图标和标题较小,所以工具面板占用的空间较少。" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "允许选定对象类型在属于建筑物时。匹配对象过滤器也必须启用。子对象可以在建筑物更新后再生。" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "允许选定对象类型在属于一个路段或另一个网络时。匹配对象过滤器也必须启用。子对象可能会在以后再生。" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "保护所有者不被确认为建筑物或网络的自有物体。 此安全选项应该继续启用 。" },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "选中的对象达到设定数量时，须在5秒内再次点击。等待确认期间，选区会显示为黄色。" },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "设定需要第二次确认的选定的对象数量." },
                { "AreaBulldozer.UI.ToolOptionsClose", "关闭选项" },
                { "AreaBulldozer.UI.CloseColorEditor", "密切的颜色编辑器" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "调整选择、确认、删除反馈、选定表面和无效的多边形颜色,并预览。" },
                { "AreaBulldozer.UI.ColorEditor", "颜色编辑器" },
                { "AreaBulldozer.UI.ColorSelection", "通常的选择" },
                { "AreaBulldozer.UI.ColorConfirmation", "确认" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "无效的多边形" },
                { "AreaBulldozer.UI.ColorDeletion", "活动删除" },
                { "AreaBulldozer.UI.ColorSurfaces", "选择的对象" },
                { "AreaBulldozer.UI.ColorPresets", "颜色预设" },
                { "AreaBulldozer.UI.ChooseColor", "选择颜色" },
                { "AreaBulldozer.UI.ColorRed", "红色" },
                { "AreaBulldozer.UI.ColorGreen", "绿色" },
                { "AreaBulldozer.UI.ColorBlue", "蓝色" },
                { "AreaBulldozer.UI.ColorHex", "十六进制色值" },
                { "AreaBulldozer.UI.ResetCurrentColor", "重新设置此颜色" },
                { "AreaBulldozer.UI.ResetAllColors", "重新设置所有颜色" },
                { "AreaBulldozer.UI.ToggleTool", "转换 Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "按住 Ctrl + 鼠标左键移动; 释放以保存" },
                { "AreaBulldozer.UI.ModMenuOpen", "开放工具" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "打开 Area Bulldozer并通过圆形或方形选择删除多个对象." },
                { "AreaBulldozer.UI.Navigation", "航行" },
                { "AreaBulldozer.UI.BackToBulldozer", "回到正常的推土机中" },
                { "AreaBulldozer.UI.Close", "关闭工具" },
                { "AreaBulldozer.UI.Active", "活动" },
                { "AreaBulldozer.UI.Subtitle", "在一个区域中删除多个对象" },
                { "AreaBulldozer.UI.Selection", "选择" },
                { "AreaBulldozer.UI.Circle", "圆形" },
                { "AreaBulldozer.UI.Square", "正方形" },
                { "AreaBulldozer.UI.CircleTooltip", "围绕导向器的圆形选择区域." },
                { "AreaBulldozer.UI.SquareTooltip", "平方选择区域,按住鼠标右键可旋转 。" },
                { "AreaBulldozer.UI.DecreaseSize", "缩小选择" },
                { "AreaBulldozer.UI.IncreaseSize", "种植选择" },
                { "AreaBulldozer.UI.RotateLeft", "转向反时针方向" },
                { "AreaBulldozer.UI.RotateRight", "转向时钟方向" },
                { "AreaBulldozer.UI.MissingAssets", "失踪资产" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "删除已确认失踪的车辆,工厂和物体,包括子物体的白色位置.建筑物和网络除外." },
                { "AreaBulldozer.UI.VegetationTooltip", "删除树木、灌木和植物。" },
                { "AreaBulldozer.UI.BuildingsTooltip", "删除该地区的建筑物." },
                { "AreaBulldozer.UI.Cars", "汽车" },
                { "AreaBulldozer.UI.CarsTooltip", "移动和停车的道路车辆. 游戏自动停下来,只能恢复之前的速度,如果调整器停下来." },
                { "AreaBulldozer.UI.RoadsTooltip", "删除该地区的道路." },
                { "AreaBulldozer.UI.PathsTooltip", "删除行人和自行车路径." },
                { "AreaBulldozer.UI.RailwaysTooltip", "删除铁路轨道." },
                { "AreaBulldozer.UI.SurfacesTooltip", "删除表面和区域." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "道具和标记的主开关。启用下面的细节过滤器 。" },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "道具/标记器" },
                { "AreaBulldozer.UI.Radius", "半径" },
                { "AreaBulldozer.UI.HalfSide", "平方尺寸" },
                { "AreaBulldozer.UI.Rotation", "旋转" },
                { "AreaBulldozer.UI.Degrees", "排放" },
                { "AreaBulldozer.UI.MainFilters", "对象过滤器" },
                { "AreaBulldozer.UI.ChooseFilters", "选择过滤器" },
                { "AreaBulldozer.UI.Vegetation", "树木和植物" },
                { "AreaBulldozer.UI.VegetationShort", "植物" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "保护资产植物" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "在用植被过滤器删除时保留属于建筑物或其他资产的植物。" },
                { "AreaBulldozer.UI.TreeAgeFilter", "树龄" },
                { "AreaBulldozer.UI.TreeAgeAll", "所有植物" },
                { "AreaBulldozer.UI.TreeAgeSapling", "幼苗" },
                { "AreaBulldozer.UI.TreeAgeYoung", "幼树" },
                { "AreaBulldozer.UI.TreeAgeMature", "成年树" },
                { "AreaBulldozer.UI.TreeAgeElderly", "老树" },
                { "AreaBulldozer.UI.TreeAgeDead", "枯树" },
                { "AreaBulldozer.UI.TreeAgePrevious", "上一个年龄阶段" },
                { "AreaBulldozer.UI.TreeAgeNext", "下一个年龄阶段" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "连同植被过滤器,可以将倒挂的或Anarchy-放置的树木移走。" },
                { "AreaBulldozer.UI.Buildings", "建筑物" },
                { "AreaBulldozer.UI.Roads", "道路" },
                { "AreaBulldozer.UI.Paths", "步行者之路" },
                { "AreaBulldozer.UI.Railways", "铁路轨道" },
                { "AreaBulldozer.UI.Surfaces", "表面和空间" },
                { "AreaBulldozer.UI.SurfacesShort", "表面" },
                { "AreaBulldozer.UI.StaticObjects", "道具和标记" },
                { "AreaBulldozer.UI.StaticObjectsShort", "道具/标记" },
                { "AreaBulldozer.UI.AdvancedFilters", "高级道具和标记过滤器" },
                { "AreaBulldozer.UI.StaticMasterNotice", "启用上述类别的道具和标记." },
                { "AreaBulldozer.UI.GeneralProps", "一般道具" },
                { "AreaBulldozer.UI.StreetLights", "路灯" },
                { "AreaBulldozer.UI.QuantityObjects", "桶和数量物体" },
                { "AreaBulldozer.UI.Branding", "品牌和广告" },
                { "AreaBulldozer.UI.ActivityLocations", "活动地点" },
                { "AreaBulldozer.UI.SpawnLocations", "即时地点" },
                { "AreaBulldozer.UI.AssetLanes", "资产行径 / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "暗淡的背景" },
                { "AreaBulldozer.UI.BackgroundDarkness", "背景黑暗" },
                { "AreaBulldozer.UI.Safety", "安全" },
                { "AreaBulldozer.UI.BuildingSubObjects", "建筑子物" },
                { "AreaBulldozer.UI.NetworkSubObjects", "网络子对象" },
                { "AreaBulldozer.UI.ProtectOwned", "保护所有物品" },
                { "AreaBulldozer.UI.ConfirmLarge", "确认大选" },
                { "AreaBulldozer.UI.Threshold", "确认阈值" },
                { "AreaBulldozer.UI.Objects", "物体" },
                { "AreaBulldozer.UI.ApplyHelp", "删除选定的对象" },
                { "AreaBulldozer.UI.RotateHelp", "保持和拖转" },
                { "AreaBulldozer.UI.ShortcutHelp", "切换工具" },

                { "AreaBulldozer.UI.Triangle", "三角形" },
                { "AreaBulldozer.UI.TriangleTooltip", "光标周围的等边选择区域。大小从中央到角落进行测量。按住鼠标右键并水平移动以旋转它。" },
                { "AreaBulldozer.UI.TriangleSize", "三角大小" },
                { "AreaBulldozer.UI.MultiPointLine", "多点线" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "走廊使用2到25个点,直或圆的过渡.左键添加点.双键最后点完成和删除.右键删除最后点;Esc取消整个选择." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "多点线宽度" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "让走廊更窄" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "扩大走廊" },
                { "AreaBulldozer.UI.PolylineStyle", "线条风格" },
                { "AreaBulldozer.UI.PolylineStraight", "直接的" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "连接所有放置的点与直线段." },
                { "AreaBulldozer.UI.PolylineCurved", "曲线" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "绕着放置点之间的过渡. 曲线保持在相邻的段落之间控制,不会过度." },
                { "AreaBulldozer.UI.PolylineRounding", "曲线圆化" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "减少四舍五入" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "增加舍入" },
                { "AreaBulldozer.UI.Spline", "多点线" },
                { "AreaBulldozer.UI.SplineTooltip", "走廊使用2到25个点.左键增加点.双键完成和删除.右键删除最后一个点; Esc取消." },
                { "AreaBulldozer.UI.SplineWidth", "多点线宽度" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "自由形式区域" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "自由形式区域" },
                { "AreaBulldozer.UI.PolygonPoints", "问题" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "定义一个自由形式删除区域,使用无限数个角点.左键点击点击.点击第一点或双键点击以关闭多边形并删除它内的所有内容.右键删除最后一个点, Esc取消当前的选择.自动交叉边缘被阻止." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "启用诊断日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "在日志中写出额外的信息和诊断信息.默认禁用.不管这个设置如何,警告和错误仍然会被记录." }
            };
        }

        public void Unload()
        {
        }
    }
}
