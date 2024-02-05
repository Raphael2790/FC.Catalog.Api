namespace FC.CodeFlix.Catalog.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string? message) : base(message) {}

    public static void ThrowIfNull(object? category, string exceptionMessage)
    {
        if (category is null)
            throw new NotFoundException(exceptionMessage);
    }
}
