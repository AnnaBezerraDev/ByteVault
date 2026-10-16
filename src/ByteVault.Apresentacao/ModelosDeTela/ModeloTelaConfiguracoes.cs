using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ByteVault.Aplicacao.Interfaces;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Interfaces;
using ByteVault.Infraestrutura.Servicos;
using ByteVault.Apresentacao.Servicos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ByteVault.Apresentacao.ModelosDeTela;

public partial class ModeloTelaConfiguracoes : ModeloTelaBase
{
    private readonly IUnidadeDeTrabalho _uow;
    private readonly IServicoBackupRestauracao _servicoBackup;
    private readonly IServicoPortabilidade _servicoPortabilidade;

    [ObservableProperty]
    private ConfiguracaoAplicacao _configuracao = new();

    [ObservableProperty]
    private string _caminhoBancoDados = string.Empty;

    [ObservableProperty]
    private string _caminhoPastaDados = string.Empty;

    [ObservableProperty]
    private string _versaoApp = "1.0.0 (NET 10 LTS)";

    [ObservableProperty]
    private bool _salvoComSucesso;

    [ObservableProperty]
    private string? _mensagemFeedback;

    public ModeloTelaConfiguracoes(
        IUnidadeDeTrabalho uow,
        IServicoBackupRestauracao servicoBackup,
        IServicoPortabilidade servicoPortabilidade)
    {
        _uow = uow;
        _servicoBackup = servicoBackup;
        _servicoPortabilidade = servicoPortabilidade;
        Titulo = "Configurações";
        CaminhoBancoDados = GerenciadorCaminhosByteVault.ObterCaminhoBancoDados();
        CaminhoPastaDados = GerenciadorCaminhosByteVault.ObterDiretorioBase();
    }

    public override async Task CarregarAsync()
    {
        try
        {
            EstaCarregando = true;
            Configuracao = await _uow.Configuracoes.ObterAsync();
            CaminhoBancoDados = GerenciadorCaminhosByteVault.ObterCaminhoBancoDados();
            CaminhoPastaDados = GerenciadorCaminhosByteVault.ObterDiretorioBase();
            ServicoTema.AplicarTema(Configuracao.Tema);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    public void SelecionarTema(string novoTema)
    {
        if (string.IsNullOrWhiteSpace(novoTema)) return;
        Configuracao.Tema = novoTema;
        ServicoTema.AplicarTema(novoTema);
    }

    [RelayCommand]
    private async Task SalvarConfiguracoesAsync()
    {
        EstaCarregando = true;
        try
        {
            await _uow.Configuracoes.SalvarAsync(Configuracao);
            ServicoTema.AplicarTema(Configuracao.Tema);
            SalvoComSucesso = true;
            await Task.Delay(2000);
            SalvoComSucesso = false;
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task CriarBackupAgoraAsync()
    {
        EstaCarregando = true;
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var caminhoZip = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioBackups(), $"bytevault_backup_{timestamp}.zip");
            await _servicoBackup.CriarBackupAsync(caminhoZip);
            await CarregarAsync();
            MessageBox.Show($"Backup gerado com sucesso em:\n{caminhoZip}", "Backup BYTEVAULT", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Falha ao gerar backup: {ex.Message}", "Erro de Backup", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task ExportarJsonAsync()
    {
        EstaCarregando = true;
        try
        {
            var caminho = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioBase(), "bytevault_exportacao.json");
            await _servicoPortabilidade.ExportarJsonCompletoAsync(caminho);
            MessageBox.Show($"Exportação JSON concluída:\n{caminho}", "Exportação JSON", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na exportação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task ExportarMarkdownAsync()
    {
        EstaCarregando = true;
        try
        {
            var pasta = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioBase(), "Anotacoes_Markdown");
            await _servicoPortabilidade.ExportarAnotacoesMarkdownAsync(pasta);
            MessageBox.Show($"Anotações exportadas em Markdown para:\n{pasta}", "Exportação Markdown", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na exportação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task ExportarSnippetsTxtAsync()
    {
        EstaCarregando = true;
        try
        {
            var caminho = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioBase(), "bytevault_snippets.txt");
            await _servicoPortabilidade.ExportarSnippetsTxtAsync(caminho);
            MessageBox.Show($"Trechos de código exportados em TXT:\n{caminho}", "Exportação TXT", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na exportação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private async Task ExportarCsvAsync()
    {
        EstaCarregando = true;
        try
        {
            var caminho = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioBase(), "relatorio_projetos.csv");
            await _servicoPortabilidade.ExportarRelatorioProjetosCsvAsync(caminho);
            MessageBox.Show($"Relatório CSV gerado:\n{caminho}", "Exportação CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro na exportação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    [RelayCommand]
    private void AbrirPastaDados()
    {
        var caminho = GerenciadorCaminhosByteVault.ObterDiretorioBase();
        if (Directory.Exists(caminho))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = caminho,
                UseShellExecute = true
            });
        }
    }
}
