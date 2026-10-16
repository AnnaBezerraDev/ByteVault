using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;

namespace ByteVault.Aplicacao.Servicos;

public class ServicoTag : IServicoTag
{
    private readonly IUnidadeDeTrabalho _uow;

    public ServicoTag(IUnidadeDeTrabalho uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<TagDto>> ObterTodasAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _uow.Tags.ObterComContadoresUsoAsync(cancellationToken);
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Nome = t.Nome,
            CorHex = t.CorHex,
            TotalUsos = t.TagsProjetos.Count + t.TagsSnippets.Count + t.TagsComandos.Count + t.TagsAnotacoes.Count + t.TagsBugs.Count,
            DataCriacao = t.DataCriacao
        }).ToList();
    }

    public async Task<TagDto> CriarOuObterAsync(string nome, string? corHex = null, CancellationToken cancellationToken = default)
    {
        var nomeLimpo = nome.Trim();
        var existente = await _uow.Tags.ObterPorNomeAsync(nomeLimpo, cancellationToken);
        if (existente != null)
        {
            return new TagDto
            {
                Id = existente.Id,
                Nome = existente.Nome,
                CorHex = existente.CorHex,
                DataCriacao = existente.DataCriacao
            };
        }

        var novaTag = new Tag
        {
            Id = Guid.NewGuid(),
            Nome = nomeLimpo,
            CorHex = corHex ?? "#6C8CFF",
            DataCriacao = DateTime.UtcNow
        };

        await _uow.Tags.AdicionarAsync(novaTag, cancellationToken);
        await _uow.SalvarAlteracoesAsync(cancellationToken);

        return new TagDto
        {
            Id = novaTag.Id,
            Nome = novaTag.Nome,
            CorHex = novaTag.CorHex,
            DataCriacao = novaTag.DataCriacao,
            TotalUsos = 0
        };
    }

    public async Task AtualizarAsync(Guid id, string novoNome, string corHex, CancellationToken cancellationToken = default)
    {
        var tag = await _uow.Tags.ObterPorIdAsync(id, cancellationToken);
        if (tag == null) return;

        var nomeLimpo = novoNome.Trim();
        var duplicada = await _uow.Tags.ExisteComNomeAsync(nomeLimpo, id, cancellationToken);
        if (duplicada)
        {
            throw new InvalidOperationException($"Já existe uma tag cadastrada com o nome '{nomeLimpo}'.");
        }

        tag.Nome = nomeLimpo;
        tag.CorHex = corHex;
        _uow.Tags.Atualizar(tag);
        await _uow.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _uow.Tags.ObterPorIdAsync(id, cancellationToken);
        if (tag != null)
        {
            _uow.Tags.Remover(tag);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }
}
