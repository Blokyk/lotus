using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class StructTypeInfo(string name, LocationRange loc, SemanticUnit unit)
    : UserTypeInfo(name, loc, unit)
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
            In = "a struct declaration"
        });

        return false;
    }

    private FunctionInfo? _ctor = null;
    public FunctionInfo Constructor {
        get {
            if (_ctor is null) {
                _ctor = new FunctionInfo(Name, Location, Unit) {
                    ReturnType = this,
                };

                foreach (var field in Fields)
                    _ctor.TryAdd(new(field.Name, field.Type, _ctor, field.Location, Unit));
            }

            return _ctor;
        }
    }

    private StructScope? _scope = null;
    Scope IScope.Scope => _scope ??= new StructScope(this);
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