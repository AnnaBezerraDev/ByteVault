using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Enumeradores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaBugs : ModeloTelaBase
{
    private readonly IServicoBug _servicoBug;
    private readonly IServicoTrechoCodigo _servicoTrechoCodigo;

    [ObservableProperty]
    private string _filtroTermo = string.Empty;

    [ObservableProperty]
    private StatusBug? _filtroStatus;

    [ObservableProperty]
    private RelatorioBugDto? _bugSelecionado;

    [ObservableProperty]
    private bool _emModoEdicao;

    [ObservableProperty]
    private RelatorioBugDto _bugEmEdicao = new();

    [ObservableProperty]
    private TrechoCodigoDto? _snippetSelecionadoParaVincular;

    partial void OnSnippetSelecionadoParaVincularChanged(TrechoCodigoDto? value)
    {
        if (value != null)
        {
            VincularSnippet(value);
            SnippetSelecionadoParaVincular = null;
        }
    }

    public ObservableCollection<RelatorioBugDto> Bugs { get; } = new();
    public ObservableCollection<TrechoCodigoDto> SnippetsDisponiveis { get; } = new();
    public ObservableCollection<TrechoCodigoDto> SnippetsVinculados { get; } = new();

    public Array ListaStatusBug => Enum.GetValues(typeof(StatusBug));
    public Array ListaSeveridadesBug => Enum.GetValues(typeof(SeveridadeBug));

    public ModeloTelaBugs(IServicoBug servicoBug, IServicoTrechoCodigo servicoTrechoCodigo)
    {
        _servicoBug = servicoBug;
        _servicoTrechoCodigo = servicoTrechoCodigo;
        Titulo = "Diário de Bugs";
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            var snippets = await _servicoTrechoCodigo.ObterTodosAsync();
            SnippetsDisponiveis.Clear();
            foreach (var s in snippets)
            {
                SnippetsDisponiveis.Add(s);
            }

            var lista = await _servicoBug.FiltrarAsync(
                string.IsNullOrWhiteSpace(FiltroTermo) ? null : FiltroTermo,
                FiltroStatus,
                null,
                null,
                null);

            Bugs.Clear();
            foreach (var b in lista)
            {
                Bugs.Add(b);
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
    private void NovoBug()
    {
        BugEmEdicao = new RelatorioBugDto
        {
            Status = StatusBug.Aberto,
            Severidade = SeveridadeBug.Media,
            EhFavorito = false
        };
        SnippetsVinculados.Clear();
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void EditarBug(RelatorioBugDto? dto)
    {
        var alvo = dto ?? BugSelecionado;
        if (alvo == null) return;

        BugEmEdicao = new RelatorioBugDto
        {
            Id = alvo.Id,
            Titulo = alvo.Titulo,
            Descricao = alvo.Descricao,
            MensagemErroOriginal = alvo.MensagemErroOriginal,
            TecnologiaRelacionada = alvo.TecnologiaRelacionada,
            Ambiente = alvo.Ambiente,
            CodigoRelevante = alvo.CodigoRelevante,
            Solucao = alvo.Solucao,
            Status = alvo.Status,
            Severidade = alvo.Severidade,
            ProjetoId = alvo.ProjetoId,
            EhFavorito = alvo.EhFavorito,
            Tags = new(alvo.Tags),
            SnippetSolucaoIds = new(alvo.SnippetSolucaoIds),
            TitulosSnippetsSolucao = new(alvo.TitulosSnippetsSolucao)
        };

        SnippetsVinculados.Clear();
        foreach (var sid in BugEmEdicao.SnippetSolucaoIds)
        {
            var snip = SnippetsDisponiveis.FirstOrDefault(s => s.Id == sid);
            if (snip != null)
            {
                SnippetsVinculados.Add(snip);
            }
        }

        EmModoEdicao = true;
    }

    [RelayCommand]
    private void VincularSnippet(TrechoCodigoDto? snippet)
    {
        if (snippet == null) return;
        if (!BugEmEdicao.SnippetSolucaoIds.Contains(snippet.Id))
        {
            BugEmEdicao.SnippetSolucaoIds.Add(snippet.Id);
            BugEmEdicao.TitulosSnippetsSolucao.Add(snippet.Titulo);
            SnippetsVinculados.Add(snippet);
        }
    }

    [RelayCommand]
    private void DesvincularSnippet(TrechoCodigoDto? snippet)
    {
        if (snippet == null) return;
        BugEmEdicao.SnippetSolucaoIds.Remove(snippet.Id);
        BugEmEdicao.TitulosSnippetsSolucao.Remove(snippet.Titulo);
        SnippetsVinculados.Remove(snippet);
    }

    [RelayCommand]
    private void CancelarEdicao()
    {
        EmModoEdicao = false;
    }

    [RelayCommand]
    private async Task SalvarEdicaoAsync()
    {
        if (string.IsNullOrWhiteSpace(BugEmEdicao.Titulo)) return;

        EstaCarregando = true;
        try
        {
            await _servicoBug.SalvarAsync(BugEmEdicao);
            EmModoEdicao = false;
            await CarregarAsync();
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task AlternarStatusResolvidoAsync(RelatorioBugDto? b)
    {
        var alvo = b ?? BugSelecionado;
        if (alvo == null) return;

        var novoStatus = alvo.Status switch
        {
            StatusBug.Aberto => StatusBug.Investigando,
            StatusBug.Investigando => StatusBug.Resolvido,
            StatusBug.Resolvido => StatusBug.NaoResolvido,
            StatusBug.NaoResolvido => StatusBug.Aberto,
            _ => StatusBug.Aberto
        };

        await _servicoBug.AlterarStatusAsync(alvo.Id, novoStatus);
        alvo.Status = novoStatus;
        if (novoStatus == StatusBug.Resolvido)
        {
            alvo.DataResolucao = DateTime.UtcNow;
        }
        else
        {
            alvo.DataResolucao = null;
        }

        await CarregarAsync();
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync(RelatorioBugDto? b)
    {
        var alvo = b ?? BugSelecionado;
        if (alvo == null) return;

        await _servicoBug.AlternarFavoritoAsync(alvo.Id);
        await CarregarAsync();
    }

    [RelayCommand]
    private async Task ExcluirBugAsync(RelatorioBugDto? b)
    {
        var alvo = b ?? BugSelecionado;
        if (alvo == null) return;

        await _servicoBug.ExcluirAsync(alvo.Id);
        Bugs.Remove(alvo);
        if (BugSelecionado?.Id == alvo.Id)
        {
            BugSelecionado = null;
        }
    }
}
