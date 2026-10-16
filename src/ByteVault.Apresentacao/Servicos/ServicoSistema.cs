using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ByteVault.Aplicacao.Interfaces;

namespace ByteVault.Apresentacao.Servicos;

public class ServicoSistema : IServicoSistema
{
    public bool AbrirDiretorioNoExplorer(string? caminho, out string mensagemErro)
    {
        mensagemErro = string.Empty;
        if (string.IsNullOrWhiteSpace(caminho))
        {
            mensagemErro = "Nenhum caminho local foi informado para este projeto.";
            return false;
        }

        try
        {
            if (!Directory.Exists(caminho))
            {
                mensagemErro = $"O diretório '{caminho}' não existe ou não foi encontrado.";
                return false;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = caminho,
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception ex)
        {
            mensagemErro = $"Não foi possível abrir o Explorer: {ex.Message}";
            return false;
        }
    }

    public bool AbrirUrlNoNavegador(string? url, out string mensagemErro)
    {
        mensagemErro = string.Empty;
        if (string.IsNullOrWhiteSpace(url))
        {
            mensagemErro = "Nenhuma URL foi especificada.";
            return false;
        }

        try
        {
            if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                mensagemErro = "A URL informada deve possuir o esquema http:// ou https://.";
                return false;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception ex)
        {
            mensagemErro = $"Falha ao abrir o link: {ex.Message}";
            return false;
        }
    }

    public void CopiarParaAreaTransferencia(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return;
        Clipboard.SetText(texto);
    }
}
