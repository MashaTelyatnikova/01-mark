using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public class AutomateReplacement
    {
        private enum States
        {
            Initial,
            OpeningUnderscore
        }

        private enum TypesCharacters
        {
            Undercsore,
            NotSpecial
        }

        private States currentState;
        private string result = string.Empty;
        private string selectedWord = string.Empty;

        public AutomateReplacement()
        {
            currentState = States.Initial;
        }

        private void Reset()
        {
            result = "";
            currentState = States.Initial;
            selectedWord = string.Empty;
        }

        public string GetResult(string line)
        {
            Reset();
            var currentPosition = 0;
            while (currentPosition != line.Length)
            {
                DoStep(line[currentPosition]);
                currentPosition++;
            }

            return result;
        }

        private void DoStep(char currentChar)
        {
            var typeCurrentChar = GetTypeChar(currentChar);
            switch (currentState)
            {
                case States.Initial:
                    {
                        result += currentChar;
                        if (typeCurrentChar.Equals(TypesCharacters.Undercsore))
                            currentState = States.OpeningUnderscore;

                        break;
                    }
                case States.OpeningUnderscore:
                    {
                        if (!typeCurrentChar.Equals(TypesCharacters.Undercsore))
                        {
                            selectedWord += currentChar;
                            result += currentChar;
                        }
                        else
                        {
                            WriteSelectedWordWithTag("em");
                            selectedWord = "";
                            currentState = States.Initial;
                        }
                        break;
                    }
            }
        }

        private void WriteSelectedWordWithTag(string nameTag)
        {
            result = result.Substring(0, result.Length - selectedWord.Length - 1)
                + string.Format("<{0}>{1}</{0}>", nameTag, selectedWord);
        }

        private static TypesCharacters GetTypeChar(char ch)
        {
            switch (ch)
            {
                case '_':
                    return TypesCharacters.Undercsore;

                default:
                    return TypesCharacters.NotSpecial;

            }
        }
    }
}
