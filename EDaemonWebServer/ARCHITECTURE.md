# EDaemonWebServer — Architecture

This document describes the technical architecture of the `EDaemonWebServer` solution so that automated agents and human developers can understand, run, test, and extend the project.

## Quick summary
- Project: `EDaemonWebServer` (ASP.NET Core Web API targeting .NET 8)
- Tests: `EDaemonWebServerTests` (xUnit + Moq)
- Database: SQLite (file path provided by the `DATABASE_PATH` environment variable)
- Pattern: Controller -> Service -> Repository

## Directory structure (main)
- `EDaemonWebServer/`
  - `Controllers/` — HTTP controllers (e.g. `SkillController.cs`)
  - `Services/` — Application logic (e.g. `SkillService.cs` + `ISkillService`)
  - `Repositories/` — Data access (e.g. `SkillRepository.cs`, `BaseRepository.cs`)
  - `Domain/Skills/` — Domain entities and filters (e.g. `BasicSkill.cs`, `BasicSkillsFilter.cs`)
  - `Utils/Enums/` — Shared enums (e.g. `AttributeType.cs`)
  - `Program.cs` — host, DI and middleware configuration

## Projects in the solution
- `EDaemonWebServer` — main application (Web API). Primary references in the `csproj`:
  - `Microsoft.Data.Sqlite` — SQLite driver
  - `Swashbuckle.AspNetCore` — Swagger/OpenAPI
- `EDaemonWebServerTests` — unit tests (xUnit + Moq)

## Architectural pattern and request flow
- Controllers receive HTTP requests and validate parameters (e.g. `SkillController`).
- Controllers call Services (interfaces under `Services/Interfaces`) that contain application logic.
- Services depend on Repositories (interfaces under `Repositories/Interfaces`) for data access.
- Repositories use `BaseRepository` to create `SqliteConnection` and conversion helpers (e.g. `IntToBaseAttribute`, `IntToBool`).
- This separation facilitates unit testing using mocks (Moq is used in existing tests).

## Dependency injection
- Registered in `Program.cs`:
  - `builder.Services.AddScoped<SkillRepository>();` — note the project registers the concrete implementation; for greater flexibility prefer `AddScoped<ISkillRepository, SkillRepository>()` and `AddScoped<ISkillService, SkillService>()`.

## Database and configuration
- The base repository (`BaseRepository`) builds the connection string from the `DATABASE_PATH` environment variable:
  - e.g. `DATABASE_PATH=/path/to/db.sqlite` ? connection string: `Data Source=/path/to/db.sqlite`
- The repository layer uses `Microsoft.Data.Sqlite` to open connections and execute raw SQL commands.
- Conventions found in tables (e.g. `BasicSkills`):
  - `BaseAttribute` stored as `INTEGER` mapping to the `AttributeType` enum.
  - `TrainedOnly` stored as `INTEGER` (0/1) converted to `bool` by `IntToBool`.

## Main types and contracts
- `BasicSkill` (domain entity)
  - `Id`, `Name`, `BaseAttribute` (`AttributeType`), `SkillGroup`, `TrainedOnly`, `Description`.
- `BasicSkillsFilter` (filter for listing)
  - Optional fields: `Name`, `BaseAttribute`, `SkillGroup`, `TrainedOnly`, `Description`.
- `ISkillRepository` / `SkillRepository`
  - Methods: `GetAllBasicSkillsAsync(BasicSkillsFilter)` and `GetBasicSkillByIdAsync(int)`.
- `ISkillService` / `SkillService`
  - Pass-through to repository methods; central place for future business rules.

## Relevant implementation details
- `SkillRepository` builds SQL queries dynamically (StringBuilder) and adds parameters only when a filter is present. It uses `ExecuteReaderAsync` and maps columns manually with `GetOrdinal`.
- `BaseRepository.IntToBaseAttribute(object?)` converts raw values to `AttributeType` with enum validation.
- `BaseRepository.IntToBool(object?)` treats nulls and invalid conversions as `false` by default.

## Exposed APIs (endpoints)
- `GET /api/Skill/basic-skills` — lists basic skills; accepts filters via form (`[FromForm] BasicSkillsFilter`).
- `GET /api/Skill/basic-skills/{id}` — gets a skill by `id`.

## How to run locally
1. Restore packages and build the solution (dotnet 8):
   - `dotnet build` (at the solution root)
2. Set up the SQLite database:
   - Create a SQLite file and set the `DATABASE_PATH` environment variable pointing to the created file.
   - Example (PowerShell): `setx DATABASE_PATH "C:\path\to\db.sqlite"` or use `set` for the current session.
3. Run the API:
   - `dotnet run --project EDaemonWebServer` or run from the IDE.
4. Access Swagger (in Development): `http://localhost:<port>/swagger`.

## How to run tests
- `dotnet test` at the solution root runs the `EDaemonWebServerTests` tests.
- Repository tests create a temporary SQLite database and set `DATABASE_PATH` to the temporary file (see `SkillRepositoryTests`).

## Tests
Testing exists in a separate project EDaemonWebServerTests. For test details, see: #file:D:\Desenvolvimento\Workspace\HitDieRoll Projects\EDaemonWebServer\EDaemonWebServerTests\TESTS.md (no deeper testing details included here).

## Best practices and recommendations
- Register dependencies by interface instead of concrete types (e.g. `AddScoped<ISkillRepository, SkillRepository>()`).
- Externalize migrations/seed: tests currently create the schema manually; consider using a migration tool (e.g. `FluentMigrator` or `EF Core Migrations`) to manage the schema for production and development.
- Handle timeouts and retries when accessing a database file located on a network share.
- Validate and sanitize inputs: current filters are passed as SQL parameters; using `Parameters.AddWithValue` prevents SQL injection, but pay attention to types and length limits.
- Logging: add `ILogger<T>` to services/repositories to ease diagnostics.

## Quick extension points for agents
- To run unit tests automatically: run `dotnet test`.
- To discover routes and types: inspect `Controllers` + `Domain` + `Services/Interfaces`.
- To get sample data or populate the DB: create a small SQL script to insert rows into the `BasicSkills` table and point `DATABASE_PATH` to the file.

## Versioning this document
- This `ARCHITECTURE.md` file is located at the solution root and should be versioned normally with Git. Suggested commit:
  - `git add ARCHITECTURE.md && git commit -m "docs: add architecture documentation"`

## Contacts and references
- Remote repository: `https://github.com/ratto/EDaemonWebServer` (configured as `origin`).

End.
