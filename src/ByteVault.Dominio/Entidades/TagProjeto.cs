using System;

namespace ByteVault.Dominio.Entidades;

public class TagProjeto
{
    public Guid ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
