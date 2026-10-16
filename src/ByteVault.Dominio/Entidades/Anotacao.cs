using System;
using System.Collections.Generic;

namespace ByteVault.Dominio.Entidades;

public class Anotacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public Guid? ProjetoId { get; set; }
    public Projeto? Projeto { get; set; }
    public bool EhFavorito { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    public ICollection<TagAnotacao> Tags { get; set; } = new List<TagAnotacao>();
}
