namespace Whisper.Structure;

public class Parser
{
    private string _source;
    private int _index;
    
    public Parser(string source)
    {
        _source = source;
        _index = 0;
        _stack.Push(new Layer(new List<Element>(), list => list.FromList()));
    }
    
    private char Current => _source[_index];
    private char Next => _source[_index + 1];

    private record Layer(List<Element> Elements, Func<List<Element>, Element> Packager);
    private readonly Stack<Layer> _stack = new();
    
    private Layer CurrentLayer => _stack.Peek();
    
    private readonly SymbolPool _symbolPool = new();

    public void Parse()
    {
        while (_index < _source.Length)
        {
            if (Current.IsSeparator())
            {
                _index++;
                continue;
            }
            switch (Current)
            {
                case '(':
                    _stack.Push(new Layer(
                        new List<Element>(), 
                        list => list.FromList()));
                    _index++;
                    break;
                case ')':
                    var frame = _stack.Pop();
                    CurrentLayer.Elements.Add(frame.Packager(frame.Elements));
                    _index++;
                    break;
                case '\'':
                    if (Next != '(')
                    {
                        CurrentLayer.Elements.Add(new Element[]
                            {
                                new Element.Symbol(_symbolPool.Refer("quote")),
                                new Element.Symbol(_symbolPool.Refer(NextWord()))
                            }
                            .FromList());
                        break;
                    }
                    _stack.Push(new Layer(
                        new List<Element>(),
                        list => new[] 
                            {
                                new Element.Symbol(_symbolPool.Refer("quote")), 
                                list.FromList()
                            }
                            .FromList()));
                    _index += 2;
                    break;
                case '\"':
                    CurrentLayer.Elements.Add(new Element.Literal(NextString()));
                    break;
                default:
                    CurrentLayer.Elements.Add(NextElement());
                    break;
            }
        }
    }

    private string NextWord()
    {
        var begin = _index;
        while (!Current.IsTerminator())
        {
            _index++;
        }
        return _source[begin.._index];
    }

    private string NextString()
    {
        var result = "";
        _index++;
        while (Current != '\"')
        {
            result += Current;
            _index++;
        }
        _index++;
        return result;
    }

    private Element NextElement()
    {
        var word = NextWord();
        return word.TryParseNumber() is { } number
            ? new Element.Number(number)
            : new Element.Symbol(_symbolPool.Refer(word));
    }

    public Element Export()
    {
        while (_stack.Count > 1)
        {
            var frame = _stack.Pop();
            CurrentLayer.Elements.Add(frame.Packager(frame.Elements));
        }
        return CurrentLayer.Elements.FromList();
    }
}