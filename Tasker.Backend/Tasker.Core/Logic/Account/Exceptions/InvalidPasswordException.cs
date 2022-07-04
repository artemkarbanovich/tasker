namespace Tasker.Core.Logic.Account.Exceptions;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(string message) : base(message) { }
}
