using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioTag : IRepositorio<Tag>
{
    Task<Tag?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<bool> ExisteComNomeAsync(string nome, Guid? excluirId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> ObterComContadoresUsoAsync(CancellationToken cancellationToken = default);
}
