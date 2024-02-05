namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Update;

public class UpdateApiTestsDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateApiTestsFixture();
        const int totalCases = 3;
        var invalidInputs = new List<object[]>(totalCases);

        for (var i = 0; i < totalCases; i++)
        {
            switch (i % totalCases)
            {
                case 0:
                    var input = fixture.GetValidInput();
                    input.Name = fixture.GetInvalidShortName();
                    invalidInputs.Add(new object[] { input, "Name should be at least 3 characters long" });
                    break;
                case 1:
                    input = fixture.GetValidInput();
                    input.Name = fixture.GetInvalidTooLongName();
                    invalidInputs.Add(new object[] { input, "Name should be less or equal 255 characters long" });
                    break;
                case 2:
                    input = fixture.GetValidInput();
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