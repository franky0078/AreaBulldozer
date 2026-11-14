using Colossal;
using System.Collections.Generic;

namespace AreaBulldozer.Localization
{
    public class LocalePTBR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePTBR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Principais" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSubObjectGroup), "Segurança" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToolGroup), "Display e lançador" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "Atalhos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutGroup), "Informações" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteTrees)), "Remover árvores e vegetação" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteTrees)), "Alvo de árvores, arbustos e outra vegetação dentro da área de seleção." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Incluir vegetação Anarchy" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.IncludeOverriddenVegetation)), "Permite remover árvores e vegetação substituídas ou colocadas Anarchy. Esta opção é desativada por padrão." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildings)), "Remover edifícios" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildings)), "As propriedades, os marcadores e as ruas não são considerados como edifícios separados." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteCars)), "Remover carros" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteCars)), "Remove veículos de estrada em movimento e estacionados na área de seleção." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRoads)), "Remover estradas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRoads)), "Os objetivos completam as principais bordas das estradas cuja curva cruza a área de selecção." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeletePaths)), "Remover os caminhos de pedestres" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeletePaths)), "Os alvos completam bordas de caminhadas pedestres independentes cuja rota cruza a área de seleção. Estradas, ferrovias, redes de marcadores e edifícios de propriedade SubNets permanecem excluídos." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteRailways)), "Remover os trilhos ferroviários" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteRailways)), "Os alvos completam as trilhas de trem, trem, metrô e metrô independentes, cuja rota cruza a área de seleção. Estradas, caminhos pedestres, redes de marcadores e edifícios de propriedade SubNets permanecem excluídos." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSurfaces)), "Remover superfícies e espaços" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSurfaces)), "As metas completam superfícies e espaços poligonais quando a área de seleção cruza sua fronteira ou seu centro está dentro da área." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStaticObjects)), "Remover objetos estáticos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStaticObjects)), "Abilita as categorias de prop e marcadores selecionadas abaixo. Os props visíveis são controlados diretamente por sua categoria; atividade sensível, spawn e marcadores de faixa ainda respeitam as opções de segurança sub-objeto." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteGeneralProps)), "Requisitos e decorações gerais" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteGeneralProps)), "Alvo de acessórios estáticos que não pertencem a uma das categorias mais específicas." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteStreetLights)), "Luzes de rua" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteStreetLights)), "Objetos com o componente de luz de rua, incluindo luzes pertencentes a edifícios ou redes." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteQuantityObjects)), "Barras de lixo e objetos de quantidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteQuantityObjects)), "Alvo de objetos de quantidade, que podem incluir caixas de lixo e adereços similares." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBrandingObjects)), "Marca e publicidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBrandingObjects)), "Alveja objetos de marca, anúncios e decorações similares." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteActivityLocations)), "Localidades de actividade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteActivityLocations)), "Alvo localizações de atividades de ativos. Estes podem afetar o comportamento do edifício e são desativados por padrão." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteSpawnLocations)), "Lugares espaçados – azul" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteSpawnLocations)), "As marcas azuis SpawnLocation que indicam onde veículos ou outros objetos de ativo podem aparecer." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Faixas de ativos / SubLanes - verde e azul" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteMarkerNetworks)), "Alvos de veículos, pedestres e estacionamentos com marcas verdes e azuis dentro dos bens do edifício. Apenas SubLanes com NetLanePrefab ou NetLaneGeometryPrefab são selecionados; as bordas normais da estrada permanecem excluídas." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DimMarkerBackground)), "O fundo é escuro" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DimMarkerBackground)), "Dimens o fundo antes de o jogo contornar enquanto os filtros de marcadores ou ativos faltantes estão ativados." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Escuridão de fundo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MarkerBackgroundDarkness)), "Define a força de diminuição da visão de jogo em percentagem." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectionLineThickness)), "Espessura da linha de selecção" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectionLineThickness)), "Estabelece uma espessura fixa de contorno para as formas circulares e quadradas de selecção." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectAssetVegetation)), "Proteger a vegetação dos ativos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectAssetVegetation)), "Mantém plantas pertencentes a edifícios ou outros bens quando são apagadas com o filtro de vegetação." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Incluir sub-objetos de construção" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteBuildingSubObjects)), "Permite os tipos de objetos selecionados quando pertencem a um edifício. O filtro de objetos de correspondência também deve ser habilitado. Os subobjetos podem regenerar-se após atualizações de edifícios." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Incluir subobjetos da rede" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DeleteNetworkSubObjects)), "Permite os tipos de objetos selecionados quando pertencem a uma estrada ou outra rede. O filtro de objetos correspondente também deve ser habilitado. Os subobjetos podem regenerar mais tarde." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ProtectOwnedObjects)), "Proteger os objetos de propriedade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ProtectOwnedObjects)), "Protege objetos de propriedade cujo proprietário não seja reconhecido como um edifício ou rede." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UseSquareBrush)), "Usar a seleção quadrada" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UseSquareBrush)), "Desativado: seleção circular. Abilitado: quadrado livremente giratório. Segure o botão direito do mouse e mova o mouse horizontalmente para girá-lo. O tamanho configurado é o raio do círculo ou o quadrado de meio lado." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BrushRadius)), "Dimensão de seleção por defeito" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BrushRadius)), "Estabelece o tamanho em metros: raio do círculo ou comprimento quadrado de meio lado." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UIScale)), "Escala da janela de ferramentas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UIScale)), "Ajusta a escala da janela do Area Bulldozer de 75 a 125 por cento. Valores menores liberam espaço, e maiores melhoram a leitura em telas de alta resolução." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherMode)), "Localização do lançador Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherMode)), "Escolhe se Area Bulldozer é iniciado a partir de um lançador separado, das opções de ferramentas normais do Bulldozer ou do menu universal de mod." },

                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[Standalone]", "Botão independente" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[VanillaBulldozer]", "Bulldozer normal" },
                { "Options.AreaBulldozer.AreaBulldozer.Mod.AREABULLDOZERLAUNCHERMODE[UniversalModMenu]", "Menu de mod universal" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LauncherButtonMovable)), "Fazer o botão flutuante móvel" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LauncherButtonMovable)), "Permite que o lançador separado seja movido com o botão Ctrl + esquerdo do mouse. A liberação do botão do mouse salva a posição. Esta opção só se aplica quando o local do lançador é definido como 'botão Standalone'." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetLauncherPosition)), "Posição do botão de reset" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetLauncherPosition)), "Move o lançador arrastável de volta para a posição padrão no canto superior esquerdo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ConfirmLargeSelection)), "Confirmar grandes seleções" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ConfirmLargeSelection)), "Requer um segundo clique dentro de cinco segundos quando o número configurado de objetos é selecionado. A forma de seleção fica amarela enquanto a confirmação está pendente." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LargeSelectionThreshold)), "Prazo de confirmação" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LargeSelectionThreshold)), "Estabelece o número de objetos selecionados para os quais é necessária uma segunda confirmação." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ActivateToolBinding)), "Activar Area Bulldozer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ActivateToolBinding)), "Ativa ou desativa a ferramenta." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset de ligações de chave" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Resete a ligação da chave ao seu valor padrão." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Version)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Version)), "Mostra a versão atualmente instalada de Area Bulldozer." },

                { m_Setting.GetBindingKeyLocaleID(Mod.ActivateToolActionName), "Activar Area Bulldozer" },
                { m_Setting.GetBindingMapLocaleID(), "Area Bulldozer" },

                { "Editor.TOOL[AreaBulldozerTool]", "Area Bulldozer" },

                // In-game tool interface
                { "AreaBulldozer.UI.ColorUnderground", "O fundo subterrâneo" },
                { "AreaBulldozer.UI.ColorUndergroundHint", "Apenas visualização subterrânea: branco = original, cores mais escuras = dimming / tint. UI e contornos de seleção permanecem inalterados." },
                { "AreaBulldozer.UI.HeightFilter", "Filtro de altura" },
                { "AreaBulldozer.UI.HeightAll", "Todas as alturas" },
                { "AreaBulldozer.UI.HeightAbove", "Acima / no terreno" },
                { "AreaBulldozer.UI.HeightBelow", "Abaixo do terreno" },
                { "AreaBulldozer.UI.HeightFilterTooltip", "Aplica-se à pré-visualização e à eliminação. Os objetos usam sua origem; as redes são amostradas ao longo de sua curva. Até 0,5 m abaixo do terreno são considerados como superfície. Segmentos de transição mistos são selecionáveis apenas com todas as alturas. Os edifícios e seus subobjetos permanecem uma unidade." },
                { "AreaBulldozer.UI.UndergroundView", "Vista subterrânea" },
                { "AreaBulldozer.UI.UndergroundViewTooltip", "Mostra objetos subterrâneos usando a visão subterrânea do jogo. Altera apenas a visão, não o filtro de altura." },

                { "AreaBulldozer.UI.Title", "Area Bulldozer" },
                { "AreaBulldozer.UI.Colors", "Cores" },
                { "AreaBulldozer.UI.OpenColorEditor", "Editor de cor aberto" },
                { "AreaBulldozer.UI.ToolOptionsTitle", "Opções" },
                { "AreaBulldozer.UI.ToolOptionsSize", "Tamanho da interfaz de utilização" },
                { "AreaBulldozer.UI.ToolOptionsNormal", "Normal" },
                { "AreaBulldozer.UI.ToolOptionsCompact", "Compacto" },
                { "AreaBulldozer.UI.ToolOptionsSafety", "Segurança" },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuilding", "Incluir sub-objetos de construção" },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetwork", "Incluir subobjetos da rede" },
                { "AreaBulldozer.UI.ToolOptionsProtectOwned", "Proteger os objetos de propriedade" },
                { "AreaBulldozer.UI.ToolOptionsThreshold", "Prazo de confirmação" },
                { "AreaBulldozer.UI.ToolOptionsConfirm", "Confirmar grandes apagões" },
                { "AreaBulldozer.UI.ToolOptionsOn", "Ativado" },
                { "AreaBulldozer.UI.ToolOptionsOff", "Desativado" },
                { "AreaBulldozer.UI.ToolOptionsNormalTooltip", "Usa o tamanho padrão para botões, ícones e títulos." },
                { "AreaBulldozer.UI.ToolOptionsCompactTooltip", "Faz os botões, ícones e cabeçalhos menores para que o painel de ferramentas ocupem menos espaço." },
                { "AreaBulldozer.UI.ToolOptionsIncludeBuildingTooltip", "Permite os tipos de objetos selecionados quando pertencem a um edifício. O filtro de objetos de correspondência também deve ser habilitado. Os subobjetos podem regenerar-se após atualizações de edifícios." },
                { "AreaBulldozer.UI.ToolOptionsIncludeNetworkTooltip", "Permite os tipos de objetos selecionados quando pertencem a uma estrada ou outra rede. O filtro de objetos correspondente também deve ser habilitado. Os subobjetos podem regenerar mais tarde." },
                { "AreaBulldozer.UI.ToolOptionsProtectOwnedTooltip", "Protege objetos de propriedade cujo proprietário não seja reconhecido como um edifício ou rede." },
                { "AreaBulldozer.UI.ToolOptionsConfirmTooltip", "Requer um segundo clique dentro de cinco segundos quando o número configurado de objetos é selecionado. A forma de seleção fica amarela enquanto a confirmação está pendente." },
                { "AreaBulldozer.UI.ToolOptionsThresholdTooltip", "Estabelece o número de objetos selecionados para os quais é necessária uma segunda confirmação." },
                { "AreaBulldozer.UI.UndoVegetation", "Restaurar vegetação" },
                { "AreaBulldozer.UI.UndoVegetationTooltip", "Restaura a vegetação removida na última exclusão, incluindo plantas de edifícios e parques que ainda existem ({count})." },
                { "AreaBulldozer.UI.ToolOptionsClose", "Opções fechadas" },
                { "AreaBulldozer.UI.CloseColorEditor", "Editor de cor fechado" },
                { "AreaBulldozer.UI.ColorEditorTooltip", "Ajusta a seleção, a confirmação, o feedback de exclusão, a superfície selecionada e as cores de polígonos inválidos com uma visualização ao vivo." },
                { "AreaBulldozer.UI.ColorEditor", "Editor de cores" },
                { "AreaBulldozer.UI.ColorSelection", "Seleção normal" },
                { "AreaBulldozer.UI.ColorConfirmation", "Confirmação" },
                { "AreaBulldozer.UI.ColorInvalidPolygon", "Polígono inválido" },
                { "AreaBulldozer.UI.ColorDeletion", "Exclusão ativa" },
                { "AreaBulldozer.UI.ColorSurfaces", "Objetos selecionados" },
                { "AreaBulldozer.UI.ColorPresets", "Preconceitos de cores" },
                { "AreaBulldozer.UI.ChooseColor", "Escolher cor" },
                { "AreaBulldozer.UI.ColorRed", "Vermelho" },
                { "AreaBulldozer.UI.ColorGreen", "Verde" },
                { "AreaBulldozer.UI.ColorBlue", "Azul" },
                { "AreaBulldozer.UI.ColorHex", "Código HEX" },
                { "AreaBulldozer.UI.ResetCurrentColor", "Resetar esta cor" },
                { "AreaBulldozer.UI.ResetAllColors", "Reset todas as cores" },
                { "AreaBulldozer.UI.ToggleTool", "Acompanhamento Area Bulldozer" },
                { "AreaBulldozer.UI.LauncherDragHint", "Segure Ctrl + botão esquerdo do mouse para mover; solte para salvar" },
                { "AreaBulldozer.UI.ModMenuOpen", "Ferramenta aberta" },
                { "AreaBulldozer.UI.VanillaLauncherTooltip", "Abrir Area Bulldozer e remover múltiplos objetos com uma seleção circular ou quadrada." },
                { "AreaBulldozer.UI.Navigation", "Navegação" },
                { "AreaBulldozer.UI.BackToBulldozer", "De volta ao Bulldozer normal" },
                { "AreaBulldozer.UI.Close", "Ferramenta fechada" },
                { "AreaBulldozer.UI.Active", "Ativos" },
                { "AreaBulldozer.UI.Subtitle", "Remover múltiplos objetos dentro de uma área" },
                { "AreaBulldozer.UI.Selection", "Seleção" },
                { "AreaBulldozer.UI.Circle", "Círculo" },
                { "AreaBulldozer.UI.Square", "Quadrado" },
                { "AreaBulldozer.UI.CircleTooltip", "Área de seleção redonda ao redor do cursor." },
                { "AreaBulldozer.UI.SquareTooltip", "Área de selecção quadrada, rotável segurando o botão direito do mouse." },
                { "AreaBulldozer.UI.DecreaseSize", "Seleção de encolhimento" },
                { "AreaBulldozer.UI.IncreaseSize", "Seleção de crescimento" },
                { "AreaBulldozer.UI.RotateLeft", "Rotar em sentido contrário ao relógio" },
                { "AreaBulldozer.UI.RotateRight", "Rotar em sentido horário" },
                { "AreaBulldozer.UI.MissingAssets", "Ativos desaparecidos" },
                { "AreaBulldozer.UI.MissingAssetsTooltip", "Eliminar os marcadores brancos de veículos, plantas e objetos desaparecidos confirmados, incluindo os sub-objetos." },
                { "AreaBulldozer.UI.VegetationTooltip", "Eliminar árvores, arbustos e plantas." },
                { "AreaBulldozer.UI.BuildingsTooltip", "Eliminar edifícios na área." },
                { "AreaBulldozer.UI.Cars", "Carros" },
                { "AreaBulldozer.UI.CarsTooltip", "Remove veículos de estrada em movimento e estacionados. O jogo pausa automaticamente e restabelece sua velocidade anterior somente se o mod o interrompeu." },
                { "AreaBulldozer.UI.RoadsTooltip", "Eliminar as estradas da área." },
                { "AreaBulldozer.UI.PathsTooltip", "Eliminar as pistas pedestres e ciclistas." },
                { "AreaBulldozer.UI.RailwaysTooltip", "Eliminar os trilhos ferroviários." },
                { "AreaBulldozer.UI.SurfacesTooltip", "Eliminar superfícies e áreas." },
                { "AreaBulldozer.UI.StaticObjectsTooltip", "O interruptor principal para adereços e marcadores, permite os filtros de detalhes abaixo." },
                { "AreaBulldozer.UI.AdvancedFiltersShort", "Requisitos / marcadores" },
                { "AreaBulldozer.UI.Radius", "Raio" },
                { "AreaBulldozer.UI.HalfSide", "Tamanho quadrado" },
                { "AreaBulldozer.UI.Rotation", "Rotatividade" },
                { "AreaBulldozer.UI.Degrees", "DEG" },
                { "AreaBulldozer.UI.MainFilters", "Filtros de objetos" },
                { "AreaBulldozer.UI.ChooseFilters", "Escolha os filtros" },
                { "AreaBulldozer.UI.Vegetation", "Árvores e vegetação" },
                { "AreaBulldozer.UI.VegetationShort", "Vegetação" },
                { "AreaBulldozer.UI.ProtectAssetVegetation", "Proteger a vegetação dos ativos" },
                { "AreaBulldozer.UI.ProtectAssetVegetationTooltip", "Mantém plantas pertencentes a edifícios ou outros bens quando são apagadas com o filtro de vegetação." },
                { "AreaBulldozer.UI.TreeAgeFilter", "Idade das árvores" },
                { "AreaBulldozer.UI.TreeAgeAll", "Todas as plantas" },
                { "AreaBulldozer.UI.TreeAgeSapling", "Mudas" },
                { "AreaBulldozer.UI.TreeAgeYoung", "Jovens" },
                { "AreaBulldozer.UI.TreeAgeMature", "Adultas" },
                { "AreaBulldozer.UI.TreeAgeElderly", "Velhas" },
                { "AreaBulldozer.UI.TreeAgeDead", "Mortas" },
                { "AreaBulldozer.UI.TreeAgePrevious", "Estágio de idade anterior" },
                { "AreaBulldozer.UI.TreeAgeNext", "Próximo estágio de idade" },
                { "AreaBulldozer.UI.AnarchyVegetation", "Anarchy" },
                { "AreaBulldozer.UI.AnarchyVegetationTooltip", "Juntamente com o filtro de vegetação, permite remover árvores sobrecarregadas ou colocadas Anarchy." },
                { "AreaBulldozer.UI.Buildings", "Edifícios" },
                { "AreaBulldozer.UI.Roads", "Estradas" },
                { "AreaBulldozer.UI.Paths", "Caminhos de pedestres" },
                { "AreaBulldozer.UI.Railways", "Ferrovias" },
                { "AreaBulldozer.UI.Surfaces", "Superfícies e espaços" },
                { "AreaBulldozer.UI.SurfacesShort", "Superfícies" },
                { "AreaBulldozer.UI.StaticObjects", "Requisitos e marcadores" },
                { "AreaBulldozer.UI.StaticObjectsShort", "Requisitos / marcadores" },
                { "AreaBulldozer.UI.AdvancedFilters", "Filtros avançados e marcadores" },
                { "AreaBulldozer.UI.StaticMasterNotice", "Habilitar os props e marcadores acima para utilizar estas categorias." },
                { "AreaBulldozer.UI.GeneralProps", "Requisitos gerais" },
                { "AreaBulldozer.UI.StreetLights", "Luzes de rua" },
                { "AreaBulldozer.UI.QuantityObjects", "Barras e objetos de quantidade" },
                { "AreaBulldozer.UI.Branding", "Marca e publicidade" },
                { "AreaBulldozer.UI.ActivityLocations", "Localidades de actividade" },
                { "AreaBulldozer.UI.SpawnLocations", "Localizações de abóbora" },
                { "AreaBulldozer.UI.AssetLanes", "Caminhos de ativos / SubLanes" },
                { "AreaBulldozer.UI.DimBackground", "O fundo é escuro" },
                { "AreaBulldozer.UI.BackgroundDarkness", "Escuridão de fundo" },
                { "AreaBulldozer.UI.Safety", "Segurança" },
                { "AreaBulldozer.UI.BuildingSubObjects", "Subobjetos de construção" },
                { "AreaBulldozer.UI.NetworkSubObjects", "Subobjetos da rede" },
                { "AreaBulldozer.UI.ProtectOwned", "Proteger os objetos de propriedade" },
                { "AreaBulldozer.UI.ConfirmLarge", "Confirmar grandes seleções" },
                { "AreaBulldozer.UI.Threshold", "Prazo de confirmação" },
                { "AreaBulldozer.UI.Objects", "Objetos" },
                { "AreaBulldozer.UI.ApplyHelp", "Eliminar objetos selecionados" },
                { "AreaBulldozer.UI.RotateHelp", "Segurar e arrastar para girar" },
                { "AreaBulldozer.UI.ShortcutHelp", "Ferramenta de toggle" },

                { "AreaBulldozer.UI.Triangle", "Triângulo" },
                { "AreaBulldozer.UI.TriangleTooltip", "Área de seleção equilateral ao redor do cursor. O tamanho é medido do centro para um canto. Segure o botão direito do mouse e mova-se horizontalmente para girá-lo." },
                { "AreaBulldozer.UI.TriangleSize", "Dimensão de triângulo" },
                { "AreaBulldozer.UI.MultiPointLine", "Linha de múltiplos pontos" },
                { "AreaBulldozer.UI.MultiPointLineTooltip", "Corredor usando 2 a 25 pontos com transições retas ou arredondadas. Clique esquerdo adiciona pontos. Clique duas vezes no ponto final para terminar e excluir. Clique direito remove o último ponto; Esc cancela toda a seleção." },
                { "AreaBulldozer.UI.MultiPointLineWidth", "Largura de linha de vários pontos" },
                { "AreaBulldozer.UI.DecreaseLineWidth", "Tornar o corredor mais estreito" },
                { "AreaBulldozer.UI.IncreaseLineWidth", "Tornar o corredor mais amplo" },
                { "AreaBulldozer.UI.PolylineStyle", "Estilo de linha" },
                { "AreaBulldozer.UI.PolylineStraight", "Direita" },
                { "AreaBulldozer.UI.PolylineStraightTooltip", "Conecta todos os pontos colocados com segmentos retos." },
                { "AreaBulldozer.UI.PolylineCurved", "Curva" },
                { "AreaBulldozer.UI.PolylineCurvedTooltip", "Arredona as transições entre os pontos colocados. A curva permanece controlada entre os segmentos adjacentes e não ultrapassa." },
                { "AreaBulldozer.UI.PolylineRounding", "Arredondamento da curva" },
                { "AreaBulldozer.UI.DecreasePolylineRounding", "Reduzir o arredondamento" },
                { "AreaBulldozer.UI.IncreasePolylineRounding", "Aumentar o arredondamento" },
                { "AreaBulldozer.UI.Spline", "Linha de múltiplos pontos" },
                { "AreaBulldozer.UI.SplineTooltip", "Corredor usando 2 a 25 pontos. Clique esquerdo adiciona pontos. Clique duplo termina e elimina. Clique direito remove o último ponto; Esc cancela." },
                { "AreaBulldozer.UI.SplineWidth", "Largura de linha de vários pontos" },
                { "AreaBulldozer.UI.FreeAreaPolygon", "Área de forma livre" },
                { "AreaBulldozer.UI.FreeAreaPolygonActive", "Área de forma livre" },
                { "AreaBulldozer.UI.PolygonPoints", "Pontos" },
                { "AreaBulldozer.UI.FreeAreaPolygonTooltip", "Definir uma área de exclusão de forma livre usando um número ilimitado de pontos de canto. Clique à esquerda para colocar pontos. Clique no primeiro ponto ou clique duas vezes para fechar o polígono e excluir tudo dentro dele. Clique à direita remove o último ponto e Esc cancela a seleção atual." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Ativar registro de diagnóstico" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDiagnosticLogging)), "Escreve mensagens de informação e diagnóstico adicionais para o log. Desativado por padrão. Alertas e erros ainda são registrados independentemente desta configuração." }
            };
        }

        public void Unload()
        {
        }
    }
}
