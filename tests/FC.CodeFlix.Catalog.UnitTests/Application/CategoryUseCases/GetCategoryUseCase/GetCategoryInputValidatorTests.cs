using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.Validator;
using FluentAssertions;
using System;
using System.Linq;
using FluentValidation;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.GetCategoryUseCase;

[Collection(nameof(GetCategoryTestsFixture))]
public class GetCategoryInputValidatorTests
{
    private readonly GetCategoryTestsFixture _fixture;
    private readonly GetCategoryInputValidator _validator;

    public GetCategoryInputValidatorTests(GetCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _validator = new GetCategoryInputValidator();
    }

    [Fact(DisplayName = nameof(GetCategoryInput_WhenValidationIsOk_ShouldBeValid))]
    [Trait("Application", "GetCategoryValidator - UseCases")]
    public void GetCategoryInput_WhenValidationIsOk_ShouldBeValid()
    {
        var input = _fixture.GetGetCategoryInput(Guid.NewGuid());

        var validationResult = _validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(GetCategoryInput_WhenValidationHasErrors_ShouldBeInvalid))]
    [Trait("Application", "GetCategoryValidator - UseCases")]
    public void GetCategoryInput_WhenValidationHasErrors_ShouldBeInvalid()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        var input = _fixture.GetGetCategoryInput(Guid.Empty);

        var validationResult = _validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty().And.HaveCount(1);
        validationResult.Errors.Select(x => x.ErrorMessage).Should().Contain("'Id' must not be empty.");
    }
}
