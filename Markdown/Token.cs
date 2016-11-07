namespace Markdown
{
    public class Token
    {
        public readonly string String;
        public readonly int start;
        public readonly int finish;

        public Token(string s, int start)
        {
            String = s;
            this.start = start;
            finish = start + s.Length;
        }
    }
}