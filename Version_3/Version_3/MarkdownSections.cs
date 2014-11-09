using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Version_3
{
    public static class MarkdownSections
    {
        private static readonly Regex CodeSections = new Regex(@"`([^`]+)`");
        private static readonly Regex ParagraphSections =  new Regex(@"\n\s*\n");
        private static readonly Regex ScreeningSections = new Regex(@"[\\].{1}");
        private static readonly Regex StrongSections = new Regex(@"(?<![\\_\w])__(?!_)(((?!__).)*)__(?![\\_\w])");
        private static readonly Regex EmSections = new Regex(@"(?<![\\_\w])_(" + @"((_{2,}|[^_])|([a-zA-Zа-яА-Я0-9]+[_]+[a-zA-Zа-яА-Я0-9]+([_]+[a-zA-Zа-яА-Я0-9]+)*))+" + @")(?<!\\)_(?![_\w])");

        public static Queue<string> GetSreeningSections(string text)
        {
            return new Queue<string>(GetWordsMatchesRegex(text, ScreeningSections));
        }

        public static Queue<string> GetStrongSections(string text)
        {
            return new Queue<string>(GetWordsMatchesRegex(text, StrongSections));
        }

        public static Queue<string> GetEmSections(string text)
        {
            return new Queue<string>(GetWordsMatchesRegex(text, EmSections));
        }

        public static Queue<string> GetCodeSections(string text)
        {
            return new Queue<string>(GetWordsMatchesRegex(text, CodeSections));
        }

        public static IEnumerable<string> GetParagraphSections(string text)
        {
            return ParagraphSections.Split(text).ToList();
        }

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);
            return from object v in result where !string.IsNullOrEmpty(v.ToString()) select v.ToString();
            
        }
    }
}
