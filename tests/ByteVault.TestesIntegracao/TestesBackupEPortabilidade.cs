using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Servicos;
using ByteVault.Dominio.Enumeradores;
using ByteVault.Infraestrutura.Backup;
using ByteVault.Infraestrutura.Persistencia;
using ByteVault.Infraestrutura.Persistencia.Repositorios;
using ByteVault.Infraestrutura.Servicos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ByteVault.TestesIntegracao;

public class TestesBackupEPortabilidade : IDisposable
{
    private readonly string _pastaTeste;
    private readonly SqliteConnection _conexao;
    private readonly ByteVaultContextoDados _contexto;
    private readonly UnidadeDeTrabalho _uow;
    private readonly ServicoTag _servicoTag;
    private readonly ServicoProjeto _servicoProjeto;
    private readonly ServicoTrechoCodigo _servicoSnippet;
    private readonly ServicoComando _servicoComando;
    private readonly ServicoAnotacao _servicoAnotacao;
    private readonly ServicoBug _servicoBug;
    private readonly ServicoPortabilidade _servicoPortabilidade;

    public TestesBackupEPortabilidade()
    {
        _pastaTeste = Path.Combine(Path.GetTempPath(), $"bytevault_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_pastaTeste);

        _conexao = new SqliteConnection("DataSource=:memory:");
        _conexao.Open();

        var opcoes = new DbContextOptionsBuilder<ByteVaultContextoDados>()
            .UseSqlite(_conexao)
            .Options;

        _contexto = new ByteVaultContextoDados(opcoes);
        _contexto.Database.EnsureCreated();
        _uow = new UnidadeDeTrabalho(_contexto);
        _servicoTag = new ServicoTag(_uow);
        _servicoProjeto = new ServicoProjeto(_uow, _servicoTag);
        _servicoSnippet = new ServicoTrechoCodigo(_uow, _servicoTag);
        _servicoComando = new ServicoComando(_uow, _servicoTag);
        _servicoAnotacao = new ServicoAnotacao(_uow, _servicoTag);
        _servicoBug = new ServicoBug(_uow, _servicoTag);
        _servicoPortabilidade = new ServicoPortabilidade(
            _uow,
            _servicoProjeto,
            _servicoSnippet,
            _servicoComando,
            _servicoAnotacao,
            _servicoBug);
    }

    [Fact]
    public async Task ExportacaoJson_DeveGerarArquivoEstruturadoValido()
    {
        await _servicoProjeto.SalvarAsync(new ProjetoDto
        {
            Nome = "ByteVault Export Test",
            Status = StatusProjeto.EmDesenvolvimento
        });

        var caminhoJson = Path.Combine(_pastaTeste, "export.json");
        await _servicoPortabilidade.ExportarJsonCompletoAsync(caminhoJson);

        Assert.True(File.Exists(caminhoJson));
        var conteudo = await File.ReadAllTextAsync(caminhoJson);
        Assert.Contains("BYTEVAULT", conteudo);
        Assert.Contains("ByteVault Export Test", conteudo);
    }

    [Fact]
    public async Task ExportacaoMarkdown_DeveGerarArquivosMdIndividuais()
    {
        await _servicoAnotacao.SalvarAsync(new AnotacaoDto
        {
            Titulo = "Arquitetura Hexagonal",
            Conteudo = "# Hexagonal\n\nConceitos principais de portas e adaptadores.",
            Categoria = "Engenharia"
        });

        var pastaMd = Path.Combine(_pastaTeste, "Notas");
        await _servicoPortabilidade.ExportarAnotacoesMarkdownAsync(pastaMd);

        Assert.True(Directory.Exists(pastaMd));
        var arquivos = Directory.GetFiles(pastaMd, "*.md");
        Assert.NotEmpty(arquivos);

        var conteudo = await File.ReadAllTextAsync(arquivos[0]);
        Assert.Contains("Arquitetura Hexagonal", conteudo);
    }

    [Fact]
    public async Task ExportacaoCsv_DeveGerarRelatorioTabular()
    {
        await _servicoProjeto.SalvarAsync(new ProjetoDto
        {
            Nome = "Projeto Tabular",
            Status = StatusProjeto.Concluido,
            LinguagensTecnologias = "C#, SQLite"
        });

        var caminhoCsv = Path.Combine(_pastaTeste, "relatorio.csv");
        await _servicoPortabilidade.ExportarRelatorioProjetosCsvAsync(caminhoCsv);

        Assert.True(File.Exists(caminhoCsv));
        var linhas = await File.ReadAllLinesAsync(caminhoCsv);
        Assert.True(linhas.Length >= 2);
        Assert.Contains("Projeto Tabular", linhas[1]);
    }

    public void Dispose()
    {
        _uow.Dispose();
        _contexto.Dispose();
        _conexao.Dispose();
        try
        {
            Directory.Delete(_pastaTeste, recursive: true);
        }
        catch
        {
            // limpa silencioso
        }
    }
}
