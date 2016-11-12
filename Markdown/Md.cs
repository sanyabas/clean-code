using System.Linq;

namespace Markdown
{
    public class Md
    {
        private TokenReader tokenizer;
        public string RenderToHtml(string markdown)
        {
            tokenizer = new TokenReader(markdown);
            var result = tokenizer.ReadTokens().Select(token => token.Text);
            return string.Join("", result);
        }
    }


}