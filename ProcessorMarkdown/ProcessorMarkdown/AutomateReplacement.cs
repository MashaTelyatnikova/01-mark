using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public class AutomateReplacement
    {
        private enum States
        {
            Initial,
            OpeningUnderscore,
            Screening,
            ScreeningInsideSelection
        }

        private enum TypesCharacters
        {
            Undercsore,
            NotSpecial,
            Slash
        }

        private States currentState;
        private readonly StringBuilder result;
        private readonly StringBuilder selectedWord;

        public AutomateReplacement()
        {
            result = new StringBuilder();
            selectedWord = new StringBuilder();
            currentState = States.Initial;
        }

        private void Reset()
        {

            currentState = States.Initial;
            result.Clear();
            selectedWord.Clear();
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

            return result.ToString();
        }

        private void DoStep(char currentChar)
        {
            var typeCurrentChar = GetTypeChar(currentChar);
            switch (currentState)
            {
                case States.Initial:
                    {
                        result.Append(currentChar);
                        if (typeCurrentChar.Equals(TypesCharacters.Undercsore))
                            currentState = States.OpeningUnderscore;
                        else if (typeCurrentChar.Equals(TypesCharacters.Slash))
                            currentState = States.Screening;
                        break;
                    }
                case States.Screening:
                    {
                        if (typeCurrentChar.Equals(TypesCharacters.Undercsore))
                            result.Remove(result.Length - 1, 1);
                        result.Append(currentChar);
                        currentState = States.Initial;
                        break;
                    }
                case States.OpeningUnderscore:
                    {
                        if (typeCurrentChar.Equals(TypesCharacters.Slash))
                        {
                            result.Append(currentChar);
                            selectedWord.Append(currentChar);
                            currentState = States.ScreeningInsideSelection;
                        }
                        else if (!typeCurrentChar.Equals(TypesCharacters.Undercsore))
                        {
                            selectedWord.Append(currentChar);
                            result.Append(currentChar);
                        }
                        else
                        {
                            WriteSelectedWordWithTag("em");
                            selectedWord.Clear();
                            currentState = States.Initial;

                        }
                        break;
                    }
                case States.ScreeningInsideSelection:
                    {
                        if (typeCurrentChar.Equals(TypesCharacters.Undercsore))
                            selectedWord.Remove(selectedWord.Length - 1, 1);
                        selectedWord.Append(currentChar);
                        currentState = States.OpeningUnderscore;
                        break;
                    }
            }
        }

        private void WriteSelectedWordWithTag(string nameTag)
        {
            result.Remove(result.Length - selectedWord.Length - 1, selectedWord.Length + 1);
            result.AppendFormat("<{0}>{1}</{0}>", nameTag, selectedWord);
        }

        private static TypesCharacters GetTypeChar(char ch)
        {
            switch (ch)
            {
                case '_':
                    return TypesCharacters.Undercsore;
                case '\\':
                    return TypesCharacters.Slash;
                default:
                    return TypesCharacters.NotSpecial;

            }
        }
    }
}
