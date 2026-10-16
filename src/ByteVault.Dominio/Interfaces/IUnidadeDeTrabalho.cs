using System;
using System.Threading;
using System.Threading.Tasks;

namespace ByteVault.Dominio.Interfaces;

public interface IUnidadeDeTrabalho : IDisposable
{
    IRepositorioProjeto Projetos { get; }
    IRepositorioTrechoCodigo TrechosCodigo { get; }
    IRepositorioComando Comandos { get; }
    IRepositorioAnotacao Anotacoes { get; }
    IRepositorioBug Bugs { get; }
    IRepositorioTag Tags { get; }
    IRepositorioConfiguracao Configuracoes { get; }
    IRepositorioAtividade Atividades { get; }

    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
