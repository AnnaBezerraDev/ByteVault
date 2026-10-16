using System;

namespace ByteVault.Aplicacao.DTOs;

public class ItemPesquisaGlobalDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // "Projeto", "Snippet", "Comando", "Anotacao", "Bug", "Tag"
    public string DetalheAdicional { get; set; } = string.Empty;
}
