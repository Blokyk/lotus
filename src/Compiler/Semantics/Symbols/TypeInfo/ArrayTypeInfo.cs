using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public sealed class ArrayTypeInfo(TypeInfo itemType, SemanticUnit unit)
    : TypeInfo(unit)
    , IScope
{
    public TypeInfo ItemType { get; } = itemType;

    private ArrayScope? _scope = null;
    Scope IScope.Scope => _scope ??= new ArrayScope(this);
    // todo: when we have (self-)traits, add scope for core methods
    private sealed class ArrayScope : Scope {
        private ArrayTypeInfo @this;
        private FieldInfo _length;

        public ArrayScope(ArrayTypeInfo @this) {
            this.@this = @this;
            var intType = @this.Unit.IntType;

            _length = new FieldInfo("length", intType, @this, LocationRange.NULL, @this.Unit);
        }
        public override SymbolInfo? Get(string name)
            => name == "length"
             ? _length
             : null;
    }

    public override T Accept<T>(ISymbolVisitor<T> visitor)
        => visitor.Visit(this);
}