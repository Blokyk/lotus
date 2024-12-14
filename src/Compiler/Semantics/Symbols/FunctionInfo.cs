using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public class FunctionInfo(string name, LocationRange loc, SemanticUnit unit)
    : SymbolInfo(unit) // todo: should be 'TypedSymbolInfo' when we have a way to represent lambdas
    , INamedSymbol
    , IMemberSymbol<NamespaceInfo>
    , IContainerSymbol<ParameterInfo>
    , IScope
    , ILocalized
{
    public LocationRange Location => loc;

    public string Name => name;

    public NamespaceInfo? ContainingNamespace { get; set; }

    public TypeInfo ReturnType { get; set; } = unit.UnknownType;

    private Dictionary<string, ParameterInfo> _params = [];
    public IReadOnlyCollection<ParameterInfo> Parameters => _params.Values;

    public bool TryAdd(ParameterInfo param) {
        if (_params.TryAdd(param.Name, param))
            return true;

        Logger.Error(new DuplicateSymbol {
            TargetSymbol = param,
            ExistingSymbol = _params[param.Name],
            In = "a method parameter list",
            ContainingSymbol = this
        });

        return false;
    }

    private MethodScope? _scope = null;
    Scope IScope.Scope => _scope ?? new(this);
    // todo: split scope for signature and body (locals can shadow params, but params can't have same name)
    private sealed class MethodScope(FunctionInfo @this) : Scope {
        public override SymbolInfo? Get(string name) {
            if (@this._params.TryGetValue(name, out var param))
                return param;
            return null;
        }
    }

    NamespaceInfo IMemberSymbol<NamespaceInfo>.ContainingSymbol => throw new NotImplementedException();

    IEnumerable<ParameterInfo> IContainerSymbol<ParameterInfo>.Children() => throw new NotImplementedException();

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}