using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioTag : RepositorioBase<Tag>, IRepositorioTag
{
    public RepositorioTag(ByteVaultContextoDados contexto) : base(contexto)
    {
    }

    public async Task<Tag?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        var nomeNormalizado = nome.Trim().ToLower();
        return await Contexto.Tags
            .FirstOrDefaultAsync(t => t.Nome.ToLower() == nomeNormalizado, cancellationToken);
    }

    public async Task<bool> ExisteComNomeAsync(string nome, Guid? excluirId = null, CancellationToken cancellationToken = default)
    {
        var nomeNormalizado = nome.Trim().ToLower();
        var consulta = Contexto.Tags.AsNoTracking().Where(t => t.Nome.ToLower() == nomeNormalizado);
        if (excluirId.HasValue)
        {
            consulta = consulta.Where(t => t.Id != excluirId.Value);
        }
        return await consulta.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> ObterComContadoresUsoAsync(CancellationToken cancellationToken = default)
    {
        return await Contexto.Tags
            .AsNoTracking()
            .Include(t => t.TagsProjetos)
            .Include(t => t.TagsSnippets)
            .Include(t => t.TagsComandos)
            .Include(t => t.TagsAnotacoes)
            .Include(t => t.TagsBugs)
            .OrderBy(t => t.Nome)
            .ToListAsync(cancellationToken);
    }
}
