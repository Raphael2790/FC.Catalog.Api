using Bogus;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Validations;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Validations;

public class DomainValidationTests
{
    private Faker Faker { get; set; } = new Faker("pt_BR");
    private string FieldName { get; set; }

    public DomainValidationTests()
    {
        FieldName = Faker.Commerce.ProductName().Replace(" ", "");
    }

    [Fact(DisplayName = nameof(NotNull_WhenValueIsNotNull_ShouldNotThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    public void NotNull_WhenValueIsNotNull_ShouldNotThrowException()
    {
        var value = Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNull(value, FieldName);
        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNull_WhenValueIsNotNull_ShouldNotThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    public void NotNull_WhenValueIsNull_ShouldThrowException()
    {
        string? value = null;
        Action action = () => DomainValidation.NotNull(value, FieldName);
        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage($"{FieldName} should not be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmpty_WhenValueIsInvalid_ShouldThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void NotNullOrEmpty_WhenValueIsInvalid_ShouldThrowException(string? value)
    {
        Action action = () => DomainValidation.NotNullOrEmpty(value, FieldName);
        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage($"{FieldName} should not be null or empty");
    }

    [Fact(DisplayName = nameof(NotNull_WhenValueIsNotNull_ShouldNotThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    public void NotNullOrEmpty_WhenValueIsValid_ShouldNotThrowException()
    {
        var value = Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNullOrEmpty(value, FieldName);
        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLength_WhenLengthIsLessThanMinValue_ShouldThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    [MemberData(nameof(GetValuesSmallerThanMinLength), parameters: 10)]
    public void MinLength_WhenLengthIsLessThanMinValue_ShouldThrowException(string value, int minLength)
    {
        Action action = () => DomainValidation.MinLength(value, minLength, FieldName);

        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage($"{FieldName} should be at least {minLength} characters long");
    }

    [Theory(DisplayName = nameof(MinLength_WhenLengthIsGreaterThanMinValue_ShouldNotThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    [MemberData(nameof(GetValuesGreaterThanMinLength), parameters: 10)]
    public void MinLength_WhenLengthIsGreaterThanMinValue_ShouldNotThrowException(string value, int minLength)
    {
        Action action = () => DomainValidation.MinLength(value, minLength, FieldName);

        action.Should().NotThrow<EntityValidationException>();
    }

    [Theory(DisplayName = nameof(MaxLength_WhenLengthIsGreaterThanMaxValue_ShouldThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    [MemberData(nameof(GetValuesGreaterThanMaxLength), parameters: 10)]
    public void MaxLength_WhenLengthIsGreaterThanMaxValue_ShouldThrowException(string value, int maxLength)
    {
        Action action = () => DomainValidation.MaxLength(value, maxLength, FieldName);

        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage($"{FieldName} should be less or equal {maxLength} characters long");
    }

    [Theory(DisplayName = nameof(MaxLength_WhenLengthIsGreaterThanMaxValue_ShouldThrowException))]
    [Trait("Domain", "DomainValidations - Validations")]
    [MemberData(nameof(GetValuesLessOrEqualThanMaxLength), parameters: 10)]
    public void MaxLength_WhenLengthIsLessOrEqualThanMaxValue_ShouldNotThrowException(string value, int maxLength)
    {
        Action action = () => DomainValidation.MaxLength(value, maxLength, FieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMinLength(int numberOfTests)
    {
        var faker = new Faker();
        yield return new object[] { "123456", 10 };
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMinLength(int numberOfTests)
    {
        var faker = new Faker();
        yield return new object[] { "123456", 6 };
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, minLength };
        }
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMaxLength(int numberOfTests)
    {
        var faker = new Faker();
        yield return new object[] { "123456", 5 };
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, maxLength };
        }
    }

    public static IEnumerable<object[]> GetValuesLessOrEqualThanMaxLength(int numberOfTests)
    {
        var faker = new Faker();
        yield return new object[] { "123456", 6 };
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length + (new Random()).Next(0, 5);
            yield return new object[] { example, maxLength };
        }
    }
}
