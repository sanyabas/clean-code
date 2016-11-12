using System.Collections.Generic;

namespace Markdown
{
    public static class TokenReaderExtensions
    {
        private const string ItalicTag = "<em>";
        private const string BoldTag = "<strong>";
        private static readonly char[] NotPlainCharacters = { '_', '\\' };

        public static IEnumerable<Token> ReadTokens(this TokenReader reader)
        {
            while (reader.CurrentPosition < reader.Text.Length)
            {
                switch (reader.Text[reader.CurrentPosition])
                {
                    case '\\':
                        yield return reader.SkipShadedToken();
                        break;
                    case '_':
                        yield return reader.LookAtNextCharacter();
                        break;
                    default:
                        yield return reader.ReadSimpleToken();
                        break;
                }
            }
        }

        public static Token ReadSimpleToken(this TokenReader reader)
        {
            var token = reader.ReadUntil(reader.CurrentPosition, false, NotPlainCharacters);
            return token;
        }

        public static Token ReadItalicToken(this TokenReader reader)
        {
            var token = reader.ReadUntil(reader.CurrentPosition + 1,false, '_');
            reader.CurrentPosition++;
            if (reader.Text.EndsWith(token.Text))
                return new Token("", 0);
            return token;
        }

        public static Token ReadBoldToken(this TokenReader reader)
        {
            var token = reader.ReadUntil(reader.CurrentPosition + 2, false, '_');
            reader.CurrentPosition+=2;
            if (reader.Text.EndsWith(token.Text))
                return new Token("", 0);
            return token;
        }

        public static Token SkipShadedToken(this TokenReader reader)
        {
            var token = reader.ReadUntil(reader.CurrentPosition + 1, true, '\\');
            reader.CurrentPosition += 2;
            return token;
        }

        private static Token LookAtNextCharacter(this TokenReader reader)
        {
            var next = reader.Text[reader.CurrentPosition + 1];
            if (next == '_')
                return reader.ReadBoldToken().SurroundWithHtmlTag(BoldTag);
            else
                return reader.ReadItalicToken().SurroundWithHtmlTag(ItalicTag);
        }
    }
}