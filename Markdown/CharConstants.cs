using System.Linq;

namespace Markdown
{
    public static class CharConstants
    {
        public const char Emphasis = '_';
        public const char Screening = '\\';
        public const char LinkTextOpening = '[';
        public const char LinkTextClosing = ']';
        public const char LinkAddressClosing = ')';
        public const char Space = ' ';
        public const char Caret = '\r';
        public const char NewLine = '\n';
        public const char Tabulation = '\t';
        public const char Header = '#';
        public static readonly char[] NonPlainCharacters = { '_', '\\', '[', ']', '(', ')' };
        public static readonly char[] ExtendedNonPlainCharacters =
            NonPlainCharacters.Concat(new[] { NewLine, Caret }).ToArray();
    }
}