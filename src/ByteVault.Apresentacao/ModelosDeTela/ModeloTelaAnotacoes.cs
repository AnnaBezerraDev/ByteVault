using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaAnotacoes : ModeloTelaBase
{
    private readonly IServicoAnotacao _servicoAnotacao;

    [ObservableProperty]
    private string _filtroTermo = string.Empty;

    [ObservableProperty]
    private AnotacaoDto? _anotacaoSelecionada;

    [ObservableProperty]
    private bool _emModoEdicao;

    [ObservableProperty]
    private AnotacaoDto _anotacaoEmEdicao = new();

    [ObservableProperty]
    private string _estadoSalvamento = "Salvo";

    [ObservableProperty]
    private bool _modoPreVisualizacao;

    public ObservableCollection<AnotacaoDto> Anotacoes { get; } = new();

    public ModeloTelaAnotacoes(IServicoAnotacao servicoAnotacao)
    {
        _servicoAnotacao = servicoAnotacao;
        Titulo = "Anotações Técnicas";
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            var lista = await _servicoAnotacao.FiltrarAsync(
                string.IsNullOrWhiteSpace(FiltroTermo) ? null : FiltroTermo,
                null,
                null,
                null);

            Anotacoes.Clear();
            foreach (var a in lista)
            {
                Anotacoes.Add(a);
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
    private void NovaAnotacao()
    {
        AnotacaoEmEdicao = new AnotacaoDto
        {
            Titulo = "Nova Anotação",
            Conteudo = "# Título da Anotação\n\nEscreva suas anotações técnicas em **Markdown** aqui...\n\n```csharp\n// Exemplo de código\n```",
            Categoria = "Geral",
            EhFavorito = false
        };
        EstadoSalvamento = "Alterações não salvas";
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void EditarAnotacao(AnotacaoDto? dto)
    {
        var alvo = dto ?? AnotacaoSelecionada;
        if (alvo == null) return;

        AnotacaoEmEdicao = new AnotacaoDto
        {
            Id = alvo.Id,
            Titulo = alvo.Titulo,
            Conteudo = alvo.Conteudo,
            Categoria = alvo.Categoria,
            ProjetoId = alvo.ProjetoId,
            EhFavorito = alvo.EhFavorito,
            Tags = new(alvo.Tags)
        };
        EstadoSalvamento = "Salvo";
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
        if (string.IsNullOrWhiteSpace(AnotacaoEmEdicao.Titulo)) return;

        EstaCarregando = true;
        try
        {
            await _servicoAnotacao.SalvarAsync(AnotacaoEmEdicao);
            EstadoSalvamento = "Salvo";
            EmModoEdicao = false;
            await CarregarAsync();
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private void AlternarModoPreVisualizacao()
    {
        ModoPreVisualizacao = !ModoPreVisualizacao;
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync(AnotacaoDto? a)
    {
        var alvo = a ?? AnotacaoSelecionada;
        if (alvo == null) return;

        await _servicoAnotacao.AlternarFavoritoAsync(alvo.Id);
        alvo.EhFavorito = !alvo.EhFavorito;
    }

    [RelayCommand]
    private async Task ExcluirAnotacaoAsync(AnotacaoDto? a)
    {
        var alvo = a ?? AnotacaoSelecionada;
        if (alvo == null) return;

        await _servicoAnotacao.ExcluirAsync(alvo.Id);
        Anotacoes.Remove(alvo);
        if (AnotacaoSelecionada?.Id == alvo.Id)
        {
            AnotacaoSelecionada = null;
        }
    }
}
