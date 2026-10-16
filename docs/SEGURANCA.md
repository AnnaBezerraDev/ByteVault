# Relatório de Arquitetura de Segurança — BYTEVAULT

O BYTEVAULT adota o princípio de **Privacidade e Segurança por Padrão** (*Security by Design*).

## 1. Armazenamento Local e Telemetria Zero
- Todos os dados permanecem 100% locais na máquina do desenvolvedor em `%LOCALAPPDATA%\BYTEVAULT\Dados\bytevault.db`.
- O software não envia métricas, rastreadores, nem telemetria externa.
- Não existem telas de autenticação fictícias ou falsos logins online.

## 2. Proteção contra Execução Arbitrária de Comandos
- **Regra Estrita**: Os comandos de terminal (PowerShell, Bash, CMD) são armazenados e manipulados estritamente como texto.
- O software jamais invoca processos de terminal automaticamente ao clicar ou inspecionar um registro, prevenindo ataques de *Command Injection*. Apenas o recurso de cópia para a área de transferência é disponibilizado.

## 3. Prevenção de Zip Slip em Backups
- Durante a restauração de arquivos ZIP de backup, o extrator valida cada entrada contra *Directory Traversal Attack* (`..`), garantindo que nenhum arquivo seja gravado fora da pasta temporária isolada.
- O arquivo SQLite restaurado é submetido à verificação de integridade estrutural (`PRAGMA integrity_check`) antes da substituição efetiva.
- Um backup de segurança do estado atual é sempre gerado antes de qualquer substituição.

## 4. Prevenção de SQL Injection
- Todas as operações com banco de dados utilizam o Entity Framework Core com consultas 100% parametrizadas e tipadas.

## 5. Validação de URLs e Caminhos do Sistema Operacional
- Links externos são rigorosamente validados antes da inicialização para garantir esquemas seguros (`http://` e `https://`).
- Caminhos de diretórios locais são verificados previamente no sistema de arquivos antes de qualquer chamada ao Windows Explorer.
