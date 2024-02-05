namespace FC.CodeFlix.Catalog.Domain.Exceptions;

public class EntityValidationException : DomainException
{
    public EntityValidationException(string? message) : base(message)
    {
    }
}
