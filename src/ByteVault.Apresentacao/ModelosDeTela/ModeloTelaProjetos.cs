using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Enumeradores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaProjetos : ModeloTelaBase
{
    private readonly IServicoProjeto _servicoProjeto;
    private readonly IServicoSistema _servicoSistema;

    [ObservableProperty]
    private string _filtroTermo = string.Empty;

    [ObservableProperty]
    private StatusProjeto? _filtroStatus;

    [ObservableProperty]
    private ProjetoDto? _projetoSelecionado;

    [ObservableProperty]
    private bool _emModoEdicao;

    [ObservableProperty]
    private ProjetoDto _projetoEmEdicao = new();

    [ObservableProperty]
    private string? _mensagemAviso;

    public ObservableCollection<ProjetoDto> Projetos { get; } = new();

    public ModeloTelaProjetos(IServicoProjeto servicoProjeto, IServicoSistema servicoSistema)
    {
        _servicoProjeto = servicoProjeto;
        _servicoSistema = servicoSistema;
        Titulo = "Projetos";
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            MensagemAviso = null;
            var lista = await _servicoProjeto.FiltrarAsync(
                string.IsNullOrWhiteSpace(FiltroTermo) ? null : FiltroTermo,
                FiltroStatus,
                null,
                null);

            Projetos.Clear();
            foreach (var p in lista)
            {
                Projetos.Add(p);
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
    private void NovoProjeto()
    {
        MensagemAviso = null;
        ProjetoEmEdicao = new ProjetoDto
        {
            Status = StatusProjeto.Planejado,
            EhFavorito = false
        };
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void EditarProjeto(ProjetoDto? dto)
    {
        var alvo = dto ?? ProjetoSelecionado;
        if (alvo == null) return;

        MensagemAviso = null;
        ProjetoEmEdicao = new ProjetoDto
        {
            Id = alvo.Id,
            Nome = alvo.Nome,
            Descricao = alvo.Descricao,
            Status = alvo.Status,
            LinguagensTecnologias = alvo.LinguagensTecnologias,
            CaminhoLocal = alvo.CaminhoLocal,
            UrlRepositorio = alvo.UrlRepositorio,
            UrlDocumentacao = alvo.UrlDocumentacao,
            Observacoes = alvo.Observacoes,
            EhFavorito = alvo.EhFavorito,
            Tags = new(alvo.Tags)
        };
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void CancelarEdicao()
    {
        EmModoEdicao = false;
        MensagemAviso = null;
    }

    [RelayCommand]
    private async Task SalvarEdicaoAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjetoEmEdicao.Nome))
        {
            MensagemAviso = "O nome do projeto é obrigatório.";
            return;
        }

        EstaCarregando = true;
        try
        {
            await _servicoProjeto.SalvarAsync(ProjetoEmEdicao);
            EmModoEdicao = false;
            MensagemAviso = null;
            await CarregarAsync();
        }
        catch (Exception ex)
        {
            MensagemAviso = ex.Message;
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        await _servicoProjeto.AlternarFavoritoAsync(alvo.Id);
        alvo.EhFavorito = !alvo.EhFavorito;
    }

    [RelayCommand]
    private async Task ArquivarProjetoAsync(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        await _servicoProjeto.ArquivarAsync(alvo.Id);
        alvo.Status = StatusProjeto.Arquivado;
    }

    [RelayCommand]
    private async Task RestaurarProjetoAsync(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        await _servicoProjeto.RestaurarAsync(alvo.Id);
        alvo.Status = StatusProjeto.EmDesenvolvimento;
    }

    [RelayCommand]
    private async Task ExcluirProjetoAsync(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        await _servicoProjeto.ExcluirAsync(alvo.Id);
        Projetos.Remove(alvo);
        if (ProjetoSelecionado?.Id == alvo.Id)
        {
            ProjetoSelecionado = null;
        }
    }

    [RelayCommand]
    private void AbrirPastaNoExplorer(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        if (!_servicoSistema.AbrirDiretorioNoExplorer(alvo.CaminhoLocal, out var erro))
        {
            MessageBox.Show(erro, "Aviso do Sistema", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private void CopiarCaminhoLocal(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null || string.IsNullOrWhiteSpace(alvo.CaminhoLocal)) return;

        _servicoSistema.CopiarParaAreaTransferencia(alvo.CaminhoLocal);
        MessageBox.Show("Caminho copiado para a área de transferência!", "ByteVault", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void AbrirUrlRepositorio(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        if (!_servicoSistema.AbrirUrlNoNavegador(alvo.UrlRepositorio, out var erro))
        {
            MessageBox.Show(erro, "Validação de Link", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void AbrirUrlDocumentacao(ProjetoDto? p)
    {
        var alvo = p ?? ProjetoSelecionado;
        if (alvo == null) return;

        if (!_servicoSistema.AbrirUrlNoNavegador(alvo.UrlDocumentacao, out var erro))
        {
            MessageBox.Show(erro, "Validação de Link", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
