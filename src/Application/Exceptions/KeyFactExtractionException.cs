using System;

namespace Application.Exceptions;

public sealed class KeyFactExtractionException : Exception
{
    public KeyFactExtractionException() { }

    public KeyFactExtractionException(string message) : base(message) { }

    public KeyFactExtractionException(string message, Exception innerException)
        : base(message, innerException) { }
}
