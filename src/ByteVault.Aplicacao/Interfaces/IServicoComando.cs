using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoComando
{
    Task<IReadOnlyList<RegistroComandoDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroComandoDto>> FiltrarAsync(string? termo, string? ferramenta, string? so, string? tag, Guid? projetoId, CancellationToken cancellationToken = default);
    Task<RegistroComandoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RegistroComandoDto> SalvarAsync(RegistroComandoDto dto, CancellationToken cancellationToken = default);
    Task<RegistroComandoDto> DuplicarAsync(Guid id, CancellationToken cancellationToken = default);
    Task AlternarFavoritoAsync(Guid id, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ObterCategoriasDisponiveisAsync(CancellationToken cancellationToken = default);
}
