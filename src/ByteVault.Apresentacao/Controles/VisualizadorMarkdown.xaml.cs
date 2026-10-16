using System;
using System.Windows;
using System.Windows.Controls;
using ByteVault.Apresentacao.Servicos;
using Markdig;

namespace ByteVault.Apresentacao.Controles;

public partial class VisualizadorMarkdown : UserControl
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static readonly DependencyProperty ConteudoMarkdownProperty =
        DependencyProperty.Register(
            nameof(ConteudoMarkdown),
            typeof(string),
            typeof(VisualizadorMarkdown),
            new PropertyMetadata(string.Empty, AoMudarConteudo));

    public string ConteudoMarkdown
    {
        get => (string)GetValue(ConteudoMarkdownProperty);
        set => SetValue(ConteudoMarkdownProperty, value);
    }

    public VisualizadorMarkdown()
    {
        InitializeComponent();
        Loaded += (s, e) => RenderizarHtml(ConteudoMarkdown);
        ServicoTema.TemaAlterado += _ => Dispatcher.Invoke(() => RenderizarHtml(ConteudoMarkdown));
    }

    private static void AoMudarConteudo(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VisualizadorMarkdown visualizador)
        {
            visualizador.RenderizarHtml(e.NewValue as string);
        }
    }

    private void RenderizarHtml(string? markdown)
    {
        try
        {
            var tema = ServicoTema.TemaAtual;
            string bgCor, textoCor, tituloCor, preBgCor, bordaCor, linkCor, codeCor;

            if (tema == ServicoTema.TemaClaro)
            {
                bgCor = "#FFFFFF";
                textoCor = "#151822";
                tituloCor = "#0B0D12";
                preBgCor = "#F0F2F7";
                bordaCor = "#D2D6E0";
                linkCor = "#3B65F5";
                codeCor = "#1E7E34";
            }
            else if (tema == ServicoTema.TemaMeiaNoite)
            {
                bgCor = "#000000";
                textoCor = "#FFFFFF";
                tituloCor = "#FFFFFF";
                preBgCor = "#111318";
                bordaCor = "#242834";
                linkCor = "#5B7EFA";
                codeCor = "#45D08E";
            }
            else
            {
                bgCor = "#101114";
                textoCor = "#F1F3F7";
                tituloCor = "#FFFFFF";
                preBgCor = "#1B1D23";
                bordaCor = "#30333C";
                linkCor = "#6C8CFF";
                codeCor = "#55C995";
            }

            var corpoHtml = string.IsNullOrWhiteSpace(markdown)
                ? $"<p style='color: #707684; font-style: italic;'>Nenhum conteúdo para visualização prévia.</p>"
                : Markdown.ToHtml(markdown, Pipeline);

            var htmlCompleto = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <style>
        body {{
            background-color: {bgCor};
            color: {textoCor};
            font-family: 'Segoe UI', sans-serif;
            font-size: 13px;
            line-height: 1.6;
            margin: 16px;
            padding: 0;
        }}
        h1, h2, h3, h4 {{
            color: {tituloCor};
            margin-top: 16px;
            margin-bottom: 8px;
            font-weight: 600;
        }}
        h1 {{ font-size: 20px; border-bottom: 1px solid {bordaCor}; padding-bottom: 6px; }}
        h2 {{ font-size: 16px; }}
        h3 {{ font-size: 14px; }}
        p {{ margin: 0 0 10px 0; }}
        a {{ color: {linkCor}; text-decoration: none; }}
        a:hover {{ text-decoration: underline; }}
        pre {{
            background-color: {preBgCor};
            border: 1px solid {bordaCor};
            border-radius: 6px;
            padding: 12px;
            overflow-x: auto;
            font-family: 'Cascadia Code', Consolas, monospace;
            font-size: 12px;
        }}
        code {{
            background-color: {preBgCor};
            color: {codeCor};
            padding: 2px 6px;
            border-radius: 4px;
            font-family: 'Cascadia Code', Consolas, monospace;
            font-size: 12px;
        }}
        pre code {{
            background-color: transparent;
            color: {textoCor};
            padding: 0;
        }}
        blockquote {{
            border-left: 3px solid {linkCor};
            margin: 0;
            padding-left: 12px;
            color: #A0A5B1;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 12px 0;
        }}
        th, td {{
            border: 1px solid {bordaCor};
            padding: 8px 12px;
            text-align: left;
        }}
        th {{
            background-color: {preBgCor};
            font-weight: 600;
        }}
        ul, ol {{
            padding-left: 20px;
            margin: 0 0 10px 0;
        }}
        li {{
            margin-bottom: 4px;
        }}
    </style>
</head>
</body>
</html>";

            NavegadorInterno.NavigateToString(htmlCompleto);
        }
        catch
        {
            // Fallback silencioso seguro
        }
    }
}
