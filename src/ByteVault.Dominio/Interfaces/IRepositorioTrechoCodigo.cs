using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioTrechoCodigo : IRepositorio<TrechoCodigo>
{
    Task<TrechoCodigo?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrechoCodigo>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrechoCodigo>> ObterFavoritosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrechoCodigo>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ObterLinguagensDistintasAsync(CancellationToken cancellationToken = default);
}
