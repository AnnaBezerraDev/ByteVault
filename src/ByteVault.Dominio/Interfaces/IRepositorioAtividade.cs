using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioAtividade
{
    Task RegistrarAsync(string titulo, string categoria, string tipoAcao, System.Guid? registroId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroAtividade>> ObterRecentesAsync(int quantidade = 10, CancellationToken cancellationToken = default);
}
