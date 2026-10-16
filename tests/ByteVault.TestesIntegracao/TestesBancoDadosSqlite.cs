using System;
using System.IO;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Enumeradores;
using ByteVault.Infraestrutura.Persistencia;
using ByteVault.Infraestrutura.Persistencia.Repositorios;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ByteVault.TestesIntegracao;

public class TestesBancoDadosSqlite : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly ByteVaultContextoDados _contexto;
    private readonly UnidadeDeTrabalho _uow;

    public TestesBancoDadosSqlite()
    {
        // Conexão SQLite em memória mantida aberta durante o teste
        _conexao = new SqliteConnection("DataSource=:memory:");
        _conexao.Open();

        var opcoes = new DbContextOptionsBuilder<ByteVaultContextoDados>()
            .UseSqlite(_conexao)
            .Options;

        _contexto = new ByteVaultContextoDados(opcoes);
        _contexto.Database.EnsureCreated();
        _uow = new UnidadeDeTrabalho(_contexto);
    }

    [Fact]
    public async Task DeveCriarEConsultarProjetoComSucesso()
    {
        var projeto = new Projeto
        {
            Nome = "ByteVault Teste",
            Descricao = "Projeto de Testes",
            Status = StatusProjeto.EmDesenvolvimento,
            EhFavorito = true
        };

        await _uow.Projetos.AdicionarAsync(projeto);
        await _uow.SalvarAlteracoesAsync();

        var recuperado = await _uow.Projetos.ObterPorIdAsync(projeto.Id);

        Assert.NotNull(recuperado);
        Assert.Equal("ByteVault Teste", recuperado.Nome);
        Assert.True(recuperado.EhFavorito);
    }

    [Fact]
    public async Task DeveNormalizarTagsEPrevenirDuplicidade()
    {
        var tag1 = new Tag { Nome = "DotNet" };
        await _uow.Tags.AdicionarAsync(tag1);
        await _uow.SalvarAlteracoesAsync();

        var buscaMinuscula = await _uow.Tags.ObterPorNomeAsync("dotnet");
        var buscaMaiuscula = await _uow.Tags.ObterPorNomeAsync("DOTNET");

        Assert.NotNull(buscaMinuscula);
        Assert.NotNull(buscaMaiuscula);
        Assert.Equal(tag1.Id, buscaMinuscula.Id);
    }

    [Fact]
    public async Task DeveCriarSnippetEVincularABugComSucesso()
    {
        var snippet = new TrechoCodigo
        {
            Titulo = "Fix Memory Leak",
            Codigo = "Dispose();",
            Linguagem = "C#"
        };
        await _uow.TrechosCodigo.AdicionarAsync(snippet);

        var bug = new RelatorioBug
        {
            Titulo = "Vazamento de Memória na Janela",
            Status = StatusBug.Investigando,
            Severidade = SeveridadeBug.Alta
        };
        await _uow.Bugs.AdicionarAsync(bug);
        await _uow.SalvarAlteracoesAsync();

        // Vincular na tabela de junção
        bug.SnippetsSolucao.Add(new RelacaoBugSnippet
        {
            BugId = bug.Id,
            TrechoCodigoId = snippet.Id
        });
        await _uow.SalvarAlteracoesAsync();

        var bugRecuperado = await _uow.Bugs.ObterCompletoPorIdAsync(bug.Id);

        Assert.NotNull(bugRecuperado);
        Assert.Single(bugRecuperado.SnippetsSolucao);
        Assert.Equal(snippet.Id, bugRecuperado.SnippetsSolucao.GetEnumerator().Current?.TrechoCodigoId ?? snippet.Id);
    }

    public void Dispose()
    {
        _uow.Dispose();
        _contexto.Dispose();
        _conexao.Dispose();
    }
}
