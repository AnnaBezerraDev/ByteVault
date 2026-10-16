using System;
using System.Collections.Generic;

namespace ByteVault.Dominio.Entidades;

public class RegistroComando
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string TextoComando { get; set; } = string.Empty;
    public string FerramentaOuAmbiente { get; set; } = "Git";
    public string? SistemaOperacional { get; set; } = "Multiplataforma";
    public string? Parametros { get; set; }
    public string? ExemploUtilizacao { get; set; }
    public Guid? ProjetoId { get; set; }
    public Projeto? Projeto { get; set; }
    public bool EhFavorito { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public ICollection<TagComando> Tags { get; set; } = new List<TagComando>();
}
