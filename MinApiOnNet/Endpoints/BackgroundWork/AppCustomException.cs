using System.Runtime.Serialization;

namespace MinApiOnNet.Endpoints.BackgroundWork;

class AppCustomException : Exception
{
    public AppCustomException()
    {
    }

    public AppCustomException(string? message) : base(message)
    {
    }

    public AppCustomException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected AppCustomException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}