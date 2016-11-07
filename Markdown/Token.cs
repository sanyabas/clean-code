namespace Markdown
{
    public class Token
    {
        public readonly string String;
        public readonly int StartPosition;
        public readonly int FinishPosition;

        public Token(string text, int start)
        {
            String = text;
            StartPosition = start;
            FinishPosition = start + text.Length;
        }
    }
}