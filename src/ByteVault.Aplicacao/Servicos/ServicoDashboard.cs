using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Enumeradores;
using ByteVault.Dominio.Interfaces;

namespace ByteVault.Aplicacao.Servicos;

public class ServicoDashboard : IServicoDashboard
{
    private readonly IUnidadeDeTrabalho _uow;

    public ServicoDashboard(IUnidadeDeTrabalho uow)
    {
        _uow = uow;
    }

    public async Task<IndicadoresDashboardDto> ObterIndicadoresAsync(CancellationToken cancellationToken = default)
    {
        var projetos = await _uow.Projetos.ObterTodosAsync(cancellationToken);
        var snippets = await _uow.TrechosCodigo.ObterTodosAsync(cancellationToken);
        var comandos = await _uow.Comandos.ObterTodosAsync(cancellationToken);
        var anotacoes = await _uow.Anotacoes.ObterTodosAsync(cancellationToken);
        var bugs = await _uow.Bugs.ObterTodosAsync(cancellationToken);
        var atividades = await _uow.Atividades.ObterRecentesAsync(8, cancellationToken);

        var dto = new IndicadoresDashboardDto
        {
            TotalProjetos = projetos.Count,
            ProjetosAtivos = projetos.Count(p => p.Status == StatusProjeto.EmDesenvolvimento || p.Status == StatusProjeto.Planejado),
            TotalSnippets = snippets.Count,
            TotalComandos = comandos.Count,
            TotalAnotacoes = anotacoes.Count,
            BugsNaoResolvidos = bugs.Count(b => b.Status != StatusBug.Resolvido),
            AtividadesRecentes = atividades.Select(a => new ItemAtividadeRecenteDto
            {
                RegistroId = a.RegistroId,
                Titulo = a.Titulo,
                Categoria = a.Categoria,
                TipoAcao = a.TipoAcao,
                DataHora = a.DataRegistro
            }).ToList()
        };

        // Coletar itens favoritados reais
        var favoritos = new List<ItemFavoritoDto>();

        foreach (var p in projetos.Where(p => p.EhFavorito))
        {
            favoritos.Add(new ItemFavoritoDto
            {
                Id = p.Id,
                Titulo = p.Nome,
                Categoria = "Projeto",
                Subtitulo = p.Status.ToString(),
                DataAtualizacao = p.DataAtualizacao
            });
        }

        foreach (var s in snippets.Where(s => s.EhFavorito))
        {
            favoritos.Add(new ItemFavoritoDto
            {
                Id = s.Id,
                Titulo = s.Titulo,
                Categoria = "Snippet",
                Subtitulo = s.Linguagem,
                DataAtualizacao = s.DataAtualizacao
            });
        }

        foreach (var c in comandos.Where(c => c.EhFavorito))
        {
            favoritos.Add(new ItemFavoritoDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Categoria = "Comando",
                Subtitulo = c.FerramentaOuAmbiente,
                DataAtualizacao = c.DataAtualizacao
            });
        }

        foreach (var a in anotacoes.Where(a => a.EhFavorito))
        {
            favoritos.Add(new ItemFavoritoDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Categoria = "Anotação",
                Subtitulo = a.Categoria ?? "Geral",
                DataAtualizacao = a.DataAtualizacao
            });
        }

        foreach (var b in bugs.Where(b => b.EhFavorito))
        {
            favoritos.Add(new ItemFavoritoDto
            {
                Id = b.Id,
                Titulo = b.Titulo,
                Categoria = "Bug",
                Subtitulo = b.Status.ToString(),
                DataAtualizacao = b.DataAtualizacao
            });
        }

        dto.Favoritos = favoritos.OrderByDescending(f => f.DataAtualizacao).Take(15).ToList();

        return dto;
    }
}
