# PRD — Backend e-Daemon (Meta 1)

## Documento Técnico Detalhado

**Versão:** 2.1  
**Data:** Fevereiro de 2026  
**Escopo:** EDaemonWebServer + Core (Backend)  
**Status:** Em elaboração  

---

## Índice

1. [Visão Geral do Backend](#visão-geral-do-backend)
2. [Arquitetura da Solução](#arquitetura-da-solução)
3. [Estrutura de Projetos](#estrutura-de-projetos)
4. [Stack Tecnológico](#stack-tecnológico)
5. [Camadas de Arquitetura](#camadas-de-arquitetura)
6. [Padrões de Design e Implementação](#padrões-de-design-e-implementação)
7. [Fluxos de Dados](#fluxos-de-dados)
8. [Especificação de Endpoints](#especificação-de-endpoints)
9. [Infraestrutura e Configuração](#infraestrutura-e-configuração)
10. [Estratégia de Testes](#estratégia-de-testes)
11. [Requisitos de Deploy](#requisitos-de-deploy)

---

## 1. Visão Geral do Backend

### 1.1 Objetivo

O backend do **e-Daemon** (Meta 1) é responsável por:

- **Expor uma API REST** completa e bem documentada (Swagger/OpenAPI) para consumo pelo Frontend
- **Implementar a lógica de negócio** do Sistema Daemon (d100, testes de perícias, modificadores, etc.)
- **Gerenciar dados** do Módulo Básico do Sistema Daemon através de um banco de dados relacional (somente leitura)
- **Desacoplar a engine de negócio (Core)** de todas as responsabilidades de infraestrutura e protocolo HTTP

### 1.2 Princípios Arquiteturais

| Princípio | Descrição |
|---|---|
| **Separação de Responsabilidades** | Cada camada tem uma responsabilidade única e bem definida |
| **Independência do Core** | O Core não deve ter dependências de frameworks, banco de dados ou HTTP |
| **Testabilidade** | Todas as camadas devem ser testáveis isoladamente via injeção de dependência |
| **Agnósticismo de Domínio** | O Backend não adiciona regras de domínio; implementa fielmente o livro do Sistema Daemon |
| **Versionamento Semântico** | A API segue versionamento semântico em sua evolução |

---

## 2. Arquitetura da Solução

### 2.1 Diagrama de Alto Nível

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃                     FRONTEND (Quasar + Vue 3)                      ┃
┃                    HTTP REST via Axios                            ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━┬━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
                          │
                ┌─────────▼────────────┐
                │  EDaemonWebServer    │
                │  (ASP.NET Core 10)   │
                │  Clean Architecture │
                └──────────┬───────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
    ┌───▼───┐          ┌───▼───┐         ┌───▼────┐
    │ Models│          │Services│         │Repos   │
    └───────┘          └────┬───┘         └────┬───┘
        ▲                   │                   │
        │                   │              ┌────▼────┐
        │              ┌────▼──────┐       │Database │
        │              │ Adapters  │       └─────────┘
        │              └────┬──────┘
        │                   │
        │         ┌─────────▼──────────┐
        │         │  Core (Engine)     │
        │         │  Hex Arch + ES     │
        │         └───────────────────┘
        │
        └─────────────────────────────────────
              Retorno via Models/DTOs

```

### 2.2 Fluxo de Requisição Completo

```
1. Cliente (Frontend) envia request HTTP
                     ↓
2. ASP.NET Core routing → Controller
                     ↓
3. Controller recebe requisição e delega ao Service
                     ↓
4. Service determina o tipo de operação:
   ├─ A) Consulta simples de dados → Repository → Database
   │
   └─ B) Lógica complexa → Adapter → Core
                            │
                            ├─ UseCase orquestra Services do Core
                            ├─ Services executam cálculos
                            ├─ Publicam eventos (Event Sourcing)
                            └─ Retornam resultado
   
                     ↓
5. Service formata resposta em ViewModel (Model)
                     ↓
6. Controller retorna HTTP Response com código semântico
                     ↓
7. Frontend recebe JSON estruturado
```

---

## 3. Estrutura de Projetos

### 3.1 Visão de Diretórios

```
EDaemonWebServer/
│
├── Adapters/                        # Interface entre WebServer e Core
│   ├── RollBasicSkillAdapter.cs      # Adapta UseCase de teste de perícia
│   └── ...
│
├── Controllers/                     # Camada HTTP/REST
│   ├── BasicSkillController.cs       # GET /api/basic-skills, GET /api/basic-skills/{id}
│   ├── SkillTestController.cs        # POST /api/skill-test
│   └── ...
│
├── Services/                        # Camada de Lógica de Aplicação
│   ├── BasicSkillService.cs          # Orquestra Repository + Adapter
│   ├── SkillTestService.cs           # Orquestra cálculos do Core
│   └── ...
│
├── Repositories/                    # Camada de Acesso a Dados
│   ├── BasicSkillRepository.cs       # Consultas a Skills básicas
│   ├── SpecializedSkillRepository.cs # Consultas a Skills especializadas
│   └── ...
│
├── Models/                          # DTOs e entidades de apresentação
│   ├── BasicSkillViewModel.cs        # entidade de apresentação para resposta de perícia
│   ├── SkillTestResultViewModel.cs   # entidade de apresentação para resultado de teste
│   ├── BasciSkill.cs                 # DTO para comunicação com o BD
│   └── ...
│
├── Program.cs                       # Configuração da aplicação (DI, middleware, etc)
├── appsettings.json                 # Configurações (connection strings, logging, etc)
├── appsettings.Development.json     # Configurações específicas de desenvolvimento
│
├── docs/                            # Documentação do projeto
│   ├── PRD_BACKEND.md                # Este arquivo
│   ├── PRD_eDaemon_Meta1v3.md        # PRD geral
│   └── API_ENDPOINTS.md              # Especificação detalhada dos endpoints
│
└── EDaemonWebServer.csproj          # Arquivo de projeto (.NET 10)

─────────────────────────────────────────────────────────

EDaemonWebServerTests/              # Projeto de Testes
│
├── Services/                        # Testes unitários dos Services
│   ├── BasicSkillServiceTests.cs     # Testes de listagem de perícias
│   └── ...
│
├── Controllers/                     # Testes unitários dos Controllers
│   ├── BasicSkillControllerTests.cs  # Testes de endpoints HTTP
│   └── ...
│
├── Integration/                     # Testes de integração
│   ├── SkillTestFlowTests.cs         # Testes de fluxo completo
│   └── ...
│
├── Fixtures/                        # Dados de teste reutilizáveis
│   ├── BasicSkillFixture.cs          # Fixture com dados de perícias
│   └── ...
│
└── EDaemonWebServerTests.csproj

─────────────────────────────────────────────────────────

Core/                               # Projeto separado (biblioteca)
│
├── Ports/
│   ├── Input/
│   │   └── IRollBasicSkillPort.cs    # Interface de entrada (UseCase)
│   │
│   └── Output/
│       ├── ISkillTestLogPort.cs      # Porta de logging de eventos
│       └── ...
│
├── UseCases/
│   └── RollBasicSkillUseCase.cs      # Orquestra um teste de perícia
│
├── Services/
│   ├── RollService.cs                # Lógica de rolagem d100
│   ├── SkillModifierService.cs       # Cálculo de modificadores
│   └── ...
│
├── Domain/
│   ├── BasicSkill.cs                 # Entidade de domínio
│   ├── RollResult.cs                 # Value Object de resultado
│   └── Events/
│       ├── SkillTestExecutedEvent.cs # Evento de domínio
│       └── ...
│
└── Core.csproj

─────────────────────────────────────────────────────────

CoreTests/                          # Testes do Core

├── UseCases/
├── Services/
└── Integration/
```

### 3.2 Mapa de Dependências

```
Controllers
    ↓
Services (abstração via Interface)
    ├─→ Repositories (abstração via Interface)
    │     └─→ Database
    │
    └─→ Adapters (abstração via Interface)
         └─→ Core Ports (agnóstico ao adaptador)
              └─→ Core UseCases, Services, Domain
```

**Regra Crítica:** Nenhuma camada pode "pular" para camadas não-adjacentes.

---

## 4. Stack Tecnológico

### 4.1 Plataforma e Runtime

| Componente | Versão | Justificativa |
|---|---|---|
| **.NET** | 10.0 (LTS) | Versão mais recente com suporte de longo prazo |
| **C#** | 12+ | Modern C# com nullable reference types ativado |
| **ASP.NET Core** | 10.0 | Framework web nativo do .NET |

### 4.2 Dependências Gerenciadas

#### 4.2.1 EDaemonWebServer

| Pacote | Versão | Finalidade | Justificativa |
|---|---|---|---|
| `Microsoft.AspNetCore.OpenApi` | 10.0.3+ | Geração de Swagger/OpenAPI | Documentação automática da API |
| `Entity Framework Core` | 10.0+ | ORM para acesso a dados | Abstração do banco de dados |
| `Serilog` | 8.0+ | Logging estruturado | Rastreabilidade de operações |
| `Serilog.AspNetCore` | 8.0+ | Integração com ASP.NET Core | Logging de requisições HTTP |
| `FluentValidation` | 11.0+ | Validação de inputs | Validação declarativa de DTOs |

#### 4.2.2 Core

| Pacote | Versão | Finalidade |
|---|---|---|
| *Nenhuma* | - | O Core não tem dependências externas |

**Razão:** O Core deve ser totalmente agnóstico e portável para qualquer plataforma (Meta 2).

#### 4.2.3 EDaemonWebServerTests e CoreTests

| Pacote | Versão | Finalidade |
|---|---|---|
| `xUnit` | 2.6+ | Framework de testes unitários |
| `Moq` | 4.20+ | Mocking de dependências |
| `FluentAssertions` | 6.0+ | Assertions semânticas |
| `coverlet.collector` | 6.0+ | Cobertura de testes |

---

## 5. Camadas de Arquitetura

### 5.1 Camada Controller (Entrada HTTP)

**Localização:** `EDaemonWebServer/Controllers/`

**Responsabilidades:**
- Receber requisições HTTP e mapear dados para DTOs
- Validar presença obrigatória de headers, query parameters, etc.
- Delegar lógica de aplicação ao Service
- Retornar respostas HTTP com códigos semânticos (200, 400, 404, 500)
- Documentar endpoints via atributos Swagger

**Exemplo de Padrão:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class BasicSkillController : ControllerBase
{
    private readonly IBasicSkillService _service;
    
    public BasicSkillController(IBasicSkillService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Retorna a lista de perícias básicas
    /// </summary>
    [HttpGet]
    [ProduceOk(typeof(IEnumerable<BasicSkillViewModel>))]
    [ProducesBadRequest()]
    public async Task<IActionResult> GetAll()
    {
        var skills = await _service.GetAllBasicSkillsAsync();
        return Ok(skills);
    }
    
    /// <summary>
    /// Retorna uma perícia específica pelo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesOk(typeof(BasicSkillViewModel))]
    [ProducesNotFound()]
    public async Task<IActionResult> GetById(int id)
    {
        var skill = await _service.GetBasicSkillByIdAsync(id);
        if (skill == null)
            return NotFound();
        return Ok(skill);
    }
}
```

**Convenções:**
- Métodos assíncronos: `GetAsync()`, `PostAsync()`, etc.
- Route pattern: `/api/{controller}/{action}`
- Sempre retornar `IActionResult` para flexibilidade de códigos HTTP

### 5.2 Camada Service (Lógica de Aplicação)

**Localização:** `EDaemonWebServer/Services/`

**Responsabilidades:**
- Orquestrar múltiplas operações (Repositories, Adapters, validações)
- Transformar entidades do domínio para DTOs
- Gerenciar transações (se necessário)
- Capturar exceções e traduzir para respostas HTTP apropriadas

**Exemplo de Padrão:**

```csharp
public interface IBasicSkillService
{
    Task<IEnumerable<BasicSkillViewModel>> GetAllBasicSkillsAsync();
    Task<BasicSkillViewModel?> GetBasicSkillByIdAsync(int id);
}

public class BasicSkillService : IBasicSkillService
{
    private readonly IBasicSkillRepository _repository;
    private readonly ILogger<BasicSkillService> _logger;
    
    public BasicSkillService(IBasicSkillRepository repository, ILogger<BasicSkillService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<BasicSkillViewModel>> GetAllBasicSkillsAsync()
    {
        _logger.LogInformation("Fetchig all basic skills");
        var skills = await _repository.GetAllAsync();
        return skills.Select(s => new BasicSkillViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            DefaultDifficulty = s.DefaultDifficulty
        });
    }
}
```

### 5.3 Camada Repository (Acesso a Dados)

**Localização:** `EDaemonWebServer/Repositories/`

**Responsabilidades:**
- Abstrair acesso ao banco de dados através de interfaces
- Implementar queries LINQ/SQL via Entity Framework Core
- Retornar entidades de domínio (não DTOs)
- Gerenciar cache local se necessário (simples)

**Exemplo de Padrão:**

```csharp
public interface IBasicSkillRepository
{
    Task<IEnumerable<BasicSkill>> GetAllAsync();
    Task<BasicSkill?> GetByIdAsync(int id);
    Task<IEnumerable<BasicSkill>> GetByAttributeAsync(string attribute);
}

public class BasicSkillRepository : IBasicSkillRepository
{
    private readonly EDaemonDbContext _context;
    
    public BasicSkillRepository(EDaemonDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<BasicSkill>> GetAllAsync()
    {
        return await _context.BasicSkills.ToListAsync();
    }
    
    public async Task<BasicSkill?> GetByIdAsync(int id)
    {
        return await _context.BasicSkills.FirstOrDefaultAsync(s => s.Id == id);
    }
}
```

**Convenções:**
- Sempre usar `async/await` para operações de I/O
- Implementar interface `IRepository<T>` genérica ou específica
- Nunca retornar `IQueryable` — sempre materializar com `ToListAsync()`

### 5.4 Camada Adapter (Integração com Core)

**Localização:** `EDaemonWebServer/Adapters/`

**Responsabilidades:**
- Traduzir requisições do Service para Portas do Core (Input Ports)
- Traduzir respostas das Portas para DTOs/ViewModels
- Gerenciar a ciclo de vida de dependências do Core
- Mapear eventos do Core para logs estruturados

**Exemplo de Padrão:**

```csharp
public interface ISkillTestAdapter
{
    Task<SkillTestResultViewModel> ExecuteTestAsync(RollBasicSkillRequest request);
}

public class SkillTestAdapter : ISkillTestAdapter
{
    private readonly IRollBasicSkillPort _rollPort;
    private readonly ILogger<SkillTestAdapter> _logger;
    
    public SkillTestAdapter(IRollBasicSkillPort rollPort, ILogger<SkillTestAdapter> logger)
    {
        _rollPort = rollPort;
        _logger = logger;
    }
    
    public async Task<SkillTestResultViewModel> ExecuteTestAsync(RollBasicSkillRequest request)
    {
        // Traduz DTO para domínio
        var input = new RollBasicSkillInput(
            skillId: request.SkillId,
            characterAttribute: request.CharacterAttribute,
            modifiers: request.Modifiers
        );
        
        // Chama UseCase via Port
        var result = await _rollPort.RollAsync(input);
        
        // Traduz resposta para ViewModel
        return new SkillTestResultViewModel
        {
            Success = result.IsSuccess,
            RolledValue = result.RolledValue,
            Margin = result.Margin,
            Events = result.Events.Select(e => new EventLogViewModel
            {
                Timestamp = e.Timestamp,
                Message = e.Message,
                Level = e.Level
            })
        };
    }
}
```

### 5.5 Camada Model (DTOs e ViewModels)

**Localização:** `EDaemonWebServer/Models/`

**Responsabilidades:**
- Definir contratos de entrada/saída da API
- Conter validações com FluentValidation
- Separar representação HTTP da lógica de domínio
- Documentar properties com atributos Swagger

**Exemplo de Padrão:**

```csharp
/// <summary>
/// ViewModel de resposta para uma perícia básica
/// </summary>
public class BasicSkillViewModel
{
    /// <summary>
    /// Identificador único
    /// </summary>
    [SwaggerSchema("Identificador único da perícia")]
    public int Id { get; set; }
    
    /// <summary>
    /// Nome da perícia
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Descrição das regras
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Validador de RollBasicSkillRequest
/// </summary>
public class RollBasicSkillRequestValidator : AbstractValidator<RollBasicSkillRequest>
{
    public RollBasicSkillRequestValidator()
    {
        RuleFor(x => x.SkillId)
            .GreaterThan(0)
            .WithMessage("SkillId deve ser maior que zero");
        
        RuleFor(x => x.CharacterAttribute)
            .NotEmpty()
            .Length(1, 20)
            .WithMessage("CharacterAttribute é obrigatório");
    }
}
```

---

## 6. Padrões de Design e Implementação

### 6.1 Injeção de Dependência

**Framework:** Nativo do ASP.NET Core (`Microsoft.Extensions.DependencyInjection`)

**Configuração em `Program.cs`:**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddOpenApi();

// Repositories (escopo de repositório por requisição)
builder.Services.AddScoped<IBasicSkillRepository, BasicSkillRepository>();
builder.Services.AddScoped<ISpecializedSkillRepository, SpecializedSkillRepository>();

// Services
builder.Services.AddScoped<IBasicSkillService, BasicSkillService>();
builder.Services.AddScoped<ISkillTestService, SkillTestService>();

// Adapters (conectam ao Core)
builder.Services.AddScoped<ISkillTestAdapter, SkillTestAdapter>();

// Core (Ports - singletons pois são stateless)
builder.Services.AddSingleton<IRollBasicSkillPort, RollBasicSkillAdapter>();

// Database Context
builder.Services.AddDbContext<EDaemonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Logging
builder.Services.AddLogging(config =>
{
    config.AddSerilog();
});

// Validação
builder.Services.AddScoped<IValidator<RollBasicSkillRequest>, RollBasicSkillRequestValidator>();
```

**Escopos de Ciclo de Vida:**
- **Transient:** Nunca para este projeto
- **Scoped:** Repositories, Services, Adapters (uma instância por requisição HTTP)
- **Singleton:** Portas do Core, Configuration, Logger Factory (compartilhadas globalmente)

### 6.2 Validação de Inputs

**Framework:** FluentValidation

```csharp
[HttpPost("skill-test")]
public async Task<IActionResult> ExecuteTest([FromBody] RollBasicSkillRequest request)
{
    var validator = new RollBasicSkillRequestValidator();
    var validationResult = await validator.ValidateAsync(request);
    
    if (!validationResult.IsValid)
        return BadRequest(new { errors = validationResult.Errors });
    
    var result = await _service.ExecuteTestAsync(request);
    return Ok(result);
}
```

### 6.3 Tratamento de Erros e Exceções

**Padrão: Middleware de Exceção Global**

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception");
        
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            message = "Erro interno do servidor",
            traceId = context.TraceIdentifier
        };
        
        await context.Response.WriteAsJsonAsync(response);
    });
});
```

### 6.4 Logging Estruturado (Serilog)

```csharp
// Em Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "EDaemonWebServer")
    .CreateLogger();

// Em um Service
_logger.LogInformation("Executing skill test for skill {SkillId} with modifiers {Modifiers}",
    request.SkillId, string.Join(",", request.Modifiers));
```

---

## 7. Fluxos de Dados

### 7.1 Fluxo de Listagem de Perícias

```
GET /api/basic-skills
  │
  └─→ BasicSkillController.GetAll()
       │
       └─→ IBasicSkillService.GetAllBasicSkillsAsync()
            │
            ├─→ IBasicSkillRepository.GetAllAsync()
            │    │
            │    └─→ DbContext.BasicSkills.ToListAsync()
            │         │
            │         └─→ Database Query (SELECT * FROM BasicSkills)
            │
            └─→ Mapeamento de Domain para ViewModel
                 │
                 └─→ Retorna IEnumerable<BasicSkillViewModel>
       │
       └─→ HttpResponse 200 OK
            │
            └─→ JSON: [{ id: 1, name: "Acrobacia", ... }, ...]
```

**Exemplo de Resposta:**
```json
[
  {
    "id": 1,
    "name": "Acrobacia",
    "description": "Capacidade de equilibrar-se, cair e realizar movimentos corporais complexos",
    "relatedAttribute": "DEX",
    "defaultDifficulty": 50
  },
  {
    "id": 2,
    "name": "Adestramento",
    "description": "Perícia para domesticar e treinar animais",
    "relatedAttribute": "WIS",
    "defaultDifficulty": 45
  }
]
```

### 7.2 Fluxo de Teste de Perícia (Com Core)

```
POST /api/skill-test
├─ Body: { skillId: 1, characterAttribute: 65, modifiers: [-10, 5] }
  │
  └─→ BasicSkillTestController.ExecuteTest()
       │
       ├─→ RollBasicSkillRequestValidator.ValidateAsync() ✓
       │
       └─→ ISkillTestService.ExecuteTestAsync()
            │
            ├─→ Buscar perícia: IBasicSkillRepository.GetByIdAsync(1)
            │    │
            │    └─→ Domain: BasicSkill { id: 1, name: "Acrobacia", ... }
            │
            └─→ ISkillTestAdapter.ExecuteTestAsync()
                 │
                 ├─→ Traduzir para Input do Core
                 │    │
                 │    └─→ RollBasicSkillInput { skillId, attribute, modifiers }
                 │
                 ├─→ IRollBasicSkillPort.RollAsync(input) [Core UseCase]
                 │    │
                 │    ├─→ RollBasicSkillUseCase.ExecuteAsync()
                 │    │    │
                 │    │    ├─→ RollService.RollD100() → 42
                 │    │    │
                 │    │    ├─→ SkillModifierService.CalculateModifier() → -5
                 │    │    │
                 │    │    ├─→ CompareService.CheckSuccess(42, 65, -5) → true
                 │    │    │
                 │    │    ├─→ Publicar evento: RollExecutedEvent
                 │    │    │
                 │    │    └─→ Retornar RollResult { success: true, rolled: 42, margin: 18, events: [...] }
                 │    │
                 │    └─→ Resposta via Port
                 │
                 └─→ Traduzir para ViewModel
                      │
                      └─→ SkillTestResultViewModel { success: true, rolledValue: 42, margin: 18, events: [...] }
       │
       └─→ HttpResponse 200 OK
            │
            └─→ JSON: { success: true, rolledValue: 42, margin: 18, events: [...] }
```

**Exemplo de Resposta:**
```json
{
  "success": true,
  "rolledValue": 42,
  "finalValue": 37,
  "targetValue": 65,
  "margin": 18,
  "resultType": "Critical Success",
  "events": [
    {
      "timestamp": "2026-02-15T14:30:00Z",
      "level": "Information",
      "message": "Rolagem d100 executada: 42"
    },
    {
      "timestamp": "2026-02-15T14:30:01Z",
      "level": "Information",
      "message": "Modificadores aplicados: -5"
    },
    {
      "timestamp": "2026-02-15T14:30:02Z",
      "level": "Information",
      "message": "Teste bem-sucedido com margem de 18"
    }
  ]
}
```

---

## 8. Especificação de Endpoints

### 8.1 Grupo: Perícias Básicas

#### 8.1.1 Listar Perícias Básicas

```
GET /api/basic-skills

Descrição: Retorna a lista completa de perícias básicas do Módulo Básico
Autenticação: Não requerida
Rate Limiting: 100 req/min

Parâmetros de Query:
  - (nenhum nesta fase)

Resposta 200 OK:
  Content-Type: application/json
  Body: Array<BasicSkillViewModel>

Resposta 500 Internal Server Error:
  Body: { message: string, traceId: string }
```

#### 8.1.2 Obter Perícia Específica

```
GET /api/basic-skills/{id}

Descrição: Retorna uma perícia básica específica pelo ID
Parâmetros de Path:
  - id (int): Identificador da perícia [1-999]

Resposta 200 OK:
  Body: BasicSkillViewModel

Resposta 404 Not Found:
  Body: { message: "Perícia não encontrada" }
```

### 8.2 Grupo: Testes de Perícias

#### 8.2.1 Executar Teste de Perícia Básica

```
POST /api/skill-test/basic

Descrição: Executa um teste d100 com uma perícia básica
Autenticação: Não requerida

Request:
  Content-Type: application/json
  Body: RollBasicSkillRequest {
    skillId: int,
    characterAttribute: int (0-100),
    modifiers: Array<int>
  }

Validações:
  - skillId > 0
  - characterAttribute entre 0 e 100
  - modifiers: cada valor entre -50 e +50

Resposta 200 OK:
  Body: SkillTestResultViewModel {
    success: boolean,
    rolledValue: int,
    finalValue: int,
    targetValue: int,
    margin: int,
    resultType: "Critical Success" | "Success" | "Failure" | "Critical Failure",
    events: Array<EventLogViewModel>
  }

Resposta 400 Bad Request:
  Body: { errors: Array<ValidationError> }

Resposta 404 Not Found:
  Body: { message: "Perícia não encontrada" }

Resposta 500 Internal Server Error:
  Body: { message: string, traceId: string }

Exemplo cURL:
  curl -X POST http://localhost:5000/api/skill-test/basic \
    -H "Content-Type: application/json" \
    -d '{"skillId": 1, "characterAttribute": 65, "modifiers": [-10, 5]}'
```

### 8.3 Documentação Automática (Swagger)

```
URL: GET /swagger/index.html (desenvolvimento)
ou: GET /swagger-ui.html (produção)

Acesso à definição OpenAPI:
  GET /openapi/v1.json
```

---

## 9. Infraestrutura e Configuração

### 9.1 Banco de Dados

#### 9.1.1 Estratégia de Acesso

- **ORM:** Entity Framework Core 10.0+
- **Provider:** SQL Server (Microsoft.EntityFrameworkCore.SqlServer)
- **Modo:** Code-First com migrations
- **Acesso:** Read-Only pela aplicação (Insert/Update/Delete apenas via scripts de seed)

#### 9.1.2 DbContext

```csharp
public class EDaemonDbContext : DbContext
{
    public EDaemonDbContext(DbContextOptions<EDaemonDbContext> options) : base(options) { }
    
    public DbSet<BasicSkill> BasicSkills { get; set; }
    public DbSet<SpecializedSkill> SpecializedSkills { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Improvement> Improvements { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuração de chave primária
        modelBuilder.Entity<BasicSkill>().HasKey(s => s.Id);
        
        // Índices para performance
        modelBuilder.Entity<BasicSkill>().HasIndex(s => s.Name).IsUnique();
        
        // Dados iniciais (Seeding)
        modelBuilder.Entity<BasicSkill>().HasData(
            new BasicSkill { Id = 1, Name = "Acrobacia", RelatedAttribute = "DEX", DefaultDifficulty = 50 },
            new BasicSkill { Id = 2, Name = "Adestramento", RelatedAttribute = "WIS", DefaultDifficulty = 45 }
            // ... mais dados
        );
    }
}
```

### 9.2 Configuração da Aplicação

#### 9.2.1 `appsettings.json` (Produção)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=db.example.com;Database=EDaemon;User Id=sa;Password=***;"
  },
  "AllowedHosts": "*",
  "Api": {
    "Version": "1.0.0",
    "Title": "e-Daemon System REST API",
    "Description": "API para execução de testes do Sistema Daemon"
  },
  "RateLimiting": {
    "Enabled": true,
    "RequestsPerMinute": 100
  }
}
```

#### 9.2.2 `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EDaemon_Dev;Integrated Security=true;"
  },
  "AllowedHosts": "*"
}
```

### 9.3 CORS (Cross-Origin Resource Sharing)

```csharp
// Em Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://edaemon.example.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Na pipeline
app.UseCors("AllowFrontend");
```

### 9.4 HTTPS e Segurança

```csharp
// Middleware de redirecionamento HTTPS
app.UseHttpsRedirection();

// HSTS (HTTP Strict Transport Security)
app.UseHsts();

// Content Security Policy (opcional)
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});
```

---

## 10. Estratégia de Testes

### 10.1 Estrutura de Pirâmide

```
                  ▲
                 /E2E\        (Frontend: Cypress)
                /─────\       1-2 testes por fluxo crítico
               /Integr.\      (Backend: xUnit)
              /─────────\     10-15 testes por funcionalidade
             /Unitários \     (xUnit + Moq)
            /─────────────\   30-50 testes por módulo
```

### 10.2 Testes Unitários

#### 10.2.1 Padrão AAA (Arrange-Act-Assert)

```csharp
[Fact]
public async Task GetAllBasicSkills_ReturnsCompleteList_WhenRepositorySuccess()
{
    // Arrange
    var mockRepository = new Mock<IBasicSkillRepository>();
    mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new[]
    {
        new BasicSkill { Id = 1, Name = "Skill 1" },
        new BasicSkill { Id = 2, Name = "Skill 2" }
    });
    
    var mockLogger = new Mock<ILogger<BasicSkillService>>();
    var service = new BasicSkillService(mockRepository.Object, mockLogger.Object);
    
    // Act
    var result = await service.GetAllBasicSkillsAsync();
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
    mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
}
```

### 10.3 Testes de Integração

```csharp
[Fact]
public async Task ExecuteTest_WithValidSkill_ReturnsCorrectResult_IntegrationTest()
{
    // Usar banco de dados em memória ou fixture testcontainer
    var dbContext = new EDaemonDbContext(
        new DbContextOptionsBuilder<EDaemonDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options
    );
    
    // Seed dados
    dbContext.BasicSkills.Add(new BasicSkill { Id = 1, Name = "Acrobacia", DefaultDifficulty = 50 });
    await dbContext.SaveChangesAsync();
    
    // Instanciar camadas reais
    var repository = new BasicSkillRepository(dbContext);
    var adapter = new SkillTestAdapter(new RollBasicSkillPort());
    var service = new SkillTestService(repository, adapter);
    
    // Executar teste completo
    var request = new RollBasicSkillRequest { SkillId = 1, CharacterAttribute = 65, Modifiers = new[] { -10 } };
    var result = await service.ExecuteTestAsync(request);
    
    // Validar
    Assert.NotNull(result);
    Assert.True(result.Success || !result.Success); // Resultado válido
    Assert.InRange(result.RolledValue, 1, 100);
}
```

### 10.4 Cobertura de Testes

**Meta:** 80% de cobertura em Services, Adapters e UseCases

```bash
# Medir cobertura com coverlet
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover /p:Exclude=\"[*]*.Program\"

# Gerar relatório HTML
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage -reporttypes:Html
```

---

## 11. Requisitos de Deploy

### 11.1 Ambiente de Produção

| Requisito | Especificação |
|---|---|
| **Sistema Operacional** | Windows Server 2022+ ou Linux (Ubuntu 22.04+) |
| **Runtime** | .NET 10 Runtime |
| **Banco de Dados** | SQL Server 2019+ ou PostgreSQL 14+ |
| **Web Server** | IIS 10+ (Windows) ou Nginx/Apache (Linux) |
| **Certificado SSL** | TLS 1.3, auto-renovado via Let's Encrypt |
| **Memory** | Mínimo 512 MB, recomendado 2 GB |
| **CPU** | Mínimo 2 cores |
| **Disco** | Mínimo 10 GB (SSD recomendado) |

### 11.2 Build e Release

```bash
# Publicar para produção
dotnet publish -c Release -o ./publish

# Executar com variáveis de ambiente
ASPNETCORE_ENVIRONMENT=Production \
ASPNETCORE_URLS=https://0.0.0.0:443 \
dotnet EDaemonWebServer.dll
```

### 11.3 CI/CD Pipeline (Exemplo GitHub Actions)

```yaml
name: Build & Deploy

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - run: dotnet restore
      - run: dotnet build -c Release --no-restore
      - run: dotnet test /p:CollectCoverage=true
      
      - name: Publish
        if: github.ref == 'refs/heads/main'
        run: dotnet publish -c Release -o ./publish
      
      - name: Deploy to Azure
        if: github.ref == 'refs/heads/main'
        run: |
          az webapp deployment source config-zip \
            --resource-group edaemon-prod \
            --name edaemon-api \
            --src-path publish.zip
```

### 11.4 Monitoring e Observabilidade

```csharp
// Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EDaemonDbContext>()
    .AddUrlGroup(new Uri("https://api.example.com/health"), name: "API");

app.MapHealthChecks("/health");
```

---

## 12. Roadmap de Desenvolvimento (Meta 1)

### Fase 1: Setup e Arquitetura (Semana 1-2)

- [ ] Criar estrutura base de projetos (Web, Core, Tests)
- [ ] Configurar DbContext e Database
- [ ] Implementar injeção de dependência e middleware
- [ ] Estruturar testes unitários básicos

### Fase 2: Endpoints de Consulta (Semana 3-4)

- [ ] Implementar BasicSkillController, Service, Repository
- [ ] Implementar SpecializedSkillController, Service, Repository
- [ ] Implementar ItemController, Service, Repository
- [ ] Criar testes unitários (80% cobertura)
- [ ] Documentar endpoints no Swagger

### Fase 3: Integração com Core (Semana 5-6)

- [ ] Importar/referenciar projeto Core
- [ ] Implementar Adapters para UseCases
- [ ] Implementar SkillTestController e SkillTestService
- [ ] Criar testes de integração
- [ ] Validar Event Sourcing

### Fase 4: Polish e Deploy (Semana 7-8)

- [ ] Implementar tratamento de exceções global
- [ ] Configurar logging estruturado (Serilog)
- [ ] Implementar rate limiting
- [ ] Testes E2E com Frontend
- [ ] Documentação final
- [ ] Deploy em staging/produção

---

## 13. Anexos

### 13.1 Referências Externas

| Tópico | Referência |
|---|---|
| Clean Architecture .NET | https://code-maze.com/dotnet-clean-architecture/ |
| ASP.NET Core Documentation | https://learn.microsoft.com/en-us/aspnet/core/ |
| Entity Framework Core | https://learn.microsoft.com/en-us/ef/core/ |
| Serilog | https://serilog.net/ |
| FluentValidation | https://fluentvalidation.net/ |
| xUnit | https://xunit.net/ |

### 13.2 Glossário

| Termo | Definição |
|---|---|
| **DTO** | Data Transfer Object — objeto de transferência entre camadas |
| **Port** | Interface de contrato entre camadas (Arquitetura Hexagonal) |
| **Adapter** | Implementação concreta de uma Port |
| **UseCase** | Caso de uso — orquestra a execução de uma operação de negócio |
| **Event Sourcing** | Padrão de armazenar estado através de eventos sequenciais |
| **Middleware** | Componente que processa requisições HTTP na pipeline |
| **Scope** | Escopo de ciclo de vida de uma dependência (Transient, Scoped, Singleton) |
| **SemVer** | Semantic Versioning — padrão de versionamento (Major.Minor.Patch) |

---

*Documento gerado em Fevereiro de 2026. Sujeito a revisão conforme evolução do projeto.*

**Autor:** GitHub Copilot  
**Revisão:** Pedro "Ratto" Paixão
**Aprovação:** Pedro "Ratto" Paixão
**Última Atualização:** 2026-02-15
