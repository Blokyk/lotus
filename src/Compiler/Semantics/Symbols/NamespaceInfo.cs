using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public class NamespaceInfo(string name, SemanticUnit unit)
    : SymbolInfo(unit)
    , INamedSymbol
    , IMemberSymbol<NamespaceInfo?>
    , IContainerSymbol<NamespaceInfo>
    , IContainerSymbol<UserTypeInfo>
    , IScope
{
    public string Name { get; } = name;

    [MemberNotNullWhen(false, nameof(ContainingNamespace))]
    public bool IsTopNamespace => ContainingNamespace == null;

    private Dictionary<string, NamespaceInfo> _namespaces = [];
    public IEnumerable<NamespaceInfo> Namespaces => _namespaces.Values;
    public bool TryAdd(NamespaceInfo ns) {
        Debug.Assert(ns.ContainingNamespace is null);
        ns.ContainingNamespace = this;
        var couldAdd = _namespaces.TryAdd(ns.Name, ns);
        // fixme: check name doesn't clash with types or funcs
        Debug.Assert(couldAdd, "Adding a new namespace should never fail; this proably means that you created a new NS instead of resolving to an existing one");
        return couldAdd;
    }

    private Dictionary<string, UserTypeInfo> _types = [];
    public IEnumerable<UserTypeInfo> Types => _types.Values;
    public bool TryAdd(UserTypeInfo type) {
        Debug.Assert(type.ContainingNamespace is null);
        type.ContainingNamespace = this;

        if (_types.TryAdd(type.Name, type))
            return true;

        // fixme: check name doesn't clash with namespaces or funcs

        Logger.Error(new DuplicateSymbol {
            TargetSymbol = type,
            ExistingSymbol = _types[type.Name],
            ContainingSymbol = this
        });

        return false;
    }

    private Dictionary<string, FunctionInfo> _funcs = [];
    public IEnumerable<FunctionInfo> Functions => _funcs.Values;
    public bool TryAdd(FunctionInfo func) {
        Debug.Assert(func.ContainingNamespace is null);
        func.ContainingNamespace = this;

        if (_funcs.TryAdd(func.Name, func))
            return true;

        // fixme: check name doesn't clash with namespaces or types

        Logger.Error(new DuplicateSymbol {
            TargetSymbol = func,
            ExistingSymbol = _funcs[func.Name],
            ContainingSymbol = this
        });

        return false;
    }

    public NamespaceInfo? ContainingNamespace { get; set; } = null;

    private NamespaceScope? _scope = null;
    Scope IScope.Scope => _scope ?? new(this);

    internal sealed class NamespaceScope(NamespaceInfo @this) : Scope {
        public override SymbolInfo? Get(string name) {
            if (@this._namespaces.TryGetValue(name, out var childNs))
                return childNs;
            if (@this._types.TryGetValue(name, out var type))
                return type;
            return null;
        }
    }

    IEnumerable<NamespaceInfo> IContainerSymbol<NamespaceInfo>.Children() => Namespaces;
    IEnumerable<UserTypeInfo> IContainerSymbol<UserTypeInfo>.Children() => Types;
    NamespaceInfo? IMemberSymbol<NamespaceInfo?>.ContainingSymbol => ContainingNamespace;

    public override string ToString() {
        if (IsTopNamespace)
            return Name;

        var fullName = Name;
        var currNs = ContainingNamespace;

        while (!currNs.IsTopNamespace) {
            fullName = currNs.Name + "." + fullName;
            currNs = currNs.ContainingNamespace;
        }

        return fullName;
    }

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}