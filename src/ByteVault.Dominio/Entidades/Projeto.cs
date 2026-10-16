using System;
using System.Collections.Generic;
using ByteVault.Dominio.Enumeradores;

namespace ByteVault.Dominio.Entidades;

public class Projeto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusProjeto Status { get; set; } = StatusProjeto.Planejado;
    public string? LinguagensTecnologias { get; set; }
    public string? CaminhoLocal { get; set; }
    public string? UrlRepositorio { get; set; }
    public string? UrlDocumentacao { get; set; }
    public string? Observacoes { get; set; }
    public bool EhFavorito { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataUltimaAbertura { get; set; }

    public ICollection<TagProjeto> Tags { get; set; } = new List<TagProjeto>();
    public ICollection<TecnologiaProjeto> Tecnologias { get; set; } = new List<TecnologiaProjeto>();
    public ICollection<TrechoCodigo> TrechosCodigo { get; set; } = new List<TrechoCodigo>();
    public ICollection<RegistroComando> Comandos { get; set; } = new List<RegistroComando>();
    public ICollection<Anotacao> Anotacoes { get; set; } = new List<Anotacao>();
    public ICollection<RelatorioBug> Bugs { get; set; } = new List<RelatorioBug>();
}
