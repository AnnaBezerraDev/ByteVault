using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioBase<T> : IRepositorio<T> where T : class
{
    protected readonly ByteVaultContextoDados Contexto;
    protected readonly DbSet<T> Entidades;

    public RepositorioBase(ByteVaultContextoDados contexto)
    {
        Contexto = contexto;
        Entidades = contexto.Set<T>();
    }

    public virtual async Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Entidades.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await Entidades.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken cancellationToken = default)
    {
        return await Entidades.AsNoTracking().Where(predicado).ToListAsync(cancellationToken);
    }

    public virtual async Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        await Entidades.AddAsync(entidade, cancellationToken);
    }

    public virtual void Atualizar(T entidade)
    {
        Entidades.Update(entidade);
    }

    public virtual void Remover(T entidade)
    {
        Entidades.Remove(entidade);
    }
}
