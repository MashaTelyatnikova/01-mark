using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ProcessorMarkdown
{
    [TestFixture]
    public class ProcessorMarkdown_should
    {
        static void Test(string inputText, string expectedResult)
        {
            var processor = new ProcessorMarkdown(inputText);
            var actualResult = processor.GetResultOfProcessing();

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void return_empty_response_for_empty_input()
        {
            Test("", "");
        }

        [Test]
        public void resturn_one_paragraph_for_simple_line()
        {
            Test("Hello, World!", "<p>Hello, World!</p>");
        }

        [Test]
        public void return_few_paragraphs_for_simple_text()
        {
            Test("Hello, World!\n\nMy name is Masha.", "<p>Hello, World!</p><p>My name is Masha.</p>");
        }

        [Test]
        public void return_lines_with_tag_em_for_input_text_contains_underscore()
        {
            Test("Hello, World!\n\nMy name is _Masha_.", "<p>Hello, World!</p><p>My name is <em>Masha</em></p>");
        }
    }
}
