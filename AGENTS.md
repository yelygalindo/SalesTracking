# AGENTS.md

## Repository Architecture

This repository follows a semi-hexagonal / layered architecture.

Main projects:

- `SalesTracking.Domain`: business entities and enums.
- `SalesTracking.Application`: use cases, application services, commands, models, results, service interfaces, repository interfaces, and common application abstractions.
- `SalesTracking.Infraestructure`: infrastructure implementations, SQL persistence, Dapper repositories, security implementations, and settings.
- `UrbanTrack.Api`: HTTP controllers, request DTOs, response DTOs, and API mappers.
- `SalesTracking.Host`: application startup and dependency injection composition root.

There are also legacy or unused folders such as `UrbanTrack.Application` and `UrbanTrack.Infrastructure.Sql`. Do not assume they are active unless the solution/project references prove it.

## Dependency Rules

Preserve the dependency direction:

```text
SalesTracking.Domain
        ↑
SalesTracking.Application
        ↑
SalesTracking.Infraestructure

UrbanTrack.Api -> SalesTracking.Application

SalesTracking.Host -> SalesTracking.Application
SalesTracking.Host -> SalesTracking.Infraestructure
SalesTracking.Host -> UrbanTrack.Api
```

Rules:

- Domain must not depend on Application, Infrastructure, API, or Host.
- Application may depend on Domain.
- Infrastructure implements Application interfaces and may depend on Application and Domain.
- API may depend on Application and should not depend on Infrastructure.
- Host is the composition root and wires Application, Infrastructure, and API together.
- Do not introduce reverse dependencies between layers.

Typical request flow:

```text
HTTP Controller
  -> API request mapper
  -> Application service
  -> Application repository interface
  -> Infrastructure repository
  -> SQL/Dapper
  -> Domain/Application model/result
  -> API response mapper
  -> HTTP response
```

## Domain Rules

Domain contains business concepts:

- Entities.
- Enums.
- Small domain invariants and state changes when appropriate.

For new features, prefer the newer richer Domain style when it makes sense:

- private setters;
- factory methods like `Create(...)`;
- domain methods for small state changes.

Do not refactor existing public-setter entities unless explicitly requested. When modifying an existing feature, follow the style already used in that feature.

Do not put HTTP, SQL, Dapper, settings, dependency injection, request/response DTOs, or infrastructure concerns in Domain.

Paged list / query result models should live in Application namespaces. Do not place new paged-list models under Domain unless they are true domain concepts. Do not move existing files or namespaces just to fix this.

## Application Rules

Application contains use-case orchestration and contracts.

Follow the existing feature folder structure:

```text
UseCases/{Feature}/Comands
UseCases/{Feature}/Interfaces
UseCases/{Feature}/Models
UseCases/{Feature}/Results
UseCases/{Feature}/Services
```

Preserve the existing `Comands` spelling. Do not rename it unless explicitly asked for a dedicated cleanup/refactor.

Application should contain:

- service interfaces such as `ICustomerService`;
- repository interfaces such as `ICustomerRepository`;
- services such as `CustomerService`;
- commands used as service input;
- models used internally by use cases and repositories;
- results returned by services;
- application-level abstractions such as `ITokenGenerator` and `IPasswordHasher`;
- external id generation through `ExternalIdGenerator` and `ExternalIdPrefixes`.

Application services should:

- perform basic validation and normalization;
- coordinate domain objects and repositories;
- return result objects or `null` following existing feature style;
- avoid HTTP-specific response types;
- avoid SQL or infrastructure details.

Use Spanish user-facing messages when matching existing service/controller responses.

## Infrastructure Rules

Infrastructure contains technical implementations.

Persistence convention:

```text
Persistence/Sql/{Feature}/{Feature}Repository.cs
Persistence/Sql/{Feature}/{Feature}RepositoryQueries.cs
Persistence/Sql/{Feature}/Rows
Persistence/Sql/{Feature}/Mappers
```

Infrastructure repositories should:

- implement repository interfaces defined in Application;
- use Dapper and SQL query constants;
- use `IOptions<T>` for settings;
- create SQL connections through a local `CreateConnection()` helper;
- map SQL rows to Domain entities or Application results through mapper extension methods;
- use transactions for multi-step writes when needed.

Do not install packages, create migrations, or introduce a new persistence framework unless explicitly requested.

Preserve the existing `Infraestructure` spelling in folders, namespaces, project names, and references. Do not rename it unless explicitly asked for a dedicated cleanup/refactor.

## API Rules

API contains HTTP concerns only.

Controllers should be thin:

- receive route/query/body data;
- map API requests to Application commands using `ToApplication()`;
- call Application services;
- map Application results to API responses using `ToResponse()`;
- translate `null`, `Succeeded`, and `NotFound` into appropriate HTTP status codes.

Keep request and response DTOs in `UrbanTrack.Api`.

Common API conventions:

- request DTOs under `Controllers/Requests/{Feature}`;
- response DTOs under `Controllers/Responses/{Feature}`;
- shared responses under `Controllers/Responses/Common`;
- request mappers under `Controllers/Requests/Mappers`;
- response mappers under `Controllers/Responses/Mappers`.

For new endpoints, prefer `MessageResponse` for simple success/error messages. Use `ErrorResponse` only when the existing controller/feature already follows that pattern or when the error contract needs to be explicit. Keep consistency with the feature being modified.

Do not put business logic, SQL, repository implementations, or infrastructure settings in API.

## Host and Dependency Injection

`SalesTracking.Host` is the composition root.

Register services, repositories, settings, and technical implementations in:

```text
SalesTracking.Host/Extensions/ServicesRegistrationExtensions.cs
```

Do not change authentication/JWT setup unless the task is specifically about authentication. It is acceptable to point out missing or incomplete auth setup when relevant, but do not modify `Program.cs` or auth registration for unrelated work.

## Naming and Style Conventions

Follow existing naming patterns:

- `{Feature}Service`
- `I{Feature}Service`
- `{Feature}Repository`
- `I{Feature}Repository`
- `{Action}{Feature}Command`
- `{Action}{Feature}Result`
- `{Feature}Request`
- `{Feature}Response`
- `{Feature}RepositoryQueries`
- `{Feature}Row`
- `{Feature}Mapper`

Mapper methods should generally be extension methods named:

- `ToApplication()`
- `ToResponse()`
- `ToDomain()`

Async conventions:

- Use `Task<T>` and async/await.
- Use `Async` suffix for asynchronous service and repository methods.
- Follow existing style; cancellation tokens are not currently a repository-wide convention.

Error handling conventions:

- Application usually returns result objects with properties such as `Succeeded`, `NotFound`, and `Message`, or returns `null` for not-found/invalid cases depending on the feature.
- Controllers translate those results into HTTP responses.
- Infrastructure may catch write-operation exceptions and return failure models following existing repository style.

## Change Discipline

- Inspect the relevant layers before changing code.
- Do not refactor unrelated code.
- Do not rename folders, namespaces, projects, or references unless explicitly requested.
- Do not move existing files just to improve conventions unless explicitly requested.
- Do not modify production code when the task asks only for documentation or inspection.
- Do not run database migrations unless explicitly requested.
- Do not install packages unless explicitly requested.
- Respect the dirty worktree. Never revert user changes unless explicitly requested.
