using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace NetPad.CodeAnalysis;

public class SyntaxNodeOrTokenSlim(bool isToken, bool isNode, SyntaxKind kind, LinePositionSpan span, bool isMissing)
{
    public bool IsToken { get; } = isToken;
    public bool IsNode { get; } = isNode;
    public SyntaxKind Kind { get; } = kind;
    public LinePositionSpan Span { get; } = span;
    public bool IsMissing { get; } = isMissing;
    public string? ValueText { get; private set; }
    public List<SyntaxTriviaSlim> LeadingTrivia { get; } = new();
    public List<SyntaxTriviaSlim> TrailingTrivia { get; } = new();
    public List<SyntaxNodeOrTokenSlim> Children { get; } = new();

    public void SetValueText(string text)
    {
        ValueText = text;
    }
}
