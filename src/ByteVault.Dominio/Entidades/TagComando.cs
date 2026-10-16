using System;

namespace ByteVault.Dominio.Entidades;

public class TagComando
{
    public Guid RegistroComandoId { get; set; }
    public RegistroComando RegistroComando { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
