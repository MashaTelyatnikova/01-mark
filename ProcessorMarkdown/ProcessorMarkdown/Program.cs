using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var processorMarkDown = new ProcessorMarkdown(File.ReadAllText(fileName));
            var result = processorMarkDown.GetResultOfProcessing();

            File.WriteAllText(Path.ChangeExtension(fileName, "html"), result);
        }
    }
}
