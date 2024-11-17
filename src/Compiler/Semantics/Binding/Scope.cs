using System.Collections;

namespace Lotus.Semantics.Binding;

internal class Scope : IEnumerable<SymbolInfo>
{
    private readonly List<SymbolInfo> _symbols = [];
    private readonly Dictionary<string, INamedSymbol> _namedSymbols = [];

    public Scope() {}

    public Scope(SymbolInfo symbol) {
        switch (symbol) {
            case NamespaceInfo ns:
                AddRange(ns.Namespaces);
                AddRange(ns.Types);
                break;
            case StructTypeInfo st:
                AddRange(st.Fields);
                break;
            case EnumTypeInfo en:
                AddRange(en.Values);
                break;
        }
    }

    public void Add(SymbolInfo symbol) {
        _symbols.Add(symbol);

        if (symbol is INamedSymbol namedSymbol)
            _namedSymbols.Add(namedSymbol.Name, namedSymbol);
    }

    public void AddNamespace(NamespaceInfo ns) {
        AddRange(ns.Namespaces);
        AddRange(ns.Types);
    }

    public void AddRange(IEnumerable<SymbolInfo> symbols) {
        _symbols.AddRange(symbols);

        foreach (var namedSymbol in symbols.OfType<INamedSymbol>())
            _namedSymbols.Add(namedSymbol.Name, namedSymbol);
    }

    public bool TryFindName(string name, [NotNullWhen(true)] out INamedSymbol? symbol)
        => _namedSymbols.TryGetValue(name, out symbol);

    public IEnumerator<SymbolInfo> GetEnumerator() => _symbols.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}