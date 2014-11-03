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
            var actualResult = processor.GetHtmlResultOfProcessing();

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
            Test("Hello, World!\n   \nMy name is Masha.", "<p>Hello, World!</p><p>My name is Masha.</p>");
        }

        [Test]
        public void return_lines_with_tag_em_for_input_text_contains_underscore()
        {
            Test("Hello, World!\n\nMy name is _Masha_.", "<p>Hello, World!</p><p>My name is <em>Masha</em>.</p>");
        }

        [Test]
        public void return_initial_paragraph_that_contains_one_underscore()
        {
            Test("Hello, World!\n\nMy name is _Masha.", "<p>Hello, World!</p><p>My name is _Masha.</p>");
        }

        [Test]
        public void return_correctly_screened_underscore()
        {
            Test("Hello, \\_World!\\_", "<p>Hello, _World!_</p>");
        }

        [Test]
        public void return_correctly_screened_underscore_inside_underscore()
        {
            Test("Hello, \\_World!\\_\n\nMy _name \\_is\\_ Masha_.", "<p>Hello, _World!_</p><p>My <em>name _is_ Masha</em>.</p>");
        }

        [Test]
        public void return_correct_conversion_special_characters()
        {
            Test("<p>Masha</p> \"Hello!\" ©M", "<p>&lt;p&gt;Masha&lt;/p&gt; &quot;Hello!&quot; &copy;M</p>");
        }

        [Test]
        public void test()
        {
            Test("My _name \\_is\\_ Masha.", "<p>My _name _is_ Masha.</p>");
        }

        [Test]
        public void test1()
        {
            Test("Hello, `world _my name is_ hahaha` _my code_", "<p>Hello, <code>world _my name is_ hahaha</code> <em>my code</em></p>");
        }

        [Test]
        public void test2()
        {
            Test("Hello, `world _my name is_ hahaha` _my 'code_", "<p>Hello, <code>world _my name is_ hahaha</code> <em>my 'code</em></p>");
        }

        [Test]
        public void test3()
        {
            Test("Hello, world__12_3 _www_!", "<p>Hello, world__12_3 <em>www</em>!</p>");
        }

        [Test]
        public void test4()
        {
            Test("_Hello __ddd kl__ , __kkk__ _ world__12_3 _www_!", "<p><em>Hello <strong>ddd kl</strong> , <strong>kkk</strong> </em> world__12_3 <em>www</em>!</p>");
        }

        [Test]
        public void test5()
        {
            Test("__непарные _символы не считаются `выделением.", "<p>__непарные _символы не считаются `выделением.</p>");
        }

        [Test]
        public void return_text_with_replaced_special_characters()
        {
            Test("Hello <p> Masha </p> \"Hi\" ©", "<p>Hello &lt;p&gt; Masha &lt;/p&gt; &quot;Hi&quot; &copy;</p>");
        }

        [Test]
        public void return_correct_sections_with_code()
        {
            Test("Hello, my name is `Masha _Telyatnikova_ hfhfh`.", "<p>Hello, my name is <code>Masha _Telyatnikova_ hfhfh</code>.</p>");
        }

        [Test]
        public void ignore_unpaired_quote()
        {
            Test("Hello, my name is `Masha _Telyatnikova_ hfhfh.", "<p>Hello, my name is `Masha <em>Telyatnikova</em> hfhfh.</p>");
        }

        [Test]
        public void correctly_replace_escape()
        {
            Test("Hello, my name \\_Masha \\r", "<p>Hello, my name _Masha r</p>");
        }

        [Test]
        public void replace_double_underline_on_tag_strong()
        {
            Test("Hello, my name_dj __Masha dd__ Hi_\n\n_msmsm __hello__ mmm_", "<p>Hello, my name_dj <strong>Masha dd</strong> Hi_</p><p><em>msmsm <strong>hello</strong> mmm</em></p>");
        }
    }
}
