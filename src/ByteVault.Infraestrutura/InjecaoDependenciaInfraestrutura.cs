using ByteVault.Dominio.Interfaces;
using ByteVault.Infraestrutura.Persistencia;
using ByteVault.Infraestrutura.Persistencia.Repositorios;
using ByteVault.Infraestrutura.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ByteVault.Infraestrutura;

public static class InjecaoDependenciaInfraestrutura
{
    public static IServiceCollection AdicionarInfraestrutura(
        this IServiceCollection servicos,
        string? caminhoBancoDados = null)
    {
        GerenciadorCaminhosByteVault.GarantirDiretoriosCriados();
        var caminhoEfetivo = caminhoBancoDados ?? GerenciadorCaminhosByteVault.ObterCaminhoBancoDados();

        servicos.AddDbContext<ByteVaultContextoDados>(opcoes =>
        {
            opcoes.UseSqlite($"Data Source={caminhoEfetivo}");
        });

        servicos.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        servicos.AddScoped<IRepositorioProjeto, RepositorioProjeto>();
        servicos.AddScoped<IRepositorioTrechoCodigo, RepositorioTrechoCodigo>();
        servicos.AddScoped<IRepositorioComando, RepositorioComando>();
        servicos.AddScoped<IRepositorioAnotacao, RepositorioAnotacao>();
        servicos.AddScoped<IRepositorioBug, RepositorioBug>();
        servicos.AddScoped<IRepositorioTag, RepositorioTag>();
        servicos.AddScoped<IRepositorioConfiguracao, RepositorioConfiguracao>();
        servicos.AddScoped<IRepositorioAtividade, RepositorioAtividade>();
        servicos.AddScoped<IServicoBackupRestauracao, ByteVault.Infraestrutura.Backup.ServicoBackupRestauracao>();

        return servicos;
    }
}
