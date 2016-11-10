using System;
using System.Collections.Generic;

namespace Markdown
{
    public static class TokenReaderExtensions
    {
        public static IEnumerable<Token> ReadTokens(this TokenReader reader)
        {
            while (reader.CurrentPosition < reader.Text.Length)
            {
                if (reader.Text[reader.CurrentPosition] != '_')
                    yield return reader.ReadSimpleToken();
                else
                {
                    yield return reader.ReadItalicToken().SurroundWithHtmlTag("<em>");
                }
            }
        }

        public static Token ReadSimpleToken(this TokenReader reader)
        {
            var token = reader.ReadUntil(reader.CurrentPosition, '_');
            return token;
        }

        public static Token ReadItalicToken(this TokenReader reader)
        {
            var token = reader.ReadUntil(reader.CurrentPosition + 1, '_');
            reader.CurrentPosition++;
            if (reader.Text.EndsWith(token.String))
                return new Token("", 0);
            return token;
        }

        public static Token ReadBoldToken(this TokenReader reader)
        {
            throw new NotImplementedException();
        }
    }
}