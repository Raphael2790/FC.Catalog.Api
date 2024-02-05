namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Create;

public class CreateCategoryApiTestsGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateApiTestsFixture();
        var totalCases = 3;
        var invalidInputs = new List<object[]>(totalCases);

        for (int i = 0; i < totalCases; i++)
        {
            switch (i % totalCases)
            {
                case 0:
                    var input = fixture.GetExampleInput();
                    input.Name = fixture.GetInvalidShortName();
                    invalidInputs.Add(new object[] { input, "Name should be at least 3 characters long" });
                    break;
                case 1:
                    input = fixture.GetExampleInput();
                    input.Name = fixture.GetInvalidTooLongName();
                    invalidInputs.Add(new object[] { input, "Name should be less or equal 255 characters long" });
                    break;
                case 2:
                    input = fixture.GetExampleInput();
                    input.Description = fixture.GetInvalidTooLongDescription();
                    invalidInputs.Add(new object[] { input, "Description should be less or equal 10000 characters long" });
                    break;
                default:
                    break;
            }

            yield return invalidInputs[i];
        }
    }
}
