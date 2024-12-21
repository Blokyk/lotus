using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

// note: a name can be dependent on non-name expr (eg: ("hello"+"world").length)
internal sealed class QualifiedName : Name
{
    public new FullNameNode SyntaxNode { get; }

    public sealed record Part(string Name, SymbolInfo Symbol);

    public ImmutableArray<Part> Parts { get; }

    public QualifiedName(
        FullNameNode name,
        ImmutableArray<Part> parts
    )
        : base(name, (TypedSymbolInfo)parts[0].Symbol)
    {
        Debug.Assert(!parts.IsEmpty);

        SyntaxNode = name;
        Parts = parts;
    }
}