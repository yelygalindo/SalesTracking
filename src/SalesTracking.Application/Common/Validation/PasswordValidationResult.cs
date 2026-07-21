namespace SalesTracking.Application.Common.Validation;

public sealed record PasswordValidationResult(bool IsValid, string? Error);
