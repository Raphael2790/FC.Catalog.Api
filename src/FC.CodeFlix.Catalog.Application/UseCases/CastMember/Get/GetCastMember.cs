using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get;

public class GetCastMember
    : IGetCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;

    public GetCastMember(ICastMemberRepository castMemberRepository) 
        => _castMemberRepository = castMemberRepository;

    public async Task<CastMemberOutputModel> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await _castMemberRepository.Get(request.Id, cancellationToken);
        return CastMemberOutputModel.FromCastMember(castMember);
    }
}