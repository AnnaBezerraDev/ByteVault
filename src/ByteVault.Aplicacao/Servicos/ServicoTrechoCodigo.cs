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

public class ServicoTrechoCodigo : IServicoTrechoCodigo
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoTag _servicoTag;

    public ServicoTrechoCodigo(IUnidadeDeTrabalho uow, IServicoTag servicoTag)
    {
        _uow = uow;
        _servicoTag = servicoTag;
    }

    public async Task<IReadOnlyList<TrechoCodigoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var snippets = await _uow.TrechosCodigo.ObterTodosComTagsEProjetoAsync(cancellationToken);
        return snippets.Select(MapearParaDto).ToList();
    }

    public async Task<IReadOnlyList<TrechoCodigoDto>> FiltrarAsync(string? termo, string? linguagem, string? tag, Guid? projetoId, CancellationToken cancellationToken = default)
    {
        var todos = await _uow.TrechosCodigo.ObterTodosComTagsEProjetoAsync(cancellationToken);
        var consulta = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoMin = termo.Trim().ToLower();
            consulta = consulta.Where(s =>
                (s.Titulo != null && s.Titulo.ToLower().Contains(termoMin)) ||
                (s.Descricao != null && s.Descricao.ToLower().Contains(termoMin)) ||
                (s.Codigo != null && s.Codigo.ToLower().Contains(termoMin)));
        }

        if (!string.IsNullOrWhiteSpace(linguagem))
        {
            var lingMin = linguagem.Trim().ToLower();
            consulta = consulta.Where(s => s.Linguagem.ToLower() == lingMin);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagMin = tag.Trim().ToLower();
            consulta = consulta.Where(s => s.Tags.Any(t => t.Tag.Nome.ToLower() == tagMin));
        }

        if (projetoId.HasValue)
        {
            consulta = consulta.Where(s => s.ProjetoId == projetoId.Value);
        }

        return consulta.Select(MapearParaDto).ToList();
    }

    public async Task<TrechoCodigoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snippet = await _uow.TrechosCodigo.ObterCompletoPorIdAsync(id, cancellationToken);
        return snippet != null ? MapearParaDto(snippet) : null;
    }

    public async Task<TrechoCodigoDto> SalvarAsync(TrechoCodigoDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
        {
            throw new ArgumentException("O título do trecho de código é obrigatório.");
        }

        TrechoCodigo? snippet = null;
        var ehNovo = false;

        if (dto.Id != Guid.Empty)
        {
            snippet = await _uow.TrechosCodigo.ObterCompletoPorIdAsync(dto.Id, cancellationToken);
        }

        if (snippet == null)
        {
            ehNovo = true;
            snippet = new TrechoCodigo
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                DataCriacao = DateTime.UtcNow
            };
            await _uow.TrechosCodigo.AdicionarAsync(snippet, cancellationToken);
        }

        snippet.Titulo = dto.Titulo.Trim();
        snippet.Descricao = dto.Descricao?.Trim();
        snippet.Codigo = dto.Codigo;
        snippet.Linguagem = string.IsNullOrWhiteSpace(dto.Linguagem) ? "Texto simples" : dto.Linguagem.Trim();
        snippet.Categoria = dto.Categoria?.Trim();
        snippet.ProjetoId = dto.ProjetoId;
        snippet.EhFavorito = dto.EhFavorito;
        snippet.DataAtualizacao = DateTime.UtcNow;

        snippet.Tags.Clear();
        foreach (var nomeTag in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(nomeTag))
            {
                var tagDto = await _servicoTag.CriarOuObterAsync(nomeTag, null, cancellationToken);
                snippet.Tags.Add(new TagSnippet
                {
                    TrechoCodigoId = snippet.Id,
                    TagId = tagDto.Id
                });
            }
        }

        await _uow.SalvarAlteracoesAsync(cancellationToken);

        await _uow.Atividades.RegistrarAsync(
            snippet.Titulo,
            "Snippet",
            ehNovo ? "Criado" : "Atualizado",
            snippet.Id,
            cancellationToken);

        return MapearParaDto(snippet);
    }

    public async Task<TrechoCodigoDto> DuplicarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existente = await _uow.TrechosCodigo.ObterCompletoPorIdAsync(id, cancellationToken);
        if (existente == null)
        {
            throw new KeyNotFoundException("Trecho de código não encontrado para duplicação.");
        }

        var clone = new TrechoCodigoDto
        {
            Id = Guid.NewGuid(),
            Titulo = $"{existente.Titulo} (Cópia)",
            Descricao = existente.Descricao,
            Codigo = existente.Codigo,
            Linguagem = existente.Linguagem,
            Categoria = existente.Categoria,
            ProjetoId = existente.ProjetoId,
            EhFavorito = false,
            Tags = existente.Tags.Select(t => t.Tag.Nome).ToList()
        };

        return await SalvarAsync(clone, cancellationToken);
    }

    public async Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snippet = await _uow.TrechosCodigo.ObterPorIdAsync(id, cancellationToken);
        if (snippet != null)
        {
            snippet.EhFavorito = !snippet.EhFavorito;
            snippet.DataAtualizacao = DateTime.UtcNow;
            _uow.TrechosCodigo.Atualizar(snippet);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snippet = await _uow.TrechosCodigo.ObterCompletoPorIdAsync(id, cancellationToken);
        if (snippet != null)
        {
            var titulo = snippet.Titulo;
            _uow.TrechosCodigo.Remover(snippet);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                titulo,
                "Snippet",
                "Excluído",
                id,
                cancellationToken);
        }
    }

    public async Task<IReadOnlyList<string>> ObterLinguagensDisponiveisAsync(CancellationToken cancellationToken = default)
    {
        var daBase = await _uow.TrechosCodigo.ObterLinguagensDistintasAsync(cancellationToken);
        var padroes = new[]
        {
            "C#", "JavaScript", "TypeScript", "HTML", "CSS", "PHP", "Python",
            "SQL", "Java", "C", "C++", "GDScript", "JSON", "XML", "YAML",
            "Bash", "PowerShell", "Markdown", "Texto simples"
        };
        return padroes.Union(daBase).Distinct().OrderBy(x => x).ToList();
    }

    private static TrechoCodigoDto MapearParaDto(TrechoCodigo s)
    {
        return new TrechoCodigoDto
        {
            Id = s.Id,
            Titulo = s.Titulo,
            Descricao = s.Descricao,
            Codigo = s.Codigo,
            Linguagem = s.Linguagem,
            Categoria = s.Categoria,
            ProjetoId = s.ProjetoId,
            NomeProjeto = s.Projeto?.Nome,
            EhFavorito = s.EhFavorito,
            DataCriacao = s.DataCriacao,
            DataAtualizacao = s.DataAtualizacao,
            Tags = s.Tags.Select(t => t.Tag.Nome).ToList()
        };
    }
}
