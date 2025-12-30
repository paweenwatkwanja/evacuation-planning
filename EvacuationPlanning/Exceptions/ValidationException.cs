namespace Exceptions;

class ValidationException : Exception
{
    public ValidationException(string errorMessage) : base(errorMessage)
    {
    }
}