namespace NotesApplication.Test;

public static class Convert
{
    public static string ToUnicode(this string text)
    {
        return string.Concat(text.Select(symbol =>
        {
            if (char.IsWhiteSpace(symbol))
            {
                return " ";
            }
            else if (char.IsAscii(symbol))
            {
                return $"{symbol}";
            }
            return @"\u" + $"{(int)symbol:x4}".ToUpper();
        }));
    }
}