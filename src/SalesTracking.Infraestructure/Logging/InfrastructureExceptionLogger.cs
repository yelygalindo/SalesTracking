using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SalesTracking.Infrastructure.Logging;

public static class InfrastructureExceptionLogger
{
    private static ILogger _logger = NullLogger.Instance;

    public static void Configure(ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _logger = loggerFactory.CreateLogger("SalesTracking.Infrastructure");
    }

    public static bool Log(
        Exception exception,
        [CallerMemberName] string operation = "",
        [CallerFilePath] string sourceFile = "")
    {
        _logger.LogError(
            exception,
            "Infrastructure error in {Repository}.{Operation}",
            Path.GetFileNameWithoutExtension(sourceFile),
            operation);

        return true;
    }
}
