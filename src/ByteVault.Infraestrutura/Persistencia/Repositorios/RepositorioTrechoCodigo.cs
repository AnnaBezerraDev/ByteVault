using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioTrechoCodigo : RepositorioBase<TrechoCodigo>, IRepositorioTrechoCodigo
{
    public RepositorioTrechoCodigo(ByteVaultContextoDados contexto) : base(contexto)
    {
    }

    public async Task<TrechoCodigo?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Contexto.TrechosCodigo
            .Include(t => t.Tags).ThenInclude(ts => ts.Tag)
            .Include(t => t.Projeto)
            .Include(t => t.BugsRelacionados).ThenInclude(r => r.Bug)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TrechoCodigo>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.TrechosCodigo
            .AsNoTracking()
            .Include(t => t.Tags).ThenInclude(ts => ts.Tag)
            .Include(t => t.Projeto)
            .OrderByDescending(t => t.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrechoCodigo>> ObterFavoritosAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.TrechosCodigo
            .AsNoTracking()
            .Where(t => t.EhFavorito)
            .Include(t => t.Tags).ThenInclude(ts => ts.Tag)
            .Include(t => t.Projeto)
            .OrderByDescending(t => t.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TrechoCodigo>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default)
    {
        return await Contexto.TrechosCodigo
            .AsNoTracking()
            .Where(t => t.ProjetoId == projetoId)
            .Include(t => t.Tags).ThenInclude(ts => ts.Tag)
            .OrderByDescending(t => t.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ObterLinguagensDistintasAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.TrechosCodigo
            .AsNoTracking()
            .Select(t => t.Linguagem)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync(cancellationToken);
    }
}
