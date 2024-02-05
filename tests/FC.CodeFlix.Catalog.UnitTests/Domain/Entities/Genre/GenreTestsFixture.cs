using System;
using System.Collections.Generic;
using Xunit;
using FC.CodeFlix.Catalog.UnitTests.Common;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entities.Genre;

[CollectionDefinition(nameof(GenreTestsFixture))]
public class GenreTestsFixtureCollection
    : ICollectionFixture<GenreTestsFixture>{}

public class GenreTestsFixture
    : BaseFixture
{
    public string GetValidName()
        => Faker.Commerce.Categories(1)[0];

    public DomainEntities.Genre GetExampleGenre(bool isActive = true,
        List<Guid>? categoriesIds = null)
    {
        var genre = new DomainEntities.Genre(GetValidName(), isActive);
        
        if (categoriesIds is null) return genre;
        
        foreach (var categoryId in categoriesIds)
            genre.AddCategory(categoryId);

        return genre;
    }
}