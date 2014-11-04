using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ProcessorMarkdown
{
    [TestFixture]
    public class Processor_should
    {
        private static void Test(string input, string expected)
        {
            var actualResult = Processor.GerHtmlResultProcess(input);

            Assert.That(actualResult, Is.EqualTo(expected));

        }

        [TestCase("", "")]
        [TestCase(null, "")]
        public static void resturn_empty_srting_for_empty_or_null(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, World!", "<p>Hello, World!</p>")]
        [TestCase("Hello!\n\t    \nWorld!", "<p>Hello!</p><p>World!</p>")]
        public static void return_one_paragraph_for_simple_line(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, <p>world </p> \"Hi\".", "<p>Hello, &lt;p&gt;world &lt;/p&gt; &quot;Hi&quot;.</p>")]
        public static void replace_special_characters_to_html_representation(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, `world my name `.", "<p>Hello, <code>world my name </code>.</p>")]
        [TestCase("Hi `code code\n\n \t  ` hello.\n\nMy name", "<p>Hi <code>code code\n\n \t  </code> hello.</p><p>My name</p>")]
        public static void return_section_with_code_for_text_with_backquotes(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, \\r \\_ world\\a", "<p>Hello, r _ worlda</p>")]
        public static void escapes_characters_after_slash(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, __world__ my name is Masha", "<p>Hello, <strong>world</strong> my name is Masha</p>")]
        public static void return_section_with_strong_for_text_with_double_underline(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, _world lala_ lala", "<p>Hello, <em>world lala</em> lala</p>")]
        public static void return_section_with_em_for_text_with_underline(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, _world __ff__ lala_ lala", "<p>Hello, <em>world <strong>ff</strong> lala</em> lala</p>")]
        public static void correctly_handle_situation_nested_selections(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi, my__name_is_1_2_22", "<p>Hi, my__name_is_1_2_22</p>")]
        [TestCase("Hello, _my name is_ Masha_12_3__5 __Dasha__", "<p>Hello, <em>my name is</em> Masha_12_3__5 <strong>Dasha</strong></p>")]
        public static void ignore_underline_between_letters_and_digits(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi _my name is", "<p>Hi _my name is</p>")]
        [TestCase("Hi _my __name is_", "<p>Hi <em>my __name is</em></p>")]
        public static void ignore_unpaired_selections(string input, string expected)
        {
            Test(input, expected);
        }
    }
}
