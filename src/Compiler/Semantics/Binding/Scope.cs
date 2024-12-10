using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal interface IScope {
    internal Scope Scope { get; }
}

internal abstract class Scope : IScope
{
    public static readonly Scope Empty = new EmptyScope();
    private sealed class EmptyScope : Scope {
        public override SymbolInfo? Get(string _) => null;
    }

    // fixme: add accessibility checks
    public abstract SymbolInfo? Get(string name);

    Scope IScope.Scope => this;

    public SymbolInfo? ResolveQualified(NameNode name)
        => ResolveQualified(name.Parts.Select(t => t.Representation));
    public SymbolInfo? ResolveQualified(IEnumerable<string> parts) {
        SymbolInfo? currSymbol = null;

        var currScope = this;
        foreach (var part in parts) {
            currSymbol = currScope.Get(part);

            if (currSymbol is null)
                return null;

            if (currSymbol is IScope { Scope: var nextScope })
                currScope = nextScope;
        }

        return currSymbol;
    }

    public static Scope From(IScope scoper) => scoper.Scope;

    // todo: maybe special case combining with another CombinedScope
    // todo: do TryGetCount and check if it's just one, in which case just return the single scope
    public static Scope Combine(params IEnumerable<Scope> scopes)
        => new CombinedScope(scopes);
    // public static Scope Combine(params IEnumerable<IScope> scopes)
    //     => new CombinedScope(scopes.Select(s => s.Scope));
    private sealed class CombinedScope(params IEnumerable<Scope> scopes) : Scope
    {
        readonly Scope[] _scopes = scopes.ToArray();
        public override SymbolInfo? Get(string name) {
            foreach (var scope in _scopes) {
                if (scope.Get(name) is SymbolInfo found)
                    return found;
            }

            return null;
        }
    }
}