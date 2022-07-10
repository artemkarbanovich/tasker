using System.Text;
using Tasker.Core.Interfaces.Services;
using ChoETL;

namespace Tasker.Infrastructure.Services;

public class CsvService : ICsvService
{
    public string JsonToCsvString(string json)
    {
        var csv = new StringBuilder();

        using var jsonReader = ChoJSONReader.LoadText(json);
        using var scvWriter = new ChoCSVWriter(csv)
            .WithFirstLineHeader()
            .WithDelimiter(",")
            .ThrowAndStopOnMissingField(false);

        scvWriter.Write(jsonReader);

        return csv.ToString();
    }
}
