using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocaleJA : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleJA(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "メイン" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "安全設定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "ディスプレイと発射機" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "キー設定" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "情報" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "樹木と植物を除去する" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "選択エリア内の木や灌木,その他の植物を対象とする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Anarchy植物を含む" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Anarchy 置かれた木や植物を取り除くことができます.このオプションはデフォルトで無効です." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "建物を取り除くこと" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "選定区内にある完全な根の建物を対象とする.建物所有の道具,マーク,レーンは別々の建物として数えられない." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "車を取り除く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "選択エリアの移動や駐車された道路車両を取り除きます. 動作が有効になったとき,ゲームは自動的に停止します." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "道路を取り除く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "標的にされた道路は,選択区域を交差する曲線を持つ主要道路の端を完成させています. 道路,鉄道,マーカーネットワーク,建物所有のネットワークは除外されています." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "歩行者道を取り除く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "標的にされるのは,選択区域を交差する路線が自立した歩行者道の端を完成させる.道路,鉄道,マーカーネットワーク,建物所有のSubNetsは除外されている." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "鉄道の軌跡を取り除く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "対象は,選択エリアを交差する線路の独立列車,電車,地下鉄,地下鉄の線路を完成させる.道路,歩行者道,マーカーネットワーク,建物所有のSubNetsは除外されている." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "表面とスペースを削除する" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "対象は,選択エリアがその境界線を交差するとき,またはその中心がその領域内に位置するときに多角形の表面と空間を完成させる.資産所有する地域はまた,適合するサブオブジェクト範囲を必要とし,資産更新後に再生することができる." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "静的なオブジェクトを削除する" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "下記の選択された道具とマーカーカテゴリーを有効にする.目に見える道具は,そのカテゴリーによって直接制御されます.敏感な活動,スポン,レーンマーカーは,依然としてサブオブジェクトの安全オプションを尊重します." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "一般的な道具と装飾品" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "特定のカテゴリーに属していない静的な道具を対象とする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "ストリートライト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "建物やネットワークに所有されているライトを含むストリートライトコンポーネントを持つ物体を対象とする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "ゴミ箱と量物" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "ごみ出し箱や類似の道具を含む量物体を対象とする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "ブランドや広告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "ブランドオブジェクト,広告,および類似の装飾をターゲットとする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "活動場所" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "資産活動の位置を標的にします. これらはビルド行動に影響を与え,デフォルトで無効化されます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "スパンの場所 – 青" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "対象は,車両または他の資産オブジェクトが表示される青い SpawnLocationマーカーである.所有されたスポン・ロケーションはまた,対応するサブオブジェクト範囲を必要とします." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "アセットレーン/SubLanes – グリーンとブルー" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "緑色と青色の車や歩行者,駐車場を狙っています SubLanes 使用する NetLanePrefab または NetLaneGeometryPrefab 選択され,通常の道路辺は除外されます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "薄い背景" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "マーカーのフィルターや欠けている資産を有効にすると,ゲームが概要になる前に背景を暗く.概要とメニューは表示されます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "背景の暗闇" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "ゲームビューの暗く力を %で設定する." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "選択線の厚さ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "円形と四角形の選択形に固定された概要厚さを設定します.選択サイズと厚さはもはや変化しません." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "資産の植物を保護する" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "植物のフィルターで削除する際に建物やその他の資産に属する植物を保持する." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "建物のサブオブジェクトを含みます" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "選択されたオブジェクトタイプは,建物に属しているときに許可されます.一致するオブジェクトフィルタも有効にする必要があります. 建物の更新後にサブオブジェクトが再生することができます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "ネットワークサブオブジェクトを含める" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "道路または他のネットワークに属しているときに選択されたオブジェクトタイプを許可します.一致するオブジェクトフィルタも有効にする必要があります.サブオブジェクトは後で再生することができます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "財産を保護する" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "建物やネットワークとして所有者が認識されていない所有物を保護する.この安全オプションは有効に維持されるべきです." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "スクエア選択を使用" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "動作停止:円形選択.有効:自由に回転できる四角形.右マウスのボタンを押して,それを回転するために平面にマウスを移動する.設定されたサイズは円半径または四角半辺長さです." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "デフォルト選択サイズ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "サイズをメートルで設定する:円半径または四角半辺長さ." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "ツールウィンドウスケール" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Area Bulldozer の 窓 を 75 から 125 パーセント に 拡大 する.より 低い 値 は,より 広い 空間 を 提供 する.より 高い 値 は,高 解像度 の ディスプレイ で 読みやすく なる." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozer ロンサーの位置" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "Area Bulldozerを別々のランチャーから,通常のブルドーザーツールオプションから,またはユニバーサルモッドメニューから起動するかどうかを選択します." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "スタンドアロンのボタン" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "普通のブルドーザー" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "ユニバーサルモッドメニュー" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "浮遊ボタンを移動させる" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Ctrl +左マウスのボタンを押して別々のランチャーを移動させることができます.マウスのボタンを押すと位置が保存されます.このオプションは,ランチャー位置が\"Standalone ボタン\"に設定された場合にのみ適用されます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "セットボタンの位置" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "引き寄せられる発射機を 上の左角のデフォルト位置に戻します" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "大規模な選択を確認する" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "選択したオブジェクトが設定数に達すると、5秒以内にもう一度クリックする必要があります。確認待ちの間は選択範囲が黄色になります。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "確認しきい値" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "選択されたオブジェクトの数を設定し,第二次確認が必要とする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "活性化 Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "ツールを有効または無効にする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "鍵をリセットする" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "鍵の結合をデフォルト値にリセットする." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "Area Bulldozer の現在インストールされているバージョンを示します." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "活性化 Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "地下背景" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "地下でのみ表示:白 = 原作,暗色 =暗色/色. UIと選択の概要は変わらない." },
                { "AreaBulldozer.UI.HeightFilter", "高さのフィルター" },
                { "AreaBulldozer.UI.HeightAll", "すべての高さ" },
                { "AreaBulldozer.UI.HeightAbove", "上 / 地形上" },
                { "AreaBulldozer.UI.HeightBelow", "地形の下" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "プレビューと削除の両方に適用されます。オブジェクトは原点で、道路などのネットワークは曲線上の位置で判定します。地表から0.5 m下までは地上扱いです。高さが混在する区間は「すべての高さ」でのみ選択できます。建物とそのサブオブジェクトは一体として扱われます。" },
                { "AreaBulldozer.UI.UndergroundView", "地下景" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "ゲームの地下視野を使用して地下物体を表示します.高度フィルタではなく,視野のみを変更します. 地形の下ではこの視野を自動的に可能にします." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "色" },
                { "AreaBulldozer.UI.OpenColorEditor", "オープンカラーエディタ" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "オプション" },
                { "AreaBulldozer.UI.ToolOptionsSize", "UIサイズ" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "標準" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "コンパクト" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "安全設定" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "建物のサブオブジェクトを含みます" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "ネットワークサブオブジェクトを含める" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "財産を保護する" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "確認しきい値" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "大規模な削除を確認する" },
                { "AreaBulldozer.UI.ToolOptionsOn", "オン" },
                { "AreaBulldozer.UI.ToolOptionsOff", "オフ" },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "ボタン,アイコン,タイトルに標準サイズを使用します." },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "ボタン,アイコン,ヘッダを小さくして ツールパネルはスペースが少なくなります" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "選択されたオブジェクトタイプは,建物に属しているときに許可されます.一致するオブジェクトフィルタも有効にする必要があります. 建物の更新後にサブオブジェクトが再生することができます." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "道路または他のネットワークに属しているときに選択されたオブジェクトタイプを許可します.一致するオブジェクトフィルタも有効にする必要があります.サブオブジェクトは後で再生することができます." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "建物やネットワークとして所有者が認識されていない所有物を保護する.この安全オプションは有効に維持されるべきです." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "選択したオブジェクトが設定数に達すると、5秒以内にもう一度クリックする必要があります。確認待ちの間は選択範囲が黄色になります。" },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "選択されたオブジェクトの数を設定し,第二次確認が必要とする." },
                { "AreaBulldozer.UI.UndoVegetation", "植生を復元" },
                { "AreaBulldozer.UI.UndoVegetationTooltip", "直前の削除操作で取り除いた植生を復元します。現存する建物や公園の植生も対象です（{count} 個）。" },
                { "AreaBulldozer.UI.ToolOptionsClose", "閉じるオプション" },
                { "AreaBulldozer.UI.CloseColorEditor", "カラーエディタを閉じる" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "選択,確認,削除フィードバック,選択された表面,無効な多角色をライブプレビューで調整します." },
                { "AreaBulldozer.UI.ColorEditor", "カラーエディター" },
                { "AreaBulldozer.UI.ColorSelection", "通常の選択" },
                { "AreaBulldozer.UI.ColorConfirmation", "確認" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "有効でない多角形" },
                { "AreaBulldozer.UI.ColorDeletion", "アクティブ削除" },
                { "AreaBulldozer.UI.ColorSurfaces", "選択されたオブジェクト" },
                { "AreaBulldozer.UI.ColorPresets", "色のプリセット" },
                { "AreaBulldozer.UI.ChooseColor", "色を選択" },
                { "AreaBulldozer.UI.ColorRed", "赤" },
                { "AreaBulldozer.UI.ColorGreen", "緑" },
                { "AreaBulldozer.UI.ColorBlue", "青" },
                { "AreaBulldozer.UI.ColorHex", "16進カラーコード" },
                { "AreaBulldozer.UI.ResetCurrentColor", "この色を再設定する" },
                { "AreaBulldozer.UI.ResetAllColors", "すべての色をリセット" },
                { "AreaBulldozer.UI.ToggleTool", "トーグル Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "Ctrl +左マウスのボタンを押して移動する.保存するために解放する." },
                { "AreaBulldozer.UI.ModMenuOpen", "オープンツール" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "Area Bulldozer を開き,円形または四角選択で複数のオブジェクトを削除する." },
                { "AreaBulldozer.UI.Navigation", "航海" },
                { "AreaBulldozer.UI.BackToBulldozer", "普通のブルドーザーに戻る" },
                { "AreaBulldozer.UI.Close", "閉ざされたツール" },
                { "AreaBulldozer.UI.Active", "アクティブ" },
                { "AreaBulldozer.UI.Subtitle", "範囲内の複数のオブジェクトを削除" },
                { "AreaBulldozer.UI.Selection", "選択" },
                { "AreaBulldozer.UI.Circle", "円" },
                { "AreaBulldozer.UI.Square", "正方形" },
                { "AreaBulldozer.UI.CircleTooltip", "カーソルの周りの丸い選択エリア" },
                { "AreaBulldozer.UI.SquareTooltip", "右マウスのボタンを押して回転できる方形選択領域" },
                { "AreaBulldozer.UI.DecreaseSize", "縮小選択" },
                { "AreaBulldozer.UI.IncreaseSize", "成長選択" },
                { "AreaBulldozer.UI.RotateLeft", "逆時計方向を回す" },
                { "AreaBulldozer.UI.RotateRight", "時計方向を回す" },
                { "AreaBulldozer.UI.MissingAssets", "欠損資産" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "確認された失踪車両,工場,オブジェクトを含むサブオブジェクトの白色の場所保持者を削除する.建物やネットワークは除外されます." },
                { "AreaBulldozer.UI.VegetationTooltip", "樹木や灌木や植物を消す" },
                { "AreaBulldozer.UI.BuildingsTooltip", "周辺の建物を削除する" },
                { "AreaBulldozer.UI.Cars", "自動車" },
                { "AreaBulldozer.UI.CarsTooltip", "移動する道路車や駐車された道路車を取り除きます. ゲームは自動的に停止し,モッドが停止した場合にのみ前の速度を回復します." },
                { "AreaBulldozer.UI.RoadsTooltip", "道路を消す" },
                { "AreaBulldozer.UI.PathsTooltip", "歩行者・自転車道を削除する" },
                { "AreaBulldozer.UI.RailwaysTooltip", "鉄道線を消す" },
                { "AreaBulldozer.UI.SurfacesTooltip", "表面と領域を削除する" },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "道具やマーカー用の マスタースイッチは 詳細フィルターを有効にする" },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "道具/マーカー" },
                { "AreaBulldozer.UI.Radius", "半径" },
                { "AreaBulldozer.UI.HalfSide", "スクエアサイズ" },
                { "AreaBulldozer.UI.Rotation", "旋回" },
                { "AreaBulldozer.UI.Degrees", "デグ" },
                { "AreaBulldozer.UI.MainFilters", "オブジェクトフィルター" },
                { "AreaBulldozer.UI.ChooseFilters", "フィルターを選択する" },
                { "AreaBulldozer.UI.Vegetation", "樹木と植物" },
                { "AreaBulldozer.UI.VegetationShort", "植物" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "資産の植物を保護する" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "植物のフィルターで削除する際に建物やその他の資産に属する植物を保持する." },
                { "AreaBulldozer.UI.TreeAgeFilter", "樹齢" },
                { "AreaBulldozer.UI.TreeAgeAll", "すべての植物" },
                { "AreaBulldozer.UI.TreeAgeSapling", "苗木" },
                { "AreaBulldozer.UI.TreeAgeYoung", "若木" },
                { "AreaBulldozer.UI.TreeAgeMature", "成木" },
                { "AreaBulldozer.UI.TreeAgeElderly", "老木" },
                { "AreaBulldozer.UI.TreeAgeDead", "枯木" },
                { "AreaBulldozer.UI.TreeAgePrevious", "前の年齢段階" },
                { "AreaBulldozer.UI.TreeAgeNext", "次の年齢段階" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "植物のフィルターとともに,覆われた木々や Anarchy - 置かれた木々を取り除くことができます." },
                { "AreaBulldozer.UI.Buildings", "建物" },
                { "AreaBulldozer.UI.Roads", "道路" },
                { "AreaBulldozer.UI.Paths", "歩行道" },
                { "AreaBulldozer.UI.Railways", "鉄道線" },
                { "AreaBulldozer.UI.Surfaces", "表面及び空間" },
                { "AreaBulldozer.UI.SurfacesShort", "表面" },
                { "AreaBulldozer.UI.StaticObjects", "道具とマーカー" },
                { "AreaBulldozer.UI.StaticObjectsShort", "道具/マーカー" },
                { "AreaBulldozer.UI.AdvancedFilters", "先進な道具とマーカーフィルター" },
                { "AreaBulldozer.UI.StaticMasterNotice", "これらのカテゴリを使用するために上記のプロプスとマーカーを有効にします." },
                { "AreaBulldozer.UI.GeneralProps", "一般道具" },
                { "AreaBulldozer.UI.StreetLights", "ストリートライト" },
                { "AreaBulldozer.UI.QuantityObjects", "缶詰と量物" },
                { "AreaBulldozer.UI.Branding", "ブランドや広告" },
                { "AreaBulldozer.UI.ActivityLocations", "活動場所" },
                { "AreaBulldozer.UI.SpawnLocations", "スポーン・場所" },
                { "AreaBulldozer.UI.AssetLanes", "資産ライン / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "薄い背景" },
                { "AreaBulldozer.UI.BackgroundDarkness", "背景の暗闇" },
                { "AreaBulldozer.UI.Safety", "安全設定" },
                { "AreaBulldozer.UI.BuildingSubObjects", "建設サブオブジェクト" },
                { "AreaBulldozer.UI.NetworkSubObjects", "ネットワークサブオブジェクト" },
                { "AreaBulldozer.UI.ProtectOwned", "財産を保護する" },
                { "AreaBulldozer.UI.ConfirmLarge", "大規模な選択を確認する" },
                { "AreaBulldozer.UI.Threshold", "確認しきい値" },
                { "AreaBulldozer.UI.Objects", "物体" },
                { "AreaBulldozer.UI.ApplyHelp", "選択されたオブジェクトを削除する" },
                { "AreaBulldozer.UI.RotateHelp", "押して 引っ張って回る" },
                { "AreaBulldozer.UI.ShortcutHelp", "トーグルツール" },

                { "AreaBulldozer.UI.Triangle", "三角形" },
                { "AreaBulldozer.UI.TriangleTooltip", "カーソルの周りに等面の選択エリア. サイズは中心から角まで測定されます. 右マウスのボタンを押して水平に移動してそれを回します." },
                { "AreaBulldozer.UI.TriangleSize", "三角形サイズ" },
                { "AreaBulldozer.UI.MultiPointLine", "多点線" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "廊下は直線または丸い移行で2〜25点を使用します.左クリックでポイントを追加します.最終点をダブルクリックして終了し削除します.右クリックで最後のポイントを取り除きます. Escは選択全体をキャンセルします." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "多点線幅" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "廊下を狭める" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "廊下を広げる" },
                { "AreaBulldozer.UI.PolylineStyle", "ラインスタイル" },
                { "AreaBulldozer.UI.PolylineStraight", "直接" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "配置されたすべての点を直線段で接続する." },
                { "AreaBulldozer.UI.PolylineCurved", "曲線" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "位置する点間の移行を丸める. 曲線は隣接する段間に制御され,超越しない." },
                { "AreaBulldozer.UI.PolylineRounding", "曲線丸化" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "丸化を減らす" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "丸化を増やす" },
                { "AreaBulldozer.UI.Spline", "多点線" },
                { "AreaBulldozer.UI.SplineTooltip", "2〜25点を使用する廊下.左クリックでポイントを追加する.ダブルクリックで終了し削除する.右クリックで最後のポイントを削除する. Escはキャンセルする." },
                { "AreaBulldozer.UI.SplineWidth", "多点線幅" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "自由形領域" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "自由形領域" },
                { "AreaBulldozer.UI.PolygonPoints", "ポイント" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "無制限の角点を使用してフリーフォーム削除エリアを定義する.点を配置するために左クリック.多角形を閉じるために最初の点をクリックするか,ダブルクリックする.右クリックすると最後の点を取り除き,Escは現在の選択をキャンセルします.自己交差する辺は自動的に防止されます." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "診断ログを有効にする" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "ログに追加情報と診断メッセージを書きます. デフォルトで無効です. この設定に関係なく警告とエラーがまだ記録されます." }
            };
        }

        public void Unload()
        {
        }
    }
}
