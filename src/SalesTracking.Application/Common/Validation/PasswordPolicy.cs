using SalesTracking.Application.Common.Interfaces;

namespace SalesTracking.Application.Common.Validation;

public sealed class PasswordPolicy : IPasswordPolicy
{
    public PasswordValidationResult Validate(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return new(false, "La contraseña debe tener al menos 8 caracteres");
        if (!password.Any(char.IsUpper))
            return new(false, "La contraseña debe incluir al menos una mayúscula.");
        if (!password.Any(char.IsLower))
            return new(false, "La contraseña debe incluir al menos una minúscula.");
        if (!password.Any(char.IsDigit))
            return new(false, "La contraseña debe incluir al menos un número.");
        return new(true, null);
    }
}
