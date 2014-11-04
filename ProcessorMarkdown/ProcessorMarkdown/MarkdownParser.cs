using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public static class MarkdownParser
    {
        private static readonly Regex SectionsCode = new Regex("(`[^`]+`)");
        private static readonly Regex SreeningSections = new Regex(@"[\\].{1}");
        private static readonly Regex SectionsStrong = new Regex(@"__[^_]+__");
        private static readonly Regex Paragraphs = new Regex(@"\n\s*\n");
        private static readonly Regex SectionsEm = new Regex(@"_[^_]+_");
        private static readonly Regex SectionsUnderlinesBetweenLettersAndDigits = new Regex(@"([A-Za-zа-яА-Я0-9]+[_]+[A-Za-zа-яА-Я0-9]+)([_]+[A-Za-zа-яА-Я0-9])*");
        private static readonly Regex SectionsUnpairedDoubleUnderline = new Regex("__{1}");
        
        public static IEnumerable<string> GetParagraphs(string text)
        {
            return Paragraphs.Split(text);
        }

        public static IEnumerable<string> GetSectionsCode(string text)
        {
            return GetWordsMatchesRegex(text, SectionsCode);
        }

        public static IEnumerable<string> GetScreeningSections(string text)
        {
            return GetWordsMatchesRegex(text, SreeningSections);
        }

        public static IEnumerable<string> GetSectionsStrong(string text)
        {
            return GetWordsMatchesRegex(text, SectionsStrong);
        }

        public static IEnumerable<string> GetSectionsEm(string text)
        {
            return GetWordsMatchesRegex(text, SectionsEm);
        }

        public static IEnumerable<string> GetSectionsUnderlinesBetweenLettersAndDigits(string text)
        {
            return GetWordsMatchesRegex(text, SectionsUnderlinesBetweenLettersAndDigits);
        }

        public static IEnumerable<string> GetSectionsUnpairedDoubleUnderlines(string text)
        {
            return GetWordsMatchesRegex(text, SectionsUnpairedDoubleUnderline);
        } 

        private static IEnumerable<string> GetWordsMatchesRegex(string text, Regex regex)
        {
            var result = regex.Matches(text);

            return from object v in result select v.ToString();
        }
    }
}
