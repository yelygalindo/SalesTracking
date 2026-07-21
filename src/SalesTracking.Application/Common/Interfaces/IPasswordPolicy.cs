using SalesTracking.Application.Common.Validation;

namespace SalesTracking.Application.Common.Interfaces;

public interface IPasswordPolicy
{
    PasswordValidationResult Validate(string? password);
}
