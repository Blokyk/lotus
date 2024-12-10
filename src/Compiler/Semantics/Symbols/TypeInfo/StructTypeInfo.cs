using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class StructTypeInfo(string name, LocationRange loc)
    : UserTypeInfo(name, loc)
    , IContainerSymbol<FieldInfo>
    , IScope
{
    private Dictionary<string, FieldInfo> _fields = [];
    public IReadOnlyCollection<FieldInfo> Fields => _fields.Values;

    Scope IScope.Scope => throw new NotImplementedException();
    private sealed class StructScope(StructTypeInfo @this) : Scope {
        public override SymbolInfo? Get(string name) {
            if (@this._fields.TryGetValue(name, out var field))
                return field;
            return null;
        }
    }

    IEnumerable<FieldInfo> IContainerSymbol<FieldInfo>.Children() => Fields;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}