namespace Lotus.Semantics;

public interface ISymbolVisitor<out T>
{
    T Visit(SymbolInfo symbol);

    T Visit(ErrorSymbolInfo symbol);
    T Visit(ErrorTypeInfo symbol);

    T Visit(NamespaceInfo symbol);

    T Visit(TypedSymbolInfo symbol);

    T Visit(TypeInfo symbol);
    T Visit(ArrayTypeInfo symbol);
    T Visit(UnionTypeInfo symbol);
    T Visit(UserTypeInfo symbol);

    internal T Visit(BoolTypeInfo symbol);
    internal T Visit(CharTypeInfo symbol);
    internal T Visit(NumberTypeInfo symbol);
    internal T Visit(StringTypeInfo symbol);
    internal T Visit(VoidTypeInfo symbol);

    T Visit(EnumTypeInfo symbol);
    T Visit(EnumValueInfo symbol);

    T Visit(StructTypeInfo symbol);
    T Visit(FieldInfo symbol);

    T Visit(FunctionInfo symbol);
    T Visit(ParameterInfo symbol);
    T Visit(LocalInfo symbol);
}