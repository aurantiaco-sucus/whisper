using Whisper.Structure;

namespace Whisper.Interpreter;

public static class Dispatch
{
    public static Dictionary<string, Func<List<Element>, Element>> Functions = new()
    {
        ["+"] = AddDispatch
    };

    private static Element AddDispatch(List<Element> args) => 
        new Element.Number(args.Aggregate(0.0, (acc, x) =>
            acc + ((Element.Number) x).Data));
}