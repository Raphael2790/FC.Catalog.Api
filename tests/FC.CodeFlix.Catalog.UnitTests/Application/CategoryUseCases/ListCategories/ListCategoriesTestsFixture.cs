using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestsFixture))]
public class ListCategoriesTestsFixtureCollection
    : ICollectionFixture<ListCategoriesTestsFixture>
{ }

public class ListCategoriesTestsFixture : CategoryUseCasesBaseFixture
{
    public List<Category> GetValidCategoriesList(int length = 10)
        => Enumerable.Range(default, length)
        .Select(_ => GetValidCategory())
        .ToList();

    public ListCategoriesInput GetExampleInput()
    {
        var random = new Random();
        return new ListCategoriesInput(
            page: random.Next(1, 15),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
    }
}
