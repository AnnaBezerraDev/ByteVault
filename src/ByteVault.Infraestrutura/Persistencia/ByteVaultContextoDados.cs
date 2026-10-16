using System;
using ByteVault.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ByteVault.Infraestrutura.Persistencia;

public class ByteVaultContextoDados : DbContext
{
    public ByteVaultContextoDados(DbContextOptions<ByteVaultContextoDados> options)
        : base(options)
    {
    }

    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<TrechoCodigo> TrechosCodigo => Set<TrechoCodigo>();
    public DbSet<RegistroComando> Comandos => Set<RegistroComando>();
    public DbSet<Anotacao> Anotacoes => Set<Anotacao>();
    public DbSet<RelatorioBug> Bugs => Set<RelatorioBug>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TecnologiaProjeto> TecnologiasProjetos => Set<TecnologiaProjeto>();
    public DbSet<TagProjeto> TagsProjetos => Set<TagProjeto>();
    public DbSet<TagSnippet> TagsSnippets => Set<TagSnippet>();
    public DbSet<TagComando> TagsComandos => Set<TagComando>();
    public DbSet<TagAnotacao> TagsAnotacoes => Set<TagAnotacao>();
    public DbSet<TagBug> TagsBugs => Set<TagBug>();
    public DbSet<RelacaoBugSnippet> RelacoesBugSnippet => Set<RelacaoBugSnippet>();
    public DbSet<ConfiguracaoAplicacao> Configuracoes => Set<ConfiguracaoAplicacao>();
    public DbSet<RegistroAtividade> Atividades => Set<RegistroAtividade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Projeto
        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.ToTable("Projetos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nome).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Descricao).HasMaxLength(2000);
            entity.Property(p => p.CaminhoLocal).HasMaxLength(1000);
            entity.Property(p => p.UrlRepositorio).HasMaxLength(1000);
            entity.Property(p => p.UrlDocumentacao).HasMaxLength(1000);
            entity.HasIndex(p => p.Nome);
            entity.HasIndex(p => p.Status);
            entity.HasIndex(p => p.EhFavorito);
            entity.HasIndex(p => p.DataAtualizacao);
        });

        // Configuração de TecnologiaProjeto
        modelBuilder.Entity<TecnologiaProjeto>(entity =>
        {
            entity.ToTable("TecnologiasProjetos");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Nome).IsRequired().HasMaxLength(100);
            entity.HasOne(t => t.Projeto)
                .WithMany(p => p.Tecnologias)
                .HasForeignKey(t => t.ProjetoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração de TrechoCodigo (Snippet)
        modelBuilder.Entity<TrechoCodigo>(entity =>
        {
            entity.ToTable("TrechosCodigo");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Codigo).IsRequired();
            entity.Property(t => t.Linguagem).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Categoria).HasMaxLength(100);
            entity.HasIndex(t => t.Titulo);
            entity.HasIndex(t => t.Linguagem);
            entity.HasIndex(t => t.EhFavorito);
            entity.HasIndex(t => t.DataAtualizacao);

            // Prevenção de perda acidental: ao excluir projeto, snippet tem ProjetoId setado como nulo
            entity.HasOne(t => t.Projeto)
                .WithMany(p => p.TrechosCodigo)
                .HasForeignKey(t => t.ProjetoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração de RegistroComando
        modelBuilder.Entity<RegistroComando>(entity =>
        {
            entity.ToTable("Comandos");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(c => c.TextoComando).IsRequired().HasMaxLength(4000);
            entity.Property(c => c.FerramentaOuAmbiente).IsRequired().HasMaxLength(100);
            entity.Property(c => c.SistemaOperacional).HasMaxLength(100);
            entity.HasIndex(c => c.Titulo);
            entity.HasIndex(c => c.FerramentaOuAmbiente);
            entity.HasIndex(c => c.EhFavorito);

            entity.HasOne(c => c.Projeto)
                .WithMany(p => p.Comandos)
                .HasForeignKey(c => c.ProjetoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração de Anotacao
        modelBuilder.Entity<Anotacao>(entity =>
        {
            entity.ToTable("Anotacoes");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Titulo).IsRequired().HasMaxLength(250);
            entity.Property(a => a.Conteudo).IsRequired();
            entity.Property(a => a.Categoria).HasMaxLength(100);
            entity.HasIndex(a => a.Titulo);
            entity.HasIndex(a => a.Categoria);
            entity.HasIndex(a => a.EhFavorito);

            entity.HasOne(a => a.Projeto)
                .WithMany(p => p.Anotacoes)
                .HasForeignKey(a => a.ProjetoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração de RelatorioBug
        modelBuilder.Entity<RelatorioBug>(entity =>
        {
            entity.ToTable("Bugs");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Titulo).IsRequired().HasMaxLength(250);
            entity.Property(b => b.Status).IsRequired();
            entity.Property(b => b.Severidade).IsRequired();
            entity.Property(b => b.TecnologiaRelacionada).HasMaxLength(150);
            entity.HasIndex(b => b.Titulo);
            entity.HasIndex(b => b.Status);
            entity.HasIndex(b => b.Severidade);
            entity.HasIndex(b => b.EhFavorito);

            entity.HasOne(b => b.Projeto)
                .WithMany(p => p.Bugs)
                .HasForeignKey(b => b.ProjetoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração de Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Nome).IsRequired().HasMaxLength(100);
            entity.Property(t => t.CorHex).IsRequired().HasMaxLength(10);
            entity.HasIndex(t => t.Nome).IsUnique();
        });

        // Junções N:N com Tags
        modelBuilder.Entity<TagProjeto>(entity =>
        {
            entity.ToTable("TagProjeto");
            entity.HasKey(tp => new { tp.ProjetoId, tp.TagId });
            entity.HasOne(tp => tp.Projeto)
                .WithMany(p => p.Tags)
                .HasForeignKey(tp => tp.ProjetoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(tp => tp.Tag)
                .WithMany(t => t.TagsProjetos)
                .HasForeignKey(tp => tp.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TagSnippet>(entity =>
        {
            entity.ToTable("TagSnippet");
            entity.HasKey(ts => new { ts.TrechoCodigoId, ts.TagId });
            entity.HasOne(ts => ts.TrechoCodigo)
                .WithMany(s => s.Tags)
                .HasForeignKey(ts => ts.TrechoCodigoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ts => ts.Tag)
                .WithMany(t => t.TagsSnippets)
                .HasForeignKey(ts => ts.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TagComando>(entity =>
        {
            entity.ToTable("TagComando");
            entity.HasKey(tc => new { tc.RegistroComandoId, tc.TagId });
            entity.HasOne(tc => tc.RegistroComando)
                .WithMany(c => c.Tags)
                .HasForeignKey(tc => tc.RegistroComandoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(tc => tc.Tag)
                .WithMany(t => t.TagsComandos)
                .HasForeignKey(tc => tc.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TagAnotacao>(entity =>
        {
            entity.ToTable("TagAnotacao");
            entity.HasKey(ta => new { ta.AnotacaoId, ta.TagId });
            entity.HasOne(ta => ta.Anotacao)
                .WithMany(a => a.Tags)
                .HasForeignKey(ta => ta.AnotacaoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ta => ta.Tag)
                .WithMany(t => t.TagsAnotacoes)
                .HasForeignKey(ta => ta.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TagBug>(entity =>
        {
            entity.ToTable("TagBug");
            entity.HasKey(tb => new { tb.RelatorioBugId, tb.TagId });
            entity.HasOne(tb => tb.RelatorioBug)
                .WithMany(b => b.Tags)
                .HasForeignKey(tb => tb.RelatorioBugId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(tb => tb.Tag)
                .WithMany(t => t.TagsBugs)
                .HasForeignKey(tb => tb.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Junção Bug <-> Snippet
        modelBuilder.Entity<RelacaoBugSnippet>(entity =>
        {
            entity.ToTable("RelacaoBugSnippet");
            entity.HasKey(r => new { r.BugId, r.TrechoCodigoId });
            entity.HasOne(r => r.Bug)
                .WithMany(b => b.SnippetsSolucao)
                .HasForeignKey(r => r.BugId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.TrechoCodigo)
                .WithMany(s => s.BugsRelacionados)
                .HasForeignKey(r => r.TrechoCodigoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração de ConfiguracaoAplicacao
        modelBuilder.Entity<ConfiguracaoAplicacao>(entity =>
        {
            entity.ToTable("Configuracoes");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.Tema).IsRequired().HasMaxLength(50);
            entity.Property(c => c.CorDestaque).IsRequired().HasMaxLength(20);
        });

        // Configuração de RegistroAtividade
        modelBuilder.Entity<RegistroAtividade>(entity =>
        {
            entity.ToTable("RegistroAtividades");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Titulo).IsRequired().HasMaxLength(250);
            entity.Property(a => a.Categoria).IsRequired().HasMaxLength(100);
            entity.Property(a => a.TipoAcao).IsRequired().HasMaxLength(50);
            entity.HasIndex(a => a.DataRegistro);
        });
    }
}
