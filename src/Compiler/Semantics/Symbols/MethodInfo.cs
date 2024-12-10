using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public class MethodInfo(NamespaceInfo containingNamespace)
    : SymbolInfo
    , IMemberSymbol<NamespaceInfo>
    , IContainerSymbol<ParameterInfo>
    , IScope
    , ILocalized
{
    private Dictionary<string, ParameterInfo> _params = [];
    public IReadOnlyCollection<ParameterInfo> Parameters => _params.Values;

    public LocationRange Location { get; init; }

    public NamespaceInfo ContainingNamespace { get; } = containingNamespace;

    private MethodScope? _scope = null;
    Scope IScope.Scope => _scope ?? new(this);
    // todo: split scope for signature and body (locals can shadow params, but params can't have same name)
    private sealed class MethodScope(MethodInfo @this) : Scope {
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