# Modelo de Dados e Persistência — BYTEVAULT

O BYTEVAULT utiliza **SQLite** com **Entity Framework Core 10** como banco de dados embutido e local.

## 1. Localização e Diretórios
Os arquivos são armazenados no diretório de dados local do usuário:
- Base: `%LOCALAPPDATA%\BYTEVAULT\`
  - `Dados\bytevault.db`: Arquivo SQLite principal
  - `Backups\`: Pacotes de backup e restauração
  - `Logs\`: Registros de execução com Serilog
  - `Configuracoes\`: Preferências persistidas

## 2. Entidades e Tabelas
1. **Projetos**: Repositórios e projetos de software do desenvolvedor.
2. **TrechosCodigo (Snippets)**: Códigos reutilizáveis em diversas linguagens.
3. **Comandos**: Comandos de CLI, ferramentas e sistemas operacionais.
4. **Anotacoes**: Documentação técnica em Markdown.
5. **Bugs**: Diário de bugs com log do erro, solução e severidade.
6. **Tags**: Classificação transversal com normalização case-insensitive.
7. **Tabelas de Junção**: `TagProjeto`, `TagSnippet`, `TagComando`, `TagAnotacao`, `TagBug`, `RelacaoBugSnippet`.
8. **Configuracoes**: Preferências visuais e comportamentais.
9. **RegistroAtividades**: Histórico de ações recentes para o painel inicial.

## 3. Integridade e Prevenção de Perda de Dados
- Chaves estrangeiras com deleções controladas (`SetNull` para código e snippets caso o projeto seja excluído, garantindo que o conhecimento nunca seja acidentalmente perdido).
- Remoção de tags elimina apenas os vínculos na tabela associativa, nunca os registros de projetos ou snippets.
