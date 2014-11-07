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

        [TestCase("", "")]
        [TestCase(null, "")]
        public void return_empty_response_for_empty_or_null_input(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, World!", "<p>Hello, World!</p>")]
        public void resturn_one_paragraph_for_simple_line(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, World!\n   \nMy name is Masha.", "<p>Hello, World!</p><p>My name is Masha.</p>")]
        [TestCase("Hello, World!\n   \nMy name is Masha.\n   \t  \nHi  \n\n", "<p>Hello, World!</p><p>My name is Masha.</p><p>Hi  </p><p></p>")]
        public void return_few_paragraphs_for_simple_text(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("<p>Masha</p> \"Hello!\" ©M", "<p>&lt;p&gt;Masha&lt;/p&gt; &quot;Hello!&quot; &copy;M</p>")]
        public void return_correct_conversion_special_characters(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, _world_ !", "<p>Hello, <em>world</em> !</p>")]
        [TestCase("Hi, my _name is_ Masha, _how are you ?", "<p>Hi, my <em>name is</em> Masha, _how are you ?</p>")]
        public void conclude_in_tag_em_text_inside_underlines(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, my name is `Masha _Telyatnikova_ hfhfh`.", "<p>Hello, my name is <code>Masha _Telyatnikova_ hfhfh</code>.</p>")]
        [TestCase("Hello, `world _my name is_ hahaha` _my code_", "<p>Hello, <code>world _my name is_ hahaha</code> <em>my code</em></p>")]
        [TestCase("Hello, `world _my name is_ hahaha` _my 'code_", "<p>Hello, <code>world _my name is_ hahaha</code> <em>my 'code</em></p>")]
        [TestCase("Hello, my name is `Masha _Telyatnikova_ hfhfh.", "<p>Hello, my name is `Masha <em>Telyatnikova</em> hfhfh.</p>")]
        public void conclude_in_tag_code_text_inside_backquotes(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, my name \\_Masha \\r", "<p>Hello, my name _Masha r</p>")]
        [TestCase("My _name \\_is\\_ Masha.", "<p>My _name _is_ Masha.</p>")]
        [TestCase("Hello, \\_World!\\_\n\nMy _name \\_is\\_ Masha_.", "<p>Hello, _World!_</p><p>My <em>name _is_ Masha</em>.</p>")]
        [TestCase("Hello, \\_World!\\_", "<p>Hello, _World!_</p>")]
        public void correctly_replace_escape(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi, my name is __Masha__!", "<p>Hi, my name is <strong>Masha</strong>!</p>")]
        [TestCase("Hello, _world_!\n\nMy _name_ is __Masha__! Im _Ok_)", "<p>Hello, <em>world</em>!</p><p>My <em>name</em> is <strong>Masha</strong>! Im <em>Ok</em>)</p>")]
        public void conclude_in_tag_strong_text_inside_double_underlines(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi, _my name is __Masha__, im student_", "<p>Hi, <em>my name is <strong>Masha</strong>, im student</em></p>")]
        [TestCase("_Hello __ddd kl__ , __kkk__ _ world__12_3 _www_!", "<p><em>Hello <strong>ddd kl</strong> , <strong>kkk</strong> </em> world__12_3 <em>www</em>!</p>")]
        public void return_correct_result_for_nested_in_em_strong_selection(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением и должны оставаться символами подчерка.", "<p>Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением и должны оставаться символами подчерка.</p>")]
        [TestCase("Hello, world__12_3 _www_!", "<p>Hello, world__12_3 <em>www</em>!</p>")]
        public void ignore_underlines_between_letters_and_digits(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi, _my name is", "<p>Hi, _my name is</p>")]
        [TestCase("__непарные _символы не считаются `выделением.", "<p>__непарные _символы не считаются `выделением.</p>")]
        [TestCase("Hi, `code lalala` `jj", "<p>Hi, <code>code lalala</code> `jj</p>")]
        public void ignore_unpaired_symbols(string input, string expected)
        {
            Test(input, expected);
        }

        [Test]
        public void tets()
        {
            Test("my _name is __hello__ _w _", "<p>my <em>name is <strong>hello</strong> _w</em></p>");
        }
    }
}
