using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.Interface;

public interface ICreateCastMember
    : IRequestHandler<CreateCastMemberInput, CastMemberOutputModel>
{
    
}