using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Servicos;
using ByteVault.Dominio.Enumeradores;
using ByteVault.Dominio.Interfaces;
using ByteVault.Infraestrutura.Persistencia;
using ByteVault.Infraestrutura.Persistencia.Repositorios;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ByteVault.TestesIntegracao;

public class TestesRegrasNegocio : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly ByteVaultContextoDados _contexto;
    private readonly UnidadeDeTrabalho _uow;
    private readonly ServicoTag _servicoTag;
    private readonly ServicoProjeto _servicoProjeto;
    private readonly ServicoTrechoCodigo _servicoSnippet;
    private readonly ServicoComando _servicoComando;
    private readonly ServicoBug _servicoBug;

    public TestesRegrasNegocio()
    {
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
        _servicoBug = new ServicoBug(_uow, _servicoTag);
    }

    [Fact]
    public async Task Projeto_ComUrlInvalida_DeveLancarExcecao()
    {
        var dto = new ProjetoDto
        {
            Nome = "Projeto Inválido",
            UrlRepositorio = "ftp://servidor-invalido.com"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _servicoProjeto.SalvarAsync(dto));
    }

    [Fact]
    public async Task Projeto_DevePermitirArquivarERestaurarComSucesso()
    {
        var dto = new ProjetoDto
        {
            Nome = "Projeto Ciclo de Vida",
            Status = StatusProjeto.EmDesenvolvimento
        };

        var salvo = await _servicoProjeto.SalvarAsync(dto);
        Assert.Equal(StatusProjeto.EmDesenvolvimento, salvo.Status);

        await _servicoProjeto.ArquivarAsync(salvo.Id);
        var arquivado = await _servicoProjeto.ObterPorIdAsync(salvo.Id);
        Assert.NotNull(arquivado);
        Assert.Equal(StatusProjeto.Arquivado, arquivado.Status);

        await _servicoProjeto.RestaurarAsync(salvo.Id);
        var restaurado = await _servicoProjeto.ObterPorIdAsync(salvo.Id);
        Assert.NotNull(restaurado);
        Assert.Equal(StatusProjeto.EmDesenvolvimento, restaurado.Status);
    }

    [Fact]
    public async Task Snippet_AoDuplicar_DeveCriarNovoRegistroComSufixoCopia()
    {
        var original = await _servicoSnippet.SalvarAsync(new TrechoCodigoDto
        {
            Titulo = "QuickSort em C#",
            Codigo = "void Sort() { }",
            Linguagem = "C#"
        });

        var copia = await _servicoSnippet.DuplicarAsync(original.Id);

        Assert.NotEqual(original.Id, copia.Id);
        Assert.Equal("QuickSort em C# (Cópia)", copia.Titulo);
        Assert.Equal(original.Codigo, copia.Codigo);
    }

    [Fact]
    public async Task Comando_AoDuplicar_DevePreservarTextoEParametros()
    {
        var original = await _servicoComando.SalvarAsync(new RegistroComandoDto
        {
            Titulo = "Git Stash Pop",
            TextoComando = "git stash pop",
            FerramentaOuAmbiente = "Git"
        });

        var copia = await _servicoComando.DuplicarAsync(original.Id);

        Assert.NotEqual(original.Id, copia.Id);
        Assert.Equal("Git Stash Pop (Cópia)", copia.Titulo);
        Assert.Equal(original.TextoComando, copia.TextoComando);
    }

    [Fact]
    public async Task Bug_AoSerMarcadoComoResolvido_DeveRegistrarDataResolucao()
    {
        var bug = await _servicoBug.SalvarAsync(new RelatorioBugDto
        {
            Titulo = "Erro de NullReferenceException",
            Status = StatusBug.Aberto
        });

        Assert.Null(bug.DataResolucao);

        await _servicoBug.AlterarStatusAsync(bug.Id, StatusBug.Resolvido);
        var resolvido = await _servicoBug.ObterPorIdAsync(bug.Id);

        Assert.NotNull(resolvido);
        Assert.Equal(StatusBug.Resolvido, resolvido.Status);
        Assert.NotNull(resolvido.DataResolucao);
    }

    public void Dispose()
    {
        _uow.Dispose();
        _contexto.Dispose();
        _conexao.Dispose();
    }
}
