using Lotus.Syntax;

namespace Lotus.Semantics.Binding;

internal static class ScopeUtils
{
    public static SymbolInfo? ResolveQualified(this Scope scope, NameNode name)
        => scope.ResolveQualified(name.Parts.Select(t => t.Representation));

    public static T? ResolveQualified<T>(this Scope scope, NameNode name)
        where T : SymbolInfo
    {
        var s = scope.ResolveQualified(name);

        if (s is null) {
            Logger.Error(new UnknownSymbol {
                SymbolName = name.ToFullString(),
                ExpectedKinds = [ SymbolUtils.GetKindString<T>() ],
                Location = name.Location
            });

            return null;
        }

        if (s is not T t) {
            Logger.Error(new UnexpectedSymbolKind {
                TargetSymbol = s,
                ExpectedKinds = [ SymbolUtils.GetKindString<T>() ],
                Location = name.Location
            });

            return null;
        }

        return t;
    }

}