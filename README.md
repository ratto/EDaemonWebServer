# e-Daemon API WebServer

Este projeto é um **API WebServer** desenvolvido em **C#.NET** utilizando o **Visual Studio 2022**.  
O objetivo é fornecer uma arquitetura robusta e escalável para serviços de backend, permitindo integração com aplicações clientes e sistemas distribuídos.

## 🚀 Tecnologias Utilizadas
- **.NET 6/7**: Framework moderno da Microsoft para desenvolvimento multiplataforma.
- **ASP.NET Core Web API**: Estrutura para criação de APIs RESTful de alto desempenho.
- **Visual Studio 2022**: Ambiente de desenvolvimento integrado (IDE) utilizado para criação, depuração e publicação.
- **Entity Framework Core** (opcional): ORM para acesso e manipulação de dados relacionais.
- **Dependency Injection**: Padrão nativo do ASP.NET Core para modularidade e testabilidade.
- **Middleware Pipeline**: Configuração flexível para autenticação, logging e tratamento de requisições.

## 📂 Estrutura do Projeto
- `Controllers/` → Endpoints da API.
- `Domain/` → Definições de entidades e DTOs.
- `Services/` → Lógica de negócio e regras de aplicação.
- `Program.cs` → Configuração inicial do servidor e pipeline.

## ⚙️ Como Executar
1. Clone o repositório:
   ```bash
   git clone https://github.com/ratto/EDaemonWebServer.git
   
------------

# e‑Daemon API WebServer

Projeto API WebServer em C# targeting .NET 8 (C# 12). Fornece uma arquitetura em camadas simples e testável para expor endpoints REST relacionados a "skills" (habilidades).

## Tecnologias
- .NET 8, C# 12
- ASP.NET Core Web API
- xUnit + Moq (testes unitários)
- Swagger (Swashbuckle)
- Arquitetura em camadas: Controllers → Services → Repositories → Domain
- Licença: GPL v2 (veja `LICENSE.txt`)

## Visão geral da arquitetura
- Controllers: expõem endpoints HTTP (ex.: `EDaemonWebServer\Controllers\SkillController.cs`).
- Services: regras de negócio e orquestração (ex.: `EDaemonWebServer\Services\SkillService.cs`).
- Repositories: acesso a dados; abstraído por interfaces para facilitar testes (ex.: `EDaemonWebServer\Repositories\Interfaces\ISkillRepository.cs` e `EDaemonWebServer\Repositories\SkillRepository.cs`).
- Domain: entidades e DTOs (ex.: `EDaemonWebServer\Domain\Skills`).
- Utils: enums e utilitários (ex.: `EDaemonWebServer\Utils\Enums\AttributeType.cs`).
- Tests: projetos de teste unitário que usam estilo "London" (mocks do repositório/serviço).

Observação importante: o arquivo `EDaemonWebServer\Repositories\SkillRepository.cs` atualmente contém métodos com `NotImplementedException`. Implemente-o usando um `DbContext` do EF Core ou um adaptador de persistência adequado.

## Estrutura de pastas (resumo)
- `EDaemonWebServer/` — projeto Web API
  - `Controllers/`
  - `Services/`
  - `Repositories/`
  - `Domain/`
  - `Utils/`
- `EDaemonWebServerTests/` — testes unitários

## Endpoints principais
- GET `/api/skill/basic-skills` — lista de basic skills (filtragem por `BasicSkillsFilter`)
- GET `/api/skill/basic-skills/{id}` — obtém skill por id

## Registro de dependências (exemplo)
Adicione as implementações concretas ao contêiner DI em `Program.cs`:
