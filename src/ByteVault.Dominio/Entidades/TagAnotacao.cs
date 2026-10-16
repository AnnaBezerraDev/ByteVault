using System;

namespace ByteVault.Dominio.Entidades;

public class TagAnotacao
{
    public Guid AnotacaoId { get; set; }
    public Anotacao Anotacao { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
