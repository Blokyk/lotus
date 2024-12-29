using Lotus.Semantics.Binding;

namespace Lotus.Semantics;

public abstract record Conversion(
    TypeInfo From,
    TypeInfo To
);

public sealed record IdentityConversion(TypeInfo From, TypeInfo To)
    : Conversion(From, To);