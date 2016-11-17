using System;

namespace Markdown
{
    public static class TokenExtensions
    {
        public static Token SurroundWithHtmlTag(this Token token, string tag, string attribute="")
        {
            if (string.IsNullOrEmpty(tag))
                return token;
            var closingTag = tag[0] + @"/" + tag.Substring(1);
            if (!string.IsNullOrEmpty(attribute))
                tag = $"{tag.Substring(0, tag.Length-1)} {attribute}{tag[tag.Length - 1]}";
            var resultText = $"{tag}{token.Text}{closingTag}";
            return new Token(resultText,token.StartPosition);
        }

        public static Token SurroundWithHtmlTag(this Token token)
        {
            return token.SurroundWithHtmlTag(token.HtmlTag, token.HtmlAttribute);
        }
    }
}