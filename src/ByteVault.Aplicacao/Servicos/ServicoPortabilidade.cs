using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;

namespace ByteVault.Aplicacao.Servicos;

public class ServicoPortabilidade : IServicoPortabilidade
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoProjeto _servicoProjeto;
    private readonly IServicoTrechoCodigo _servicoSnippet;
    private readonly IServicoComando _servicoComando;
    private readonly IServicoAnotacao _servicoAnotacao;
    private readonly IServicoBug _servicoBug;

    public ServicoPortabilidade(
        IUnidadeDeTrabalho uow,
        IServicoProjeto servicoProjeto,
        IServicoTrechoCodigo servicoSnippet,
        IServicoComando servicoComando,
        IServicoAnotacao servicoAnotacao,
        IServicoBug servicoBug)
    {
        _uow = uow;
        _servicoProjeto = servicoProjeto;
        _servicoSnippet = servicoSnippet;
        _servicoComando = servicoComando;
        _servicoAnotacao = servicoAnotacao;
        _servicoBug = servicoBug;
    }

    public async Task ExportarJsonCompletoAsync(string caminhoArquivoJson, CancellationToken cancellationToken = default)
    {
        var pacote = new
        {
            Cabecalho = new
            {
                Origem = "BYTEVAULT",
                Versao = "1.0.0",
                DataExportacaoUtc = DateTime.UtcNow
            },
            Projetos = await _servicoProjeto.ObterTodosAsync(cancellationToken),
            Snippets = await _servicoSnippet.ObterTodosAsync(cancellationToken),
            Comandos = await _servicoComando.ObterTodosAsync(cancellationToken),
            Anotacoes = await _servicoAnotacao.ObterTodosAsync(cancellationToken),
            Bugs = await _servicoBug.ObterTodosAsync(cancellationToken)
        };

        var json = JsonSerializer.Serialize(pacote, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(caminhoArquivoJson, json, Encoding.UTF8, cancellationToken);
    }

    public async Task ImportarJsonCompletoAsync(string caminhoArquivoJson, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(caminhoArquivoJson))
        {
            throw new FileNotFoundException("O arquivo JSON especificado não existe.");
        }

        var json = await File.ReadAllTextAsync(caminhoArquivoJson, Encoding.UTF8, cancellationToken);
        using var documento = JsonDocument.Parse(json);
        var raiz = documento.RootElement;

        // Validar cabeçalho
        if (!raiz.TryGetProperty("Cabecalho", out var cabecalho))
        {
            throw new InvalidOperationException("Formato inválido: o arquivo não possui o cabeçalho estruturado do BYTEVAULT.");
        }

        // Importar Projetos
        if (raiz.TryGetProperty("Projetos", out var projetosElem) && projetosElem.ValueKind == JsonValueKind.Array)
        {
            foreach (var elem in projetosElem.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dto = JsonSerializer.Deserialize<ProjetoDto>(elem.GetRawText());
                if (dto != null && !string.IsNullOrWhiteSpace(dto.Nome))
                {
                    dto.Id = Guid.Empty; // Gera novos IDs para evitar colisão
                    await _servicoProjeto.SalvarAsync(dto, cancellationToken);
                }
            }
        }

        // Importar Snippets
        if (raiz.TryGetProperty("Snippets", out var snippetsElem) && snippetsElem.ValueKind == JsonValueKind.Array)
        {
            foreach (var elem in snippetsElem.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dto = JsonSerializer.Deserialize<TrechoCodigoDto>(elem.GetRawText());
                if (dto != null && !string.IsNullOrWhiteSpace(dto.Titulo))
                {
                    dto.Id = Guid.Empty;
                    dto.ProjetoId = null;
                    await _servicoSnippet.SalvarAsync(dto, cancellationToken);
                }
            }
        }

        // Importar Comandos
        if (raiz.TryGetProperty("Comandos", out var comandosElem) && comandosElem.ValueKind == JsonValueKind.Array)
        {
            foreach (var elem in comandosElem.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dto = JsonSerializer.Deserialize<RegistroComandoDto>(elem.GetRawText());
                if (dto != null && !string.IsNullOrWhiteSpace(dto.Titulo))
                {
                    dto.Id = Guid.Empty;
                    dto.ProjetoId = null;
                    await _servicoComando.SalvarAsync(dto, cancellationToken);
                }
            }
        }

        // Importar Anotações
        if (raiz.TryGetProperty("Anotacoes", out var anotacoesElem) && anotacoesElem.ValueKind == JsonValueKind.Array)
        {
            foreach (var elem in anotacoesElem.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dto = JsonSerializer.Deserialize<AnotacaoDto>(elem.GetRawText());
                if (dto != null && !string.IsNullOrWhiteSpace(dto.Titulo))
                {
                    dto.Id = Guid.Empty;
                    dto.ProjetoId = null;
                    await _servicoAnotacao.SalvarAsync(dto, cancellationToken);
                }
            }
        }

        // Importar Bugs
        if (raiz.TryGetProperty("Bugs", out var bugsElem) && bugsElem.ValueKind == JsonValueKind.Array)
        {
            foreach (var elem in bugsElem.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dto = JsonSerializer.Deserialize<RelatorioBugDto>(elem.GetRawText());
                if (dto != null && !string.IsNullOrWhiteSpace(dto.Titulo))
                {
                    dto.Id = Guid.Empty;
                    dto.ProjetoId = null;
                    dto.SnippetSolucaoIds.Clear();
                    await _servicoBug.SalvarAsync(dto, cancellationToken);
                }
            }
        }
    }

    public async Task ExportarAnotacoesMarkdownAsync(string pastaDestino, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(pastaDestino);
        var anotacoes = await _servicoAnotacao.ObterTodosAsync(cancellationToken);

        foreach (var a in anotacoes)
        {
            var nomeArquivoSeguro = string.Join("_", a.Titulo.Split(Path.GetInvalidFileNameChars())) + ".md";
            var caminho = Path.Combine(pastaDestino, nomeArquivoSeguro);

            var sb = new StringBuilder();
            sb.AppendLine($"# {a.Titulo}");
            sb.AppendLine();
            sb.AppendLine($"> Categoria: {a.Categoria ?? "Geral"} | Atualizado em: {a.DataAtualizacao:dd/MM/yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine(a.Conteudo);

            await File.WriteAllTextAsync(caminho, sb.ToString(), Encoding.UTF8, cancellationToken);
        }
    }

    public async Task ExportarSnippetsTxtAsync(string caminhoArquivoTxt, CancellationToken cancellationToken = default)
    {
        var snippets = await _servicoSnippet.ObterTodosAsync(cancellationToken);
        var sb = new StringBuilder();

        sb.AppendLine("================================================================================");
        sb.AppendLine("BYTEVAULT — EXPORTAÇÃO DE TRECHOS DE CÓDIGO (SNIPPETS)");
        sb.AppendLine($"Data de Geração: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine("================================================================================");
        sb.AppendLine();

        foreach (var s in snippets)
        {
            sb.AppendLine($"[SNIPPET] {s.Titulo}");
            sb.AppendLine($"Linguagem: {s.Linguagem} | Categoria: {s.Categoria ?? "N/A"}");
            sb.AppendLine($"Descrição: {s.Descricao ?? "Sem descrição"}");
            sb.AppendLine("--- CÓDIGO FONTE ---");
            sb.AppendLine(s.Codigo);
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine();
        }

        await File.WriteAllTextAsync(caminhoArquivoTxt, sb.ToString(), Encoding.UTF8, cancellationToken);
    }

    public async Task ExportarRelatorioProjetosCsvAsync(string caminhoArquivoCsv, CancellationToken cancellationToken = default)
    {
        var projetos = await _servicoProjeto.ObterTodosAsync(cancellationToken);
        var sb = new StringBuilder();

        // Cabeçalho CSV
        sb.AppendLine("Id;Nome;Status;Tecnologias;Favorito;DataCriacao;DataAtualizacao;CaminhoLocal;UrlRepositorio");

        foreach (var p in projetos)
        {
            var linha = $"\"{p.Id}\";\"{p.Nome.Replace("\"", "\"\"")}\";\"{p.Status}\";\"{p.LinguagensTecnologias}\";\"{p.EhFavorito}\";\"{p.DataCriacao:yyyy-MM-dd HH:mm:ss}\";\"{p.DataAtualizacao:yyyy-MM-dd HH:mm:ss}\";\"{p.CaminhoLocal}\";\"{p.UrlRepositorio}\"";
            sb.AppendLine(linha);
        }

        await File.WriteAllTextAsync(caminhoArquivoCsv, sb.ToString(), Encoding.UTF8, cancellationToken);
    }
}
