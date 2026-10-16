using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaComandos : ModeloTelaBase
{
    private readonly IServicoComando _servicoComando;

    [ObservableProperty]
    private string _filtroTermo = string.Empty;

    [ObservableProperty]
    private string? _filtroCategoria;

    [ObservableProperty]
    private RegistroComandoDto? _comandoSelecionado;

    [ObservableProperty]
    private bool _emModoEdicao;

    [ObservableProperty]
    private RegistroComandoDto _comandoEmEdicao = new();

    [ObservableProperty]
    private bool _exibindoAvisoCopiado;

    public ObservableCollection<RegistroComandoDto> Comandos { get; } = new();
    public ObservableCollection<string> Categorias { get; } = new();

    public ModeloTelaComandos(IServicoComando servicoComando)
    {
        _servicoComando = servicoComando;
        Titulo = "Comandos de Terminal";
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            var cats = await _servicoComando.ObterCategoriasDisponiveisAsync();
            Categorias.Clear();
            foreach (var c in cats)
            {
                Categorias.Add(c);
            }

            var lista = await _servicoComando.FiltrarAsync(
                string.IsNullOrWhiteSpace(FiltroTermo) ? null : FiltroTermo,
                FiltroCategoria,
                null,
                null,
                null);

            Comandos.Clear();
            foreach (var c in lista)
            {
                Comandos.Add(c);
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
    private void NovoComando()
    {
        ComandoEmEdicao = new RegistroComandoDto
        {
            FerramentaOuAmbiente = "Git",
            SistemaOperacional = "Multiplataforma",
            EhFavorito = false
        };
        EmModoEdicao = true;
    }

    [RelayCommand]
    private void EditarComando(RegistroComandoDto? dto)
    {
        var alvo = dto ?? ComandoSelecionado;
        if (alvo == null) return;

        ComandoEmEdicao = new RegistroComandoDto
        {
            Id = alvo.Id,
            Titulo = alvo.Titulo,
            Descricao = alvo.Descricao,
            TextoComando = alvo.TextoComando,
            FerramentaOuAmbiente = alvo.FerramentaOuAmbiente,
            SistemaOperacional = alvo.SistemaOperacional,
            Parametros = alvo.Parametros,
            ExemploUtilizacao = alvo.ExemploUtilizacao,
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
        if (string.IsNullOrWhiteSpace(ComandoEmEdicao.Titulo) ||
            string.IsNullOrWhiteSpace(ComandoEmEdicao.TextoComando)) return;

        EstaCarregando = true;
        try
        {
            await _servicoComando.SalvarAsync(ComandoEmEdicao);
            EmModoEdicao = false;
            await CarregarAsync();
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task DuplicarComandoAsync(RegistroComandoDto? c)
    {
        var alvo = c ?? ComandoSelecionado;
        if (alvo == null) return;

        await _servicoComando.DuplicarAsync(alvo.Id);
        await CarregarAsync();
    }

    [RelayCommand]
    private async Task CopiarComandoAsync(RegistroComandoDto? c)
    {
        var alvo = c ?? ComandoSelecionado;
        if (alvo == null || string.IsNullOrEmpty(alvo.TextoComando)) return;

        // Regra de segurança: apenas copia o texto, nunca executa!
        Clipboard.SetText(alvo.TextoComando);
        ExibindoAvisoCopiado = true;
        await Task.Delay(2000);
        ExibindoAvisoCopiado = false;
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync(RegistroComandoDto? c)
    {
        var alvo = c ?? ComandoSelecionado;
        if (alvo == null) return;

        await _servicoComando.AlternarFavoritoAsync(alvo.Id);
        alvo.EhFavorito = !alvo.EhFavorito;
    }

    [RelayCommand]
    private async Task ExcluirComandoAsync(RegistroComandoDto? c)
    {
        var alvo = c ?? ComandoSelecionado;
        if (alvo == null) return;

        await _servicoComando.ExcluirAsync(alvo.Id);
        Comandos.Remove(alvo);
        if (ComandoSelecionado?.Id == alvo.Id)
        {
            ComandoSelecionado = null;
        }
    }
}
