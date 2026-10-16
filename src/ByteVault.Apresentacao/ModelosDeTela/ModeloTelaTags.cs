using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaTags : ModeloTelaBase
{
    private readonly IServicoTag _servicoTag;

    [ObservableProperty]
    private string _novaTagNome = string.Empty;

    [ObservableProperty]
    private string _novaTagCor = "#6C8CFF";

    [ObservableProperty]
    private TagDto? _tagSelecionada;

    [ObservableProperty]
    private string? _mensagemErro;

    public ObservableCollection<TagDto> Tags { get; } = new();

    public ModeloTelaTags(IServicoTag servicoTag)
    {
        _servicoTag = servicoTag;
        Titulo = "Gerenciamento de Tags";
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            MensagemErro = null;
            var lista = await _servicoTag.ObterTodasAsync();
            Tags.Clear();
            foreach (var t in lista)
            {
                Tags.Add(t);
            }
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task CriarTagAsync()
    {
        if (string.IsNullOrWhiteSpace(NovaTagNome)) return;

        try
        {
            MensagemErro = null;
            await _servicoTag.CriarOuObterAsync(NovaTagNome.Trim(), NovaTagCor);
            NovaTagNome = string.Empty;
            await CarregarAsync();
        }
        catch (Exception ex)
        {
            MensagemErro = ex.Message;
        }
    }

    [RelayCommand]
    private async Task ExcluirTagAsync(TagDto? t)
    {
        var alvo = t ?? TagSelecionada;
        if (alvo == null) return;

        await _servicoTag.ExcluirAsync(alvo.Id);
        Tags.Remove(alvo);
        if (TagSelecionada?.Id == alvo.Id)
        {
            TagSelecionada = null;
        }
    }
}
