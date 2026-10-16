using System;
using System.Collections.Generic;

namespace ByteVault.Dominio.Entidades;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string CorHex { get; set; } = "#6C8CFF";
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<TagProjeto> TagsProjetos { get; set; } = new List<TagProjeto>();
    public ICollection<TagSnippet> TagsSnippets { get; set; } = new List<TagSnippet>();
    public ICollection<TagComando> TagsComandos { get; set; } = new List<TagComando>();
    public ICollection<TagAnotacao> TagsAnotacoes { get; set; } = new List<TagAnotacao>();
    public ICollection<TagBug> TagsBugs { get; set; } = new List<TagBug>();
}
