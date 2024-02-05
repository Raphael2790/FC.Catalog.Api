using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.Validator;
using FluentAssertions;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.UpdateCategoyUseCase;

[Collection(nameof(UpdateCategoryTestsFixture))]
public class UpdateCategoryInputValidatorTests
{
    private readonly UpdateCategoryTestsFixture _fixture;
    private readonly UpdateCategoryInputValidator _validator;

    public UpdateCategoryInputValidatorTests(UpdateCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _validator = new();
    }

    [Fact(DisplayName = nameof(Validate_WhenPassedValidInputs_UpdateCategoryInputShouldBeValid))]
    [Trait("Application", "UpdateCategoryValidator - Use Cases")]
    public void Validate_WhenPassedValidInputs_UpdateCategoryInputShouldBeValid()
    {
        var input = _fixture.GetValidUpdateCategoryInput();

        var validationResult = _validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Validate_WhenPassedInvalidInputs_UpdateCategoryInputShouldBeInvalid))]
    [Trait("Application", "UpdateCategoryValidator - Use Cases")]
    public void Validate_WhenPassedInvalidInputs_UpdateCategoryInputShouldBeInvalid()
    {
        var input = _fixture.GetInvalidUpdateCategoryInput();

        var validationResult = _validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
    }
}
