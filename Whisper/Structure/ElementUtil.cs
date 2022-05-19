namespace Whisper.Structure;

public static class ElementUtil
{
    public static void DebugPrint(this Element element)
    {
        switch (element)
        {
            case Element.Nothing:
                Console.Write("NIL");
                break;
            case Element.Number number:
                Console.Write($"#{number.Data}");
                break;
            case Element.Literal literal:
                Console.Write($"\"{literal.Data}\"");
                break;
            case Element.Symbol symbol:
                Console.Write($":{symbol.Name}");
                break;
            case Construct pair:
                var cur = pair;
                Console.Write("[");
                while (cur.Cdr is Construct next)
                {
                    cur.Car.DebugPrint();
                    Console.Write(" ");
                    cur = next;
                }
                cur.Car.DebugPrint();
                if (cur.Cdr is Element.Nothing)
                {
                    Console.Write("]");
                    break;
                }
                Console.Write(" ");
                cur.Cdr.DebugPrint();
                Console.Write("]");
                break;
        }
    }
}