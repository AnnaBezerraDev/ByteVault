using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioAtividade : IRepositorioAtividade
{
    private readonly ByteVaultContextoDados _contexto;

    public RepositorioAtividade(ByteVaultContextoDados contexto)
    {
        _contexto = contexto;
    }

    public async Task RegistrarAsync(string titulo, string categoria, string tipoAcao, Guid? registroId = null, CancellationToken cancellationToken = default)
    {
        var atividade = new RegistroAtividade
        {
            Titulo = titulo,
            Categoria = categoria,
            TipoAcao = tipoAcao,
            RegistroId = registroId,
            DataRegistro = DateTime.UtcNow
        };
        await _contexto.Atividades.AddAsync(atividade, cancellationToken);
        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegistroAtividade>> ObterRecentesAsync(int quantidade = 10, CancellationToken cancellationToken = default)
    {
        return await _contexto.Atividades
            .AsNoTracking()
            .OrderByDescending(a => a.DataRegistro)
            .Take(quantidade)
            .ToListAsync(cancellationToken);
    }
}
