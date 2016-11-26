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

        public static Token SurroundWithHtmlTags(this Token token, Dictionary<string, string> tags)
        {
            var resultToken = token;
            foreach (var tag in tags)
                resultToken = new Token($"{tag.Key}{resultToken.Text}{tag.Value}", token.StartPosition);
            return resultToken;
        }
        //public static Token SurroundWithHtmlTag(this Token token)
        //{
        //    return token.SurroundWithHtmlTag(token.HtmlTag, TransformAttributesToStrings(token.HtmlAttributes));
        //}

        public static Token SurroundWithHtmlTags(this Token token)
        {
            return SurroundWithHtmlTags(token,TransformTagsToStrings(token, token.HtmlTags, TransformAttributesToStrings(token.HtmlAttributes)));
        }

        public static string TransformAttributesToStrings(Dictionary<string, string> attributes)
        {
            return string.Join(" ",attributes.Select(pair => $"{pair.Key}=\"{pair.Value}\""));
        }

        public static Dictionary<string, string> TransformTagsToStrings(this Token token, IEnumerable<string> tags,
            string attributes)
        {
            if (!string.IsNullOrEmpty(attributes))
                attributes = " " + attributes;
            return tags.ToDictionary(tag => $"{tag.Substring(0, tag.Length - 1)}{attributes}{tag[tag.Length - 1]}",
                tag => $"{tag[0]}/{tag.Substring(1)}");
        }

    }
}