using System;
using System.Collections.Generic;

namespace ByteVault.Aplicacao.DTOs;

public class IndicadoresDashboardDto
{
    public int TotalProjetos { get; set; }
    public int ProjetosAtivos { get; set; }
    public int TotalSnippets { get; set; }
    public int TotalComandos { get; set; }
    public int TotalAnotacoes { get; set; }
    public int BugsNaoResolvidos { get; set; }

    public List<ItemAtividadeRecenteDto> AtividadesRecentes { get; set; } = new();
    public List<ItemFavoritoDto> Favoritos { get; set; } = new();
}

public class ItemAtividadeRecenteDto
{
    public Guid? RegistroId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string TipoAcao { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
}

public class ItemFavoritoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // "Projeto", "Snippet", "Comando", "Anotação", "Bug"
    public string Subtitulo { get; set; } = string.Empty;
    public DateTime DataAtualizacao { get; set; }
}
