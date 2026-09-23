namespace Ejemplo.Domain.Exceptions;

public class BusinessConflictException : BusinessException
{
    public BusinessConflictException(string message)
        : base(message)
    {
    }
}
