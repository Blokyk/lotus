namespace Lotus.Semantics;

public class TraitInfo(
    string name,
    LocationRange loc,
    SemanticUnit unit
) : UserTypeInfo(name, loc, unit) {
    private Dictionary<string, FunctionInfo> _funcs = [];
    public IReadOnlyCollection<FunctionInfo> Functions => _funcs.Values;

    internal bool TryAddFunction(FunctionInfo func) {
        if (_funcs.TryAdd(func.Name, func))
            return true;

        Logger.Error(new DuplicateSymbol {
            TargetSymbol = func,
            ExistingSymbol = _funcs[func.Name],
            ContainingSymbol = this,
            In = "a trait declaration"
        });
    }
}