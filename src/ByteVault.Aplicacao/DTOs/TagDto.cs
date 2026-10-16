using System;

namespace ByteVault.Aplicacao.DTOs;

public class TagDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string CorHex { get; set; } = "#6C8CFF";
    public int TotalUsos { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
