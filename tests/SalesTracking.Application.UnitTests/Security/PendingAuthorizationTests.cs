namespace SalesTracking.Application.UnitTests.Security;

public sealed class PendingAuthorizationTests
{
    [Fact(Skip = "Pendiente: definir roles, emitirlos en el JWT y aplicar políticas a los controladores.")]
    public void ProtectedEndpoint_WhenRoleIsIncorrect_ShouldReturn403() { }

    [Fact(Skip = "Pendiente: aplicar CompanyId del JWT en todos los repositorios o mediante una política de tenant.")]
    public void ProtectedResource_WhenItBelongsToAnotherCompany_ShouldReturn404Or403() { }

    [Fact(Skip = "Pendiente: definir autorización por propietario para registros asignados a vendedores.")]
    public void Seller_WhenModifyingAnotherSellersRecord_ShouldReturn403() { }

    [Fact(Skip = "Pendiente: configurar rate limiting para POST /api/auth/login.")]
    public void Login_WhenTooManyAttemptsAreMade_ShouldReturn429() { }
}
