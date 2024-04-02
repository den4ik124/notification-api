namespace NotesApplication.Core;

public static class Convert
{
    public static string ToUnicode(this string text)
        => string.Concat(text.Select(c =>
            char.IsWhiteSpace(c)
                ? " "
                : @"\u" + $"{(int)c:x4}".ToUpper()));
}