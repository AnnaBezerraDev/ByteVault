using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaPrincipal : ModeloTelaBase
{
    private readonly IServicoPesquisaGlobal _servicoPesquisa;
    private readonly Dictionary<string, ModeloTelaBase> _telasRegistradas = new();
    private CancellationTokenSource? _ctsPesquisa;

    [ObservableProperty]
    private ModeloTelaBase _telaAtual;

    [ObservableProperty]
    private string _secaoAtiva = "Painel";

    [ObservableProperty]
    private bool _popupPesquisaAberto;

    [ObservableProperty]
    private string _termoPesquisaGlobal = string.Empty;

    public ObservableCollection<ItemPesquisaGlobalDto> ResultadosPesquisaGlobal { get; } = new();

    public ModeloTelaPrincipal(
        IServicoPesquisaGlobal servicoPesquisa,
        ModeloTelaDashboard telaDashboard,
        ModeloTelaProjetos telaProjetos,
        ModeloTelaSnippets telaSnippets,
        ModeloTelaComandos telaComandos,
        ModeloTelaAnotacoes telaAnotacoes,
        ModeloTelaBugs telaBugs,
        ModeloTelaTags telaTags,
        ModeloTelaFavoritos telaFavoritos,
        ModeloTelaConfiguracoes telaConfiguracoes)
    {
        _servicoPesquisa = servicoPesquisa;

        _telasRegistradas["Painel"] = telaDashboard;
        _telasRegistradas["Projetos"] = telaProjetos;
        _telasRegistradas["Snippets"] = telaSnippets;
        _telasRegistradas["Comandos"] = telaComandos;
        _telasRegistradas["Anotacoes"] = telaAnotacoes;
        _telasRegistradas["Bugs"] = telaBugs;
        _telasRegistradas["Tags"] = telaTags;
        _telasRegistradas["Favoritos"] = telaFavoritos;
        _telasRegistradas["Configuracoes"] = telaConfiguracoes;

        telaDashboard.DefinirAcaoNavegacao(secao => _ = NavegarParaAsync(secao));
        telaFavoritos.DefinirAcaoNavegacao(secao => _ = NavegarParaAsync(secao));

        _telaAtual = telaDashboard;
        MensagemStatus = "BYTEVAULT pronto. Armazenamento local ativo.";
    }

    public override async Task CarregarAsync()
    {
        await NavegarParaAsync(SecaoAtiva);
    }

    [RelayCommand]
    public async Task NavegarParaAsync(string secao)
    {
        if (_telasRegistradas.TryGetValue(secao, out var tela))
        {
            if (TelaAtual != tela || string.IsNullOrEmpty(SecaoAtiva))
            {
                SecaoAtiva = secao;
                TelaAtual = tela;
                await tela.CarregarAsync();
                MensagemStatus = $"Exibindo: {tela.Titulo}";
            }
        }
    }

    [RelayCommand]
    private void AbrirPesquisaGlobal()
    {
        PopupPesquisaAberto = true;
        TermoPesquisaGlobal = string.Empty;
        ResultadosPesquisaGlobal.Clear();
    }

    [RelayCommand]
    private void FecharPesquisaGlobal()
    {
        PopupPesquisaAberto = false;
        TermoPesquisaGlobal = string.Empty;
        ResultadosPesquisaGlobal.Clear();
    }

    partial void OnTermoPesquisaGlobalChanged(string value)
    {
        _ctsPesquisa?.Cancel();
        _ctsPesquisa = new CancellationTokenSource();
        var token = _ctsPesquisa.Token;

        _ = Task.Run(async () =>
        {
            await Task.Delay(250, token);
            if (token.IsCancellationRequested) return;

            var resultados = await _servicoPesquisa.PesquisarAsync(value, token);
            if (token.IsCancellationRequested) return;

            App.Current.Dispatcher.Invoke(() =>
            {
                ResultadosPesquisaGlobal.Clear();
                foreach (var r in resultados)
                {
                    ResultadosPesquisaGlobal.Add(r);
                }
            });
        }, token);
    }

    [RelayCommand]
    private async Task SelecionarResultadoPesquisaAsync(ItemPesquisaGlobalDto? item)
    {
        if (item == null) return;
        FecharPesquisaGlobal();

        switch (item.Tipo)
        {
            case "Projeto":
                await NavegarParaAsync("Projetos");
                break;
            case "Snippet":
                await NavegarParaAsync("Snippets");
                break;
            case "Comando":
                await NavegarParaAsync("Comandos");
                break;
            case "Anotação":
                await NavegarParaAsync("Anotacoes");
                break;
            case "Bug":
                await NavegarParaAsync("Bugs");
                break;
            case "Tag":
                await NavegarParaAsync("Tags");
                break;
        }
    }
}
