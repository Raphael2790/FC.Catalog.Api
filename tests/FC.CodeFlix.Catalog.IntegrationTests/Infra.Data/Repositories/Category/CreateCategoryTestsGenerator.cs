using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Create;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.Repositories.Category;

public class CreateCategoryTestsGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int numberOfExecutions = 12)
    {
        var fixture = new CreateCategoryTestsFixture();
        var invalidInputs = new List<object[]>(numberOfExecutions);
        var totalCases = 4;

        for (int i = 0; i < numberOfExecutions; i++)
        {
            switch (i % totalCases)
            {
                case 0:
                    invalidInputs.Add(new object[] { fixture.GetInvalidCategoryInputShortName(), "Name should be at least 3 characters long" });
                    break;
                case 1:
                    invalidInputs.Add(new object[] { fixture.GetInvalidCategoryInputTooLongName(), "Name should be less or equal 255 characters long" });
                    break;
                case 2:
                    invalidInputs.Add(new object[] { fixture.GetInvalidCreateCategoryInputNullDescription(), "Description should not be null" });
                    break;
                case 3:
                    invalidInputs.Add(new object[] { fixture.GetInvalidCategoryInputTooLongDescription(), "Description should be less or equal 10000 characters long" });
                    break;
                default:
                    break;
            }

            yield return invalidInputs[i];
        }
    }
}
