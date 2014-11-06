using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Version_3
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var result = MarkdownParser.ParseToHtml(File.ReadAllText(fileName));
            File.WriteAllText(Path.ChangeExtension(fileName, "html"), GetHtmlTextWithHeaders(result), Encoding.UTF8);

            var text = "__s _ss s__ _sdk l_";
            var regex = new Regex(@"(?<![\w])__(?!_)(((?!__).)*)__(?![_\w])");
           // var regex = new Regex(@"\s*__[^_]+__\s*");
            var res = regex.Matches(text);

            foreach (var re in res)
            {
                Console.WriteLine(re);
            }
        }

        private static string GetHtmlTextWithHeaders(string text)
        {
            return string.Format("<!DOCTYPE HTML><HTML><HEAD><TITLE> </TITLE><META http-equiv=\"Content-Type\" Content=\"text/html; charset=utf-8\"></HEAD>{0}</HTML>", text);
        }
    }
}
