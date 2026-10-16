using System;
using ByteVault.Dominio.Entidades;
using ByteVault.Dominio.Enumeradores;
using Xunit;

namespace ByteVault.TestesUnitarios;

public class TestesEntidades
{
    [Fact]
    public void Projeto_DeveInicializarComValoresPadraoCorretos()
    {
        var projeto = new Projeto();

        Assert.NotEqual(Guid.Empty, projeto.Id);
        Assert.Equal(StatusProjeto.Planejado, projeto.Status);
        Assert.False(projeto.EhFavorito);
        Assert.NotNull(projeto.Tags);
        Assert.NotNull(projeto.Tecnologias);
    }

    [Fact]
    public void Tag_DevePossuirCorHexPadraoEListasInicializadas()
    {
        var tag = new Tag { Nome = "CSharp" };

        Assert.Equal("#6C8CFF", tag.CorHex);
        Assert.NotNull(tag.TagsProjetos);
        Assert.NotNull(tag.TagsSnippets);
        Assert.NotNull(tag.TagsComandos);
        Assert.NotNull(tag.TagsAnotacoes);
        Assert.NotNull(tag.TagsBugs);
    }

    [Fact]
    public void Bug_AoSerInstanciado_DevePossuirStatusAbertoESeveridadeMedia()
    {
        var bug = new RelatorioBug();

        Assert.Equal(StatusBug.Aberto, bug.Status);
        Assert.Equal(SeveridadeBug.Media, bug.Severidade);
        Assert.Null(bug.DataResolucao);
    }
}
