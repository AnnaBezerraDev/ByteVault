using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Enumeradores;
using ByteVault.Dominio.Interfaces;

namespace ByteVault.Aplicacao.Servicos;

public class ServicoBug : IServicoBug
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoTag _servicoTag;

    public ServicoBug(IUnidadeDeTrabalho uow, IServicoTag servicoTag)
    {
        _uow = uow;
        _servicoTag = servicoTag;
    }

    public async Task<IReadOnlyList<RelatorioBugDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var bugs = await _uow.Bugs.ObterTodosComTagsEProjetoAsync(cancellationToken);
        return bugs.Select(MapearParaDto).ToList();
    }

    public async Task<IReadOnlyList<RelatorioBugDto>> FiltrarAsync(string? termo, StatusBug? status, SeveridadeBug? severidade, string? tag, Guid? projetoId, CancellationToken cancellationToken = default)
    {
        var todos = await _uow.Bugs.ObterTodosComTagsEProjetoAsync(cancellationToken);
        var consulta = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoMin = termo.Trim().ToLower();
            consulta = consulta.Where(b =>
                (b.Titulo != null && b.Titulo.ToLower().Contains(termoMin)) ||
                (b.Descricao != null && b.Descricao.ToLower().Contains(termoMin)) ||
                (b.MensagemErroOriginal != null && b.MensagemErroOriginal.ToLower().Contains(termoMin)) ||
                (b.Solucao != null && b.Solucao.ToLower().Contains(termoMin)) ||
                (b.TecnologiaRelacionada != null && b.TecnologiaRelacionada.ToLower().Contains(termoMin)));
        }

        if (status.HasValue)
        {
            consulta = consulta.Where(b => b.Status == status.Value);
        }

        if (severidade.HasValue)
        {
            consulta = consulta.Where(b => b.Severidade == severidade.Value);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagMin = tag.Trim().ToLower();
            consulta = consulta.Where(b => b.Tags.Any(t => t.Tag.Nome.ToLower() == tagMin));
        }

        if (projetoId.HasValue)
        {
            consulta = consulta.Where(b => b.ProjetoId == projetoId.Value);
        }

        return consulta.Select(MapearParaDto).ToList();
    }

    public async Task<RelatorioBugDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bug = await _uow.Bugs.ObterCompletoPorIdAsync(id, cancellationToken);
        return bug != null ? MapearParaDto(bug) : null;
    }

    public async Task<RelatorioBugDto> SalvarAsync(RelatorioBugDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
        {
            throw new ArgumentException("O título do bug é obrigatório.");
        }

        RelatorioBug? bug = null;
        var ehNovo = false;

        if (dto.Id != Guid.Empty)
        {
            bug = await _uow.Bugs.ObterCompletoPorIdAsync(dto.Id, cancellationToken);
        }

        if (bug == null)
        {
            ehNovo = true;
            bug = new RelatorioBug
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                DataCriacao = DateTime.UtcNow
            };
            await _uow.Bugs.AdicionarAsync(bug, cancellationToken);
        }

        bug.Titulo = dto.Titulo.Trim();
        bug.Descricao = dto.Descricao?.Trim();
        bug.MensagemErroOriginal = dto.MensagemErroOriginal?.Trim();
        bug.TecnologiaRelacionada = dto.TecnologiaRelacionada?.Trim();
        bug.Ambiente = dto.Ambiente?.Trim();
        bug.CodigoRelevante = dto.CodigoRelevante?.Trim();
        bug.Solucao = dto.Solucao?.Trim();
        bug.Status = dto.Status;
        bug.Severidade = dto.Severidade;
        bug.ProjetoId = dto.ProjetoId;
        bug.EhFavorito = dto.EhFavorito;
        bug.DataAtualizacao = DateTime.UtcNow;

        if (dto.Status == StatusBug.Resolvido && !bug.DataResolucao.HasValue)
        {
            bug.DataResolucao = DateTime.UtcNow;
        }
        else if (dto.Status != StatusBug.Resolvido)
        {
            bug.DataResolucao = null;
        }

        bug.Tags.Clear();
        foreach (var nomeTag in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(nomeTag))
            {
                var tagDto = await _servicoTag.CriarOuObterAsync(nomeTag, null, cancellationToken);
                bug.Tags.Add(new TagBug
                {
                    RelatorioBugId = bug.Id,
                    TagId = tagDto.Id
                });
            }
        }

        // Vínculo com Snippets de Solução
        bug.SnippetsSolucao.Clear();
        foreach (var snippetId in dto.SnippetSolucaoIds.Distinct())
        {
            bug.SnippetsSolucao.Add(new RelacaoBugSnippet
            {
                BugId = bug.Id,
                TrechoCodigoId = snippetId
            });
        }

        await _uow.SalvarAlteracoesAsync(cancellationToken);

        await _uow.Atividades.RegistrarAsync(
            bug.Titulo,
            "Bug",
            ehNovo ? "Registrado" : "Atualizado",
            bug.Id,
            cancellationToken);

        return MapearParaDto(bug);
    }

    public async Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bug = await _uow.Bugs.ObterPorIdAsync(id, cancellationToken);
        if (bug != null)
        {
            bug.EhFavorito = !bug.EhFavorito;
            bug.DataAtualizacao = DateTime.UtcNow;
            _uow.Bugs.Atualizar(bug);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    public async Task AlterarStatusAsync(Guid id, StatusBug novoStatus, CancellationToken cancellationToken = default)
    {
        var bug = await _uow.Bugs.ObterPorIdAsync(id, cancellationToken);
        if (bug != null)
        {
            bug.Status = novoStatus;
            bug.DataAtualizacao = DateTime.UtcNow;
            if (novoStatus == StatusBug.Resolvido)
            {
                bug.DataResolucao = DateTime.UtcNow;
            }
            else
            {
                bug.DataResolucao = null;
            }
            _uow.Bugs.Atualizar(bug);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                $"{bug.Titulo} -> {novoStatus}",
                "Bug",
                "Status Alterado",
                bug.Id,
                cancellationToken);
        }
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bug = await _uow.Bugs.ObterCompletoPorIdAsync(id, cancellationToken);
        if (bug != null)
        {
            var titulo = bug.Titulo;
            _uow.Bugs.Remover(bug);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                titulo,
                "Bug",
                "Excluído",
                id,
                cancellationToken);
        }
    }

    public async Task VincularSnippetAsync(Guid bugId, Guid snippetId, CancellationToken cancellationToken = default)
    {
        var bug = await _uow.Bugs.ObterCompletoPorIdAsync(bugId, cancellationToken);
        if (bug != null && !bug.SnippetsSolucao.Any(s => s.TrechoCodigoId == snippetId))
        {
            bug.SnippetsSolucao.Add(new RelacaoBugSnippet
            {
                BugId = bugId,
                TrechoCodigoId = snippetId
            });
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    public async Task DesvincularSnippetAsync(Guid bugId, Guid snippetId, CancellationToken cancellationToken = default)
    {
        var bug = await _uow.Bugs.ObterCompletoPorIdAsync(bugId, cancellationToken);
        if (bug != null)
        {
            var vinculo = bug.SnippetsSolucao.FirstOrDefault(s => s.TrechoCodigoId == snippetId);
            if (vinculo != null)
            {
                bug.SnippetsSolucao.Remove(vinculo);
                await _uow.SalvarAlteracoesAsync(cancellationToken);
            }
        }
    }

    private static RelatorioBugDto MapearParaDto(RelatorioBug b)
    {
        return new RelatorioBugDto
        {
            Id = b.Id,
            Titulo = b.Titulo,
            Descricao = b.Descricao,
            MensagemErroOriginal = b.MensagemErroOriginal,
            TecnologiaRelacionada = b.TecnologiaRelacionada,
            Ambiente = b.Ambiente,
            CodigoRelevante = b.CodigoRelevante,
            Solucao = b.Solucao,
            Status = b.Status,
            Severidade = b.Severidade,
            ProjetoId = b.ProjetoId,
            NomeProjeto = b.Projeto?.Nome,
            EhFavorito = b.EhFavorito,
            DataCriacao = b.DataCriacao,
            DataAtualizacao = b.DataAtualizacao,
            DataResolucao = b.DataResolucao,
            Tags = b.Tags.Select(t => t.Tag.Nome).ToList(),
            SnippetSolucaoIds = b.SnippetsSolucao.Select(s => s.TrechoCodigoId).ToList(),
            TitulosSnippetsSolucao = b.SnippetsSolucao.Select(s => s.TrechoCodigo?.Titulo ?? "Snippet").ToList()
        };
    }
}
