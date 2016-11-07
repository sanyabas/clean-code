using System;

namespace Markdown
{
    public class TokenReader
    {
        public int CurrentPosition { get; private set; }
        private readonly string text;

        public TokenReader(string text)
        {
            this.text = text;
            CurrentPosition = 0;
        }

        public Token ReadUntil(params char[] stopChars)
        {
            throw new NotImplementedException();
        }

        public Token ReadWhile(params char[] acceptableChars)
        {
            throw new NotImplementedException();
        }
    }
}