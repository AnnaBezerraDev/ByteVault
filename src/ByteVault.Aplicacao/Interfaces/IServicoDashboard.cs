using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoDashboard
{
    Task<IndicadoresDashboardDto> ObterIndicadoresAsync(CancellationToken cancellationToken = default);
}
