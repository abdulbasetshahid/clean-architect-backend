namespace Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key) : base($"{name} is not found")
    {

    }

}
