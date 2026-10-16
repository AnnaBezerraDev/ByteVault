using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Enumeradores;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioBug : RepositorioBase<RelatorioBug>, IRepositorioBug
{
    public RepositorioBug(ByteVaultContextoDados contexto) : base(contexto)
    {
    }

    public async Task<RelatorioBug?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Contexto.Bugs
            .Include(b => b.Tags).ThenInclude(tb => tb.Tag)
            .Include(b => b.Projeto)
            .Include(b => b.SnippetsSolucao).ThenInclude(r => r.TrechoCodigo)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RelatorioBug>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Bugs
            .AsNoTracking()
            .Include(b => b.Tags).ThenInclude(tb => tb.Tag)
            .Include(b => b.Projeto)
            .OrderByDescending(b => b.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RelatorioBug>> ObterFavoritosAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Bugs
            .AsNoTracking()
            .Where(b => b.EhFavorito)
            .Include(b => b.Tags).ThenInclude(tb => tb.Tag)
            .Include(b => b.Projeto)
            .OrderByDescending(b => b.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RelatorioBug>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default)
    {
        return await Contexto.Bugs
            .AsNoTracking()
            .Where(b => b.ProjetoId == projetoId)
            .Include(b => b.Tags).ThenInclude(tb => tb.Tag)
            .OrderByDescending(b => b.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ObterTotalNaoResolvidosAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Bugs
            .AsNoTracking()
            .CountAsync(b => b.Status != StatusBug.Resolvido, cancellationToken);
    }
}
