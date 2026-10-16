# Arquitetura de Software — BYTEVAULT

O **BYTEVAULT** é um aplicativo desktop nativo para Windows construído para centralizar conhecimentos, trechos de código, comandos de CLI, anotações e histórico de bugs.

## 1. Princípios Arquiteturais
- **100% Nativo**: Construído com .NET 10 LTS e Windows Presentation Foundation (WPF). Sem Electron ou WebViews embutidos.
- **Separação em Camadas**:
  - `ByteVault.Dominio`: Entidades POCO, enums, interfaces de repositórios e regras de negócio sem acoplamento a frameworks visuais.
  - `ByteVault.Aplicacao`: Casos de uso, DTOs, interfaces e serviços de orquestração.
  - `ByteVault.Infraestrutura`: Persistência via Entity Framework Core, provedor SQLite, migrações versionadas, repositórios concretos e gerenciamento de pastas locais.
  - `ByteVault.Apresentacao`: UI em WPF com padrão MVVM (`CommunityToolkit.Mvvm`), Injeção de Dependência (`Microsoft.Extensions.Hosting`) e Serilog para logging seguro em arquivos.
- **Nomenclatura 100% em Português Brasileiro**: Classes, métodos, propriedades, tabelas e identificadores técnicos consistentes em português.

## 2. Injeção de Dependência e Ciclo de Vida
A inicialização ocorre através de um `IHost` em `App.xaml.cs`:
- Serviços de Infraestrutura e DbContext registrados com escopo seguro.
- ViewModels registrados no contêiner com injeção por construtor.
- Navegação entre as 9 seções do aplicativo com reutilização de instâncias, eliminando recriações desnecessárias.

## 3. Pesquisa Global e Atalhos
- Atalho `Ctrl+K`: Abre a pesquisa rápida global unificada entre Projetos, Snippets, Comandos, Anotações, Bugs e Tags.
- Operações assíncronas com `CancellationToken` (debounce) para fluidez na UI thread.
