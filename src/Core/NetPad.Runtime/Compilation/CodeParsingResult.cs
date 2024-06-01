using NetPad.DotNet;

namespace NetPad.Compilation;

public class CodeParsingResult
{
    private int? _userProgramStartLineNumber;

    public CodeParsingResult(
        SourceCode userProgram,
        SourceCode bootstrapperProgram,
        SourceCodeCollection? additionalCodeProgram)
    {
        UserProgram = userProgram;
        BootstrapperProgram = bootstrapperProgram;
        AdditionalCodeProgram = additionalCodeProgram;
    }

    public SourceCode UserProgram { get; }
    public SourceCode BootstrapperProgram { get; }
    public SourceCodeCollection? AdditionalCodeProgram { get; }

    public int UserProgramStartLineNumber
    {
        get
        {
            if (_userProgramStartLineNumber == null)
            {
                _userProgramStartLineNumber = GetFullProgram().GetAllUsings().Count + 1;
            }

            return _userProgramStartLineNumber.Value;
        }
    }

    public SourceCodeCollection GetFullProgram()
    {
        var combined = new SourceCodeCollection();

        combined.Add(UserProgram);

        if (AdditionalCodeProgram != null)
        {
            combined.AddRange(AdditionalCodeProgram);
        }

        combined.Add(BootstrapperProgram);

        return combined;
    }
}
