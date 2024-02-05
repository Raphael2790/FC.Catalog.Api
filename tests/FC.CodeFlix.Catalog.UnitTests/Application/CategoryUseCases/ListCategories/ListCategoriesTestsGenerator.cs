using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using System.Collections.Generic;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.ListCategories;

public class ListCategoriesTestsGenerator
{
    public static IEnumerable<object[]> GetInputsWithoutAllParameters(int times = 12)
    {
        var fixture = new ListCategoriesTestsFixture();
        var exampleInput = fixture.GetExampleInput();
        int totalCases = 6;

        for (int i = 0; i < times; i++)
        {
            switch (i % totalCases)
            {
                case 0:
                    yield return new object[]
                    {
                        new ListCategoriesInput()
                    };
                    break;
                case 1:
                    yield return new object[]
                    {
                        new ListCategoriesInput((int)exampleInput.Page!)
                    };
                    break;
                case 2:
                    yield return new object[]
                    {
                        new ListCategoriesInput((int)exampleInput.Page!,
                        (int)exampleInput.PerPage!)
                    };
                    break;
                case 3:
                    yield return new object[]
                    {
                        new ListCategoriesInput((int)exampleInput.Page!,
                        (int)exampleInput.PerPage!, exampleInput.Search!)
                    };
                    break;
                case 4:
                    yield return new object[]
                    {
                        new ListCategoriesInput((int)exampleInput.Page!,
                        (int)exampleInput.PerPage!, exampleInput.Search!,
                        exampleInput.Sort!)
                    };
                    break;
                case 5:
                    yield return new object[]
                    {
                        exampleInput
                    };
                    break;
                default:
                    yield return new object[]
                    {
                        new ListCategoriesInput()
                    };
                    break;
            }
        }
    }
}
