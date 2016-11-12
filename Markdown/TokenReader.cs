using System;
using System.Linq;
using System.Text;

namespace Markdown
{
    public class TokenReader
    {
        public int CurrentPosition { get; set; }
        public string Text { get; }

        public TokenReader(string text)
        {
            Text = text;
            CurrentPosition = 0;
        }

        public Token ReadUntil(int startPosition, bool isScreening, params char[] stopChars)
        {
            var previousPosision = CurrentPosition;
            var result = new StringBuilder();
            for (CurrentPosition = startPosition; CurrentPosition < Text.Length; CurrentPosition++)
            {
                if (stopChars.Contains(Text[CurrentPosition]))
                {
                    if (isScreening && Text[CurrentPosition] == '\\')
                        result.Append(Text[CurrentPosition + 1]);
                    return new Token(result.ToString(), previousPosision);
                }
                result.Append(Text[CurrentPosition]);
            }
            return new Token(result.ToString(), previousPosision);
        }
    }
}