using NetPad.Exceptions;

namespace NetPad.DotNet;

public class AssemblyImageReference(AssemblyImage assemblyImage)
    : Reference(assemblyImage.AssemblyName.Name ?? assemblyImage.AssemblyName.FullName ?? "(Unknown)")
{
    public AssemblyImage AssemblyImage { get; } = assemblyImage;

    public override void EnsureValid()
    {
        if (AssemblyImage == null)
            throw new InvalidReferenceException(this, $"{nameof(AssemblyImage)} cannot be null.");
    }
}
