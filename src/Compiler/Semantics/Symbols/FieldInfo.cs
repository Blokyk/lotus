namespace Lotus.Semantics;

public sealed class FieldInfo(
    string name,
    TypeInfo type,
    TypeInfo containingType,
    LocationRange loc,
    SemanticUnit unit
)
    : TypedSymbolInfo(unit)
    , INamedSymbol
    , IMemberSymbol<StructTypeInfo>
    , ILocalized
{
    public string Name { get; } = name;
    public override TypeInfo Type => type;
    public LocationRange Location => loc;

    // fixme: change to some other type (specific type to represent const vals?) when binding is done
    public object? DefaultValue { get; init; }
    [MemberNotNullWhen(true, nameof(DefaultValue))]
    public bool HasDefaultValue => DefaultValue is not null;

    public TypeInfo FromStruct { get; } = containingType;
    StructTypeInfo IMemberSymbol<StructTypeInfo>.ContainingSymbol => throw new NotImplementedException();

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}