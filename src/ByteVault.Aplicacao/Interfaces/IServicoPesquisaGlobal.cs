using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoPesquisaGlobal
{
    Task<IReadOnlyList<ItemPesquisaGlobalDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
}
