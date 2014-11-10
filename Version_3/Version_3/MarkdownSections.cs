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

        public static IEnumerable<Section> GetSreeningSections(string text)
        {
            return GetWordsMatchesRegex(text, ScreeningSections).Select(section => new Section(section, ""));
        }

        public static IEnumerable<Section> GetStrongSections(string text)
        {
            return GetWordsMatchesRegex(text, StrongSections).Select(i => new Section(i, CutExtremeCharactresInText(i, 2)));
        }

        public static IEnumerable<Section> GetEmSections(string text)
        {
            return GetWordsMatchesRegex(text, EmSections).Select(i => new Section(i, CutExtremeCharactresInText(i, 1)));
        }

        public static IEnumerable<Section> GetCodeSections(string text)
        {
            return GetWordsMatchesRegex(text, CodeSections).Select(i => new Section(i, CutExtremeCharactresInText(i, 1)));
        }

        public static IEnumerable<Section> GetParagraphSections(string text)
        {
            var lines = text.Split('\n').ToList();
            var currentParagraph = new StringBuilder();
            var usedItems = new bool[lines.Count];

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
                    yield return new Section("", Regex.Replace(currentParagraph.ToString().Trim(), @"\s+", " "));
                    currentParagraph.Clear();
                }

                for (var j = curIndex; j < i; j++)
                    usedItems[j] = true;
            }

            var indexesUnusedItems = usedItems
                                                .Select((value, index) => Tuple.Create(value, index))
                                                .Where(i => i.Item1 == false)
                                                .Select(i => i.Item2)
                                                .ToList();

            foreach (var index in indexesUnusedItems)
                currentParagraph.Append(lines[index]);
            

            yield return new Section("", Regex.Replace(currentParagraph.ToString().Trim(), @"\s+", " "));
        }

        public static string ReplaceSectionsWithMarksOnSpecialCharacter(string text, IEnumerable<Section> sections, char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c.LineWithMarkers, specialCharacter + ""));
        }

        public static IEnumerable<Section> WrapSectionsWithoutMarksInTag(IEnumerable<Section> sections, string tag)
        {
            return sections.Select(code => new Section(code.LineWithMarkers, string.Format("<{0}>{1}</{0}>", tag, code.LineWithoutMarkers)));
        }

        public static string ReplaceSymbolOnSectionsWithoutMarks(string text, IEnumerable<Section> sections, char symbol)
        {
            var queueCodeSections = new Queue<Section>(sections);
            var result = new StringBuilder();
            foreach (var c in text)
            {
                if (c == symbol)
                {
                    result.Append(queueCodeSections.Dequeue().LineWithoutMarkers);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static string ReplaceScreeningSections(string text)
        {
            var screeningSections = GetSreeningSections(text);
            return screeningSections.Aggregate(text, (current, c) => current.Replace(c.LineWithMarkers, c.LineWithMarkers.Substring(1)));
        }

        private static string CutExtremeCharactresInText(string text, int count)
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
