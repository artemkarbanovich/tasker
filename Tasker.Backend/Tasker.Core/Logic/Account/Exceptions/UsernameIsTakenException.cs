namespace Tasker.Core.Logic.Account.Exceptions;

public class UsernameIsTakenException : Exception
{
    public UsernameIsTakenException(string message) : base(message) { }
}
