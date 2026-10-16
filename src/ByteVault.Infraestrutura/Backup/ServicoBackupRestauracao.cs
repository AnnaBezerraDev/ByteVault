using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Interfaces;
using ByteVault.Infraestrutura.Persistencia;
using ByteVault.Infraestrutura.Servicos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Backup;

public class ServicoBackupRestauracao : IServicoBackupRestauracao
{
    private readonly ByteVaultContextoDados _contexto;

    public ServicoBackupRestauracao(ByteVaultContextoDados contexto)
    {
        _contexto = contexto;
    }

    public async Task CriarBackupAsync(string caminhoArquivoZip, CancellationToken cancellationToken = default)
    {
        GerenciadorCaminhosByteVault.GarantirDiretoriosCriados();
        var caminhoBanco = GerenciadorCaminhosByteVault.ObterCaminhoBancoDados();

        if (!File.Exists(caminhoBanco))
        {
            throw new FileNotFoundException("O banco de dados do cofre não foi encontrado no disco.");
        }

        // Criar pasta de destino se não existir
        var dirDestino = Path.GetDirectoryName(caminhoArquivoZip);
        if (!string.IsNullOrEmpty(dirDestino))
        {
            Directory.CreateDirectory(dirDestino);
        }

        // Excluir arquivo anterior se já existir
        if (File.Exists(caminhoArquivoZip))
        {
            File.Delete(caminhoArquivoZip);
        }

        // Criar pacote ZIP seguro contendo o banco e metadados
        using (var arquivoZip = ZipFile.Open(caminhoArquivoZip, ZipArchiveMode.Create))
        {
            // Adicionar manifesto com versão
            var manifesto = new
            {
                Aplicativo = "BYTEVAULT",
                VersaoEsquema = "1.0.0",
                DataCriacaoUtc = DateTime.UtcNow,
                TotalProjetos = await _contexto.Projetos.CountAsync(cancellationToken),
                TotalSnippets = await _contexto.TrechosCodigo.CountAsync(cancellationToken),
                TotalComandos = await _contexto.Comandos.CountAsync(cancellationToken)
            };

            var manifestoEntrada = arquivoZip.CreateEntry("manifesto.json", CompressionLevel.Optimal);
            using (var streamManifesto = manifestoEntrada.Open())
            {
                await JsonSerializer.SerializeAsync(streamManifesto, manifesto, cancellationToken: cancellationToken);
            }

            // Adicionar banco bytevault.db
            arquivoZip.CreateEntryFromFile(caminhoBanco, "bytevault.db", CompressionLevel.Optimal);
        }

        // Atualizar configuração com data do último backup
        var config = await _contexto.Configuracoes.FirstOrDefaultAsync(c => c.Id == 1, cancellationToken);
        if (config != null)
        {
            config.DataUltimoBackup = DateTime.UtcNow;
            await _contexto.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RestaurarBackupAsync(string caminhoArquivoZip, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(caminhoArquivoZip))
        {
            throw new FileNotFoundException("O arquivo de backup selecionado não existe.");
        }

        GerenciadorCaminhosByteVault.GarantirDiretoriosCriados();
        var caminhoBanco = GerenciadorCaminhosByteVault.ObterCaminhoBancoDados();
        var pastaTemp = Path.Combine(Path.GetTempPath(), $"bytevault_restore_{Guid.NewGuid():N}");
        Directory.CreateDirectory(pastaTemp);

        try
        {
            using (var arquivoZip = ZipFile.OpenRead(caminhoArquivoZip))
            {
                // 1. Prevenção de ataques de Zip Slip (traversal path)
                foreach (var entrada in arquivoZip.Entries)
                {
                    var caminhoCompleto = Path.GetFullPath(Path.Combine(pastaTemp, entrada.FullName));
                    if (!caminhoCompleto.StartsWith(pastaTemp, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Tentativa de caminho inválido ou malicioso detectada no pacote ZIP (Zip Slip).");
                    }
                    var dir = Path.GetDirectoryName(caminhoCompleto);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    entrada.ExtractToFile(caminhoCompleto, overwrite: true);
                }

                // 2. Validar integridade do banco extraído
                var arquivoBancoExtraido = Path.Combine(pastaTemp, "bytevault.db");
                if (!File.Exists(arquivoBancoExtraido))
                {
                    throw new InvalidOperationException("O pacote não contém um banco de dados 'bytevault.db' válido.");
                }

                using (var conexaoTeste = new SqliteConnection($"Data Source={arquivoBancoExtraido}"))
                {
                    await conexaoTeste.OpenAsync(cancellationToken);
                    using var comando = conexaoTeste.CreateCommand();
                    comando.CommandText = "PRAGMA integrity_check;";
                    var resultado = await comando.ExecuteScalarAsync(cancellationToken);
                    if (resultado?.ToString() != "ok")
                    {
                        throw new InvalidOperationException("O banco de dados do backup falhou na verificação de integridade do SQLite.");
                    }
                }

                // 3. Gerar backup automático de segurança pré-restauração
                if (File.Exists(caminhoBanco))
                {
                    var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                    var backupPre = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioBackups(), $"pre_restauracao_{timestamp}.db");
                    File.Copy(caminhoBanco, backupPre, overwrite: true);
                }

                // 4. Fechar conexões ativas do contexto atual
                await _contexto.Database.CloseConnectionAsync();

                // 5. Substituir banco com segurança
                File.Copy(arquivoBancoExtraido, caminhoBanco, overwrite: true);
            }
        }
        finally
        {
            try
            {
                Directory.Delete(pastaTemp, recursive: true);
            }
            catch
            {
                // limpeza segura
            }
        }
    }
}
