using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteVault.Aplicacao.DTOs;

namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoTag
{
    Task<IReadOnlyList<TagDto>> ObterTodasAsync(CancellationToken cancellationToken = default);
    Task<TagDto> CriarOuObterAsync(string nome, string? corHex = null, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Guid id, string novoNome, string corHex, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
}
