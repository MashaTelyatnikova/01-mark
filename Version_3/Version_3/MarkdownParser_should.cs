using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Version_3
{
    [TestFixture]
    public class MarkdownParser_should
    {
        private static void Test(string input, string expected)
        {
            var actualResult = MarkdownParser.ParseToHtml(input);

            Assert.That(actualResult, Is.EqualTo(expected));
        }

        [TestCase("", "<body></body>")]
        [TestCase(null, "<body></body>")]
        public static void resturn_empty_body_for_empty_or_null(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, World!", "<body><p>Hello, World!</p></body>")]
        [TestCase("Hello!\n\t    \nWorld!", "<body><p>Hello!</p><p>World!</p></body>")]
        public static void return_one_paragraph_for_simple_line(string input, string expected)
        {
            Test(input, expected);
        }
    }
}
