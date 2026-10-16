using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioComando : RepositorioBase<RegistroComando>, IRepositorioComando
{
    public RepositorioComando(ByteVaultContextoDados contexto) : base(contexto)
    {
    }

    public async Task<RegistroComando?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Contexto.Comandos
            .Include(c => c.Tags).ThenInclude(tc => tc.Tag)
            .Include(c => c.Projeto)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RegistroComando>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Comandos
            .AsNoTracking()
            .Include(c => c.Tags).ThenInclude(tc => tc.Tag)
            .Include(c => c.Projeto)
            .OrderByDescending(c => c.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistroComando>> ObterFavoritosAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Comandos
            .AsNoTracking()
            .Where(c => c.EhFavorito)
            .Include(c => c.Tags).ThenInclude(tc => tc.Tag)
            .Include(c => c.Projeto)
            .OrderByDescending(c => c.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistroComando>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default)
    {
        return await Contexto.Comandos
            .AsNoTracking()
            .Where(c => c.ProjetoId == projetoId)
            .Include(c => c.Tags).ThenInclude(tc => tc.Tag)
            .OrderByDescending(c => c.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ObterCategoriasDistintasAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Comandos
            .AsNoTracking()
            .Select(c => c.FerramentaOuAmbiente)
            .Distinct()
            .OrderBy(cat => cat)
            .ToListAsync(cancellationToken);
    }
}
