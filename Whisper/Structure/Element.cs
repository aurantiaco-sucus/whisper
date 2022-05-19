namespace Whisper.Structure;

public abstract class Element
{
    public abstract class BoxElement<TData> : Element
    {
        public abstract TData Data { get; }
    }
    
    public abstract class MutableBoxElement<TData> : BoxElement<TData>
    {
        public abstract void Mutate(TData data);
    }
    
    public class Number : BoxElement<double>
    {
        public override double Data { get; }
        
        public Number(double data)
        {
            Data = data;
        }
    }

    public class Symbol : Element
    {
        private readonly Func<string> _ref;
        public string Name => _ref();
        
        public Symbol(Func<string> refFunc)
        {
            _ref = refFunc;
        }
    }
    
    public class Literal : BoxElement<string>
    {
        public override string Data { get; }
        
        public Literal(string data)
        {
            Data = data;
        }
    }

    public class Nothing : Element
    {
        public static Nothing Instance = new Nothing();
    }
}