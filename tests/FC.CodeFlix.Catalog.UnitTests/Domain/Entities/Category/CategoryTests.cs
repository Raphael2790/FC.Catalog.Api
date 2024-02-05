using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entities.Category;

[Collection(nameof(CategoryTestsFixture))]
public class CategoryTests
{
    private readonly CategoryTestsFixture _categoryTestsFixture;

    public CategoryTests(CategoryTestsFixture categoryTestsFixture)
        => _categoryTestsFixture = categoryTestsFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregate")]
    public void Instantiate()
    {
        var validData = _categoryTestsFixture.GetValidCategory();

        var dateBefore = DateTime.Now;
        var category = new DomainEntities.Category(validData.Name, validData.Description);
        var dateAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        category.IsActive.Should().BeTrue();
        category.CreatedAt.Should().BeAfter(dateBefore).And.BeBefore(dateAfter);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validData = _categoryTestsFixture.GetValidCategory();

        var dateBefore = DateTime.Now;
        var category = new DomainEntities.Category(validData.Name, validData.Description, isActive);
        var dateAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default(DateTime));
        category.IsActive.Should().Be(isActive);
        category.CreatedAt.Should().BeAfter(dateBefore).And.BeBefore(dateAfter);
    }

    [Theory(DisplayName = nameof(DoIntantiate_WhenNameIsEmpty_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void DoIntantiate_WhenNameIsEmpty_ShouldThrowDomainException(string? name)
    {
        var validData = _categoryTestsFixture.GetValidCategory();
        Action action = () => new DomainEntities.Category(name!, validData.Description);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Name should not be null or empty");
    }

    [Fact(DisplayName = nameof(DoInstantiate_WhenDescriptionIsNull_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    public void DoInstantiate_WhenDescriptionIsNull_ShouldThrowDomainException()
    {
        var validData = _categoryTestsFixture.GetValidCategory();
        Action action = () => new DomainEntities.Category(validData.Name, null!);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Description should not be null");
    }

    [Theory(DisplayName = nameof(DoIntantiate_WhenNameHasLessThen3Characters_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesLessThen3Characters), parameters: 10)]
    public void DoIntantiate_WhenNameHasLessThen3Characters_ShouldThrowDomainException(string? name)
    {
        var validData = _categoryTestsFixture.GetValidCategory();
        Action action = () => new DomainEntities.Category(name!, validData.Description);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(DoIntantiate_WhenNameHasMoreThen255Characters_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    public void DoIntantiate_WhenNameHasMoreThen255Characters_ShouldThrowDomainException()
    {
        var validData = _categoryTestsFixture.GetValidCategory();
        Action action = () => new DomainEntities.Category(string.Join(default(string),Enumerable.Range(0,256).Select( _ => "a")), validData.Description);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Name should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(DoIntantiate_WhenDescriptionHasMoreThen10_000Characters_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    public void DoIntantiate_WhenDescriptionHasMoreThen10_000Characters_ShouldThrowDomainException()
    {
        var validData = _categoryTestsFixture.GetValidCategory();
        Action action = () => new DomainEntities.Category(validData.Name, string.Join(default(string), Enumerable.Range(0, 10_001).Select(_ => "a")));

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Description should be less or equal 10000 characters long");
    }

    [Fact(DisplayName = nameof(DoCategoryInactive_WhenActivated_ShouldBeActive))]
    [Trait("Domain", "Category - Aggregate")]
    public void DoCategoryInactive_WhenActivated_ShouldBeActive()
    {
        var category = _categoryTestsFixture.GetValidCategory(false);

        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(DoCategoryInactive_WhenActivated_ShouldBeActive))]
    [Trait("Domain", "Category - Aggregate")]
    public void DoCategoryActive_WhenDeactivated_ShouldBeInactive()
    {
        var category = _categoryTestsFixture.GetValidCategory();

        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(DoUpdate_WhenUpdated_ShouldHaveNewInformations))]
    [Trait("Domain", "Category - Aggregate")]
    public void DoUpdate_WhenUpdated_ShouldHaveNewInformations()
    {
        var category = _categoryTestsFixture.GetValidCategory();
        var newCategoryValues = _categoryTestsFixture.GetValidCategory();

        category.Update(newCategoryValues.Name, newCategoryValues.Description);

        category.Name.Should().Be(newCategoryValues.Name);
        category.Description.Should().Be(newCategoryValues.Description);
    }

    [Fact(DisplayName = nameof(DoUpdate_WhenUpdatedOnlyName_ShouldHaveNewNameInformationWithoutChangeDescription))]
    [Trait("Domain", "Category - Aggregate")]
    public void DoUpdate_WhenUpdatedOnlyName_ShouldHaveNewNameInformationWithoutChangeDescription()
    {
        var category = _categoryTestsFixture.GetValidCategory();
        var newName = _categoryTestsFixture.GetValidCategoryName();
        var categoryCurrentDescription = category.Description;

        category.Update(newName);

        category.Name.Should().Be(newName);
        category.Description.Should().Be(categoryCurrentDescription);
    }

    [Theory(DisplayName = nameof(DoUpdate_WhenNameIsEmpty_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void DoUpdate_WhenNameIsEmpty_ShouldThrowDomainException(string? name)
    {
        var category = _categoryTestsFixture.GetValidCategory();
        Action action = () => category.Update(name!);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Name should not be null or empty");
    }

    [Theory(DisplayName = nameof(DoUpdate_WhenNameHasLessThen3Characters_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesLessThen3Characters), parameters: 10)]
    public void DoUpdate_WhenNameHasLessThen3Characters_ShouldThrowDomainException(string invalidName)
    {
        var category = _categoryTestsFixture.GetValidCategory();
        Action action = () => category.Update(invalidName);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(DoUpdate_WhenNameHasMoreThen255Characters_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    public void DoUpdate_WhenNameHasMoreThen255Characters_ShouldThrowDomainException()
    {
        var category = _categoryTestsFixture.GetValidCategory();
        var invalidName = _categoryTestsFixture.Faker.Lorem.Letter(256);
        Action action = () => category.Update(invalidName);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Name should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(DoUpdate_WhenDescriptionHasMoreThen10_000Characters_ShouldThrowDomainException))]
    [Trait("Domain", "Category - Aggregates")]
    public void DoUpdate_WhenDescriptionHasMoreThen10_000Characters_ShouldThrowDomainException()
    {
        var category = _categoryTestsFixture.GetValidCategory();
        var invalidDescription = _categoryTestsFixture.Faker.Commerce.ProductDescription();
        while (invalidDescription.Length <= 10_000)
            invalidDescription = $"{invalidDescription} {_categoryTestsFixture.Faker.Commerce.ProductDescription()}";

        Action action = () => category.Update(_categoryTestsFixture.Faker.Commerce.Categories(1)[0], invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .And.Message.Should().Be("Description should be less or equal 10000 characters long");
    }

    public static IEnumerable<object[]> GetNamesLessThen3Characters(int iterationTimes = 6)
    {
        var fixture = new CategoryTestsFixture();

        for (int i = 0; i < iterationTimes; i++)
        {
            var isOdd = i % 2 == 1;
            //object array representa os parametros do teste
            //Será feito o bind e devem ser do mesmo tipo
            yield return new object[] { fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)] };
        }
    }
}

