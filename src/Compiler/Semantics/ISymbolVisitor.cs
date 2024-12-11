namespace Lotus.Semantics;

public interface ISymbolVisitor<out T>
{
    T Visit(SymbolInfo symbol);

    T Visit(NamespaceInfo symbol);

    T Visit(TypeInfo symbol);
    T Visit(ArrayTypeInfo symbol);
    T Visit(UnionTypeInfo symbol);
    T Visit(MissingTypeInfo symbol);
    T Visit(UserTypeInfo symbol);

    T Visit(EnumTypeInfo symbol);
    T Visit(EnumValueInfo symbol);

    T Visit(StructTypeInfo symbol);
    T Visit(FieldInfo symbol);

    T Visit(FunctionInfo symbol);
    T Visit(ParameterInfo symbol);
    T Visit(LocalInfo symbol);
}