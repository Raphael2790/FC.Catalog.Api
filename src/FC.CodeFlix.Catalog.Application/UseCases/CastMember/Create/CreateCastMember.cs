using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create;

public class CreateCastMember : ICreateCastMember
{
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
    {
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CastMemberOutputModel> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = new Domain.Entities.CastMember(request.Name, request.Type);
        
        await _castMemberRepository.Insert(castMember, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        
        return CastMemberOutputModel.FromCastMember(castMember);
    }
}