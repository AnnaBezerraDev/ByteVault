# Guia de Desenvolvimento — BYTEVAULT

Este guia contém as orientações técnicas para compilação, arquitetura e extensão do **BYTEVAULT**.

## 1. Pré-requisitos
- .NET 10 SDK (ou fallback .NET 8 LTS)
- Windows 10 ou 11
- Visual Studio 2022 / JetBrains Rider / VS Code com C# Dev Kit

## 2. Comandos Principais

### Compilação da Solução:
```bash
dotnet build ByteVault.sln
```

### Execução da Suíte de Testes Automatizados:
```bash
dotnet test
```

### Executar o Aplicativo:
```bash
dotnet run --project src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj
```

### Gerenciar Migrações do Banco de Dados:
```bash
# Adicionar nova migração
dotnet ef migrations add NomeDaMigracao -p src/ByteVault.Infraestrutura -s src/ByteVault.Infraestrutura

# Aplicar migrações ao banco local
dotnet ef database update -p src/ByteVault.Infraestrutura -s src/ByteVault.Infraestrutura
```

## 3. Padrões de Código
- **100% Português Brasileiro**: Classes, métodos, propriedades, enums e tabelas devem seguir rigorosamente o português sem acentuação em identificadores (`ServicoProjeto`, `ObterPorId`, `DataCriacao`).
- **MVVM Limpo**: Uso de `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`).
- **Desacoplamento**: Camada de Domínio sem referências a WPF ou EF Core.
