using System;
using System.Collections.Generic;
using ByteVault.Dominio.Enumeradores;

namespace ByteVault.Aplicacao.DTOs;

public class RelatorioBugDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? MensagemErroOriginal { get; set; }
    public string? TecnologiaRelacionada { get; set; }
    public string? Ambiente { get; set; }
    public string? CodigoRelevante { get; set; }
    public string? Solucao { get; set; }
    public StatusBug Status { get; set; } = StatusBug.Aberto;
    public SeveridadeBug Severidade { get; set; } = SeveridadeBug.Media;
    public Guid? ProjetoId { get; set; }
    public string? NomeProjeto { get; set; }
    public bool EhFavorito { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataResolucao { get; set; }

    public List<string> Tags { get; set; } = new();
    public List<Guid> SnippetSolucaoIds { get; set; } = new();
    public List<string> TitulosSnippetsSolucao { get; set; } = new();
}
