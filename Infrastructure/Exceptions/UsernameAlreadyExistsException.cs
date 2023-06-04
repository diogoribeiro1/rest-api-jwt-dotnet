namespace Infrastructure.Exceptions;

public class UsernameAlreadyExistsException : Exception
{
    public UsernameAlreadyExistsException(string? message) : base(message)
    {
    }

    public UsernameAlreadyExistsException()
    {
    }
}