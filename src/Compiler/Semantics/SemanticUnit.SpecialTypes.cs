namespace Lotus.Semantics;

public partial class SemanticUnit
{
    private UserTypeInfo _void, _bool, _char, _string, _int, _uint, _long, _ulong, _float, _double;

    public TypeInfo VoidType => _void;
    public TypeInfo BoolType => _bool;
    public TypeInfo CharType => _char;
    public TypeInfo StringType => _string;
    public TypeInfo IntType => _int;
    public TypeInfo UIntType => _uint;
    public TypeInfo LongType => _long;
    public TypeInfo ULongType => _ulong;
    public TypeInfo FloatType => _float;
    public TypeInfo DoubleType => _double;

    [MemberNotNull(
        nameof(_void), nameof(_bool), nameof(_char),
        nameof(_string), nameof(_int), nameof(_uint),
        nameof(_long), nameof(_ulong), nameof(_float), nameof(_double))]
    private void InitAndAddSpecialTypes(NamespaceInfo global) {
        _void = new VoidTypeInfo(this);
        _bool = new BoolTypeInfo(this);
        _char = new CharTypeInfo(this);
        _string = new StringTypeInfo(this);
        _int = new NumberTypeInfo(NumberKind.Int, this);
        _uint = new NumberTypeInfo(NumberKind.UInt, this);
        _long = new NumberTypeInfo(NumberKind.Long, this);
        _ulong = new NumberTypeInfo(NumberKind.ULong, this);
        _float = new NumberTypeInfo(NumberKind.Float, this);
        _double = new NumberTypeInfo(NumberKind.Double, this);

        global.TryAdd(_void);
        global.TryAdd(_bool);
        global.TryAdd(_char);
        global.TryAdd(_string);
        global.TryAdd(_int);
        global.TryAdd(_uint);
        global.TryAdd(_long);
        global.TryAdd(_ulong);
        global.TryAdd(_float);
        global.TryAdd(_double);
    }
}