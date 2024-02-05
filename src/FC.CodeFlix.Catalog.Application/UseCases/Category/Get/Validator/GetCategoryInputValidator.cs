using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using FluentValidation;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Get.Validator;

public class GetCategoryInputValidator : AbstractValidator<GetCategoryInput>
{
    public GetCategoryInputValidator()
      => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    
}
