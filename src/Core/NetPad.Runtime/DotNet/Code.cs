using System.Text;
using System.Text.Json.Serialization;

namespace NetPad.DotNet;

[method: JsonConstructor]
public class Code(Namespace? @namespace, string? value) : SourceCodeElement<string?>(value)
{
    public Code(string? value) : this(null, value)
    {
    }

    public Namespace? Namespace { get; } = @namespace;

    public override bool ValueChanged()
    {
        return _valueChanged || (Namespace != null && Namespace.ValueChanged());
    }

    public override string ToCodeString()
    {
        var sb = new StringBuilder();

        if (Namespace != null)
        {
            sb.AppendLine(Namespace.ToCodeString()).AppendLine();
        }

        if (Value != null)
        {
            sb.AppendLine(Value);
        }

        return sb.ToString();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return base.GetEqualityComponents();
        yield return Namespace;
    }
}
