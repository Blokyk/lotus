
namespace Lotus.Semantics;

public sealed class EnumTypeInfo(string name)
    : TypeInfo
    , INamedSymbol
    , IContainerSymbol<EnumValueInfo>
{
    public string Name { get; } = name;

    public List<EnumValueInfo> Values { get; } = [];
    IEnumerable<EnumValueInfo> IContainerSymbol<EnumValueInfo>.Children() => Values;
}