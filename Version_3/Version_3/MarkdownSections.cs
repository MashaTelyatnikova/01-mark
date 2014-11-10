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

        public static IEnumerable<string> GetSreeningSections(string text)
        {
            return GetWordsMatchesRegex(text, ScreeningSections);
        }

        public static IEnumerable<string> GetStrongSections(string text)
        {
            return GetWordsMatchesRegex(text, StrongSections).Select(i => CutExtremeCharactresInSection(i, 2));
        }

        public static IEnumerable<string> GetEmSections(string text)
        {
            return GetWordsMatchesRegex(text, EmSections).Select(i => CutExtremeCharactresInSection(i, 1));
        }

        public static IEnumerable<string> GetCodeSections(string text)
        {
            return GetWordsMatchesRegex(text, CodeSections).Select(i => CutExtremeCharactresInSection(i, 1));
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

        public static IEnumerable<string> WrapSectionsInOriginalSeparator(IEnumerable<string> sections,
           string originalSeparator)
        {
            return sections.Select(section => string.Format("{0}{1}{0}", originalSeparator, section));
        }

        public static string ReplaceSectionsOnSpecialCharacter(string text, IEnumerable<string> sections, char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c, specialCharacter + ""));
        }

        public static IEnumerable<string> WrapSectionsInTag(IEnumerable<string> sections, string tag)
        {
            return sections.Select(code => string.Format("<{0}>{1}</{0}>", tag, code));
        }

        public static string ReplaceSymbolOnSections(string text, IEnumerable<string> codeSections, char symbol)
        {
            var queueCodeSections = new Queue<string>(codeSections);
            var result = new StringBuilder();
            foreach (var c in text)
            {
                if (c == symbol)
                {
                    result.Append(queueCodeSections.Dequeue());
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private static string CutExtremeCharactresInSection(string text, int count)
        {
            return text.Substring(0 + count, text.Length - 2 * count);
        }

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);
            return from object v in result where !string.IsNullOrEmpty(v.ToString()) select v.ToString();

        }
    }
}
