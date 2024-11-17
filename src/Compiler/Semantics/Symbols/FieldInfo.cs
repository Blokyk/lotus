namespace Lotus.Semantics;

public class FieldInfo(string name)
    : SymbolInfo
    , INamedSymbol
    , IMemberSymbol<StructTypeInfo>
{
    public string Name { get; } = name;
}