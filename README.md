# SalesTracking

API REST para el seguimiento de clientes, proyectos, productos, entregas y actividad comercial. Está construida con ASP.NET Core 9, SQL Server y Dapper, siguiendo una arquitectura por capas.

## Requisitos

- .NET SDK 9
- SQL Server
- Una base de datos con el esquema requerido por los repositorios Dapper

El repositorio no contiene migraciones automáticas. La aplicación no crea ni actualiza el esquema de base de datos al iniciar.

## Arquitectura

```text
SalesTracking.Domain
        ↑
SalesTracking.Application
        ↑
SalesTracking.Infraestructure

UrbanTrack.Api -> SalesTracking.Application
SalesTracking.Host -> Application + Infraestructure + API
```

- `SalesTracking.Domain`: entidades y enums de negocio.
- `SalesTracking.Application`: servicios, comandos, resultados e interfaces.
- `SalesTracking.Infraestructure`: persistencia SQL/Dapper, seguridad y almacenamiento.
- `UrbanTrack.Api`: controladores y contratos HTTP.
- `SalesTracking.Host`: inicio, autenticación y composición de dependencias.
- `tests/SalesTracking.Application.UnitTests`: pruebas unitarias de servicios de Application.

Se conserva el nombre histórico `Infraestructure` porque forma parte de los proyectos y namespaces actuales.

## Configuración

Para desarrollo se puede usar `appsettings.Development.json`. En ambientes compartidos o productivos, configura secretos mediante variables de entorno o un proveedor seguro:

```powershell
$env:DatabaseSettings__ConnectionString="Server=.;Database=SalesTracking;Trusted_Connection=True;TrustServerCertificate=True"
$env:JwtSettings__Secret="una-clave-segura-de-al-menos-32-bytes"
$env:JwtSettings__Issuer="SalesTracking"
$env:JwtSettings__Audience="SalesTrackingUsers"
$env:AuthSettings__AccessTokenExpirationHours="1"
$env:AuthSettings__RefreshTokenExpirationDays="7"
$env:AuthSettings__PasswordResetTokenExpirationHours="5"
$env:Storage__RootPath="storage"
```

No publiques claves JWT ni credenciales reales en archivos versionados.

## Ejecutar localmente

```powershell
dotnet restore SalesTracking.sln
dotnet run --project src\SalesTracking.Host\SalesTracking.Host.csproj
```

Con el perfil HTTPS, Swagger queda disponible en `https://localhost:7127/swagger`. El perfil HTTP usa `http://localhost:5131/swagger`.

## Autenticación

La API usa JWT Bearer. Todos los endpoints requieren autenticación por defecto, salvo los marcados con `[AllowAnonymous]`, como:

- `POST /api/auth/login`
- `POST /api/auth/refresh-token`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`
- endpoints públicos del flujo de invitaciones

Después de iniciar sesión, copia el `accessToken`, presiona **Authorize** en Swagger e ingresa únicamente el token. Swagger agrega el prefijo `Bearer`.

Los refresh tokens y tokens de recuperación se guardan como hashes. Al restablecer una contraseña se revocan las sesiones activas. El envío del enlace de recuperación por correo requiere integrar un proveedor de correo.

## Compilar y probar

```powershell
dotnet build SalesTracking.sln --configuration Release
dotnet test SalesTracking.sln --configuration Release --no-build
```

## Integración continua

El workflow `.github/workflows/ci.yml` se ejecuta en pushes a `main`, `master` y `develop`, y en pull requests. Restaura dependencias, compila la solución en Release y ejecuta las pruebas sin requerir una base de datos.
