using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioAnotacao : RepositorioBase<Anotacao>, IRepositorioAnotacao
{
    public RepositorioAnotacao(ByteVaultContextoDados contexto) : base(contexto)
    {
    }

    public async Task<Anotacao?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Contexto.Anotacoes
            .Include(a => a.Tags).ThenInclude(ta => ta.Tag)
            .Include(a => a.Projeto)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Anotacao>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Anotacoes
            .AsNoTracking()
            .Include(a => a.Tags).ThenInclude(ta => ta.Tag)
            .Include(a => a.Projeto)
            .OrderByDescending(a => a.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Anotacao>> ObterFavoritosAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Anotacoes
            .AsNoTracking()
            .Where(a => a.EhFavorito)
            .Include(a => a.Tags).ThenInclude(ta => ta.Tag)
            .Include(a => a.Projeto)
            .OrderByDescending(a => a.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Anotacao>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default)
    {
        return await Contexto.Anotacoes
            .AsNoTracking()
            .Where(a => a.ProjetoId == projetoId)
            .Include(a => a.Tags).ThenInclude(ta => ta.Tag)
            .OrderByDescending(a => a.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }
}
