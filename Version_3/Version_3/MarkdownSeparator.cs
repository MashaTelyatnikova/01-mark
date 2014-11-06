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
        private static readonly Regex StrongSections = new Regex(@"(?<![\w])__(?!_)(((?!__).)*)__(?![_\w])");
        private static readonly Regex EmSections = new Regex(@"(?<=[^_]+|\b)\s*_[^_]+(_)*[^_]+_{1}\s*");
        private static readonly Regex UnderlinesBetweenLettersAndDigits =
            new Regex(@"[a-zA-Zа-яА-Я0-9]+[_]+[a-zA-Zа-яА-Я0-9]+([_]+[a-zA-Zа-яА-Я0-9]+)*");

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
            return ParagraphSections.Split(text).ToList();
        }

        public static List<string> GetUnderlinwBetweenLettersAndDigits(string text)
        {
            return GetWordsMatchesRegex(text, UnderlinesBetweenLettersAndDigits).ToList();
        }

        public static List<string> GetStrongSections(string text)
        {
            return GetWordsMatchesRegex(text, StrongSections).ToList();
        }

        public static List<string> GetEmSections(string text)
        {
            return GetWordsMatchesRegex(text, EmSections).ToList();
        }

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);

            return from object word in result select word.ToString().Trim();
        }
    }
}
