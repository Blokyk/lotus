using Lotus.Syntax;

namespace Lotus.Semantics;

public class SemanticUnit
{
    private readonly NamespaceInfo _global = new("<global>");

    public IEnumerable<NamespaceInfo> Namespaces => _global.Namespaces;
    public IEnumerable<TypeInfo> GlobalTypes => _global.Types;

    public SemanticUnit(IEnumerable<SyntaxTree> trees) {
        
    }
}