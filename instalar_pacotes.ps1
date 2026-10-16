Set-Location "C:\Users\Anna Luiza\.gemini\antigravity-ide\scratch\ByteVault"

# Infraestrutura
dotnet add src/ByteVault.Infraestrutura/ByteVault.Infraestrutura.csproj package Microsoft.EntityFrameworkCore.Design

# Aplicacao
dotnet add src/ByteVault.Aplicacao/ByteVault.Aplicacao.csproj package CommunityToolkit.Mvvm
dotnet add src/ByteVault.Aplicacao/ByteVault.Aplicacao.csproj package Microsoft.Extensions.DependencyInjection.Abstractions

# Apresentacao
dotnet add src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj package CommunityToolkit.Mvvm
dotnet add src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj package Microsoft.Extensions.Hosting
dotnet add src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj package Serilog
dotnet add src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj package Serilog.Extensions.Hosting
dotnet add src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj package Serilog.Sinks.File

Write-Output "Pacotes instalados com sucesso."
