namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Update;

public class UpdateCategoryTestsDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestsFixture();
        for (int indice = 0; indice < times; indice++)
        {
            var exempleCategory = fixture.GetValidCategory();
            var input = fixture.GetValidUpdateCategoryInput(exempleCategory.Id);
            yield return new object[] { exempleCategory, input };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int numberOfExecutions = 12)
    {
        var fixture = new UpdateCategoryTestsFixture();
        var invalidInputs = new List<object[]>(numberOfExecutions);
        var totalCases = 3;

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
                    invalidInputs.Add(new object[] { fixture.GetInvalidCategoryInputTooLongDescription(), "Description should be less or equal 10000 characters long" });
                    break;
                default:
                    break;
            }
        }

        return invalidInputs;
    }
}
