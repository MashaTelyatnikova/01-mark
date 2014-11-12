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

            var i = 0;
            while (i < lines.Count - 2)
            {
                var subset = lines.GetRange(i, 3);
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

                if (!paragraphAccumulated) continue;

                yield return new Section("", CutExtraSpacesInParagraphSection(currentParagraph.ToString()));
                currentParagraph.Clear();
            }

            for (var j = i; j < lines.Count; ++j)
                currentParagraph.Append(lines[j]);

            yield return new Section("", CutExtraSpacesInParagraphSection(currentParagraph.ToString()));
        }

        private static string CutExtraSpacesInParagraphSection(string paragraph)
        {
            return Regex.Replace(paragraph.Trim(), @"\s+", " ");
        }

        public static string ReplaceSectionsWithMarkersOnSpecialCharacter(string text, IEnumerable<Section> sections, char specialCharacter)
        {
            return sections.Aggregate(text, (current, c) => current.Replace(c.LineWithMarkers, specialCharacter + ""));
        }

        public static string ReplaceSymbolOnContentSections(string text, IEnumerable<string> contentSections, char symbol)
        {
            var queueCodeSections = new Queue<string>(contentSections);
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
