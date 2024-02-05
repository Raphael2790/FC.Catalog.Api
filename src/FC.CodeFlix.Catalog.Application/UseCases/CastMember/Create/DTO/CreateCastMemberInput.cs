using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Domain.Enums;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.DTO;

public class CreateCastMemberInput
    : IRequest<CastMemberOutputModel>
{
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }

    public CreateCastMemberInput(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
    }
}