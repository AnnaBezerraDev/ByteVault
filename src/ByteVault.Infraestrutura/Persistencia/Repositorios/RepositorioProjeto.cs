using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioProjeto : RepositorioBase<Projeto>, IRepositorioProjeto
{
    public RepositorioProjeto(ByteVaultContextoDados contexto) : base(contexto)
    {
    }

    public async Task<Projeto?> ObterCompletoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Contexto.Projetos
            .Include(p => p.Tags).ThenInclude(tp => tp.Tag)
            .Include(p => p.Tecnologias)
            .Include(p => p.TrechosCodigo)
            .Include(p => p.Comandos)
            .Include(p => p.Anotacoes)
            .Include(p => p.Bugs)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Projeto>> ObterTodosComTagsAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Projetos
            .AsNoTracking()
            .Include(p => p.Tags).ThenInclude(tp => tp.Tag)
            .Include(p => p.Tecnologias)
            .OrderByDescending(p => p.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Projeto>> ObterFavoritosAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Projetos
            .AsNoTracking()
            .Where(p => p.EhFavorito)
            .Include(p => p.Tags).ThenInclude(tp => tp.Tag)
            .OrderByDescending(p => p.DataAtualizacao)
            .ToListAsync(cancellationToken);
    }
}
