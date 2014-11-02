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
        [Test]
        public void return_empty_response_for_empty_input()
        {
            var processor = new ProcessorMarkdown("");
            var actualResult = processor.GetResultOfProcessing();

            Assert.That(actualResult, Is.Empty);
        }

        [Test]
        public void resturn_one_paragraph_for_simple_line()
        {
            var processor = new ProcessorMarkdown("Hello, World!");
            var actualResult = processor.GetResultOfProcessing();
            var expectedResult = "<p>Hello, World!</p>";

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void return_few_paragraphs_for_simple_text()
        {
            var processor = new ProcessorMarkdown("Hello, World!\n\nMy name is Masha.");
            var actualResult = processor.GetResultOfProcessing();
            var expectedResult = "<p>Hello, World!</p><p>My name is Masha.</p>";

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}
