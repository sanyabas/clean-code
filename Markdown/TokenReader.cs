using System;
using System.Linq;
using System.Text;

namespace Markdown
{
    public class TokenReader
    {
        public int CurrentPosition { get; set; }
        public char CurrentChar { get; private set; }
        public string Text { get; private set; }

        public TokenReader(string text)
        {
            this.Text = text;
            CurrentPosition = 0;
        }

        public Token ReadUntil(int startPosition, params char[] stopChars)
        {
            var previousPosision = CurrentPosition;
            var result = new StringBuilder();
            for (CurrentPosition = startPosition; CurrentPosition < Text.Length; CurrentPosition++)
            {
                if (stopChars.Contains(Text[CurrentPosition]))
                {

                    return new Token(result.ToString(), previousPosision);
                }
                result.Append(Text[CurrentPosition]);
            }
            return new Token(result.ToString(), previousPosision);
        }

        public Token ReadWhile(params char[] acceptableChars)
        {
            throw new NotImplementedException();
        }
    }
}