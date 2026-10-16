using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorio<T> where T : class
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken cancellationToken = default);
    Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default);
    void Atualizar(T entidade);
    void Remover(T entidade);
}
