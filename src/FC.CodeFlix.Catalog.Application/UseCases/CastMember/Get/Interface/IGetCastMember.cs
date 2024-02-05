using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.Interface;

public interface IGetCastMember
    : IRequestHandler<GetCastMemberInput, CastMemberOutputModel> { }