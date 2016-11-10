using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using static System.String;

namespace Markdown
{
    public class Md
    {
        private TokenReader tokenizer;
        public string RenderToHtml(string markdown)
        {
            tokenizer = new TokenReader(markdown);
            var result = tokenizer.ReadTokens().Select(token => token.String);
            return Join("", result);
        }
    }


}