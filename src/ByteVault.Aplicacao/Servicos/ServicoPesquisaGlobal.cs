using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Interfaces;

namespace ByteVault.Aplicacao.Servicos;

public class ServicoPesquisaGlobal : IServicoPesquisaGlobal
{
    private readonly IUnidadeDeTrabalho _uow;

    public ServicoPesquisaGlobal(IUnidadeDeTrabalho uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<ItemPesquisaGlobalDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo) || termo.Trim().Length < 2)
        {
            return Array.Empty<ItemPesquisaGlobalDto>();
        }

        var t = termo.Trim();
        var tMin = t.ToLower();
        var resultados = new List<ItemPesquisaGlobalDto>();

        // Pesquisa em Projetos
        var projetos = await _uow.Projetos.BuscarAsync(
            p => (p.Nome != null && p.Nome.ToLower().Contains(tMin)) ||
                 (p.Descricao != null && p.Descricao.ToLower().Contains(tMin)),
            cancellationToken);

        resultados.AddRange(projetos.Take(10).Select(p => new ItemPesquisaGlobalDto
        {
            Id = p.Id,
            Titulo = p.Nome,
            Descricao = p.Descricao ?? "Sem descrição",
            Tipo = "Projeto",
            DetalheAdicional = p.Status.ToString()
        }));

        cancellationToken.ThrowIfCancellationRequested();

        // Pesquisa em Snippets
        var snippets = await _uow.TrechosCodigo.BuscarAsync(
            s => (s.Titulo != null && s.Titulo.ToLower().Contains(tMin)) ||
                 (s.Descricao != null && s.Descricao.ToLower().Contains(tMin)) ||
                 (s.Codigo != null && s.Codigo.ToLower().Contains(tMin)),
            cancellationToken);

        resultados.AddRange(snippets.Take(10).Select(s => new ItemPesquisaGlobalDto
        {
            Id = s.Id,
            Titulo = s.Titulo,
            Descricao = s.Descricao ?? (s.Codigo.Length > 80 ? s.Codigo[..80] + "..." : s.Codigo),
            Tipo = "Snippet",
            DetalheAdicional = s.Linguagem
        }));

        cancellationToken.ThrowIfCancellationRequested();

        // Pesquisa em Comandos
        var comandos = await _uow.Comandos.BuscarAsync(
            c => (c.Titulo != null && c.Titulo.ToLower().Contains(tMin)) ||
                 (c.TextoComando != null && c.TextoComando.ToLower().Contains(tMin)) ||
                 (c.Descricao != null && c.Descricao.ToLower().Contains(tMin)),
            cancellationToken);

        resultados.AddRange(comandos.Take(10).Select(c => new ItemPesquisaGlobalDto
        {
            Id = c.Id,
            Titulo = c.Titulo,
            Descricao = c.TextoComando,
            Tipo = "Comando",
            DetalheAdicional = c.FerramentaOuAmbiente
        }));

        cancellationToken.ThrowIfCancellationRequested();

        // Pesquisa em Anotações
        var anotacoes = await _uow.Anotacoes.BuscarAsync(
            a => (a.Titulo != null && a.Titulo.ToLower().Contains(tMin)) ||
                 (a.Conteudo != null && a.Conteudo.ToLower().Contains(tMin)),
            cancellationToken);

        resultados.AddRange(anotacoes.Take(10).Select(a => new ItemPesquisaGlobalDto
        {
            Id = a.Id,
            Titulo = a.Titulo,
            Descricao = a.Conteudo.Length > 80 ? a.Conteudo[..80] + "..." : a.Conteudo,
            Tipo = "Anotação",
            DetalheAdicional = a.Categoria ?? "Geral"
        }));

        cancellationToken.ThrowIfCancellationRequested();

        // Pesquisa em Bugs
        var bugs = await _uow.Bugs.BuscarAsync(
            b => (b.Titulo != null && b.Titulo.ToLower().Contains(tMin)) ||
                 (b.MensagemErroOriginal != null && b.MensagemErroOriginal.ToLower().Contains(tMin)) ||
                 (b.Solucao != null && b.Solucao.ToLower().Contains(tMin)),
            cancellationToken);

        resultados.AddRange(bugs.Take(10).Select(b => new ItemPesquisaGlobalDto
        {
            Id = b.Id,
            Titulo = b.Titulo,
            Descricao = b.MensagemErroOriginal ?? b.Solucao ?? "Sem detalhes",
            Tipo = "Bug",
            DetalheAdicional = b.Status.ToString()
        }));

        cancellationToken.ThrowIfCancellationRequested();

        // Pesquisa em Tags
        var tags = await _uow.Tags.BuscarAsync(
            tag => tag.Nome.ToLower().Contains(tMin),
            cancellationToken);

        resultados.AddRange(tags.Take(5).Select(tag => new ItemPesquisaGlobalDto
        {
            Id = tag.Id,
            Titulo = $"#{tag.Nome}",
            Descricao = "Tag para classificação",
            Tipo = "Tag",
            DetalheAdicional = tag.CorHex
        }));

        return resultados;
    }
}
