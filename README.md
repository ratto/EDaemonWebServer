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
- `Models/` → Definições de entidades e DTOs.
- `Services/` → Lógica de negócio e regras de aplicação.
- `Program.cs` → Configuração inicial do servidor e pipeline.

## ⚙️ Como Executar
1. Clone o repositório:
   ```bash
   git clone https://github.com/ratto/EDaemonWebServer.git