using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioAnotacao : IRepositorio<Anotacao>
{
    Task<Anotacao?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Anotacao>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Anotacao>> ObterFavoritosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Anotacao>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default);
}
