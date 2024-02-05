using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FluentValidation;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Update.Validator;

public class UpdateCategoryInputValidator : AbstractValidator<UpdateCategoryInput>
{
    public UpdateCategoryInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Description)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}
