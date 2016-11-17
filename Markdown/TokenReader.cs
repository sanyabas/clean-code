using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
        private const char LinkAddressOpeningChar = '(';
        private const char LinkAddressClosingChar = ')';
        public string BaseAddress { get; }
        private static readonly char[] NotPlainCharacters = {'_', '\\', '[',']','(',')'};

        public TokenReader(string text, string baseAddress = null)
        {
            this.text = text;
            BaseAddress = baseAddress;
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
                result.Append(text[currentPosition]);
            }
            return new Token(result.ToString(), previousPosision);
        }

        public Token ReadNextSurroundedToken()
        {
            switch (text[currentPosition])
            {
                case ScreeningChar:
                    return SkipShadedToken();
                case LinkTextOpeningChar:
                    return ReadLink();
                case EmphasisChar:
                    return LookAtNextCharacter();
                default:
                    return ReadSimpleToken();
            }
        }

        public Token ReadSimpleToken()
        {
            var token = ReadUntil(currentPosition, false, NotPlainCharacters);
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
            var attribute = $"href=\"{link}\"";
            textToken.HtmlTag = LinkTag;
            textToken.HtmlAttribute = attribute;
            return textToken;
        }

        public Token LookAtNextCharacter()
        {
            var next = text[currentPosition + 1];
            if (next == EmphasisChar)
                return ReadBoldToken();
            else
                return ReadItalicToken();
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