using System;
using System.Windows;

namespace ByteVault.Apresentacao.Servicos;

public static class ServicoTema
{
    public const string TemaEscuro = "Escuro";
    public const string TemaClaro = "Claro";
    public const string TemaMeiaNoite = "Meia-Noite";

    public static string TemaAtual { get; private set; } = TemaEscuro;

    public static event Action<string>? TemaAlterado;

    public static void AplicarTema(string? nomeTema)
    {
        if (string.IsNullOrWhiteSpace(nomeTema))
        {
            nomeTema = TemaEscuro;
        }

        var app = Application.Current;
        if (app == null) return;

        var arquivoTema = nomeTema.Trim() switch
        {
            TemaClaro => "TemaClaro.xaml",
            TemaMeiaNoite => "TemaMeiaNoite.xaml",
            _ => "TemaEscuro.xaml"
        };

        var uriTema = new Uri($"/ByteVault.Apresentacao;component/Recursos/Temas/{arquivoTema}", UriKind.RelativeOrAbsolute);
        var novoDict = new ResourceDictionary { Source = uriTema };

        // 1. Substitui o dicionário de cores em MergedDictionaries
        for (int i = 0; i < app.Resources.MergedDictionaries.Count; i++)
        {
            var md = app.Resources.MergedDictionaries[i];
            if (md.Source != null && (md.Source.OriginalString.Contains("Cores.xaml") || md.Source.OriginalString.Contains("Tema")))
            {
                app.Resources.MergedDictionaries[i] = novoDict;
                break;
            }
        }

        // 2. Atualizar as chaves diretamente no dicionário raiz para DynamicResources
        foreach (var key in novoDict.Keys)
        {
            app.Resources[key] = novoDict[key];
        }

        TemaAtual = nomeTema.Trim();
        TemaAlterado?.Invoke(TemaAtual);
    }

    public static string AlternarProximoTema()
    {
        var proximo = TemaAtual switch
        {
            TemaEscuro => TemaClaro,
            TemaClaro => TemaMeiaNoite,
            TemaMeiaNoite => TemaEscuro,
            _ => TemaEscuro
        };

        AplicarTema(proximo);
        return proximo;
    }
}
