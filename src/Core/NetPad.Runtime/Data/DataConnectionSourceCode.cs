using NetPad.DotNet;

namespace NetPad.Data;

public class DataConnectionSourceCode
{
    public SourceCodeCollection DataAccessCode { get; init; } = new();
    public SourceCodeCollection ApplicationCode { get; init; } = new();

    public bool IsEmpty() => !DataAccessCode.Any() && !ApplicationCode.Any();
}
