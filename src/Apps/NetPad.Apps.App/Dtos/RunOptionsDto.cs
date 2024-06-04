using System.Collections.Generic;
using NetPad.ExecutionModel;

namespace NetPad.Dtos;

public class RunOptionsDto
{
    public string? SpecificCodeToRun { get; set; }

    public RunOptions ToRunOptions() => new RunOptions(SpecificCodeToRun);

    public class SourceCodeDto
    {
        public HashSet<string>? Usings { get; set; }
        public string? Code { get; set; }
    }
}
