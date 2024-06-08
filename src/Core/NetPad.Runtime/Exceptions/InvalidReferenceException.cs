using NetPad.DotNet;

namespace NetPad.Exceptions;

public class InvalidReferenceException : Exception
{
    public InvalidReferenceException(Reference reference, string message) : base(message)
    {
        Reference = reference;
    }

    public Reference Reference { get; }
}
