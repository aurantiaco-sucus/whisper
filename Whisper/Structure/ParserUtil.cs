namespace Whisper.Structure;

public static class ParserUtil
{
    private static readonly char[] Separators = { ' ', '\t', '\r', '\n' };
    private static readonly char[] Terminators = Separators.Concat(new[] { '(', ')', '[', ']' }).ToArray();
    
    public static bool OneOf<T>(this T obj, params T[] values) => values.Contains(obj);

    public static bool IsSeparator(this char ch) => ch.OneOf(Separators);
    public static bool IsTerminator(this char ch) => ch.OneOf(Terminators);
    
    public static double? TryParseNumber(this string str)
    {
        if (double.TryParse(str, out var result))
            return result;
        return null;
    }
}