using System.Windows;
using System.Windows.Input;
using ByteVault.Apresentacao.ModelosDeTela;
using ByteVault.Apresentacao.Servicos;

namespace ByteVault.Apresentacao.Telas;

public partial class JanelaPrincipal : Window
{
    public JanelaPrincipal(ModeloTelaPrincipal modeloTela)
    {
        InitializeComponent();
        DataContext = modeloTela;
        Loaded += async (s, e) => await modeloTela.CarregarAsync();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Atalho F11: Alternar Tela Cheia / Modo Maximizado
        if (e.Key == Key.F11)
        {
            AlternarTelaCheia();
            e.Handled = true;
            return;
        }

        if (DataContext is ModeloTelaPrincipal vm)
        {
            // Atalho Ctrl+K: Pesquisa Global
            if (e.Key == Key.K && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                vm.PopupPesquisaAberto = true;
                CampoPesquisaGlobal.Focus();
                e.Handled = true;
            }
            // Atalho Esc: Fechar Pesquisa Global
            else if (e.Key == Key.Escape && vm.PopupPesquisaAberto)
            {
                vm.FecharPesquisaGlobalCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void BotaoAlternarTelaCheia_Click(object sender, RoutedEventArgs e)
    {
        AlternarTelaCheia();
    }

    private void AlternarTelaCheia()
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void BotaoAlternarTema_Click(object sender, RoutedEventArgs e)
    {
        ServicoTema.AlternarProximoTema();
    }

    private void Cabecalho_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            AlternarTelaCheia();
        }
    }
}
