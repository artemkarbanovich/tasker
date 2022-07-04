namespace Tasker.Api.Models;

public class ValidationErrorModel
{
    public string FieldName { get; set; }
    public List<string> Messages { get; set; }
}
