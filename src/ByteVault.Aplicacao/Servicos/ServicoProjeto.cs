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

public class ServicoProjeto : IServicoProjeto
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoTag _servicoTag;

    public ServicoProjeto(IUnidadeDeTrabalho uow, IServicoTag servicoTag)
    {
        _uow = uow;
        _servicoTag = servicoTag;
    }

    public async Task<IReadOnlyList<ProjetoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var projetos = await _uow.Projetos.ObterTodosComTagsAsync(cancellationToken);
        return projetos.Select(MapearParaDto).ToList();
    }

    public async Task<IReadOnlyList<ProjetoDto>> FiltrarAsync(string? termo, StatusProjeto? status, string? tecnologia, string? tag, CancellationToken cancellationToken = default)
    {
        var todos = await _uow.Projetos.ObterTodosComTagsAsync(cancellationToken);
        var consulta = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoMin = termo.Trim().ToLower();
            consulta = consulta.Where(p =>
                (p.Nome != null && p.Nome.ToLower().Contains(termoMin)) ||
                (p.Descricao != null && p.Descricao.ToLower().Contains(termoMin)) ||
                (p.LinguagensTecnologias != null && p.LinguagensTecnologias.ToLower().Contains(termoMin)));
        }

        if (status.HasValue)
        {
            consulta = consulta.Where(p => p.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(tecnologia))
        {
            var tecMin = tecnologia.Trim().ToLower();
            consulta = consulta.Where(p =>
                p.Tecnologias.Any(t => t.Nome.ToLower().Contains(tecMin)) ||
                (p.LinguagensTecnologias != null && p.LinguagensTecnologias.ToLower().Contains(tecMin)));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagMin = tag.Trim().ToLower();
            consulta = consulta.Where(p => p.Tags.Any(t => t.Tag.Nome.ToLower() == tagMin));
        }

        return consulta.Select(MapearParaDto).ToList();
    }

    public async Task<ProjetoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var projeto = await _uow.Projetos.ObterCompletoPorIdAsync(id, cancellationToken);
        return projeto != null ? MapearParaDto(projeto) : null;
    }

    public async Task<ProjetoDto> SalvarAsync(ProjetoDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ArgumentException("O nome do projeto é obrigatório.");
        }

        if (!string.IsNullOrWhiteSpace(dto.UrlRepositorio))
        {
            if (!Uri.TryCreate(dto.UrlRepositorio.Trim(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("A URL do repositório deve ser válida e iniciar com http:// ou https://.");
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.UrlDocumentacao))
        {
            if (!Uri.TryCreate(dto.UrlDocumentacao.Trim(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("A URL da documentação deve ser válida e iniciar com http:// ou https://.");
            }
        }

        Projeto? projeto = null;
        var ehNovo = false;

        if (dto.Id != Guid.Empty)
        {
            projeto = await _uow.Projetos.ObterCompletoPorIdAsync(dto.Id, cancellationToken);
        }

        if (projeto == null)
        {
            ehNovo = true;
            projeto = new Projeto
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                DataCriacao = DateTime.UtcNow
            };
            await _uow.Projetos.AdicionarAsync(projeto, cancellationToken);
        }

        projeto.Nome = dto.Nome.Trim();
        projeto.Descricao = dto.Descricao?.Trim();
        projeto.Status = dto.Status;
        projeto.LinguagensTecnologias = dto.LinguagensTecnologias?.Trim();
        projeto.CaminhoLocal = dto.CaminhoLocal?.Trim();
        projeto.UrlRepositorio = dto.UrlRepositorio?.Trim();
        projeto.UrlDocumentacao = dto.UrlDocumentacao?.Trim();
        projeto.Observacoes = dto.Observacoes?.Trim();
        projeto.EhFavorito = dto.EhFavorito;
        projeto.DataAtualizacao = DateTime.UtcNow;

        // Atualizar Tags
        projeto.Tags.Clear();
        foreach (var nomeTag in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(nomeTag))
            {
                var tagDto = await _servicoTag.CriarOuObterAsync(nomeTag, null, cancellationToken);
                projeto.Tags.Add(new TagProjeto
                {
                    ProjetoId = projeto.Id,
                    TagId = tagDto.Id
                });
            }
        }

        // Atualizar Tecnologias a partir do texto
        projeto.Tecnologias.Clear();
        if (!string.IsNullOrWhiteSpace(projeto.LinguagensTecnologias))
        {
            var tecs = projeto.LinguagensTecnologias
                .Split(new[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var tec in tecs.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                projeto.Tecnologias.Add(new TecnologiaProjeto
                {
                    ProjetoId = projeto.Id,
                    Nome = tec
                });
            }
        }

        await _uow.SalvarAlteracoesAsync(cancellationToken);

        await _uow.Atividades.RegistrarAsync(
            projeto.Nome,
            "Projeto",
            ehNovo ? "Criado" : "Atualizado",
            projeto.Id,
            cancellationToken);

        return MapearParaDto(projeto);
    }

    public async Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var projeto = await _uow.Projetos.ObterPorIdAsync(id, cancellationToken);
        if (projeto != null)
        {
            projeto.EhFavorito = !projeto.EhFavorito;
            projeto.DataAtualizacao = DateTime.UtcNow;
            _uow.Projetos.Atualizar(projeto);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    public async Task AlterarStatusAsync(Guid id, StatusProjeto novoStatus, CancellationToken cancellationToken = default)
    {
        var projeto = await _uow.Projetos.ObterPorIdAsync(id, cancellationToken);
        if (projeto != null)
        {
            projeto.Status = novoStatus;
            projeto.DataAtualizacao = DateTime.UtcNow;
            _uow.Projetos.Atualizar(projeto);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                $"{projeto.Nome} -> {novoStatus}",
                "Projeto",
                "Status Alterado",
                projeto.Id,
                cancellationToken);
        }
    }

    public async Task ArquivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await AlterarStatusAsync(id, StatusProjeto.Arquivado, cancellationToken);
    }

    public async Task RestaurarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await AlterarStatusAsync(id, StatusProjeto.EmDesenvolvimento, cancellationToken);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var projeto = await _uow.Projetos.ObterCompletoPorIdAsync(id, cancellationToken);
        if (projeto != null)
        {
            var nome = projeto.Nome;
            _uow.Projetos.Remover(projeto);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                nome,
                "Projeto",
                "Excluído",
                id,
                cancellationToken);
        }
    }

    public async Task RegistrarAberturaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var projeto = await _uow.Projetos.ObterPorIdAsync(id, cancellationToken);
        if (projeto != null)
        {
            projeto.DataUltimaAbertura = DateTime.UtcNow;
            _uow.Projetos.Atualizar(projeto);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    private static ProjetoDto MapearParaDto(Projeto p)
    {
        return new ProjetoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Status = p.Status,
            LinguagensTecnologias = p.LinguagensTecnologias,
            CaminhoLocal = p.CaminhoLocal,
            UrlRepositorio = p.UrlRepositorio,
            UrlDocumentacao = p.UrlDocumentacao,
            Observacoes = p.Observacoes,
            EhFavorito = p.EhFavorito,
            DataCriacao = p.DataCriacao,
            DataAtualizacao = p.DataAtualizacao,
            DataUltimaAbertura = p.DataUltimaAbertura,
            Tags = p.Tags.Select(t => t.Tag.Nome).ToList(),
            TotalSnippets = p.TrechosCodigo?.Count ?? 0,
            TotalComandos = p.Comandos?.Count ?? 0,
            TotalAnotacoes = p.Anotacoes?.Count ?? 0,
            TotalBugs = p.Bugs?.Count ?? 0
        };
    }
}
