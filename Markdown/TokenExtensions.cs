namespace Markdown
{
    public static class TokenExtensions
    {
        public static Token SurroundWithHtmlTag(this Token token, string tag)
        {
            var closingTag = tag[0] + @"/" + tag.Substring(1);
            var resultText = $"{tag}{token.String}{closingTag}";
            return new Token(resultText,token.StartPosition);
        }
    }
}