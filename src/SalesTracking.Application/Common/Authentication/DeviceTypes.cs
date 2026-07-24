namespace SalesTracking.Application.Common.Authentication;

public static class DeviceTypes
{
    public const string Web = "web";
    public const string Android = "android";
    public const string Ios = "ios";

    public static IReadOnlyCollection<string> All { get; } =
        [Web, Android, Ios];

    public static bool IsSupported(string? value) =>
        All.Contains(value?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
}
