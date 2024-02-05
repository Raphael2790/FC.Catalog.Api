using FC.CodeFlix.Catalog.Domain.Enums;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.Validations;

namespace FC.CodeFlix.Catalog.Domain.Entities;

public class CastMember : AggregateRoot
{
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public CastMember(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
        CreatedAt = DateTime.Now;
        Validate();
    }
    
    public void Update(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
        Validate();
    }

    private void Validate() 
        => DomainValidation.NotNullOrEmpty(Name, nameof(Name));
}