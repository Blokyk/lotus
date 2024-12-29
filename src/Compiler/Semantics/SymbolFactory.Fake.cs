using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

internal sealed partial class SymbolFactory
{
    public ErrorTypeInfo CreateMissingType(string name)
        => new(name, _unit);
    public ErrorTypeInfo CreateMissingType(NameNode name)
        => CreateMissingType(name.ToFullString());

    public ErrorTypeInfo CreateMissingType(NameNode name, Scope scope) {
        if (name is IdentNode { Value: var typeName })
            return new(typeName, _unit);

        var nameParts = ((FullNameNode)name).Parts;

        SymbolInfo container = new ErrorSymbolInfo(nameParts[0], _unit);
        var currScope = scope;

        foreach (var part in nameParts.SkipLast(1)) { // don't try to resolve the type name, obviously
            var nextContainer = currScope.Get(part);

            if (scope.Get(part) is NamespaceInfo ns)
                nextContainer = ns;
            else
                nextContainer = new ErrorSymbolInfo(part, _unit) { ContainingSymbol = container };

            container = nextContainer;
            currScope = Scope.From((container as IScope)!);
        }

        return new ErrorTypeInfo(nameParts[^1], _unit) { ContainingSymbol = container };
    }

    public ErrorSymbolInfo CreateMissingSymbol(string name)
        => new(name, _unit);
    public ErrorSymbolInfo CreateMissingSymbol(NameNode name)
        => CreateMissingSymbol(name.ToFullString());

    public ErrorSymbolInfo CreateMissingSymbol(NameNode name, Scope scope) {
        if (name is IdentNode { Value: var singleName })
            return new ErrorSymbolInfo(singleName, _unit);

        var nameParts = ((FullNameNode)name).Parts;

        SymbolInfo container = new ErrorSymbolInfo(nameParts[0], _unit);
        var currScope = scope;

        foreach (var part in nameParts.SkipLast(1)) { // no need to try to resolve the last part
            var nextContainer = currScope.Get(part);

            if (scope.Get(part) is NamespaceInfo ns)
                nextContainer = ns;
            else
                nextContainer = new ErrorSymbolInfo(part, _unit) { ContainingSymbol = container };

            container = nextContainer;
            currScope = Scope.From((container as IScope)!);
        }

        return new ErrorSymbolInfo(nameParts[^1], _unit) { ContainingSymbol = container };
    }
}