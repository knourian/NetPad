using System.IO;
using NetPad.Presentation;
using NetPad.Presentation.Html;

namespace NetPad.ExecutionModel.External.Interface;

/// <summary>
/// Provides IO utils to a running script.
/// </summary>
public static class ScriptProcessIO
{
    private static readonly TextReader _defaultConsoleInput;
    private static readonly TextWriter _defaultConsoleOutput;
    private static IExternalProcessOutputWriter? _output;
    private static bool _isHtmlOutput;

    static ScriptProcessIO()
    {
        // Capture default IO
        _defaultConsoleInput = Console.In;
        _defaultConsoleOutput = Console.Out;
    }

    public static void UseHtmlOutput()
    {
        Console.SetIn(new ActionTextReader(() =>
        {
            RawConsoleWriteLine("[INPUT_REQUEST]");
            return _defaultConsoleInput.ReadLine();
        }));
        Console.SetOut(new ActionTextWriter((value, appendNewLine) => ResultWrite(value, new DumpOptions(AppendNewLineToAllTextOutput: appendNewLine))));

        _isHtmlOutput = true;
        _output = new ExternalProcessOutputHtmlWriter(async str => await _defaultConsoleOutput.WriteLineAsync(str));
    }

    public static void UseTextOutput(bool useConsoleColors)
    {
        Console.SetIn(_defaultConsoleInput);
        Console.SetOut(_defaultConsoleOutput);

        _isHtmlOutput = false;
        _output = new ExternalProcessOutputTextWriter(useConsoleColors, async str => await _defaultConsoleOutput.WriteLineAsync(str));
    }

    public static void UseConsoleOutput(bool useConsoleColors)
    {
        Console.SetIn(_defaultConsoleInput);
        Console.SetOut(_defaultConsoleOutput);

        _isHtmlOutput = false;
        _output = new ExternalProcessOutputConsoleWriter(useConsoleColors);
    }

    public static void RawConsoleWriteLine(string text)
    {
        _defaultConsoleOutput.WriteLine(text);
    }

    public static void ResultWrite<T>(T? o, DumpOptions? options = null)
    {
        options ??= DumpOptions.Default;

        if (_isHtmlOutput && options.AppendNewLineToAllTextOutput == null)
        {
            // When using Dump() its implied that a new line is added to the end of it when rendered
            // When rendering objects or collections (ie. objects that are NOT rendered as strings)
            // they are rendered in an HTML block element that automatically pushes elements after it
            // to a new line. However when rendering strings (or objects that are rendered as strings)
            // HTML renders them in-line. So here we detect that, and manually add a new line
            bool shouldAddNewLineAfter = options.Title == null || HtmlPresenter.IsDotNetTypeWithStringRepresentation(typeof(T));

            if (shouldAddNewLineAfter)
            {
                options = options with { AppendNewLineToAllTextOutput = true };
            }
        }

        _ = _output?.WriteResultAsync(o, options);
    }

    public static void SqlWrite<T>(T? o, DumpOptions? options = null)
    {
        _ = _output?.WriteSqlAsync(o, options);
    }
}
