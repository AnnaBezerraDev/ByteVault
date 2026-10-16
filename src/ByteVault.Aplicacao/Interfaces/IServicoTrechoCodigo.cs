using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoTrechoCodigo
{
    Task<IReadOnlyList<TrechoCodigoDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrechoCodigoDto>> FiltrarAsync(string? termo, string? linguagem, string? tag, Guid? projetoId, CancellationToken cancellationToken = default);
    Task<TrechoCodigoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TrechoCodigoDto> SalvarAsync(TrechoCodigoDto dto, CancellationToken cancellationToken = default);
    Task<TrechoCodigoDto> DuplicarAsync(Guid id, CancellationToken cancellationToken = default);
    Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ObterLinguagensDisponiveisAsync(CancellationToken cancellationToken = default);
}
