using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioComando : IRepositorio<RegistroComando>
{
    Task<RegistroComando?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroComando>> ObterTodosComTagsEProjetoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroComando>> ObterFavoritosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroComando>> ObterPorProjetoIdAsync(Guid projetoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ObterCategoriasDistintasAsync(CancellationToken cancellationToken = default);
}
