using System.Text;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NetPad.DotNet;

[method: JsonConstructor]
public class SourceCode(Code? code, IEnumerable<Using>? usings = null)
{
    private readonly HashSet<Using> _usings = usings?.ToHashSet() ?? new HashSet<Using>();
    private bool _valueChanged;

    public SourceCode() : this(null, Array.Empty<Using>())
    {
    }

    public SourceCode(IEnumerable<string> usings) : this(null, usings)
    {
    }

    public SourceCode(params string[] usings) : this(null, usings)
    {
    }

    public SourceCode(IEnumerable<Using> usings) : this(null, usings)
    {
    }

    public SourceCode(params Using[] usings) : this(null, usings)
    {
    }

    public SourceCode(string? code, IEnumerable<string>? usings = null)
        : this(
            code == null ? null : new Code(code),
            usings?.Select(u => new Using(u))
        )
    {
    }

    public IEnumerable<Using> Usings => _usings;
    public Code Code { get; } = code ?? new Code(null, null);
    public bool ValueChanged() => _valueChanged || Code.ValueChanged() || _usings.Any(u => u.ValueChanged());

    public void AddUsing(string @using)
    {
        bool added = _usings.Add(@using);

        if (added) _valueChanged = true;
    }

    public void RemoveUsing(string @using)
    {
        bool removed = _usings.Remove(@using);

        if (removed) _valueChanged = true;
    }

    public string ToCodeString(bool useGlobalNotation = false)
    {
        var builder = new StringBuilder();

        builder
            .AppendJoin(Environment.NewLine, Usings.Select(ns => ns.ToCodeString(useGlobalNotation)))
            .AppendLine();

        builder.AppendLine();
        builder.AppendLine(Code.ToCodeString());

        return builder.ToString();
    }

    public static SourceCode Parse(string text)
    {
        var root = CSharpSyntaxTree.ParseText(text).GetRoot();

        var usingDirectives = root
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .ToArray();

        var usings = usingDirectives
            .Select(u => string.Join(
                ' ',
                u.NormalizeWhitespace().ChildNodes().Select(x => x.ToFullString()))
            )
            .ToArray();

        var code = root
            .RemoveNodes(usingDirectives, SyntaxRemoveOptions.KeepNoTrivia)?
            .NormalizeWhitespace()
            .ToFullString();

        return new SourceCode(code, usings);
    }
}
