using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public class ProcessorMarkdown
    {
        private readonly string textMarkdown;
        public ProcessorMarkdown(string textMarkdown)
        {
            this.textMarkdown = textMarkdown;
        }

        public string GetResultOfProcessing()
        {
            if (string.IsNullOrEmpty(textMarkdown)) return string.Empty;

            return string.Format("<p>{0}</p>", textMarkdown);
        }
    }
}
