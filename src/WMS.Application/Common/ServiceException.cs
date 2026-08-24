namespace WMS.Application.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) : base($"{name} with key '{key}' was not found.") { }
}

public class ValidationException : Exception
{
    public List<string> Errors { get; }
    public ValidationException(List<string> errors) : base("Validation failed.") => Errors = errors;
}

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}
