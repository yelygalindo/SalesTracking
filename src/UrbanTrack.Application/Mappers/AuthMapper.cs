using UrbanTrack.Application.DTOs.Responses.Auth;
using UrbanTrack.Infrastructure.Sql.Entities;

namespace UrbanTrack.Application.Mappers
{
    // Mapper sencillo que potencialmente podría tener lógica adicional.
    // Nota: Este archivo está dentro de Application y no referencia Infrastructure en compilación;
    // si tu solución organiza proyectos por ensamblados, elimina esta clase o muévela a Infrastructure.
    // La dejo como placeholder si prefieres tener mappers en Application.
    public static class AuthMapper
    {
        public static AuthUserResponse ToAuthUserResponse(this AuthUserSqlEntity entity)
        {
            if (entity == null) return null;
            return new AuthUserResponse
            {
                Id = entity.Id,
                Username = entity.Username,
                FullName = entity.FullName,
                Company = new CompanyResponse { Id = entity.Company?.Id, Name = entity.Company?.Name }
            };
        }
    }
}