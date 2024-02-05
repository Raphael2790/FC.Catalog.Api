using System;
using System.Linq;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;
using FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.CreateGenreUseCase;

[CollectionDefinition(nameof(CreateGenreTestsFixture))]
public class CreateGenreTestsFixtureCollection
    : ICollectionFixture<CreateGenreTestsFixture>{}

public class CreateGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new(GetValidGenreName(),
            GetRandomBoolean());
    
    public CreateGenreInput GetExampleInputWithoutName(string? name = null)
        => new(name!,
            GetRandomBoolean());
    
    public CreateGenreInput GetExampleInputWithCategories()
    {
        var randomNumber = new Random().Next(1, 10);
        var categoriesIds = Enumerable.Range(1, randomNumber)
            .Select(_ => Guid.NewGuid()).ToList();
        return new CreateGenreInput(GetValidGenreName(),
            GetRandomBoolean(), categoriesIds);
    }
}