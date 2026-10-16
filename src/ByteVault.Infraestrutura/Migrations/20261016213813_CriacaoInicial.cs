using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteVault.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuracoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Tema = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CorDestaque = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DensidadeVisual = table.Column<string>(type: "TEXT", nullable: false),
                    ComportamentoAoIniciar = table.Column<string>(type: "TEXT", nullable: false),
                    ConfirmarExclusao = table.Column<bool>(type: "INTEGER", nullable: false),
                    SalvarAutomaticamente = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataUltimoBackup = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CaminhoBanco = table.Column<string>(type: "TEXT", nullable: true),
                    VersaoEsquema = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projetos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LinguagensTecnologias = table.Column<string>(type: "TEXT", nullable: true),
                    CaminhoLocal = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UrlRepositorio = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UrlDocumentacao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    EhFavorito = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataUltimaAbertura = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projetos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistroAtividades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TipoAcao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RegistroId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DataRegistro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAtividades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CorHex = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Anotacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Conteudo = table.Column<string>(type: "TEXT", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ProjetoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EhFavorito = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anotacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anotacoes_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    MensagemErroOriginal = table.Column<string>(type: "TEXT", nullable: true),
                    TecnologiaRelacionada = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Ambiente = table.Column<string>(type: "TEXT", nullable: true),
                    CodigoRelevante = table.Column<string>(type: "TEXT", nullable: true),
                    Solucao = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Severidade = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjetoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EhFavorito = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataResolucao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bugs_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Comandos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    TextoComando = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    FerramentaOuAmbiente = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SistemaOperacional = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Parametros = table.Column<string>(type: "TEXT", nullable: true),
                    ExemploUtilizacao = table.Column<string>(type: "TEXT", nullable: true),
                    ProjetoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EhFavorito = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comandos_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TecnologiasProjetos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjetoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TecnologiasProjetos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TecnologiasProjetos_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrechosCodigo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Linguagem = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ProjetoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EhFavorito = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrechosCodigo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrechosCodigo_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TagProjeto",
                columns: table => new
                {
                    ProjetoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagProjeto", x => new { x.ProjetoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagProjeto_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagProjeto_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagAnotacao",
                columns: table => new
                {
                    AnotacaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAnotacao", x => new { x.AnotacaoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagAnotacao_Anotacoes_AnotacaoId",
                        column: x => x.AnotacaoId,
                        principalTable: "Anotacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagAnotacao_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagBug",
                columns: table => new
                {
                    RelatorioBugId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagBug", x => new { x.RelatorioBugId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagBug_Bugs_RelatorioBugId",
                        column: x => x.RelatorioBugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagBug_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagComando",
                columns: table => new
                {
                    RegistroComandoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagComando", x => new { x.RegistroComandoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagComando_Comandos_RegistroComandoId",
                        column: x => x.RegistroComandoId,
                        principalTable: "Comandos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagComando_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelacaoBugSnippet",
                columns: table => new
                {
                    BugId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TrechoCodigoId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelacaoBugSnippet", x => new { x.BugId, x.TrechoCodigoId });
                    table.ForeignKey(
                        name: "FK_RelacaoBugSnippet_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelacaoBugSnippet_TrechosCodigo_TrechoCodigoId",
                        column: x => x.TrechoCodigoId,
                        principalTable: "TrechosCodigo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagSnippet",
                columns: table => new
                {
                    TrechoCodigoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagSnippet", x => new { x.TrechoCodigoId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagSnippet_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagSnippet_TrechosCodigo_TrechoCodigoId",
                        column: x => x.TrechoCodigoId,
                        principalTable: "TrechosCodigo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anotacoes_Categoria",
                table: "Anotacoes",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Anotacoes_EhFavorito",
                table: "Anotacoes",
                column: "EhFavorito");

            migrationBuilder.CreateIndex(
                name: "IX_Anotacoes_ProjetoId",
                table: "Anotacoes",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Anotacoes_Titulo",
                table: "Anotacoes",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_EhFavorito",
                table: "Bugs",
                column: "EhFavorito");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_ProjetoId",
                table: "Bugs",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_Severidade",
                table: "Bugs",
                column: "Severidade");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_Status",
                table: "Bugs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_Titulo",
                table: "Bugs",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Comandos_EhFavorito",
                table: "Comandos",
                column: "EhFavorito");

            migrationBuilder.CreateIndex(
                name: "IX_Comandos_FerramentaOuAmbiente",
                table: "Comandos",
                column: "FerramentaOuAmbiente");

            migrationBuilder.CreateIndex(
                name: "IX_Comandos_ProjetoId",
                table: "Comandos",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comandos_Titulo",
                table: "Comandos",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_DataAtualizacao",
                table: "Projetos",
                column: "DataAtualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_EhFavorito",
                table: "Projetos",
                column: "EhFavorito");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_Nome",
                table: "Projetos",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_Status",
                table: "Projetos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAtividades_DataRegistro",
                table: "RegistroAtividades",
                column: "DataRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_RelacaoBugSnippet_TrechoCodigoId",
                table: "RelacaoBugSnippet",
                column: "TrechoCodigoId");

            migrationBuilder.CreateIndex(
                name: "IX_TagAnotacao_TagId",
                table: "TagAnotacao",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagBug_TagId",
                table: "TagBug",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagComando_TagId",
                table: "TagComando",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagProjeto_TagId",
                table: "TagProjeto",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Nome",
                table: "Tags",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagSnippet_TagId",
                table: "TagSnippet",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TecnologiasProjetos_ProjetoId",
                table: "TecnologiasProjetos",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_TrechosCodigo_DataAtualizacao",
                table: "TrechosCodigo",
                column: "DataAtualizacao");

            migrationBuilder.CreateIndex(
                name: "IX_TrechosCodigo_EhFavorito",
                table: "TrechosCodigo",
                column: "EhFavorito");

            migrationBuilder.CreateIndex(
                name: "IX_TrechosCodigo_Linguagem",
                table: "TrechosCodigo",
                column: "Linguagem");

            migrationBuilder.CreateIndex(
                name: "IX_TrechosCodigo_ProjetoId",
                table: "TrechosCodigo",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_TrechosCodigo_Titulo",
                table: "TrechosCodigo",
                column: "Titulo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuracoes");

            migrationBuilder.DropTable(
                name: "RegistroAtividades");

            migrationBuilder.DropTable(
                name: "RelacaoBugSnippet");

            migrationBuilder.DropTable(
                name: "TagAnotacao");

            migrationBuilder.DropTable(
                name: "TagBug");

            migrationBuilder.DropTable(
                name: "TagComando");

            migrationBuilder.DropTable(
                name: "TagProjeto");

            migrationBuilder.DropTable(
                name: "TagSnippet");

            migrationBuilder.DropTable(
                name: "TecnologiasProjetos");

            migrationBuilder.DropTable(
                name: "Anotacoes");

            migrationBuilder.DropTable(
                name: "Bugs");

            migrationBuilder.DropTable(
                name: "Comandos");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "TrechosCodigo");

            migrationBuilder.DropTable(
                name: "Projetos");
        }
    }
}
