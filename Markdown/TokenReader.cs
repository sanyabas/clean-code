using System.Linq;
using System.Text;

namespace Markdown
{
    public class TokenReader
    {
        private int currentPosition;
        private readonly string text;
        private const string ItalicTag = "<em>";
        private const string BoldTag = "<strong>";
        private const string LinkTag = "<a>";
        private const char EmphasisChar = '_';
        private const char ScreeningChar = '\\';
        private const char LinkTextOpeningChar = '[';
        private const char LinkTextClosingChar = ']';
        private const char LinkAddressClosingChar = ')';
        private const char SpaceChar = ' ';
        private const char CaretChar = '\r';
        private const char NewLineChar = '\n';
        private const char HeaderChar = '#';
        public string BaseAddress { get; }
        public string HtmlClass { get; }
        private static readonly char[] NonPlainCharacters = { '_', '\\', '[', ']', '(', ')'};

        private static readonly char[] ExtendedNonPlainCharacters =
            NonPlainCharacters.Concat(new[] {SpaceChar, CaretChar}).ToArray();

        public TokenReader(string text, string baseAddress = null, string htmlClass = null)
        {
            this.text = text;
            BaseAddress = baseAddress;
            HtmlClass = htmlClass;
        }

        public Token ReadUntil(int startPosition, bool isScreening, params char[] stopChars)
        {
            var previousPosision = currentPosition;
            var result = new StringBuilder();
            for (currentPosition = startPosition; currentPosition < text.Length; currentPosition++)
            {
                if (stopChars.Contains(text[currentPosition]))
                {
                    if (isScreening && text[currentPosition] == ScreeningChar)
                        result.Append(text[currentPosition + 1]);
                    return new Token(result.ToString(), previousPosision);
                }
                if (text[currentPosition] == ' ' && text[currentPosition + 1] == ' ')
                {
                    currentPosition += 2;
                    return new Token(result.ToString(),previousPosision);
                }
                result.Append(text[currentPosition]);
            }
            return new Token(result.ToString(), previousPosision);
        }

        public Token ReadWhile(int startPosition, bool isScreening, params char[] acceptableChars)
        {
            var previousPosition = currentPosition;
            var result = new StringBuilder();
            for (currentPosition = startPosition; currentPosition < text.Length; currentPosition++)
            {
                if (!acceptableChars.Contains(text[currentPosition]))
                {
                    if (isScreening && text[currentPosition] == ScreeningChar)
                        result.Append(text[currentPosition + 1]);
                    return new Token(result.ToString(), previousPosition);
                }
                result.Append(text[currentPosition]);
            }
            return new Token(result.ToString(),previousPosition);
        }

        public Token ReadNextSurroundedToken()
        {
            Token result;
            switch (text[currentPosition])
            {
                case ScreeningChar:
                    result = SkipShadedToken();
                    break;
                case LinkTextOpeningChar:
                    result = ReadLink();
                    break;
                case EmphasisChar:
                    result = LookAtNextCharacter(EmphasisChar);
                    break;
                case CaretChar:
                    result = ReadWindowsLineBreakToken();
                    break;
                case NewLineChar:
                    result = ReadLinuxLineBreakToken();
                    break;
                case HeaderChar:
                    result = ReadHeader();
                    break;
                default:
                    result = ReadSimpleToken();
                    break;
            }
            if (!string.IsNullOrEmpty(HtmlClass))
                result.HtmlAttributes.Add("class", HtmlClass);
            return result;
        }

        public Token ReadHeader()
        {
            var headerChars = ReadWhile(currentPosition, false, HeaderChar);
            var spaces = ReadWhile(currentPosition, false, SpaceChar);
            var headerLevel = headerChars.Text.Length;
            var header = ReadUntil(currentPosition, false, HeaderChar, NewLineChar, CaretChar);
            var headerClosing = ReadWhile(currentPosition + 1, false, HeaderChar);
            header.HtmlTag = FormHeaderTag(headerLevel);
            return header;
        }

        public string FormHeaderTag(int headerLevel)
        {
            return $"<H{headerLevel}>";
        }

        public Token ReadLinuxLineBreakToken()
        {
            if (text.Substring(currentPosition - 2, 2) != "  ")
                return ReadUntil(currentPosition, false, NonPlainCharacters);
            currentPosition++;
            return new Token("<br />",currentPosition-2);
        }

        public Token ReadWindowsLineBreakToken()
        {
            if (text.Substring(currentPosition - 2, 2) != "  ")
                return ReadUntil(currentPosition, false, NonPlainCharacters);
            currentPosition+=2;
            return new Token("<br />", currentPosition-2);
        }

        public Token ReadSimpleToken()
        {
            var token = ReadUntil(currentPosition, false, ExtendedNonPlainCharacters);
            return token;
        }

        public Token SkipShadedToken()
        {
            var token = ReadUntil(currentPosition + 1, true, ScreeningChar);
            currentPosition += 2;
            return token;
        }

        public Token ReadLink()
        {
            var textToken = ReadUntil(currentPosition + 1, false, LinkTextClosingChar);
            var linkToken = ReadUntil(currentPosition + 2, false, LinkAddressClosingChar);
            currentPosition++;
            var link = !linkToken.Text.StartsWith("http://") ? CombineUrl(BaseAddress, linkToken.Text) : linkToken.Text;
            textToken.HtmlTag = LinkTag;
            textToken.HtmlAttributes.Add("href", link);
            return textToken;
        }

        public Token LookAtNextCharacter(char previous)
        {
            var next = text[currentPosition + 1];
            return next == EmphasisChar ? ReadBoldToken() : ReadItalicToken();
        }

        public Token ReadItalicToken()
        {
            var token = ReadUntil(currentPosition + 1, false, EmphasisChar);
            currentPosition++;
            if (text.EndsWith(token.Text))
                return new Token("", 0);
            token.HtmlTag = ItalicTag;
            return token;
        }

        public Token ReadBoldToken()
        {
            var token = ReadUntil(currentPosition + 2, false, EmphasisChar);
            currentPosition += 2;
            if (text.EndsWith(token.Text))
                return new Token("", 0);
            token.HtmlTag = BoldTag;
            return token;
        }

        public bool IsNotEnded()
        {
            return currentPosition < text.Length;
        }

        public string CombineUrl(string baseUrl, string relativeUrl)
        {
            return $"{baseUrl.TrimEnd('/')}/{relativeUrl.TrimStart('/')}";

        }
    }
}