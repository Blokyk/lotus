using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class StructTypeInfo(string name, LocationRange loc)
    : UserTypeInfo(name, loc)
    , IContainerSymbol<FieldInfo>
    , IScope
{
    private Dictionary<string, FieldInfo> _fields = [];
    public IReadOnlyCollection<FieldInfo> Fields => _fields.Values;

    internal bool TryAddField(FieldInfo field) {
        if (_fields.TryAdd(field.Name, field))
            return true;

        Logger.Error(new DuplicateSymbol {
            TargetSymbol = field,
            ExistingSymbol = _fields[field.Name],
            ContainingSymbol = this,
            In = "struct declaration"
        });

        return false;
    }

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