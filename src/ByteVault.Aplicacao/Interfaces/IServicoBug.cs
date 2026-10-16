using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Dominio.Enumeradores;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoBug
{
    Task<IReadOnlyList<RelatorioBugDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RelatorioBugDto>> FiltrarAsync(string? termo, StatusBug? status, SeveridadeBug? severidade, string? tag, Guid? projetoId, CancellationToken cancellationToken = default);
    Task<RelatorioBugDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RelatorioBugDto> SalvarAsync(RelatorioBugDto dto, CancellationToken cancellationToken = default);
    Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default);
    Task AlterarStatusAsync(Guid id, StatusBug novoStatus, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task VincularSnippetAsync(Guid bugId, Guid snippetId, CancellationToken cancellationToken = default);
    Task DesvincularSnippetAsync(Guid bugId, Guid snippetId, CancellationToken cancellationToken = default);
}
