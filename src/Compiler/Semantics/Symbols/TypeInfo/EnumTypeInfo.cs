using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class EnumTypeInfo(string name, LocationRange loc, SemanticUnit unit)
    : UserTypeInfo(name, loc, unit)
    , IContainerSymbol<EnumValueInfo>
    , IScope
{
    private Dictionary<string, EnumValueInfo> _values = [];
    public IReadOnlyCollection<EnumValueInfo> Values => _values.Values;

    internal bool TryAddValue(EnumValueInfo val) {
        if (_values.TryAdd(val.Name, val))
            return true;

        Logger.Error(new DuplicateSymbol {
            TargetSymbol = val,
            ExistingSymbol = _values[val.Name],
            ContainingSymbol = this,
            In = "enum declaration",
        });

        return false;
    }

    private EnumScope? _scope = null;
    Scope IScope.Scope => _scope ?? new(this);
    private sealed class EnumScope(EnumTypeInfo @this) : Scope {
        public override SymbolInfo? Get(string name) {
            if (@this._values.TryGetValue(name, out var val))
                return val;
            return null;
        }
    }

    IEnumerable<EnumValueInfo> IContainerSymbol<EnumValueInfo>.Children() => Values;

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}