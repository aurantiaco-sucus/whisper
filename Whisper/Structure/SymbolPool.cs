namespace Whisper.Structure;

public class SymbolPool
{
    private string[] _symbols;
    private int _nextIndex = 0;

    public SymbolPool(int capacity = 0x0010_0000)
    {
        _symbols = new string[capacity];
    }

    public Func<string> Refer(string symbol)
    {
        var index = Array.IndexOf(_symbols, symbol);
        if (index != -1) return () => _symbols[index];
        if (_nextIndex == _symbols.Length)
        {
            var newSymbols = new string[_symbols.Length * 2];
            Array.Copy(_symbols, newSymbols, _symbols.Length);
            _symbols = newSymbols;
        }
        index = _nextIndex;
        _symbols[index] = symbol;
        _nextIndex++;
        return () => _symbols[index];
    }
}