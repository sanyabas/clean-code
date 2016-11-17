namespace Markdown
{
    public class Token
    {
        private readonly int finishPosition;
        public string Text { get; }
        public int StartPosition { get; }

        public string HtmlTag { get; set; }
        public string HtmlAttribute { get; set; }

        public Token(string text, int start)
        {
            Text = text;
            StartPosition = start;
            finishPosition = start + text.Length;
        }

        public override string ToString()
        {
            return $"[{StartPosition}:{finishPosition}] {Text}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Token))
                return false;
            var other = (Token)obj;
            return Text == other.Text && StartPosition == other.StartPosition && HtmlTag==other.HtmlTag && HtmlAttribute==other.HtmlAttribute;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}