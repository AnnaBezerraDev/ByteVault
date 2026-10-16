namespace ByteVault.Aplicacao.Interfaces;

public interface IServicoSistema
{
    bool AbrirDiretorioNoExplorer(string? caminho, out string mensagemErro);
    bool AbrirUrlNoNavegador(string? url, out string mensagemErro);
    void CopiarParaAreaTransferencia(string texto);
}
