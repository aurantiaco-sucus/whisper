namespace Whisper.Structure;

public static class ConstructUtil
{
    public static Element FromList(this IList<Element> list)
    {
        if (!list.Any()) return Element.Nothing.Instance;
        var current = new Construct(list[0], Element.Nothing.Instance);
        var head = current;
        var index = 1;
        while (index < list.Count)
        {
            current.MutateCdr(new Construct(list[index], Element.Nothing.Instance));
            current = (Construct) current.Cdr;
            index++;
        }
        return head;
    }

    public static Element FromList(this IEnumerable<Element> enumerable) =>
        enumerable.ToList().FromList();

    public static IEnumerable<Element> Enumerate(this Element head)
    {
        var current = head;
        if (current is Element.Nothing) yield break;
        while (current is Construct cons)
        {
            yield return cons.Car;
            current = cons.Cdr;
        }
    }

    public static List<Element> ToList(this Element element) => element.Enumerate().ToList();
}