using ByteVault.Aplicacao.Interfaces;
using ByteVault.Aplicacao.Servicos;
using Microsoft.Extensions.DependencyInjection;

namespace ByteVault.Aplicacao;

public static class InjecaoDependenciaAplicacao
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection servicos)
    {
        servicos.AddScoped<IServicoTag, ServicoTag>();
        servicos.AddScoped<IServicoProjeto, ServicoProjeto>();
        servicos.AddScoped<IServicoTrechoCodigo, ServicoTrechoCodigo>();
        servicos.AddScoped<IServicoComando, ServicoComando>();
        servicos.AddScoped<IServicoAnotacao, ServicoAnotacao>();
        servicos.AddScoped<IServicoBug, ServicoBug>();
        servicos.AddScoped<IServicoDashboard, ServicoDashboard>();
        servicos.AddScoped<IServicoPesquisaGlobal, ServicoPesquisaGlobal>();
        servicos.AddScoped<IServicoPortabilidade, ServicoPortabilidade>();

        return servicos;
    }
}
