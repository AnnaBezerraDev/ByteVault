using System;

namespace ByteVault.Dominio.Entidades;

public class TagBug
{
    public Guid RelatorioBugId { get; set; }
    public RelatorioBug RelatorioBug { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
