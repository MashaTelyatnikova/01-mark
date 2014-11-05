﻿using System;
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

        [TestCase("Masha _Telyatnikova lalal_", "<body><p>Masha <em>Telyatnikova lalal</em></p></body>")]
        [TestCase("lalaal _la_ dk", "<body><p>lalaal <em>la</em> dk</p></body>")]
        [TestCase("_hahaha_", "<body><p><em>hahaha</em></p></body>")]
        [TestCase(" _hahah_ ", "<body><p> <em>hahah</em> </p></body>")]
        [TestCase(" _hahah_", "<body><p> <em>hahah</em></p></body>")]
        public static void wrap_em_text_surrounded_by_underlines(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, \\r \\_ world\\a", "<body><p>Hello, r _ worlda</p></body>")]
        [TestCase("Hi _my name is\\_", "<body><p>Hi _my name is_</p></body>")]
        [TestCase("Hello, \\_world lalal\\_", "<body><p>Hello, _world lalal_</p></body>")]
        [TestCase("\\r \\_ \\\\", "<body><p>r _ \\</p></body>")]
        public static void screen_characters_after_slash(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, __world__ my name is Masha", "<body><p>Hello, <strong>world</strong> my name is Masha</p></body>")]
        [TestCase("__Hello__", "<body><p><strong>Hello</strong></p></body>")]
        [TestCase(" __Hello__", "<body><p> <strong>Hello</strong></p></body>")]
        [TestCase("  __Hello__  ", "<body><p>  <strong>Hello</strong>  </p></body>")]
        public static void wrap_strong_text_surrounded_by_double_underlines(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Masha _ttt __kk ll__ lo_ hah", "<body><p>Masha <em>ttt <strong>kk ll</strong> lo</em> hah</p></body>")]
        [TestCase("_Masha __Telyatnikova___", "<body><p><em>Masha <strong>Telyatnikova</strong></em></p></body>")]
        public static void allow_include_strong_in_em(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, `world my name `.", "<body><p>Hello, <code>world my name </code>.</p></body>")]
        [TestCase("`Hi`", "<body><p><code>Hi</code></p></body>")]
        [TestCase("lalal `code _my code_ __mmm__ _p __l__ k_` aaa", "<body><p>lalal <code>code _my code_ __mmm__ _p __l__ k_</code> aaa</p></body>")]
        [TestCase("mamamam `code \n\t\t\n lalala` code\n \n papa", "<body><p>mamamam <code>code \n\t\t\n lalala</code> code</p><p> papa</p></body>")]
        public static void wrap_code_text_surrounded_by_backticks(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi, my__name_is_1_2_22", "<body><p>Hi, my__name_is_1_2_22</p></body>")]
        [TestCase("Подчерки_внутри_текста__и__цифр_12_3 не считаются.", "<body><p>Подчерки_внутри_текста__и__цифр_12_3 не считаются.</p></body>")]
        [TestCase("Hello, _my name is_ Masha_12_3__5 __Dasha__", "<body><p>Hello, <em>my name is</em> Masha_12_3__5 <strong>Dasha</strong></p></body>")]
        public static void ignore_underlines_inside_text_and_digits(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hi _my name is", "<body><p>Hi _my name is</p></body>")]
        [TestCase("Hi _my __name is_", "<body><p>Hi <em>my __name is</em></p></body>")]
        [TestCase("__непарные _символы не считаются `выделением.", "<body><p>__непарные _символы не считаются `выделением.</p></body>")]
        public static void ignore_unpaired_characters(string input, string expected)
        {
            Test(input, expected);
        }

        [TestCase("Hello, <p>world </p> \"Hi\".", "<body><p>Hello, &lt;p&gt;world &lt;/p&gt; &quot;Hi&quot;.</p></body>")]
        public static void replace_special_characters_to_html_representation(string input, string expected)
        {
            Test(input, expected);
        }
    }
}
