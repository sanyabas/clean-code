using System.Collections.Generic;

namespace Markdown
{
    public static class TokenReaderExtensions
    {
        public static IEnumerable<Token> ReadTokens(this TokenReader reader)
        {
            while (reader.IsNotEnded())
                yield return reader.ReadNextSurroundedToken();
        }
    }
}