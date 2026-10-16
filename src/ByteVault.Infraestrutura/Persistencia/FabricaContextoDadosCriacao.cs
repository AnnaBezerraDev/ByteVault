using System.IO;
using ByteVault.Infraestrutura.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ByteVault.Infraestrutura.Persistencia;

public class FabricaContextoDadosCriacao : IDesignTimeDbContextFactory<ByteVaultContextoDados>
{
    public ByteVaultContextoDados CreateDbContext(string[] args)
    {
        GerenciadorCaminhosByteVault.GarantirDiretoriosCriados();
        var caminhoBanco = GerenciadorCaminhosByteVault.ObterCaminhoBancoDados();

        var opcoesBuilder = new DbContextOptionsBuilder<ByteVaultContextoDados>();
        opcoesBuilder.UseSqlite($"Data Source={caminhoBanco}");

        return new ByteVaultContextoDados(opcoesBuilder.Options);
    }
}
