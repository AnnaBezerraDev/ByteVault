using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ByteVault.Aplicacao;
using ByteVault.Apresentacao.ModelosDeTela;
using ByteVault.Apresentacao.Servicos;
using ByteVault.Apresentacao.Telas;
using ByteVault.Infraestrutura;
using ByteVault.Infraestrutura.Persistencia;
using ByteVault.Infraestrutura.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ByteVault.Apresentacao;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // 1. Garantir diretórios locais em %LOCALAPPDATA%\BYTEVAULT
        GerenciadorCaminhosByteVault.GarantirDiretoriosCriados();
        var caminhoLogs = Path.Combine(GerenciadorCaminhosByteVault.ObterDiretorioLogs(), "bytevault-.log");

        // 2. Configurar Serilog estruturado com sink em arquivo
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(caminhoLogs, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
            .CreateLogger();

        Log.Information("=== Inicializando BYTEVAULT v1.0.0 ===");

        // 3. Tratamento Global de Exceções
        DispatcherUnhandledException += TratarDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TratarTaskSchedulerUnobservedException;
        AppDomain.CurrentDomain.UnhandledException += TratarAppDomainUnhandledException;

        try
        {
            // 4. Configurar Injeção de Dependência e Serviços
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((contexto, servicos) =>
                {
                    // Infraestrutura (EF Core, SQLite, Repositórios, UoW)
                    servicos.AdicionarInfraestrutura();

                    // Aplicação (Casos de uso e serviços)
                    servicos.AdicionarAplicacao();

                    // Serviços de Sistema da Apresentação
                    servicos.AddSingleton<ByteVault.Aplicacao.Interfaces.IServicoSistema, ByteVault.Apresentacao.Servicos.ServicoSistema>();

                    // ViewModels
                    servicos.AddSingleton<ModeloTelaDashboard>();
                    servicos.AddSingleton<ModeloTelaFavoritos>();

                    servicos.AddSingleton<ModeloTelaProjetos>();
                    servicos.AddSingleton<ModeloTelaSnippets>();
                    servicos.AddSingleton<ModeloTelaComandos>();
                    servicos.AddSingleton<ModeloTelaAnotacoes>();
                    servicos.AddSingleton<ModeloTelaBugs>();
                    servicos.AddSingleton<ModeloTelaTags>();
                    servicos.AddSingleton<ModeloTelaConfiguracoes>();
                    servicos.AddSingleton<ModeloTelaPrincipal>();

                    // Janela Principal
                    servicos.AddSingleton<JanelaPrincipal>();
                })
                .Build();

            await _host.StartAsync();

            // 5. Garantir aplicação de migrações automáticas e sementes iniciais
            using (var escopo = _host.Services.CreateScope())
            {
                var dbContexto = escopo.ServiceProvider.GetRequiredService<ByteVaultContextoDados>();
                await dbContexto.Database.MigrateAsync();
                await InicializadorSementesDados.InicializarAsync(dbContexto);
                Log.Information("Banco de dados SQLite verificado e atualizado com sucesso.");

                var repoConfig = escopo.ServiceProvider.GetRequiredService<ByteVault.Dominio.Interfaces.IRepositorioConfiguracao>();
                var config = await repoConfig.ObterAsync();
                ServicoTema.AplicarTema(config.Tema);
            }

            // 6. Exibir Janela Principal
            var janelaPrincipal = _host.Services.GetRequiredService<JanelaPrincipal>();
            MainWindow = janelaPrincipal;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            janelaPrincipal.Show();
            Log.Information("Janela principal exibida com sucesso.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Falha crítica durante a inicialização do BYTEVAULT.");
            MessageBox.Show(
                $"Ocorreu um erro ao inicializar o aplicativo:\n{ex.Message}\n\nConsulte os logs em {caminhoLogs}",
                "Erro de Inicialização — BYTEVAULT",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        Log.Information("=== Encerrando BYTEVAULT ===");
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void TratarDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Exceção não tratada na UI Thread (Dispatcher).");
        MessageBox.Show(
            $"Ocorreu um erro inesperado na interface:\n{e.Exception.Message}",
            "Aviso — BYTEVAULT",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
        e.Handled = true;
    }

    private void TratarTaskSchedulerUnobservedException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(e.Exception, "Exceção assíncrona não observada no TaskScheduler.");
        e.SetObserved();
    }

    private void TratarAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Log.Fatal(ex, "Exceção não tratada no AppDomain.");
        }
    }
}
