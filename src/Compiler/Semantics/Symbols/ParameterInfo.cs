namespace Lotus.Semantics;

public class ParameterInfo(string name, TypeInfo type, FunctionInfo containingFunc)
    : SymbolInfo
    , INamedSymbol
    , ILocalized
    , IMemberSymbol<FunctionInfo>
{
    public string Name { get; } = name;
    public TypeInfo Type { get; } = type;

    public LocationRange Location { get; init; }

    public FunctionInfo ContainingFunction { get; } = containingFunc;
    FunctionInfo IMemberSymbol<FunctionInfo>.ContainingSymbol => ContainingFunction;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}