using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleKO : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleKO(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "주요" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "안전 설정" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "디스플레이 및 발사기" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "단축키" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "정보" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "나무와 식물을 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "선택 구역 내의 나무, 덤불, 그리고 다른 식물들을 대상으로 한다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Anarchy 식물을 포함한다" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Overridden 또는 Anarchy - 나무와 vegetation을 제거 할 수 있습니다. 이 옵션은 기본적으로 비활성화됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "건물을 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "선택 구역 내 에 있는 뿌리 건물 을 완수 하는 것 이다. 건물 에 속하는 자재, 마크너 및 라인 들 은 별도의 건물 으로 간주 되지 않는다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "자동차를 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "선택 영역에서 이동 및 주차 도로 차량을 제거합니다. 활성화 될 때 게임 일시 중지." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "도로 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "표적 은 선택 구역 을 교차 하는 곡선이 있는 주요 도로 의 가장자리 를 완성 합니다. 경로, 철도, 마커 네트워크 및 건물 소유 네트워크 는 제외 되어 있습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "보행자 경로를 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "대상 전체 독립 보행자-경로 가장자리는 경로가 선택 영역을 교차합니다. 도로, 철도, 마커 네트워크 및 건물 소유 SubNets 제외 남아 있습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "철도 트랙을 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "대상 전체 독립 열차, 트램, 지하철 및 지하철 트랙 가장자리는 경로가 선택 영역을 교차합니다. 도로, 보행자 경로, 마커 네트워크 및 건물 소유 SubNets 제외됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "표면과 공간을 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "표적은 선택 영역이 그들의 경계를 교차하거나 그 중심이 해당 영역 내에 위치할 때 다각형 표면과 공간을 완성합니다. 자산 소유 지역은 또한 일치하는 하위 객체 범위를 필요로 하며 자산 업데이트 후 재생될 수 있습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "정적 객체를 제거" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "아래의 선택된 프로프 및 마커 카테고리를 활성화합니다. 가시적인 프로프가들은 그들의 카테고리에 의해 직접 제어됩니다. 민감한 활동, 자생 및 라인 마커들은 여전히 소재 안전 옵션을 존중합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "일반 소재 및 장식품" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "더 구체적인 범주 중 하나에 속하지 않는 정적 버팀대를 표합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "거리등" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "건물이나 네트워크에 속한 조명을 포함한 거리 조명 구성 요소를 가진 물체를 대상으로 합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "쓰레기통과 대량물질" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "쓰레기통과 비슷한 소재들을 포함할 수 있는 대량 물체를 대상으로 합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "브랜딩 및 광고" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "대상 브랜드 객체, 광고 및 유사한 장식." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "활동 장소" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "자산 활동 위치를 대상으로 합니다. 이것은 건물 행동에 영향을 미칠 수 있으며 기본적으로 비활성화됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "Spawn 위치 – 푸른" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "차량이나 다른 자산 객체가 나타날 수 있는 푸른 SpawnLocation 마크를 대상으로 한다. 소유된 자생 장소는 또한 일치하는 하위 객체 범위를 요구한다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Asset lanes / SubLanes – 녹색과 푸른" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "건물 자산 내부의 녹색 및 파란색 띠 차량, 보행자 및 주차 경로를 대상으로 합니다. NetLanePrefab 또는 NetLaneGeometryPrefab를 사용하는 SubLanes 만 선택됩니다. 일반 도로 가장자리는 제외됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "희미한 배경" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Marker 필터 또는 누락 된 자산이 활성화되어있는 동안 게임 개요 전에 배경을 흐리게합니다. 개요 및 메뉴는 볼 수 있습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "배경 어둠" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "%의 게임 뷰 디밍 강도를 설정합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "선택 라인 두께" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "원형과 사각 선택 모양을 위한 조정 윤곽 간격을 놓으십시오. 간격은 더 이상 선택 크기로 변화하지 않습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "자산 식물 보호" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "Vegetation 필터로 삭제할 때 건물 또는 다른 자산에 속하는 식물을 유지하십시오." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "건물 하위 객체를 포함합니다" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "선택된 객체 유형은 건물에 속할 때 허용됩니다. 일치하는 객체 필터 또한 활성화되어야 합니다. 하위 객체들은 건물 업데이트 후에 재생될 수 있습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "네트워크 하위 객체를 포함합니다" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "선택된 객체 유형은 도로 또는 다른 네트워크에 속할 때 허용됩니다. 일치하는 객체 필터 또한 활성화되어야합니다. 하위 객체들은 나중에 재생될 수 있습니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "소유물 보호" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "건물이나 네트워크로 소유자가 인정되지 않은 소유물들을 보호합니다. 이 안전 옵션은 여전히 활성화되어 있어야 합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "사각형 선택 사용" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "Disabled: 원형 선택. Enabled: 자유롭게 돌릴수 있는 사각. 적당한 쥐 단추를 붙들고 그것을 자전하기 위하여 수평으로 마우스를 이동하십시오. 형성된 크기는 원형 반경 또는 정연한 반 측 길이입니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "기본 선택 크기는" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "미터의 크기를 설정합니다 : 원형 반경 또는 광장 반쪽 길이." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "도구 창 규모" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Area Bulldozer 창 을 75 에서 125 퍼센트 까지 확장 한다. 낮은 값 은 더 많은 공간을 제공 하며, 높은 값 은 고 해상도 디스플레이 에서 읽을 수 있게 한다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer 발사기 위치" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer를 별도의 발사기에서 시작하거나 일반적인 툴 옵션에서 시작하거나 보편적인 모드 메뉴에서 시작하는지 선택합니다." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "단독 버튼" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "정상적인 불도저" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "유니버설 모드 메뉴" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "떠 있는 버튼을 움직일 수 있도록" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Ctrl + 왼쪽 마우스 버튼을 사용하여 별도의 런처를 이동할 수 있습니다. 마우스 버튼을 풀면 위치를 저장합니다. 이 옵션은 런처 위치를 'Standalone 버튼'으로 설정할 때만 적용됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "다시 설정 버튼 위치" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "왼쪽 상단 모서리의 기본 위치로 드래그 가능한 발사기를 이동합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "큰 선택지를 확인합니다" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "선택한 오브젝트가 설정한 수에 도달하면 5초 안에 한 번 더 클릭해야 합니다. 확인을 기다리는 동안 선택 영역이 노란색으로 표시됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "확인률" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "선택된 개체 수를 두 번째 확인이 필요합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "활성화 Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "도구를 활성화 또는 비활성화합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "리셋 키 결합" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "기본 값에 키 바인딩을 재설정합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "현재 설치된 Area Bulldozer의 버전을 보여줍니다." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "활성화 Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "지하 배경" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "Underground view only: white = original, darker colors = dimming / tint. UI 및 선택 개요는 변경되지 않습니다." },
                { "AreaBulldozer.UI.HeightFilter", "높이 필터" },
                { "AreaBulldozer.UI.HeightAll", "모든 높이는" },
                { "AreaBulldozer.UI.HeightAbove", "위 / 지형" },
                { "AreaBulldozer.UI.HeightBelow", "지형 아래" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "미리보기 및 삭제에 적용됩니다. 객체는 그들의 기원을 사용합니다. 네트워크는 곡선을 따라 샘플링됩니다. 지형 아래 0.5m까지 표면으로 간주됩니다. 혼합 전환 세그먼트는 모든 높이를 사용하여만 선택할 수 있습니다. 건물과 그 하위 객체는 하나의 단위로 남아 있습니다." },
                { "AreaBulldozer.UI.UndergroundView", "지하 전망" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "게임 언더그라운드 뷰를 사용하여 지하 물체를 표시합니다. 높이 필터가 아닌 뷰만 변경합니다. 지형 아래에서는 이 뷰를 자동으로 가능하게합니다." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "색상" },
                { "AreaBulldozer.UI.OpenColorEditor", "오픈 컬러 에디터" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "옵션" },
                { "AreaBulldozer.UI.ToolOptionsSize", "UI 크기는" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "기본" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "간결" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "안전 설정" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "건물 하위 객체를 포함합니다" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "네트워크 하위 객체를 포함합니다" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "소유물 보호" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "확인률" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "큰 삭제 확인" },
                { "AreaBulldozer.UI.ToolOptionsOn", "켜기" },
                { "AreaBulldozer.UI.ToolOptionsOff", "끄기" },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "버튼, 아이콘 및 헤드링에 대한 표준 크기를 사용합니다." },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "버튼, 아이콘, 헤드라인을 작게 하기 때문에 도구 패널은 공간을 덜 차지합니다." },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "선택된 객체 유형은 건물에 속할 때 허용됩니다. 일치하는 객체 필터 또한 활성화되어야 합니다. 하위 객체들은 건물 업데이트 후에 재생될 수 있습니다." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "선택된 객체 유형은 도로 또는 다른 네트워크에 속할 때 허용됩니다. 일치하는 객체 필터 또한 활성화되어야합니다. 하위 객체들은 나중에 재생될 수 있습니다." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "건물이나 네트워크로 소유자가 인정되지 않은 소유물들을 보호합니다. 이 안전 옵션은 여전히 활성화되어 있어야 합니다." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "선택한 오브젝트가 설정한 수에 도달하면 5초 안에 한 번 더 클릭해야 합니다. 확인을 기다리는 동안 선택 영역이 노란색으로 표시됩니다." },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "선택된 개체 수를 두 번째 확인이 필요합니다." },
                { "AreaBulldozer.UI.ToolOptionsClose", "닫힌 옵션" },
                { "AreaBulldozer.UI.CloseColorEditor", "꽉 차 있는 색상 편집기" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "선택, 확인, 삭제 피드백, 선택된 표면 및 라이브 미리보기를 가진 잘못된 다각형 색상을 조정합니다." },
                { "AreaBulldozer.UI.ColorEditor", "색상 편집기" },
                { "AreaBulldozer.UI.ColorSelection", "일반적인 선택" },
                { "AreaBulldozer.UI.ColorConfirmation", "확인" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "유효하지 않은 다각형" },
                { "AreaBulldozer.UI.ColorDeletion", "활성 삭제" },
                { "AreaBulldozer.UI.ColorSurfaces", "선택된 객체" },
                { "AreaBulldozer.UI.ColorPresets", "색상 미리 설정" },
                { "AreaBulldozer.UI.ChooseColor", "색상 선택" },
                { "AreaBulldozer.UI.ColorRed", "빨강" },
                { "AreaBulldozer.UI.ColorGreen", "초록" },
                { "AreaBulldozer.UI.ColorBlue", "파랑" },
                { "AreaBulldozer.UI.ColorHex", "HEX 코드" },
                { "AreaBulldozer.UI.ResetCurrentColor", "이 색을 다시 설정" },
                { "AreaBulldozer.UI.ResetAllColors", "모든 색상을 재설정" },
                { "AreaBulldozer.UI.ToggleTool", "타글 Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "Ctrl + 왼쪽 마우스 버튼을 잡고 움직이고, 저장하기 위해 풀어주십시오." },
                { "AreaBulldozer.UI.ModMenuOpen", "오픈 도구" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "Area Bulldozer를 열고 원형 또는 사각형 선택으로 여러 객체를 제거한다." },
                { "AreaBulldozer.UI.Navigation", "항해" },
                { "AreaBulldozer.UI.BackToBulldozer", "정상적인 툴도저로 돌아가" },
                { "AreaBulldozer.UI.Close", "닫힌 도구" },
                { "AreaBulldozer.UI.Active", "활동" },
                { "AreaBulldozer.UI.Subtitle", "선택 영역 안의 여러 오브젝트를 제거합니다." },
                { "AreaBulldozer.UI.Selection", "선택" },
                { "AreaBulldozer.UI.Circle", "원" },
                { "AreaBulldozer.UI.Square", "정사각형" },
                { "AreaBulldozer.UI.CircleTooltip", "커서 주변의 원형 선택 영역." },
                { "AreaBulldozer.UI.SquareTooltip", "좌변 선택 영역, 마우스 오른쪽 버튼을 잡고 회전할 수 있습니다." },
                { "AreaBulldozer.UI.DecreaseSize", "축소 선택" },
                { "AreaBulldozer.UI.IncreaseSize", "성장 선택" },
                { "AreaBulldozer.UI.RotateLeft", "시계 방향으로 회전" },
                { "AreaBulldozer.UI.RotateRight", "시계 방향으로 회전" },
                { "AreaBulldozer.UI.MissingAssets", "실종된 자산" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "누락된 차량, 식물 및 개체의 흰색 홀더를 삭제합니다. 건물 및 네트워크는 제외됩니다." },
                { "AreaBulldozer.UI.VegetationTooltip", "나무, 덤불, 식물을 삭제하십시오." },
                { "AreaBulldozer.UI.BuildingsTooltip", "그 지역에 있는 건물을 지워라." },
                { "AreaBulldozer.UI.Cars", "자동차" },
                { "AreaBulldozer.UI.CarsTooltip", "이동 및 주차 도로 차량 제거. 게임 일시 중지 자동으로 및 mod가 그것을 일시 중지 한 경우에만 이전 속도를 복원합니다." },
                { "AreaBulldozer.UI.RoadsTooltip", "그 지역의 도로들을 삭제하세요." },
                { "AreaBulldozer.UI.PathsTooltip", "보행자 및 자전거 경로를 삭제하십시오." },
                { "AreaBulldozer.UI.RailwaysTooltip", "철도 경로를 삭제하세요." },
                { "AreaBulldozer.UI.SurfacesTooltip", "표면과 영역을 삭제하십시오." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "Props와 markers를 위한 주된 스위치. 아래에 세부사항 여과기를 가능하게 하십시오." },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "프로페 / 마커" },
                { "AreaBulldozer.UI.Radius", "반지름" },
                { "AreaBulldozer.UI.HalfSide", "사각형 크기는" },
                { "AreaBulldozer.UI.Rotation", "회전" },
                { "AreaBulldozer.UI.Degrees", "기하급수" },
                { "AreaBulldozer.UI.MainFilters", "객체 필터" },
                { "AreaBulldozer.UI.ChooseFilters", "필터를 선택" },
                { "AreaBulldozer.UI.Vegetation", "나무와 식물" },
                { "AreaBulldozer.UI.VegetationShort", "식물" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "자산 식물 보호" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "Vegetation 필터로 삭제할 때 건물 또는 다른 자산에 속하는 식물을 유지하십시오." },
                { "AreaBulldozer.UI.TreeAgeFilter", "나무 수령" },
                { "AreaBulldozer.UI.TreeAgeAll", "모든 식물" },
                { "AreaBulldozer.UI.TreeAgeSapling", "묘목" },
                { "AreaBulldozer.UI.TreeAgeYoung", "어린 나무" },
                { "AreaBulldozer.UI.TreeAgeMature", "성목" },
                { "AreaBulldozer.UI.TreeAgeElderly", "노목" },
                { "AreaBulldozer.UI.TreeAgeDead", "고사목" },
                { "AreaBulldozer.UI.TreeAgePrevious", "이전 연령 단계" },
                { "AreaBulldozer.UI.TreeAgeNext", "다음 나이 단계" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "식물성 필터와 함께, 과장되거나 Anarchy - 배치 된 나무를 제거 할 수 있습니다." },
                { "AreaBulldozer.UI.Buildings", "건물" },
                { "AreaBulldozer.UI.Roads", "도로" },
                { "AreaBulldozer.UI.Paths", "보행자 경로" },
                { "AreaBulldozer.UI.Railways", "철도 트랙" },
                { "AreaBulldozer.UI.Surfaces", "표면과 공간" },
                { "AreaBulldozer.UI.SurfacesShort", "표면" },
                { "AreaBulldozer.UI.StaticObjects", "소재 및 표시기" },
                { "AreaBulldozer.UI.StaticObjectsShort", "소재 / 표시기" },
                { "AreaBulldozer.UI.AdvancedFilters", "첨단 프로프 및 마커 필터" },
                { "AreaBulldozer.UI.StaticMasterNotice", "위의 프로프와 마크를 이 카테고리를 사용할 수 있도록 합니다." },
                { "AreaBulldozer.UI.GeneralProps", "일반 자재" },
                { "AreaBulldozer.UI.StreetLights", "거리등" },
                { "AreaBulldozer.UI.QuantityObjects", "캔과 양 물체" },
                { "AreaBulldozer.UI.Branding", "브랜딩 및 광고" },
                { "AreaBulldozer.UI.ActivityLocations", "활동 장소" },
                { "AreaBulldozer.UI.SpawnLocations", "Spawn 위치" },
                { "AreaBulldozer.UI.AssetLanes", "자산 라인 / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "희미한 배경" },
                { "AreaBulldozer.UI.BackgroundDarkness", "배경 어둠" },
                { "AreaBulldozer.UI.Safety", "안전 설정" },
                { "AreaBulldozer.UI.BuildingSubObjects", "건설 부시설" },
                { "AreaBulldozer.UI.NetworkSubObjects", "네트워크 하위 객체" },
                { "AreaBulldozer.UI.ProtectOwned", "소유물 보호" },
                { "AreaBulldozer.UI.ConfirmLarge", "큰 선택지를 확인합니다" },
                { "AreaBulldozer.UI.Threshold", "확인률" },
                { "AreaBulldozer.UI.Objects", "물체" },
                { "AreaBulldozer.UI.ApplyHelp", "선택된 객체를 삭제" },
                { "AreaBulldozer.UI.RotateHelp", "돌기 위해 붙잡고 끌고" },
                { "AreaBulldozer.UI.ShortcutHelp", "타그 툴" },

                { "AreaBulldozer.UI.Triangle", "삼각형" },
                { "AreaBulldozer.UI.TriangleTooltip", "커서 주변의 측면 선택 영역. 크기는 중앙에서 코너로 측정됩니다. 오른쪽 마우스 버튼을 잡고 수평으로 회전합니다." },
                { "AreaBulldozer.UI.TriangleSize", "삼각형 크기는" },
                { "AreaBulldozer.UI.MultiPointLine", "다점선" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "2~25개의 포인트를 사용하여 직선 또는 주둥이 전환을 합니다. 왼쪽 클릭으로 포인트를 추가합니다. 마지막 포인트를 두 번 클릭하여 종료하고 삭제합니다. 오른쪽 클릭으로 마지막 포인트를 제거합니다. Esc 전체 선택을 취소합니다." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "여러 점 직선 너비" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "통로를 좁히고" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "통로를 넓게 해" },
                { "AreaBulldozer.UI.PolylineStyle", "라인 스타일" },
                { "AreaBulldozer.UI.PolylineStraight", "직선" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "모든 배치된 점들을 직선 세그먼트로 연결합니다." },
                { "AreaBulldozer.UI.PolylineCurved", "곡선" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "배치 점 사이의 전환을 라운드. 곡선은 인접한 세그먼트 사이에 제어되며 지나치지 않습니다." },
                { "AreaBulldozer.UI.PolylineRounding", "곡선 구형" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "주둥이를 줄이는" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "라운딩을 증가시키" },
                { "AreaBulldozer.UI.Spline", "다점선" },
                { "AreaBulldozer.UI.SplineTooltip", "2~25점으로 코디도르. 왼쪽 클릭으로 포인트를 추가합니다. 두 번의 클릭으로 완료하고 삭제합니다. 오른쪽 클릭으로 마지막 포인트를 제거합니다. Esc 취소합니다." },
                { "AreaBulldozer.UI.SplineWidth", "여러 점 직선 너비" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "자유형 영역" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "자유형 영역" },
                { "AreaBulldozer.UI.PolygonPoints", "점" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "자유형 삭제 영역을 제한되지 않은 수의 각점으로 정의하십시오. 포인트를 배치하기 위해 왼쪽을 클릭하십시오. 다각형을 닫기 위해 첫 번째 점을 클릭하거나 두 번 클릭하여 그 안의 모든 것을 삭제하십시오. 오른쪽을 클릭하면 마지막 포인트가 제거되며, Esc는 현재 선택을 취소합니다. 자율적으로 교차하는 가장자리는 차단됩니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "진단 로그 사용" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "추가 정보 및 진단 메시지가 로그에 기록합니다. 기본적으로 비활성화됩니다. 경고 및 오류는 여전히이 설정에 관계없이 기록됩니다." }
            };
        }

        public void Unload()
        {
        }
    }
}
