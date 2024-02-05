namespace FC.CodeFlix.Catalog.EndToEndTests.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimMilliseconds(this DateTime dateTime)
        => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, default,
            dateTime.Kind);
}