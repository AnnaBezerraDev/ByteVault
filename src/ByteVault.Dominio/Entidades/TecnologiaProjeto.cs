using System;

namespace ByteVault.Dominio.Entidades;

public class TecnologiaProjeto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjetoId { get; set; }
    public Projeto Projeto { get; set; } = null!;
    public string Nome { get; set; } = string.Empty;
}
