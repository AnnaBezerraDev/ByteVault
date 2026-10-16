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

public class ServicoComando : IServicoComando
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoTag _servicoTag;

    public ServicoComando(IUnidadeDeTrabalho uow, IServicoTag servicoTag)
    {
        _uow = uow;
        _servicoTag = servicoTag;
    }

    public async Task<IReadOnlyList<RegistroComandoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var comandos = await _uow.Comandos.ObterTodosComTagsEProjetoAsync(cancellationToken);
        return comandos.Select(MapearParaDto).ToList();
    }

    public async Task<IReadOnlyList<RegistroComandoDto>> FiltrarAsync(string? termo, string? ferramenta, string? so, string? tag, Guid? projetoId, CancellationToken cancellationToken = default)
    {
        var todos = await _uow.Comandos.ObterTodosComTagsEProjetoAsync(cancellationToken);
        var consulta = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoMin = termo.Trim().ToLower();
            consulta = consulta.Where(c =>
                (c.Titulo != null && c.Titulo.ToLower().Contains(termoMin)) ||
                (c.Descricao != null && c.Descricao.ToLower().Contains(termoMin)) ||
                (c.TextoComando != null && c.TextoComando.ToLower().Contains(termoMin)) ||
                (c.Parametros != null && c.Parametros.ToLower().Contains(termoMin)));
        }

        if (!string.IsNullOrWhiteSpace(ferramenta))
        {
            var ferMin = ferramenta.Trim().ToLower();
            consulta = consulta.Where(c => c.FerramentaOuAmbiente.ToLower() == ferMin);
        }

        if (!string.IsNullOrWhiteSpace(so))
        {
            var soMin = so.Trim().ToLower();
            consulta = consulta.Where(c => c.SistemaOperacional != null && c.SistemaOperacional.ToLower() == soMin);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagMin = tag.Trim().ToLower();
            consulta = consulta.Where(c => c.Tags.Any(t => t.Tag.Nome.ToLower() == tagMin));
        }

        if (projetoId.HasValue)
        {
            consulta = consulta.Where(c => c.ProjetoId == projetoId.Value);
        }

        return consulta.Select(MapearParaDto).ToList();
    }

    public async Task<RegistroComandoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comando = await _uow.Comandos.ObterCompletoPorIdAsync(id, cancellationToken);
        return comando != null ? MapearParaDto(comando) : null;
    }

    public async Task<RegistroComandoDto> SalvarAsync(RegistroComandoDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
        {
            throw new ArgumentException("O título do comando é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(dto.TextoComando))
        {
            throw new ArgumentException("O texto do comando é obrigatório.");
        }

        RegistroComando? comando = null;
        var ehNovo = false;

        if (dto.Id != Guid.Empty)
        {
            comando = await _uow.Comandos.ObterCompletoPorIdAsync(dto.Id, cancellationToken);
        }

        if (comando == null)
        {
            ehNovo = true;
            comando = new RegistroComando
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                DataCriacao = DateTime.UtcNow
            };
            await _uow.Comandos.AdicionarAsync(comando, cancellationToken);
        }

        comando.Titulo = dto.Titulo.Trim();
        comando.Descricao = dto.Descricao?.Trim();
        comando.TextoComando = dto.TextoComando.Trim();
        comando.FerramentaOuAmbiente = string.IsNullOrWhiteSpace(dto.FerramentaOuAmbiente) ? "Git" : dto.FerramentaOuAmbiente.Trim();
        comando.SistemaOperacional = string.IsNullOrWhiteSpace(dto.SistemaOperacional) ? "Multiplataforma" : dto.SistemaOperacional.Trim();
        comando.Parametros = dto.Parametros?.Trim();
        comando.ExemploUtilizacao = dto.ExemploUtilizacao?.Trim();
        comando.ProjetoId = dto.ProjetoId;
        comando.EhFavorito = dto.EhFavorito;
        comando.DataAtualizacao = DateTime.UtcNow;

        comando.Tags.Clear();
        foreach (var nomeTag in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(nomeTag))
            {
                var tagDto = await _servicoTag.CriarOuObterAsync(nomeTag, null, cancellationToken);
                comando.Tags.Add(new TagComando
                {
                    RegistroComandoId = comando.Id,
                    TagId = tagDto.Id
                });
            }
        }

        await _uow.SalvarAlteracoesAsync(cancellationToken);

        await _uow.Atividades.RegistrarAsync(
            comando.Titulo,
            "Comando",
            ehNovo ? "Criado" : "Atualizado",
            comando.Id,
            cancellationToken);

        return MapearParaDto(comando);
    }

    public async Task<RegistroComandoDto> DuplicarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existente = await _uow.Comandos.ObterCompletoPorIdAsync(id, cancellationToken);
        if (existente == null)
        {
            throw new KeyNotFoundException("Comando não encontrado para duplicação.");
        }

        var clone = new RegistroComandoDto
        {
            Id = Guid.NewGuid(),
            Titulo = $"{existente.Titulo} (Cópia)",
            Descricao = existente.Descricao,
            TextoComando = existente.TextoComando,
            FerramentaOuAmbiente = existente.FerramentaOuAmbiente,
            SistemaOperacional = existente.SistemaOperacional,
            Parametros = existente.Parametros,
            ExemploUtilizacao = existente.ExemploUtilizacao,
            ProjetoId = existente.ProjetoId,
            EhFavorito = false,
            Tags = existente.Tags.Select(t => t.Tag.Nome).ToList()
        };

        return await SalvarAsync(clone, cancellationToken);
    }

    public async Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comando = await _uow.Comandos.ObterPorIdAsync(id, cancellationToken);
        if (comando != null)
        {
            comando.EhFavorito = !comando.EhFavorito;
            comando.DataAtualizacao = DateTime.UtcNow;
            _uow.Comandos.Atualizar(comando);
            await _uow.SalvarAlteracoesAsync(cancellationToken);
        }
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comando = await _uow.Comandos.ObterCompletoPorIdAsync(id, cancellationToken);
        if (comando != null)
        {
            var titulo = comando.Titulo;
            _uow.Comandos.Remover(comando);
            await _uow.SalvarAlteracoesAsync(cancellationToken);

            await _uow.Atividades.RegistrarAsync(
                titulo,
                "Comando",
                "Excluído",
                id,
                cancellationToken);
        }
    }

    public async Task<IReadOnlyList<string>> ObterCategoriasDisponiveisAsync(CancellationToken cancellationToken = default)
    {
        var daBase = await _uow.Comandos.ObterCategoriasDistintasAsync(cancellationToken);
        var padroes = new[]
        {
            "Git", "PowerShell", "CMD", "Linux", "npm", "Node.js", ".NET CLI",
            "Docker", "SQL", "Android", "Unity", "Godot", "Arduino", "Redes", "Outros"
        };
        return padroes.Union(daBase).Distinct().OrderBy(x => x).ToList();
    }

    private static RegistroComandoDto MapearParaDto(RegistroComando c)
    {
        return new RegistroComandoDto
        {
            Id = c.Id,
            Titulo = c.Titulo,
            Descricao = c.Descricao,
            TextoComando = c.TextoComando,
            FerramentaOuAmbiente = c.FerramentaOuAmbiente,
            SistemaOperacional = c.SistemaOperacional,
            Parametros = c.Parametros,
            ExemploUtilizacao = c.ExemploUtilizacao,
            ProjetoId = c.ProjetoId,
            NomeProjeto = c.Projeto?.Nome,
            EhFavorito = c.EhFavorito,
            DataCriacao = c.DataCriacao,
            DataAtualizacao = c.DataAtualizacao,
            Tags = c.Tags.Select(t => t.Tag.Nome).ToList()
        };
    }
}
