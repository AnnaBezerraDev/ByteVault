using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaFavoritos : ModeloTelaBase
{
    private readonly IServicoDashboard _servicoDashboard;
    private Action<string>? _navegarPara;

    public ObservableCollection<ItemFavoritoDto> ItensFavoritos { get; } = new();

    public ModeloTelaFavoritos(IServicoDashboard servicoDashboard)
    {
        _servicoDashboard = servicoDashboard;
        Titulo = "Favoritos";
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
            var indicadores = await _servicoDashboard.ObterIndicadoresAsync();

            ItensFavoritos.Clear();
            foreach (var f in indicadores.Favoritos)
            {
                ItensFavoritos.Add(f);
            }
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private void AbrirItem(ItemFavoritoDto? item)
    {
        if (item == null) return;

        switch (item.Categoria)
        {
            case "Projeto":
                _navegarPara?.Invoke("Projetos");
                break;
            case "Snippet":
                _navegarPara?.Invoke("Snippets");
                break;
            case "Comando":
                _navegarPara?.Invoke("Comandos");
                break;
            case "Anotação":
                _navegarPara?.Invoke("Anotacoes");
                break;
            case "Bug":
                _navegarPara?.Invoke("Bugs");
                break;
        }
    }
}
