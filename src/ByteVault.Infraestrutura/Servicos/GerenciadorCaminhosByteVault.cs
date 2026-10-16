using System;
using System.IO;

namespace ByteVault.Infraestrutura.Servicos;

public static class GerenciadorCaminhosByteVault
{
    private static readonly string DiretorioBasePadrao = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "BYTEVAULT"
    );

    public static string ObterDiretorioBase() => DiretorioBasePadrao;

    public static string ObterDiretorioDados() => Path.Combine(DiretorioBasePadrao, "Dados");

    public static string ObterDiretorioBackups() => Path.Combine(DiretorioBasePadrao, "Backups");

    public static string ObterDiretorioLogs() => Path.Combine(DiretorioBasePadrao, "Logs");

    public static string ObterDiretorioConfiguracoes() => Path.Combine(DiretorioBasePadrao, "Configuracoes");

    public static string ObterCaminhoBancoDados() => Path.Combine(ObterDiretorioDados(), "bytevault.db");

    public static void GarantirDiretoriosCriados()
    {
        Directory.CreateDirectory(ObterDiretorioBase());
        Directory.CreateDirectory(ObterDiretorioDados());
        Directory.CreateDirectory(ObterDiretorioBackups());
        Directory.CreateDirectory(ObterDiretorioLogs());
        Directory.CreateDirectory(ObterDiretorioConfiguracoes());
    }
}
