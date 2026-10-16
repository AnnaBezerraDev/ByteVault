using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;

namespace ByteVault.Dominio.Interfaces;

public interface IRepositorioConfiguracao
{
    Task<ConfiguracaoAplicacao> ObterAsync(CancellationToken cancellationToken = default);
    Task SalvarAsync(ConfiguracaoAplicacao configuracao, CancellationToken cancellationToken = default);
}
