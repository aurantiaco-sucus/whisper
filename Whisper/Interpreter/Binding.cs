using Whisper.Structure;

namespace Whisper.Interpreter;

public class Binding
{
    private Func<Element> _getter;
    private Action<Element> _setter;
    
    public Binding(Func<Element> getter, Action<Element> setter)
    {
        _getter = getter;
        _setter = setter;
    }
    
    public Element Get()
    {
        return _getter();
    }
    
    // TODO: See the TODO of Frame::Lookup for details about what's wrong with this function.
    public void Set(Element value)
    {
        _setter(value);
    }
}