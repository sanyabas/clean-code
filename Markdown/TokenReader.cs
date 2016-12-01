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
        public string BaseAddress { get; }
        public string CssClass { get; }
        private readonly Dictionary<char, Func<Token>> charsToFunctions;

        public TokenReader(string text, string baseAddress = null, string cssClass = null)
        {
            this.text = text;
            BaseAddress = baseAddress;
            CssClass = cssClass;
            charsToFunctions = new Dictionary<char, Func<Token>>
            {
                [CharConstants.Screening] = SkipShadedToken,
                [CharConstants.LinkTextOpening] = ReadLink,
                [CharConstants.Emphasis] = LookAtNextCharacter,
                [CharConstants.Caret] = ReadWindowsLineBreakToken,
                [CharConstants.NewLine] = ReadLinuxLineBreakToken,
                [CharConstants.Header] = ReadHeader
            };
        }
        
        public Token ReadUntil(int startPosition, bool isScreening, params char[] stopChars)
        {
            var previousPosision = currentPosition;
            var result = new StringBuilder();
            for (currentPosition = startPosition; currentPosition < text.Length; currentPosition++)
            {
                if (IsPrecoding())
                {
                    result.Append(text.Substring(currentPosition, 2));
                    break;
                }
                if (stopChars.Contains(text[currentPosition]))
                {
                    if (isScreening && text[currentPosition] == CharConstants.Screening)
                        result.Append(text[currentPosition + 1]);
                    break;
                }
                if (currentPosition<text.Length-2 && text.Substring(currentPosition,2)=="  ")
                {
                    currentPosition += 2;
                    break;
                }
                result.Append(text[currentPosition]);
            }
            return new Token(result.ToString(), previousPosision);
        }

        private bool IsPrecoding()
        {
            return currentPosition + 2 < text.Length && text[currentPosition + 1] == CharConstants.NewLine && (text[currentPosition + 2] == CharConstants.Space || text[currentPosition + 2] == '\t');
        }

        public Token ReadWhile(int startPosition, bool isScreening, params char[] acceptableChars)
        {
            var previousPosition = currentPosition;
            var result = new StringBuilder();
            for (currentPosition = startPosition; currentPosition < text.Length; currentPosition++)
            {
                if (!acceptableChars.Contains(text[currentPosition]))
                {
                    if (isScreening && text[currentPosition] == CharConstants.Screening)
                        result.Append(text[currentPosition + 1]);
                    return new Token(result.ToString(), previousPosition);
                }
                result.Append(text[currentPosition]);
            }
            return new Token(result.ToString(),previousPosition);
        }

        public Token ReadNextToken()
        {
            var currentChar = text[currentPosition];
            var result = !charsToFunctions.ContainsKey(currentChar) ? ReadSimpleToken() : charsToFunctions[currentChar]();
            result.AddCssClass(CssClass);
            return result;

        }

        public Token ReadHeader()
        {
            var headerChars = ReadWhile(currentPosition, false, CharConstants.Header);
            var spaces = ReadWhile(currentPosition, false, CharConstants.Space);
            var headerLevel = headerChars.Text.Length;
            var header = ReadUntil(currentPosition, false, CharConstants.Header, CharConstants.NewLine, CharConstants.Caret);
            var headerClosing = ReadWhile(currentPosition + 1, false, CharConstants.Header);
            header.HtmlTags.Add(FormHeaderTag(headerLevel));
            return header;
        }

        public string FormHeaderTag(int headerLevel)
        {
            return $"<H{headerLevel}>";
        }

        public Token ReadLinuxLineBreakToken()
        {
            if (text.Substring(currentPosition - 2, 2) != "  ")
                return ReadUntil(currentPosition, false, CharConstants.NonPlainCharacters);
            currentPosition++;
            return new Token("<br />",currentPosition-2);
        }

        public Token ReadWindowsLineBreakToken()
        {
            if (text.Substring(currentPosition - 2, 2) != "  ")
            {
                var nextFourCharacters = text.Substring(currentPosition + 2, 4);
                if (nextFourCharacters == "    " || nextFourCharacters[0] == '\t')
                    return ReadPreCode();
                else
                    return ReadUntil(currentPosition, false, CharConstants.NonPlainCharacters);
            }
            currentPosition+=2;
            return new Token("<br />", currentPosition-2);
        }

        public Token ReadPreCode()
        {
            var offset=1;
            if (text[currentPosition+2] == CharConstants.Space)
                offset = 4;
            var token = ReadUntil(currentPosition + offset + 2, false, CharConstants.Caret, CharConstants.NewLine);
            while (text[currentPosition] == CharConstants.Caret)
            {
                var nextFourCharacters = text.Substring(currentPosition + 2, 4);
                if (nextFourCharacters == "    " || nextFourCharacters[0] == CharConstants.Tabulation)
                {
                    var tempToken = ReadUntil(currentPosition + 4, false, CharConstants.Caret, CharConstants.NewLine);
                    token = new Token(token.Text + tempToken.Text, token.StartPosition);
                }
                else
                {
                    token.HtmlTags.AddRange(new [] { HtmlTags.Code, HtmlTags.Pre});
                    return token;
                }
            }
            return token;
        }

        public Token ReadSimpleToken()
        {
            var token = ReadUntil(currentPosition, false, CharConstants.ExtendedNonPlainCharacters);
            return token;
        }

        public Token SkipShadedToken()
        {
            var token = ReadUntil(currentPosition + 1, true, CharConstants.Screening);
            currentPosition += 2;
            return token;
        }

        public Token ReadLink()
        {
            var textToken = ReadUntil(currentPosition + 1, false, CharConstants.LinkTextClosing);
            var linkToken = ReadUntil(currentPosition + 2, false, CharConstants.LinkAddressClosing);
            currentPosition++;
            var link = !linkToken.Text.StartsWith("http://") ? CombineUrl(BaseAddress, linkToken.Text) : linkToken.Text;
            textToken.HtmlTags.Add(HtmlTags.Link);
            textToken.HtmlAttributes.Add("href", link);
            return textToken;
        }

        public Token LookAtNextCharacter()
        {
            var next = text[currentPosition + 1];
            return next == CharConstants.Emphasis ? ReadBoldToken() : ReadItalicToken();
        }

        public Token ReadItalicToken()
        {
            var token = ReadUntil(currentPosition + 1, false, CharConstants.Emphasis);
            currentPosition++;
            if (text.EndsWith(token.Text))
                return new Token("", 0);
            token.HtmlTags.Add(HtmlTags.Italic);
            return token;
        }

        public Token ReadBoldToken()
        {
            var token = ReadUntil(currentPosition + 2, false, CharConstants.Emphasis);
            currentPosition += 2;
            if (text.EndsWith(token.Text))
                return new Token("", 0);
            token.HtmlTags.Add(HtmlTags.Bold);
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