using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.Validator;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.DeleteCategoryUseCase;

[Collection(nameof(DeleteCategoryTestsFixture))]
public class DeleteCategoryInputValidatorTests
{
    private readonly DeleteCategoryTestsFixture _fixture;
    private readonly DeleteCategoryInputValidator _sut;

    public DeleteCategoryInputValidatorTests(DeleteCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _sut = new DeleteCategoryInputValidator();
    }

    [Fact(DisplayName = nameof(Validate_WhenDeleteCategoyInputIsOK_ShouldBeValid))]
    [Trait("Application", "DeleteCategoryValidator - Use Cases")]
    public void Validate_WhenDeleteCategoyInputIsOK_ShouldBeValid()
    {
        var input = _fixture.GetValidDeleteCategoryInput();

        var validationResult = _sut.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Validate_WhenDeleteCategoyInputIsntOK_ShouldBeInvalid))]
    [Trait("Application", "DeleteCategoryValidator - Use Cases")]
    public void Validate_WhenDeleteCategoyInputIsntOK_ShouldBeInvalid()
    {
        var input = _fixture.GetInvalidDeleteCategoryInput();

        var validationResult = _sut.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Select(x => x.ErrorMessage).Should().Contain("'Id' deve ser informado.");
    }
}
