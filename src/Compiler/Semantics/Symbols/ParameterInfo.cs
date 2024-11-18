namespace Lotus.Semantics;

public class ParameterInfo(string name, TypeInfo type) : SymbolInfo
{
    public string Name { get; } = name;
    public TypeInfo Type { get; } = type;
}