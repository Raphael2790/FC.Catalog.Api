using System;
using System.Collections.Generic;
using System.Linq;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.UnitTests.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.Common;

public class GenreUseCasesBaseFixture
    : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<Genre> GetExampleGenreList(int count = 10)
        => Enumerable.Range(1, count)
            .Select(_ => GetExampleGenre(categoriesIds: GetRandomIdsList()))
            .ToList();

    public List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable.Range(1, count ?? new Random().Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();
}