using System.Threading;
using System.Threading.Tasks;

namespace ByteVault.Dominio.Interfaces;

public interface IServicoBackupRestauracao
{
    Task CriarBackupAsync(string caminhoArquivoZip, CancellationToken cancellationToken = default);
    Task RestaurarBackupAsync(string caminhoArquivoZip, CancellationToken cancellationToken = default);
}
