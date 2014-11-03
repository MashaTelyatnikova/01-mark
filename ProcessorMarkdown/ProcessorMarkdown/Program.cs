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

            File.WriteAllText(Path.ChangeExtension(fileName, "html"), GetHtmlTextWithHeaders(result), Encoding.UTF8);
        }

        private static string GetHtmlTextWithHeaders(string text)
        {
            return string.Format("<!DOCTYPE HTML><HTML><HEAD><TITLE> </TITLE><META http-equiv=\"Content-Type\" content=\"text/html; charset=utf8\"></HEAD><BODY>{0}</BODY></HTML>", text);
        }
    }
}
