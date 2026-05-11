namespace SalesTracking.Application.Common.ExternalIds
{
    public static class ExternalIdGenerator
    {
        private const int DefaultLength = 8;

        public static string New(string prefix, int length = DefaultLength)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("El prefijo es requerido.", nameof(prefix));

            if (length <= 0 || length > 32)
                throw new ArgumentOutOfRangeException(nameof(length));

            string value = Guid.NewGuid().ToString("N")[..length];

            return $"{prefix}-{value}";
        }
    }
}
