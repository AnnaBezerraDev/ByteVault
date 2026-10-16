using System.Threading;
using System.Threading.Tasks;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoPortabilidade
{
    Task ExportarJsonCompletoAsync(string caminhoArquivoJson, CancellationToken cancellationToken = default);
    Task ImportarJsonCompletoAsync(string caminhoArquivoJson, CancellationToken cancellationToken = default);
    Task ExportarAnotacoesMarkdownAsync(string pastaDestino, CancellationToken cancellationToken = default);
    Task ExportarSnippetsTxtAsync(string caminhoArquivoTxt, CancellationToken cancellationToken = default);
    Task ExportarRelatorioProjetosCsvAsync(string caminhoArquivoCsv, CancellationToken cancellationToken = default);
}
