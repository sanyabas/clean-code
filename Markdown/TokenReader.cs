using System;
using System.Collections.Generic;
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
        private static readonly char[] NotPlainCharacters = {'_', '\\'};

        public TokenReader(string text)
        {
            this.text = text;
            currentPosition = 0;
        }

        public Token ReadSimpleToken()
        {
            var token = ReadUntil(currentPosition, false, NotPlainCharacters);
            return token;
        }

        public Token ReadItalicToken()
        {
            var token = ReadUntil(currentPosition + 1, false, '_');
            currentPosition++;
            if (text.EndsWith(token.Text))
                return new Token("", 0);
            return token;
        }

        public Token ReadBoldToken()
        {
            var token = ReadUntil(currentPosition + 2, false, '_');
            currentPosition += 2;
            if (text.EndsWith(token.Text))
                return new Token("", 0);
            return token;
        }

        public Token SkipShadedToken()
        {
            var token = ReadUntil(currentPosition + 1, true, '\\');
            currentPosition += 2;
            return token;
        }

        public Token LookAtNextCharacter()
        {
            var next = text[currentPosition + 1];
            if (next == '_')
                return ReadBoldToken().SurroundWithHtmlTag(BoldTag);
            else
                return ReadItalicToken().SurroundWithHtmlTag(ItalicTag);
        }

        public Token ReadUntil(int startPosition, bool isScreening, params char[] stopChars)
        {
            var previousPosision = currentPosition;
            var result = new StringBuilder();
            for (currentPosition = startPosition; currentPosition < text.Length; currentPosition++)
            {
                if (stopChars.Contains(text[currentPosition]))
                {
                    if (isScreening && text[currentPosition] == '\\')
                        result.Append(text[currentPosition + 1]);
                    return new Token(result.ToString(), previousPosision);
                }
                result.Append(text[currentPosition]);
            }
            return new Token(result.ToString(), previousPosision);
        }

        public bool IsNotEnded()
        {
            return currentPosition < text.Length;
        }

        public Token ReadNextSurroundedToken()
        {
            switch (text[currentPosition])
            {
                case '\\':
                    return SkipShadedToken();
                case '_':
                    return LookAtNextCharacter();
                default:
                    return ReadSimpleToken();
            }
        }
}
}