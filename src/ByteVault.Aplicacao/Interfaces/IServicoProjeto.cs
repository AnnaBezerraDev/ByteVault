using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;
using ByteVault.Dominio.Enumeradores;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoProjeto
{
    Task<IReadOnlyList<ProjetoDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjetoDto>> FiltrarAsync(string? termo, StatusProjeto? status, string? tecnologia, string? tag, CancellationToken cancellationToken = default);
    Task<ProjetoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjetoDto> SalvarAsync(ProjetoDto dto, CancellationToken cancellationToken = default);
    Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default);
    Task AlterarStatusAsync(Guid id, StatusProjeto novoStatus, CancellationToken cancellationToken = default);
    Task ArquivarAsync(Guid id, CancellationToken cancellationToken = default);
    Task RestaurarAsync(Guid id, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task RegistrarAberturaAsync(Guid id, CancellationToken cancellationToken = default);
}
