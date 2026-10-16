using System;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ByteVault.Apresentacao.Controles;

public partial class EditorCodigoAvalon : UserControl
{
    private bool _atualizandoInternamente;

    public static readonly DependencyProperty TextoProperty =
        DependencyProperty.Register(
            nameof(Texto),
            typeof(string),
            typeof(EditorCodigoAvalon),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AoMudarTexto));

    public static readonly DependencyProperty LinguagemProperty =
        DependencyProperty.Register(
            nameof(Linguagem),
            typeof(string),
            typeof(EditorCodigoAvalon),
            new PropertyMetadata("C#", AoMudarLinguagem));

    public static readonly DependencyProperty SomenteLeituraProperty =
        DependencyProperty.Register(
            nameof(SomenteLeitura),
            typeof(bool),
            typeof(EditorCodigoAvalon),
            new PropertyMetadata(false, AoMudarSomenteLeitura));

    public string Texto
    {
        get => (string)GetValue(TextoProperty);
        set => SetValue(TextoProperty, value);
    }

    public string Linguagem
    {
        get => (string)GetValue(LinguagemProperty);
        set => SetValue(LinguagemProperty, value);
    }

    public bool SomenteLeitura
    {
        get => (bool)GetValue(SomenteLeituraProperty);
        set => SetValue(SomenteLeituraProperty, value);
    }

    public EditorCodigoAvalon()
    {
        InitializeComponent();
        EditorInterno.TextChanged += (s, e) =>
        {
            if (!_atualizandoInternamente)
            {
                _atualizandoInternamente = true;
                Texto = EditorInterno.Text;
                _atualizandoInternamente = false;
            }
        };

        AtualizarRealceSintaxe(Linguagem);
    }

    private static void AoMudarTexto(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EditorCodigoAvalon controle && !controle._atualizandoInternamente)
        {
            controle._atualizandoInternamente = true;
            controle.EditorInterno.Text = e.NewValue as string ?? string.Empty;
            controle._atualizandoInternamente = false;
        }
    }

    private static void AoMudarLinguagem(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EditorCodigoAvalon controle)
        {
            controle.AtualizarRealceSintaxe(e.NewValue as string);
        }
    }

    private static void AoMudarSomenteLeitura(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EditorCodigoAvalon controle && e.NewValue is bool b)
        {
            controle.EditorInterno.IsReadOnly = b;
        }
    }

    private void AtualizarRealceSintaxe(string? linguagem)
    {
        if (string.IsNullOrWhiteSpace(linguagem))
        {
            EditorInterno.SyntaxHighlighting = null;
            return;
        }

        var nomeLinguagem = linguagem.Trim().ToLower() switch
        {
            "c#" or "csharp" or "cs" => "C#",
            "javascript" or "js" => "JavaScript",
            "typescript" or "ts" => "JavaScript",
            "html" or "htm" => "HTML",
            "xml" or "xaml" => "XML",
            "php" => "PHP",
            "java" => "Java",
            "c++" or "cpp" or "c" => "C++",
            "python" or "py" => "Python",
            _ => null
        };

        if (nomeLinguagem != null)
        {
            EditorInterno.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(nomeLinguagem);
        }
        else
        {
            EditorInterno.SyntaxHighlighting = null;
        }
    }
}
