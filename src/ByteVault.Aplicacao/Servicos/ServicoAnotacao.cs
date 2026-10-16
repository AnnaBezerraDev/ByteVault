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

public class ServicoAnotacao : IServicoAnotacao
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoTag _servicoTag;

    public ServicoAnotacao(IUnidadeDeTrabalho uow, IServicoTag servicoTag)
    {
        _uow = uow;
        _servicoTag = servicoTag;
    }

    public async Task<IReadOnlyList<AnotacaoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var anotacoes = await _uow.Anotacoes.ObterTodosComTagsEProjetoAsync(cancellationToken);
        return anotacoes.Select(MapearParaDto).ToList();
    }

    public async Task<IReadOnlyList<AnotacaoDto>> FiltrarAsync(string? termo, string? categoria, string? tag, Guid? projetoId, CancellationToken cancellationToken = default)
    {
        var todos = await _uow.Anotacoes.ObterTodosComTagsEProjetoAsync(cancellationToken);
        var consulta = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoMin = termo.Trim().ToLower();
            consulta = consulta.Where(a =>
                (a.Titulo != null && a.Titulo.ToLower().Contains(termoMin)) ||
                (a.Conteudo != null && a.Conteudo.ToLower().Contains(termoMin)) ||
                (a.Categoria != null && a.Categoria.ToLower().Contains(termoMin)));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            var catMin = categoria.Trim().ToLower();
            consulta = consulta.Where(a => a.Categoria != null && a.Categoria.ToLower() == catMin);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagMin = tag.Trim().ToLower();
            consulta = consulta.Where(a => a.Tags.Any(t => t.Tag.Nome.ToLower() == tagMin));
        }

        if (projetoId.HasValue)
        {
            consulta = consulta.Where(a => a.ProjetoId == projetoId.Value);
        }

        return consulta.Select(MapearParaDto).ToList();
    }

    public async Task<AnotacaoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var anotacao = await _uow.Anotacoes.ObterCompletoPorIdAsync(id, cancellationToken);
        return anotacao != null ? MapearParaDto(anotacao) : null;
    }

    public async Task<AnotacaoDto> SalvarAsync(AnotacaoDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
        {
            throw new ArgumentException("O título da anotação é obrigatório.");
        }

        Anotacao? anotacao = null;
        var ehNovo = false;

        if (dto.Id != Guid.Empty)
        {
            anotacao = await _uow.Anotacoes.ObterCompletoPorIdAsync(dto.Id, cancellationToken);
        }

        if (anotacao == null)
        {
            ehNovo = true;
            anotacao = new Anotacao
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                DataCriacao = DateTime.UtcNow
            };
            await _uow.Anotacoes.AdicionarAsync(anotacao, cancellationToken);
        }

        anotacao.Titulo = dto.Titulo.Trim();
        anotacao.Conteudo = dto.Conteudo;
        anotacao.Categoria = dto.Categoria?.Trim();
        anotacao.ProjetoId = dto.ProjetoId;
        anotacao.EhFavorito = dto.EhFavorito;
        anotacao.DataAtualizacao = DateTime.UtcNow;

        anotacao.Tags.Clear();
        foreach (var nomeTag in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(nomeTag))
            {
                var tagDto = await _servicoTag.CriarOuObterAsync(nomeTag, null, cancellationToken);
                anotacao.Tags.Add(new TagAnotacao
                {
                    AnotacaoId = anotacao.Id,
                    TagId = tagDto.Id
                });
            }
        }

        await _uow.SalvarAlteracoesAsync(cancellationToken);

        await _uow.Atividades.RegistrarAsync(
            anotacao.Titulo,
            "Anotação",
            ehNovo ? "Criada" : "Atualizada",
            anotacao.Id,
            cancellationToken);

        return MapearParaDto(anotacao);
    }

    public async Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var anotacao = await _uow.Anotacoes.ObterPorIdAsync(id, cancellationToken);
        if (anotacao != null)
        {
            anotacao.EhFavorito = !anotacao.EhFavorito;
            anotacao.DataAtualizacao = DateTime.UtcNow;
            _uow.Anotacoes.Atualizar(anotacao);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var anotacao = await _uow.Anotacoes.ObterCompletoPorIdAsync(id, cancellationToken);
        if (anotacao != null)
        {
            var titulo = anotacao.Titulo;
            _uow.Anotacoes.Remover(anotacao);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                titulo,
                "Anotação",
                "Excluída",
                id,
                cancellationToken);
        }
    }

    private static AnotacaoDto MapearParaDto(Anotacao a)
    {
        return new AnotacaoDto
        {
            Id = a.Id,
            Titulo = a.Titulo,
            Conteudo = a.Conteudo,
            Categoria = a.Categoria,
            ProjetoId = a.ProjetoId,
            NomeProjeto = a.Projeto?.Nome,
            EhFavorito = a.EhFavorito,
            DataCriacao = a.DataCriacao,
            DataAtualizacao = a.DataAtualizacao,
            Tags = a.Tags.Select(t => t.Tag.Nome).ToList()
        };
    }
}
