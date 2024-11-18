namespace Lotus.Semantics;

public sealed class MissingTypeInfo(string name) : TypeInfo
{
    public string Name { get; } = name;
}