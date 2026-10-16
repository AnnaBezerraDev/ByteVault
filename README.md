# BYTEVAULT

**BYTEVAULT** é um ambiente desktop nativo de organização e produtividade pessoal para desenvolvedores e estudantes de Engenharia de Software.

Centralize conhecimento técnico, trechos de código (snippets), comandos de terminal, projetos, anotações em Markdown e seu diário de bugs com persistência local confiável, pesquisa global instantânea (Ctrl+K) e privacidade por padrão.

---

## 🚀 Tecnologias Utilizadas
- **Linguagem**: C# 13
- **Framework**: .NET 10 LTS
- **Interface**: WPF (Windows Presentation Foundation) & XAML
- **Arquitetura**: Camadas com padrão MVVM (`CommunityToolkit.Mvvm`) e Injeção de Dependência (`Microsoft.Extensions.Hosting`)
- **Banco de Dados**: SQLite & Entity Framework Core 10 com Migrações Versionadas
- **Logging**: Serilog estruturado com rotação de arquivos
- **Testes**: xUnit para testes unitários e de integração com SQLite em memória

---

## 📁 Estrutura da Solução
```
BYTEVAULT/
├── ByteVault.sln
├── src/
│   ├── ByteVault.Dominio/         # Entidades, Enums, Interfaces em Português
│   ├── ByteVault.Aplicacao/       # Casos de uso, DTOs, Serviços e Validações
│   ├── ByteVault.Infraestrutura/  # EF Core, SQLite, Migrações, Repositórios
│   └── ByteVault.Apresentacao/    # WPF, XAML, MVVM, Telas e Design System
├── tests/
│   ├── ByteVault.TestesUnitarios/ # Testes de validação e regras de negócio
│   └── ByteVault.TestesIntegracao/# Testes de persistência com SQLite isolado
└── docs/                          # Documentação detalhada
```

---

## 🛠️ Como Executar
1. Certifique-se de possuir o **.NET 10 SDK** instalado:
   ```bash
   dotnet --version
   ```
2. Restaure e compile a solução:
   ```bash
   dotnet build ByteVault.sln
   ```
3. Execute os testes automatizados:
   ```bash
   dotnet test
   ```
4. Inicie o aplicativo:
   ```bash
   dotnet run --project src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj
   ```
