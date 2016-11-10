namespace Markdown
{
    public class Token
    {
        public string String { get; }
        public int StartPosition { get; }
        public int FinishPosition { get; }

        public Token(string text, int start)
        {
            String = text;
            StartPosition = start;
            FinishPosition = start + text.Length;
        }

        public override string ToString()
        {
            return $"[{StartPosition}:{FinishPosition}] {String}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Token))
                return false;
            var other = (Token)obj;
            return String == other.String && StartPosition == other.StartPosition;
        }

        public override int GetHashCode()
        {
            return String.GetHashCode();
        }
    }
}