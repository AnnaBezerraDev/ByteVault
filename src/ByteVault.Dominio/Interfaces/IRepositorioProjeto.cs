using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioProjeto : IRepositorio<Projeto>
{
    Task<Projeto?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Projeto>> ObterTodosComTagsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Projeto>> ObterFavoritosAsync(CancellationToken cancellationToken = default);
}
