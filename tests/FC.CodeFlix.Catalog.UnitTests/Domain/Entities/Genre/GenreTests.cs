using System;
using System.Collections.Generic;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entities.Genre;

[Collection(nameof(GenreTestsFixture))]
public class GenreTests
{
    private readonly GenreTestsFixture _fixture;

    public GenreTests(GenreTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregate")]
    public void Instantiate()
    {
        var genreName = _fixture.GetValidName();

        var genre = new DomainEntities.Genre(genreName);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
    
    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var genreName = _fixture.GetValidName();

        var genre = new DomainEntities.Genre(genreName, isActive);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
    
    [Theory(DisplayName = nameof(InstantiateThrowsWhenNameIsEmpty))]
    [Trait("Domain", "Genre - Aggregate")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void InstantiateThrowsWhenNameIsEmpty(string? name)
    {
        var action = () => new DomainEntities.Genre(name);

        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
    
    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);
        var genreOldName = genre.Name;

        genre.Activate();
        
        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreOldName);
        genre.IsActive.Should().BeTrue();
    }
    
    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregate")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);
        var genreOldName = genre.Name;

        genre.Deactivate();
        
        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genreOldName);
        genre.IsActive.Should().BeFalse();
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregate")]
    public void Update()
    {
        var genre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidName();
        var oldIsActive = genre.IsActive;

        genre.Update(newName);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(newName);
        genre.IsActive.Should().Be(oldIsActive);
        genre.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
    
    [Theory(DisplayName = nameof(UpdateThrowsWhenNameIsEmpty))]
    [Trait("Domain", "Genre - Aggregate")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void UpdateThrowsWhenNameIsEmpty(string? newName)
    {
        var genre = _fixture.GetExampleGenre();

        var action = () => genre.Update(newName);

        action.Should().ThrowExactly<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
    
    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregate")]
    public void AddCategory()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryId = Guid.NewGuid();
        
        genre.AddCategory(categoryId);

        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryId);
    }
    
    [Fact(DisplayName = nameof(AddTwoCategory))]
    [Trait("Domain", "Genre - Aggregate")]
    public void AddTwoCategory()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        
        genre.AddCategory(categoryId1);
        genre.AddCategory(categoryId2);

        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryId1);
        genre.Categories.Should().Contain(categoryId2);
    }
    
    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Genre - Aggregate")]
    public void RemoveCategory()
    {
        var exampleGuid = Guid.NewGuid();
        var genre = _fixture
            .GetExampleGenre(categoriesIds: new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                exampleGuid,
                Guid.NewGuid(),
                Guid.NewGuid()
            });
        
        genre.RemoveCategory(exampleGuid);

        genre.Categories.Should().HaveCount(4);
        genre.Categories.Should().NotContain(exampleGuid);
    }
    
    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Genre - Aggregate")]
    public void RemoveAllCategories()
    {
        var genre = _fixture
            .GetExampleGenre(categoriesIds: new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            });
        
        genre.RemoveAllCategories();

        genre.Categories.Should().HaveCount(default(int));
    }
}