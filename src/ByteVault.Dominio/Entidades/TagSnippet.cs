using System;

namespace ByteVault.Dominio.Entidades;

public class TagSnippet
{
    public Guid TrechoCodigoId { get; set; }
    public TrechoCodigo TrechoCodigo { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
