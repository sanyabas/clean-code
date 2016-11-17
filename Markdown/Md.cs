﻿using System.Linq;

namespace Markdown
{
    public class Md
    {
        private TokenReader tokenizer;
        public string BaseAddress { get; set; }
        public string RenderToHtml(string markdown)
        {
            tokenizer = new TokenReader(markdown, BaseAddress);
            var result = tokenizer.ReadTokens().Select(token => token.Text);
            return string.Join("", result);
        }
    }


}