using System;

namespace ByteVault.Dominio.Entidades;

public class ConfiguracaoAplicacao
{
    public int Id { get; set; } = 1;
    public string Tema { get; set; } = "Escuro";
    public string CorDestaque { get; set; } = "#6C8CFF";
    public string DensidadeVisual { get; set; } = "Confortavel";
    public string ComportamentoAoIniciar { get; set; } = "Painel";
    public bool ConfirmarExclusao { get; set; } = true;
    public bool SalvarAutomaticamente { get; set; } = true;
    public DateTime? DataUltimoBackup { get; set; }
    public string? CaminhoBanco { get; set; }
    public string VersaoEsquema { get; set; } = "1.0.0";
}
