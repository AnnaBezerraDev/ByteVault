using System;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Dominio.Interfaces;

namespace ByteVault.Infraestrutura.Persistencia.Repositorios;

public class UnidadeDeTrabalho : IUnidadeDeTrabalho
{
    private readonly ByteVaultContextoDados _contexto;
    private IRepositorioProjeto? _projetos;
    private IRepositorioTrechoCodigo? _trechosCodigo;
    private IRepositorioComando? _comandos;
    private IRepositorioAnotacao? _anotacoes;
    private IRepositorioBug? _bugs;
    private IRepositorioTag? _tags;
    private IRepositorioConfiguracao? _configuracoes;
    private IRepositorioAtividade? _atividades;

    public UnidadeDeTrabalho(ByteVaultContextoDados contexto)
    {
        _contexto = contexto;
    }

    public IRepositorioProjeto Projetos => _projetos ??= new RepositorioProjeto(_contexto);
    public IRepositorioTrechoCodigo TrechosCodigo => _trechosCodigo ??= new RepositorioTrechoCodigo(_contexto);
    public IRepositorioComando Comandos => _comandos ??= new RepositorioComando(_contexto);
    public IRepositorioAnotacao Anotacoes => _anotacoes ??= new RepositorioAnotacao(_contexto);
    public IRepositorioBug Bugs => _bugs ??= new RepositorioBug(_contexto);
    public IRepositorioTag Tags => _tags ??= new RepositorioTag(_contexto);
    public IRepositorioConfiguracao Configuracoes => _configuracoes ??= new RepositorioConfiguracao(_contexto);
    public IRepositorioAtividade Atividades => _atividades ??= new RepositorioAtividade(_contexto);

    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return await _contexto.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _contexto.Dispose();
        GC.SuppressFinalize(this);
    }
}
