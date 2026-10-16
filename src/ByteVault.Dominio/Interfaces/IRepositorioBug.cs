using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioBug : IRepositorio<RelatorioBug>
{
    Task<RelatorioBug?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RelatorioBug>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RelatorioBug>> ObterFavoritosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RelatorioBug>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default);
    Task<int> ObterTotalNaoResolvidosAsync(CancellationToken cancellationToken = default);
}
