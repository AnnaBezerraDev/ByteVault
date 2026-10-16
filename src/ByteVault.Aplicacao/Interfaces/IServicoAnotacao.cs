using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoAnotacao
{
    Task<IReadOnlyList<AnotacaoDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AnotacaoDto>> FiltrarAsync(string? termo, string? categoria, string? tag, Guid? projetoId, CancellationToken cancellationToken = default);
    Task<AnotacaoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AnotacaoDto> SalvarAsync(AnotacaoDto dto, CancellationToken cancellationToken = default);
    Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
}
