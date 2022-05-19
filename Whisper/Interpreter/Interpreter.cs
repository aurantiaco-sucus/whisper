using System.Linq.Expressions;
using Whisper.Structure;

namespace Whisper.Interpreter;

public class Interpreter
{
    private class IpFrame
    {
        public EnvFrame Env { get; }
        public List<Element> Ins { get; set; }
        public int InsPtr = 0;
        public List<Element> ResultList { get; } = new();
        public Action<IpFrame, IpFrame?> Callback { get; }

        public IpFrame(EnvFrame env, List<Element> ins, Action<IpFrame, IpFrame?>? callback = null)
        {
            Env = env;
            Ins = ins;
            Callback = callback ?? ((last, cur) =>
            {
                cur!.ResultList.Add(last.ResultList.Last());
            });
        }
    }
    
    private readonly Stack<IpFrame> _stack = new();
    private IpFrame CurrentFrame => _stack.Peek();

    private readonly Dictionary<string, Element> _foundation;
    private Element? _globalResult = null;
    
    private bool FrameExhausted => CurrentFrame.InsPtr >= CurrentFrame.Ins.Count;
    private Element CurrentInstruction => CurrentFrame.Ins[CurrentFrame.InsPtr];
    
    private Element CurrentInstructionAndAdvance()
    {
        var result = CurrentInstruction;
        CurrentFrame.InsPtr++;
        return result;
    }

    public Interpreter(Dictionary<string, Element> foundation)
    {
        _foundation = foundation;
    }

    public Element Interpret(Element element) => InterpretList(new[] {element});
    public Element InterpretList(Element elements) => InterpretList(elements.Enumerate());

    public Element InterpretList(IEnumerable<Element> elements)
    {
        _stack.Push(
            new IpFrame(new EnvFrame(null, _foundation), 
                new List<Element>(),
                (last, _) =>
                {
                    _globalResult = last.ResultList.Last();
                }));
        CurrentFrame.Ins = elements.ToList();
        Launch();
        return _globalResult ?? Element.Nothing.Instance;
    }

    private void Launch()
    {
        while (_stack.Any())
        {
            if (FrameExhausted)
            {
                var last =_stack.Pop();
                last.Callback.Invoke(last, _stack.Any() ? CurrentFrame : null);
                continue;
            }
            switch (CurrentInstructionAndAdvance())
            {
                case Construct cons:
                    _stack.Push(new IpFrame(CurrentFrame.Env, cons.ToList(), (last, cur) =>
                    {
                        var op = last.ResultList[0];
                        if (op is Element.Nothing)
                        {
                            // Ready for dispatch.
                            cur!.ResultList.Add(
                                Dispatch.Functions[((Element.Symbol) last.Ins[0]).Name]
                                    (last.ResultList.Skip(1).ToList()));
                            return;
                        }
                        var args = last.ResultList.Skip(1).ToList();
                        if (op is not Construct opCons)
                            throw new Exception("Expected a construct");
                        var lambda =  opCons.TryParseLambda();
                        if (lambda is not {} lambdaN)
                            throw new Exception("Expected a lambda");
                        var (paramList, body) = lambdaN;
                        if (paramList.Count != args.Count) 
                            throw new Exception("Parameters and arguments do not match");
                        var newEnv = new EnvFrame(cur!.Env, new Dictionary<string, Element>(
                            paramList.Zip(args, (param, arg) => 
                                new KeyValuePair<string, Element>(param.Name, arg))));
                        _stack.Push(new IpFrame(newEnv, body));
                    }));
                    break;
                case Element.Literal literal:
                    CurrentFrame.ResultList.Add(literal);
                    break;
                case Element.Nothing nothing:
                    CurrentFrame.ResultList.Add(nothing);
                    break;
                case Element.Number number:
                    CurrentFrame.ResultList.Add(number);
                    break;
                case Element.Symbol variable:
                    if (Dispatch.Functions.ContainsKey(variable.Name))
                    {
                        CurrentFrame.ResultList.Add(Element.Nothing.Instance);
                        break;
                    }
                    CurrentFrame.ResultList.Add(
                        CurrentFrame.Env.Lookup(variable.Name)?.Get() 
                        ?? Element.Nothing.Instance);
                    break;
            }
        }
    }
}