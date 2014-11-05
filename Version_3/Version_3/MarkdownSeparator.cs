using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Version_3
{
    public static class MarkdownSeparator
    {
        private static readonly Regex CodeSections = new Regex(@"`([^`]+)`");
        private static readonly Regex ScreeningSections = new Regex(@"[\\].{1}");
        private static readonly Regex ParagraphSections = new Regex(@"\n\s*\n");

        public static List<string> GetCodeSections(string text)
        {
            return GetWordsMatchesRegex(text, CodeSections).ToList();
        }

        public static List<string> GetSreeningSections(string text)
        {
            return GetWordsMatchesRegex(text, ScreeningSections).ToList();
        }

        public static List<string> GetParagraphSections(string text)
        {
            return GetWordsMatchesRegex(text, ParagraphSections).ToList();
        }

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);
           
            return from object word in result select word.ToString().Trim();
        } 
    }

    [TestFixture]
    public static class MarkdownRegex_Tests
    {
        [Test]
        public static void test1()
        {
            var text = "`Masha <<<tt>>t \n\n hahaha` lalalalal `code`";
            var expected = new List<string>() { "`Masha <<<tt>>t \n\n hahaha`", "`code`" };

            Assert.That(MarkdownSeparator.GetCodeSections(text), Is.EqualTo(expected));
        }
    }
}
