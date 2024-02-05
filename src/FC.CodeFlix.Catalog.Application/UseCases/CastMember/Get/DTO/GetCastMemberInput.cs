using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.DTO;

public class GetCastMemberInput
    : IRequest<CastMemberOutputModel>
{
    public Guid Id { get; private set; }

    public GetCastMemberInput(Guid id) 
        => Id = id;
}