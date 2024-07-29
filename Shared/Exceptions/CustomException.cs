using System.Net;

namespace BusinessLayer.Middlewares;

public class CustomException : Exception
{
    public int StatusCode { get; }

    public CustomException(string message, int statusCode = (int)HttpStatusCode.BadRequest) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : CustomException
{
    public NotFoundException(string message) : base(message, (int)HttpStatusCode.NotFound) { }
}

public class BadRequestException : CustomException
{
    public BadRequestException(string message) : base(message, (int)HttpStatusCode.BadRequest) { }
}

public class ConflictException : CustomException
{
    public ConflictException(string message) : base(message, (int)HttpStatusCode.Conflict) { }
}

public class IsUsedException : CustomException
{
    public IsUsedException(string message) : base(message, (int)HttpStatusCode.Conflict) { }
}
