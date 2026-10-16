using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaDashboard : ModeloTelaBase
{
    private readonly IServicoDashboard _servicoDashboard;
    private Action<string>? _navegarPara;

    [ObservableProperty]
    private IndicadoresDashboardDto _indicadores = new();

    [ObservableProperty]
    private bool _bancoVazio = true;

    public ObservableCollection<ItemAtividadeRecenteDto> AtividadesRecentes { get; } = new();
    public ObservableCollection<ItemFavoritoDto> Favoritos { get; } = new();

    public ModeloTelaDashboard(IServicoDashboard servicoDashboard)
    {
        _servicoDashboard = servicoDashboard;
        Titulo = "Painel Inicial";
    }

    public void DefinirAcaoNavegacao(Action<string> navegarPara)
    {
        _navegarPara = navegarPara;
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            Indicadores = await _servicoDashboard.ObterIndicadoresAsync();

            AtividadesRecentes.Clear();
            foreach (var a in Indicadores.AtividadesRecentes)
            {
                AtividadesRecentes.Add(a);
            }

            Favoritos.Clear();
            foreach (var f in Indicadores.Favoritos)
            {
                Favoritos.Add(f);
            }

            BancoVazio = Indicadores.TotalProjetos == 0 &&
                         Indicadores.TotalSnippets == 0 &&
                         Indicadores.TotalComandos == 0 &&
                         Indicadores.TotalAnotacoes == 0 &&
                         Indicadores.BugsNaoResolvidos == 0;
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private void NavegarParaSecao(string secao)
    {
        _navegarPara?.Invoke(secao);
    }
}
