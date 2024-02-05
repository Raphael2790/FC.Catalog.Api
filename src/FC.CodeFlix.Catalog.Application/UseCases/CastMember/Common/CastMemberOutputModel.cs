using FC.CodeFlix.Catalog.Domain.Enums;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;

public class CastMemberOutputModel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public CastMemberOutputModel(Guid id, string name, CastMemberType type, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Type = type;
        CreatedAt = createdAt;
    }
    
    public static CastMemberOutputModel FromCastMember(Domain.Entities.CastMember castMember) 
        => new(castMember.Id, castMember.Name, castMember.Type, castMember.CreatedAt);
}