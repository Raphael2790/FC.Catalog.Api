using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using FluentValidation;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.Validator;

public class DeleteCategoryInputValidator : AbstractValidator<DeleteCategoryInput>
{
    public DeleteCategoryInputValidator()
    =>  RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
}
