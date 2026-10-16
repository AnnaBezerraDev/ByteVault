using System;

namespace ByteVault.Dominio.Entidades;

public class RegistroAtividade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string TipoAcao { get; set; } = string.Empty;
    public Guid? RegistroId { get; set; }
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
}
