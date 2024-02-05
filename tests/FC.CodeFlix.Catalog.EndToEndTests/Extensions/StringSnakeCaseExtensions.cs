using Newtonsoft.Json.Serialization;

namespace FC.CodeFlix.Catalog.EndToEndTests.Extensions;

public static class StringSnakeCaseExtensions
{
    private static readonly NamingStrategy _snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string stringToConvert)
    {
        ArgumentNullException.ThrowIfNull(stringToConvert, nameof(stringToConvert));
        return _snakeCaseNamingStrategy.GetPropertyName(stringToConvert, false);
    }
}