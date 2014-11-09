using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Version_3
{
    public static class MarkdownSections
    {
        private static readonly Regex CodeSections = new Regex(@"`([^`]+)`");
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
            var lines = text.Split('\n').ToList();
            var currentParagraph = new StringBuilder();
            var usedItems = Enumerable.Range(0, lines.Count)
                                        .Select(i => false)
                                        .ToList();
            
            for (var i = 0; i < lines.Count - 2; )
            {
                var subset = lines.GetRange(i, 3);
                var curIndex = i;
                var paragraphAccumulated = false;
                currentParagraph.Append(subset[0]);
                
                
                if (string.IsNullOrEmpty(subset[1].Trim()))
                {
                    paragraphAccumulated = true;
                    i += 2;
                }
                else if (string.IsNullOrWhiteSpace(subset[1]) && string.IsNullOrEmpty(subset[2].Trim()))
                {
                    paragraphAccumulated = true;
                    i += 3;
                }
                else
                {
                    currentParagraph.Append("\n");
                    i += 1;
                }

                if (paragraphAccumulated)
                {
                    yield return currentParagraph.ToString();
                    currentParagraph.Clear();
                }

                for (var j = curIndex; j < i; j++)
                    usedItems[j] = true;
            }

            var indexesUnusedItems =
                usedItems.Select((i, j) => Tuple.Create(i, j)).Where(i => i.Item1 == false).Select(i => i.Item2).ToList();
            
            foreach (var index in indexesUnusedItems)
                currentParagraph.Append(lines[index]);

            yield return currentParagraph.ToString();
        }

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);
            return from object v in result where !string.IsNullOrEmpty(v.ToString()) select v.ToString();

        }
    }
}
