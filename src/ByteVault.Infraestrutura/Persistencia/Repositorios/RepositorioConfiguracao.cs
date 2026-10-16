using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class RepositorioConfiguracao : IRepositorioConfiguracao
{
    private readonly ByteVaultContextoDados _contexto;

    public RepositorioConfiguracao(ByteVaultContextoDados contexto)
    {
        _contexto = contexto;
    }

    public async Task<ConfiguracaoAplicacao> ObterAsync(CancellationToken cancellationToken = default)
    {
        var config = await _contexto.Configuracoes.FirstOrDefaultAsync(c => c.Id == 1, cancellationToken);
        if (config == null)
        {
            config = new ConfiguracaoAplicacao
            {
                Id = 1,
                Tema = "Escuro",
                CorDestaque = "#6C8CFF",
                DensidadeVisual = "Confortavel",
                ComportamentoAoIniciar = "Painel",
                ConfirmarExclusao = true,
                SalvarAutomaticamente = true,
                VersaoEsquema = "1.0.0"
            };
            await _contexto.Configuracoes.AddAsync(config, cancellationToken);
            await _contexto.SaveChangesAsync(cancellationToken);
        }
        return config;
    }

    public async Task SalvarAsync(ConfiguracaoAplicacao configuracao, CancellationToken cancellationToken = default)
    {
        var existente = await _contexto.Configuracoes.FirstOrDefaultAsync(c => c.Id == 1, cancellationToken);
        if (existente == null)
        {
            configuracao.Id = 1;
            await _contexto.Configuracoes.AddAsync(configuracao, cancellationToken);
        }
        else
        {
            _contexto.Entry(existente).CurrentValues.SetValues(configuracao);
        }
        await _contexto.SaveChangesAsync(cancellationToken);
    }
}
