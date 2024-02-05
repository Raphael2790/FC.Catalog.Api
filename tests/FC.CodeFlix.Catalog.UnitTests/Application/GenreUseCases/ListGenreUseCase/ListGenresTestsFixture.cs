using System;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.ListGenreUseCase;

[CollectionDefinition(nameof(ListGenresTestsFixture))]
public class ListGenresTestsFixtureCollection
    : ICollectionFixture<ListGenresTestsFixture>{}

public class ListGenresTestsFixture
    : GenreUseCasesBaseFixture
{
    public ListGenresInput GetExampleInput()
    {
        var random = new Random();
        return new ListGenresInput(
            page: random.Next(1, 15),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
    }
}