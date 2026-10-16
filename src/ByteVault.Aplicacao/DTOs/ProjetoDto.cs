using System;
using System.Collections.Generic;
using ByteVault.Dominio.Enumeradores;

namespace ByteVault.Aplicacao.DTOs;

public class ProjetoDto
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

    public List<string> Tags { get; set; } = new();
    public int TotalSnippets { get; set; }
    public int TotalComandos { get; set; }
    public int TotalAnotacoes { get; set; }
    public int TotalBugs { get; set; }
}
