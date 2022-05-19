using Whisper.Structure;

namespace Whisper.Interpreter;

public static class InterpreterUtil
{
    public static (List<Element.Symbol> paramList, List<Element> body)? TryParseLambda(this Construct cons)
    {
        var list = cons.ToList();
        if (list.Count < 3) return null;
        if (list[0] is not Element.Symbol {Name: "lambda"}) return null;
        if (list[1] is not Construct paramCons) return null;
        var paramList = paramCons.ToList();
        if (paramList.Any(param => param is not Element.Symbol)) return null;
        var body = list.Skip(2).ToList();
        return (paramList.Select(elem => (Element.Symbol) elem).ToList(), body);
    }
}