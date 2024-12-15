using Lotus.Extras.Graphs;
using Lotus.Semantics;

namespace Lotus.Extras;

public static class ExtraUtils
{
    internal static readonly GraphMaker GraphMaker = new();

    public static string PrintNode(Node node) => ASTUtils.PrintNode(node);

    public static string PrintTuple<T>(Syntax.Tuple<T> tuple, string sep, Func<T, string> transform) where T : ILocalized
        => ASTUtils.PrintTuple<T>(tuple, sep, transform);

    public static string PrintTopLevel(TopLevelNode node) => ASTUtils.PrintNode(node);
    public static string PrintStatement(StatementNode node) => ASTUtils.PrintNode(node);
    public static string PrintValue(ValueNode node) => ASTUtils.PrintNode(node);
    public static string PrintTypeName(TypeDecName typeDec) => ASTUtils.PrintTypeName(typeDec);
    public static string PrintToken(Token token) => ASTUtils.PrintToken(token);
    public static string PrintUnion<T, U>(Union<T, U> u) where T : Node
                                                         where U : Node
        => ASTUtils.PrintUnion(u);

    public static GraphNode ToGraphNode(Token token) => GraphMaker.ToGraphNode(token);
    public static GraphNode ToGraphNode(ValueNode node) => GraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(StatementNode node) => GraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(TopLevelNode node) => GraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(TypeDecName typeDec) => GraphMaker.ToGraphNode(typeDec);
    public static GraphNode UnionToGraphNode<T, U>(Union<T, U> u) where T : Node
                                                                  where U : Node
        => u.Match(ToGraphNode, ToGraphNode);
    public static GraphNode ToGraphNode(Node node)
        => node switch {
            ValueNode vn     => ToGraphNode(vn),
            StatementNode sn => ToGraphNode(sn),
            TopLevelNode tn  => ToGraphNode(tn),
            _                => throw new NotImplementedException(
                                    "There's no ToGraphNode() method for type " + node.GetType().GetDisplayName() + " or any of its base types"
                                )
        };

    public static string PrintDeclarations(SemanticUnit unit)
        => SemanticVisualizer.Print(unit);
}