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
        public void should_return_empty_response_for_empty_input()
        {
            var processor = new ProcessorMarkdown("");
            var actualResult = processor.GetResultOfProcessing();

            Assert.That(actualResult, Is.Empty);
        }
    }
}
