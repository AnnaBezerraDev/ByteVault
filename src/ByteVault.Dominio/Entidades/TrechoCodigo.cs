using System;
using System.Collections.Generic;

namespace ByteVault.Dominio.Entidades;

public class TrechoCodigo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Linguagem { get; set; } = "C#";
    public string? Categoria { get; set; }
    public Guid? ProjetoId { get; set; }
    public Projeto? Projeto { get; set; }
    public bool EhFavorito { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public ICollection<TagSnippet> Tags { get; set; } = new List<TagSnippet>();
    public ICollection<RelacaoBugSnippet> BugsRelacionados { get; set; } = new List<RelacaoBugSnippet>();
}
