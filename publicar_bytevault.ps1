# Script de Publicação Release para BYTEVAULT Windows x64
param(
    [string]$Configuracao = "Release",
    [string]$DiretorioSaida = "dist/ByteVault"
)

Write-Host "Iniciando compilação de publicação do BYTEVAULT ($Configuracao)..." -ForegroundColor Cyan

dotnet publish src/ByteVault.Apresentacao/ByteVault.Apresentacao.csproj `
    -c $Configuracao `
    -r win-x64 `
    --self-contained false `
    -o $DiretorioSaida

if ($LASTEXITCODE -eq 0) {
    Write-Host "Publicação concluída com sucesso no diretório: $DiretorioSaida" -ForegroundColor Green
    Write-Host "Executável gerado: $DiretorioSaida/ByteVault.Apresentacao.exe" -ForegroundColor Green
} else {
    Write-Host "Erro durante a publicação do BYTEVAULT." -ForegroundColor Red
}
