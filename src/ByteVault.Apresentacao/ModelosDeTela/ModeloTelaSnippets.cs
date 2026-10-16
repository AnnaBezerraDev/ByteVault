using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaSnippets : ModeloTelaBase
{
    private readonly IServicoTrechoCodigo _servicoTrechoCodigo;

    [ObservableProperty]
    private string _filtroTermo = string.Empty;

    [ObservableProperty]
    private string? _filtroLinguagem;

    [ObservableProperty]
    private TrechoCodigoDto? _snippetSelecionado;

    [ObservableProperty]
    private bool _emModoEdicao;

    [ObservableProperty]
    private TrechoCodigoDto _snippetEmEdicao = new();

    [ObservableProperty]
    private bool _exibindoAvisoCopiado;

    public ObservableCollection<TrechoCodigoDto> Snippets { get; } = new();
    public ObservableCollection<string> Linguagens { get; } = new();

    public ModeloTelaSnippets(IServicoTrechoCodigo servicoTrechoCodigo)
    {
        _servicoTrechoCodigo = servicoTrechoCodigo;
        Titulo = "Trechos de Código (Snippets)";
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            var lings = await _servicoTrechoCodigo.ObterLinguagensDisponiveisAsync();
            Linguagens.Clear();
            foreach (var l in lings)
            {
                Linguagens.Add(l);
            }

            var lista = await _servicoTrechoCodigo.FiltrarAsync(
                string.IsNullOrWhiteSpace(FiltroTermo) ? null : FiltroTermo,
                FiltroLinguagem,
                null,
                null);

            Snippets.Clear();
            foreach (var s in lista)
            {
                Snippets.Add(s);
            }
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        await CarregarAsync();
    }

    [RelayCommand]
    private void NovoSnippet()
    {
        SnippetEmEdicao = new TrechoCodigoDto
        {
            Linguagem = "C#",
            EhFavorito = false
        };
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void EditarSnippet(TrechoCodigoDto? dto)
    {
        var alvo = dto ?? SnippetSelecionado;
        if (alvo == null) return;

        SnippetEmEdicao = new TrechoCodigoDto
        {
            Id = alvo.Id,
            Titulo = alvo.Titulo,
            Descricao = alvo.Descricao,
            Codigo = alvo.Codigo,
            Linguagem = alvo.Linguagem,
            Categoria = alvo.Categoria,
            ProjetoId = alvo.ProjetoId,
            EhFavorito = alvo.EhFavorito,
            Tags = new(alvo.Tags)
        };
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void CancelarEdicao()
    {
        EmModoEdicao = false;
    }

    [RelayCommand]
    private async Task SalvarEdicaoAsync()
    {
        if (string.IsNullOrWhiteSpace(SnippetEmEdicao.Titulo)) return;

        EstaCarregando = true;
        try
        {
            await _servicoTrechoCodigo.SalvarAsync(SnippetEmEdicao);
            EmModoEdicao = false;
            await CarregarAsync();
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task DuplicarSnippetAsync(TrechoCodigoDto? s)
    {
        var alvo = s ?? SnippetSelecionado;
        if (alvo == null) return;

        await _servicoTrechoCodigo.DuplicarAsync(alvo.Id);
        await CarregarAsync();
    }

    [RelayCommand]
    private async Task CopiarCodigoAsync(TrechoCodigoDto? s)
    {
        var alvo = s ?? SnippetSelecionado;
        if (alvo == null || string.IsNullOrEmpty(alvo.Codigo)) return;

        Clipboard.SetText(alvo.Codigo);
        ExibindoAvisoCopiado = true;
        await Task.Delay(2000);
        ExibindoAvisoCopiado = false;
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync(TrechoCodigoDto? s)
    {
        var alvo = s ?? SnippetSelecionado;
        if (alvo == null) return;

        await _servicoTrechoCodigo.AlternarFavoritoAsync(alvo.Id);
        alvo.EhFavorito = !alvo.EhFavorito;
    }

    [RelayCommand]
    private async Task ExcluirSnippetAsync(TrechoCodigoDto? s)
    {
        var alvo = s ?? SnippetSelecionado;
        if (alvo == null) return;

        await _servicoTrechoCodigo.ExcluirAsync(alvo.Id);
        Snippets.Remove(alvo);
        if (SnippetSelecionado?.Id == alvo.Id)
        {
            SnippetSelecionado = null;
        }
    }
}
