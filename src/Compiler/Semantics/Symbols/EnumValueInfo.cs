namespace Lotus.Semantics;

public class EnumValueInfo(string name, EnumTypeInfo fromEnum, LocationRange loc, SemanticUnit unit)
    : SymbolInfo(unit)
    , INamedSymbol
    , IMemberSymbol<EnumTypeInfo>
    , ILocalized
{
    public string Name { get; } = name;

    // fixme: replace with BoundLiteralValue when expr binding is done
    public int? Value { get; set; }

    public EnumTypeInfo FromEnum { get; } = fromEnum;

    public LocationRange Location { get; } = loc;

    EnumTypeInfo IMemberSymbol<EnumTypeInfo>.ContainingSymbol => FromEnum;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}