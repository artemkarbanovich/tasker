namespace Tasker.Core.Logic.Account.Exceptions;

public class EmailIsTakenException : Exception
{
    public EmailIsTakenException(string message) : base(message) { }
}
