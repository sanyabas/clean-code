using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdown
{
    public static class TokenExtensions
    {
        public static Token SurroundWithHtmlTag(this Token token, string tag, string attributes=null)
        {
            if (string.IsNullOrEmpty(tag))
                return token;
            var closingTag = tag[0] + @"/" + tag.Substring(1);
            if (!string.IsNullOrEmpty(attributes))
                tag = $"{tag.Substring(0, tag.Length-1)} {attributes}{tag[tag.Length - 1]}";
            var resultText = $"{tag}{token.Text}{closingTag}";
            return new Token(resultText,token.StartPosition);
        }

        public static Token SurroundWithHtmlTag(this Token token)
        {
            return token.SurroundWithHtmlTag(token.HtmlTag, TransformAttributesToStrings(token.HtmlAttributes));
        }

        public static string TransformAttributesToStrings(Dictionary<string, string> attributes)
        {
            return string.Join(" ",attributes.Select(pair => $"{pair.Key}=\"{pair.Value}\""));
        }
    }
}