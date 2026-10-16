using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ByteVault.Apresentacao.ModelosDeTela;

public abstract partial class ModeloTelaBase : ObservableObject
{
    [ObservableProperty]
    private string _titulo = string.Empty;

    [ObservableProperty]
    private bool _estaCarregando;

    [ObservableProperty]
    private string? _mensagemStatus;

    public virtual Task CarregarAsync()
    {
        return Task.CompletedTask;
    }
}
