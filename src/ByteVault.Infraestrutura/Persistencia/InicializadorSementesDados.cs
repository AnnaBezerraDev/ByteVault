using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Enumeradores;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia;

public static class InicializadorSementesDados
{
    public static async Task InicializarAsync(ByteVaultContextoDados contexto, CancellationToken cancellationToken = default)
    {
        if (await contexto.Projetos.AnyAsync(cancellationToken))
        {
            return; // Já existem dados, preserva o banco
        }

        // 1. Tags Iniciais
        var tagCSharp = new Tag { Id = Guid.NewGuid(), Nome = "C#", CorHex = "#6C8CFF", DataCriacao = DateTime.UtcNow };
        var tagDotNet = new Tag { Id = Guid.NewGuid(), Nome = ".NET 10", CorHex = "#55C995", DataCriacao = DateTime.UtcNow };
        var tagGit = new Tag { Id = Guid.NewGuid(), Nome = "Git", CorHex = "#E8B86D", DataCriacao = DateTime.UtcNow };
        var tagDocker = new Tag { Id = Guid.NewGuid(), Nome = "Docker", CorHex = "#6C8CFF", DataCriacao = DateTime.UtcNow };
        var tagArquitetura = new Tag { Id = Guid.NewGuid(), Nome = "Arquitetura", CorHex = "#9B7DFF", DataCriacao = DateTime.UtcNow };

        await contexto.Tags.AddRangeAsync(new[] { tagCSharp, tagDotNet, tagGit, tagDocker, tagArquitetura }, cancellationToken);

        // 2. Projeto de Demonstração
        var projetoDemo = new Projeto
        {
            Id = Guid.NewGuid(),
            Nome = "ByteVault Core",
            Descricao = "Cofre de organização pessoal e biblioteca de produtividade para engenheiros de software.",
            Status = StatusProjeto.EmDesenvolvimento,
            LinguagensTecnologias = "C#, WPF, SQLite, EF Core, .NET 10",
            CaminhoLocal = @"C:\Projetos\ByteVault",
            UrlRepositorio = "https://github.com/exemplo/bytevault",
            UrlDocumentacao = "https://learn.microsoft.com/dotnet",
            EhFavorito = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow,
            DataUltimaAbertura = DateTime.UtcNow
        };
        projetoDemo.Tags.Add(new TagProjeto { ProjetoId = projetoDemo.Id, TagId = tagCSharp.Id });
        projetoDemo.Tags.Add(new TagProjeto { ProjetoId = projetoDemo.Id, TagId = tagDotNet.Id });

        await contexto.Projetos.AddAsync(projetoDemo, cancellationToken);

        // 3. Snippets Iniciais
        var snippetDispose = new TrechoCodigo
        {
            Id = Guid.NewGuid(),
            Titulo = "Padrão IDisposable Seguro em C#",
            Descricao = "Implementação canônica de IDisposable com supressão de finalizador.",
            Linguagem = "C#",
            Categoria = "Boas Práticas",
            Codigo = @"public class GerenciadorRecurso : IDisposable
{
    private bool _descartado;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool descartando)
    {
        if (!_descartado)
        {
            if (descartando)
            {
                // Liberar recursos gerenciados
            }
            _descartado = true;
        }
    }
}",
            ProjetoId = projetoDemo.Id,
            EhFavorito = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        snippetDispose.Tags.Add(new TagSnippet { TrechoCodigoId = snippetDispose.Id, TagId = tagCSharp.Id });

        var snippetDebounce = new TrechoCodigo
        {
            Id = Guid.NewGuid(),
            Titulo = "Debounce com CancellationToken",
            Descricao = "Algoritmo assíncrono para pesquisa reativa com cancelamento prévio.",
            Linguagem = "C#",
            Categoria = "Assíncrono",
            Codigo = @"private CancellationTokenSource? _cts;

public void AoDigitarTexto(string novoTexto)
{
    _cts?.Cancel();
    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    Task.Run(async () =>
    {
        await Task.Delay(300, token);
        if (token.IsCancellationRequested) return;

        await ExecutarBuscaAsync(novoTexto, token);
    }, token);
}",
            ProjetoId = projetoDemo.Id,
            EhFavorito = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        snippetDebounce.Tags.Add(new TagSnippet { TrechoCodigoId = snippetDebounce.Id, TagId = tagCSharp.Id });

        await contexto.TrechosCodigo.AddRangeAsync(new[] { snippetDispose, snippetDebounce }, cancellationToken);

        // 4. Comandos Iniciais
        var cmdGitClean = new RegistroComando
        {
            Id = Guid.NewGuid(),
            Titulo = "Limpar Branches Mescladas Localmente",
            Descricao = "Exclui todas as branches locais que já foram incorporadas à branch principal.",
            TextoComando = "git branch --merged | grep -v '\\*' | xargs -n 1 git branch -d",
            FerramentaOuAmbiente = "Git",
            SistemaOperacional = "Linux / macOS / Git Bash",
            Parametros = "--merged: lista branches mescladas; grep -v: ignora branch atual",
            ExemploUtilizacao = "Executar periodicamente para manutenção do repositório.",
            EhFavorito = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        cmdGitClean.Tags.Add(new TagComando { RegistroComandoId = cmdGitClean.Id, TagId = tagGit.Id });

        var cmdDockerPrune = new RegistroComando
        {
            Id = Guid.NewGuid(),
            Titulo = "Limpar Contêineres e Imagens Órfãs",
            Descricao = "Remove todos os recursos do Docker não utilizados liberando espaço em disco.",
            TextoComando = "docker system prune -a --volumes",
            FerramentaOuAmbiente = "Docker",
            SistemaOperacional = "Multiplataforma",
            Parametros = "-a: todas as imagens; --volumes: remove volumes anônimos",
            ExemploUtilizacao = "docker system prune -a --volumes",
            EhFavorito = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        cmdDockerPrune.Tags.Add(new TagComando { RegistroComandoId = cmdDockerPrune.Id, TagId = tagDocker.Id });

        await contexto.Comandos.AddRangeAsync(new[] { cmdGitClean, cmdDockerPrune }, cancellationToken);

        // 5. Anotação em Markdown
        var anotacaoArquitetura = new Anotacao
        {
            Id = Guid.NewGuid(),
            Titulo = "Princípios de Arquitetura Limpa e MVVM",
            Categoria = "Arquitetura",
            Conteudo = @"# Princípios de Arquitetura no BYTEVAULT

A arquitetura do ByteVault foi projetada com quatro camadas estritas:

1. **Dominio**: Entidades puras, regras de negócio e interfaces de repositórios.
2. **Aplicacao**: Casos de uso orquestrados, validações e DTOs.
3. **Infraestrutura**: EF Core 10 com SQLite e persistência em `%LOCALAPPDATA%`.
4. **Apresentacao**: WPF nativo, ViewModels com `CommunityToolkit.Mvvm` e Serilog.

> **Importante**: As telas (Views) não acessam o banco de dados diretamente e os comandos não executam scripts externos automaticamente.",
            ProjetoId = projetoDemo.Id,
            EhFavorito = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        anotacaoArquitetura.Tags.Add(new TagAnotacao { AnotacaoId = anotacaoArquitetura.Id, TagId = tagArquitetura.Id });

        await contexto.Anotacoes.AddAsync(anotacaoArquitetura, cancellationToken);

        // 6. Relatório de Bug Inicial Solucionado
        var bugResolvido = new RelatorioBug
        {
            Id = Guid.NewGuid(),
            Titulo = "Vazamento de Conexão com SQLite em Memória nos Testes",
            Descricao = "Ao executar os testes simultaneamente, conexões eram finalizadas antes da conclusão das consultas.",
            MensagemErroOriginal = "Microsoft.Data.Sqlite.SqliteException: SQLite Error 1: 'no such table: Projetos'",
            TecnologiaRelacionada = "SQLite / EF Core",
            Ambiente = "Windows 11 / xUnit",
            Solucao = "Manter a instância de SqliteConnection aberta durante toda a vida útil da classe de teste e chamar .Close() apenas no método Dispose().",
            Status = StatusBug.Resolvido,
            Severidade = SeveridadeBug.Alta,
            DataResolucao = DateTime.UtcNow,
            EhFavorito = false,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };
        bugResolvido.SnippetsSolucao.Add(new RelacaoBugSnippet
        {
            BugId = bugResolvido.Id,
            TrechoCodigoId = snippetDispose.Id
        });

        await contexto.Bugs.AddAsync(bugResolvido, cancellationToken);

        // 7. Registro de Atividade Inicial
        await contexto.Atividades.AddRangeAsync(new[]
        {
            new RegistroAtividade { Titulo = "Cofre Inicializado", Categoria = "Sistema", TipoAcao = "Inicialização", DataRegistro = DateTime.UtcNow.AddMinutes(-5) },
            new RegistroAtividade { Titulo = "ByteVault Core", Categoria = "Projeto", TipoAcao = "Criado", RegistroId = projetoDemo.Id, DataRegistro = DateTime.UtcNow.AddMinutes(-3) },
            new RegistroAtividade { Titulo = "Padrão IDisposable Seguro", Categoria = "Snippet", TipoAcao = "Criado", RegistroId = snippetDispose.Id, DataRegistro = DateTime.UtcNow.AddMinutes(-2) }
        }, cancellationToken);

        await contexto.SaveChangesAsync(cancellationToken);
    }
}
