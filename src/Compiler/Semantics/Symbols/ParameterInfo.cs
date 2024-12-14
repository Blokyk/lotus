namespace Lotus.Semantics;

public class ParameterInfo(string name, TypeInfo type, FunctionInfo containingFunc, SemanticUnit unit)
    : TypedSymbolInfo(unit)
    , INamedSymbol
    , ILocalized
    , IMemberSymbol<FunctionInfo>
{
    public string Name { get; } = name;
    public override TypeInfo Type => type;

    public LocationRange Location { get; init; }

    public FunctionInfo ContainingFunction { get; } = containingFunc;
    FunctionInfo IMemberSymbol<FunctionInfo>.ContainingSymbol => ContainingFunction;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}