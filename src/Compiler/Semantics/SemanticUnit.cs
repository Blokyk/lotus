using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

// fixme: none of this handles accessibility
public class SemanticUnit
{
    internal NamespaceInfo Global { get; } = new("<global>");

    private Scope GlobalScope => ((IScope)Global).Scope;

    public IEnumerable<NamespaceInfo> Namespaces => Global.Namespaces;
    public IEnumerable<TypeInfo> GlobalTypes => Global.Types;

    private readonly SymbolFactory _factory;

    public bool IsValid { get; } = true;

    public SemanticUnit(IEnumerable<SyntaxTree> trees) {
        _factory = new(this);

        foreach (var tree in trees)
            IsValid &= TryAddTree(tree);
    }

    public bool TryAddTree(SyntaxTree tree) {
        var ns = GetTreeNamespace(tree);
        Console.WriteLine($"Tree for file '{tree.Location.filename}' is in namespace {ns}");

        var importedScope = GetImportedScope(tree);

        var fileScope = Scope.Combine(importedScope, Scope.From(ns), GlobalScope);

        var isValid = true;

        // we first want to add the declarations to the scope
        // and then fill-in (resolve) the actual type of everything.
        // To implement that, we have a list of pending "fillings"
        // that need to be done; each time we process a new declaration,
        // we just queue up an action that will be executed after every
        // declaration has been added to the scope. only then can we start
        // resolving type names and stuff
        var fillingActions = new List<Func<Scope, bool>>();

        // todo: as a slight enhancement of the above, maybe we can
        // already fill-in the field names (or value names for enums)
        // during the initial declaration, but we'll live the resolving
        // of types and default values and such to later.

        foreach (var node in tree.TopNodes) {
            switch (node) {
                case EnumNode enumNode:
                    var enumType = _factory.GetEmptyEnumSymbol(enumNode);
                    isValid &= ns.TryAdd(enumType);
                    fillingActions.Add(scope => {
                        _factory.FillEnumSymbol(enumType, enumNode, scope);
                        return enumType.IsValid;
                    });
                    break;
                case StructNode structNode:
                    var structType = _factory.GetEmptyStructSymbol(structNode);
                    isValid &= ns.TryAdd(structType);
                    fillingActions.Add(scope => {
                        _factory.FillStructSymbol(structType, structNode, scope);
                        return structType.IsValid;
                    });
                    break;
                case FunctionDeclarationNode funcNode:
                    break; // todo: handle func decl
                default:
                    // the other nodes are just imports, usings, and namespaces
                    // so we can just ignore those for now
                    continue;
            }
        }

        foreach (var action in fillingActions)
            isValid &= action(fileScope);

        return isValid;
    }

    private Scope GetImportedScope(SyntaxTree tree) {
        var usingScopes
            = tree
                .TopNodes
                .OfType<UsingNode>()
                .Select(GetNamespaceForUsing)
                .Select(ns =>
                    ns is null ? Scope.Empty : Scope.From(ns)
                );

        var combinedUsings = Scope.Combine(usingScopes!);

        var imports = tree.TopNodes.OfType<ImportNode>();
        // todo: `from foo import bar, zab` needs specific/synthetic scope
        var combinedImports = Scope.Empty;

        return Scope.Combine(combinedUsings, combinedImports);
    }

    private NamespaceInfo? GetNamespaceForUsing(UsingNode usingNode) {
        var symbol = usingNode.Name.Match(
            strName => throw new NotImplementedException("using from file not implemented yet"),
            qualifiedName => GlobalScope.ResolveQualified(qualifiedName)
        );

        if (symbol is NamespaceInfo ns)
            return ns;

        Logger.Error(new UnknownSymbol(ErrorArea.Semantics) {
            ExpectedKinds = [ "namespace" ],
            SymbolName = usingNode.Name.Match(s => s.Value, n => n.ToFullString()),
            In = "using statement",
            Location = usingNode.Location
        });

        return null;
    }

    private NamespaceInfo GetTreeNamespace(SyntaxTree tree) {
        var nsDecl = tree.TopNodes.OfType<NamespaceNode>().SingleOrDefault();

        if (nsDecl is null)
            return Global;

        var currNs = Global;
        foreach (var part in nsDecl.Name.Parts) {
            if (Scope.From(currNs).Get(part) is not NamespaceInfo nextNs) {
                nextNs = new NamespaceInfo(part);
                _ = currNs.TryAdd(nextNs);
            }

            currNs = nextNs;
        }

        return currNs;
    }

    public override string ToString() => SemanticVisualizer.Format(Global);
}