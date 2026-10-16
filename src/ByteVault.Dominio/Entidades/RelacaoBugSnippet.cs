using System;

namespace ByteVault.Dominio.Entidades;

public class RelacaoBugSnippet
{
    public Guid BugId { get; set; }
    public RelatorioBug Bug { get; set; } = null!;

    public Guid TrechoCodigoId { get; set; }
    public TrechoCodigo TrechoCodigo { get; set; } = null!;
}
