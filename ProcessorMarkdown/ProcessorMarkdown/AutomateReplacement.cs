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

        private delegate void OperationDelegate(char c);

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

        private readonly Dictionary<Tuple<States, TypesCharacters>, States> transictions = new Dictionary<Tuple<States, TypesCharacters>, States>()
            {
                {Tuple.Create(States.Initial, TypesCharacters.Undercsore), States.OpeningUnderscore},
                {Tuple.Create(States.Initial, TypesCharacters.Slash), States.Screening},
                {Tuple.Create(States.Initial, TypesCharacters.NotSpecial), States.Initial},
                {Tuple.Create(States.Screening, TypesCharacters.NotSpecial), States.Initial},
                {Tuple.Create(States.Screening, TypesCharacters.Slash), States.Initial},
                {Tuple.Create(States.Screening, TypesCharacters.Undercsore), States.Initial},
                {Tuple.Create(States.OpeningUnderscore, TypesCharacters.NotSpecial),States.OpeningUnderscore },
                {Tuple.Create(States.OpeningUnderscore, TypesCharacters.Slash), States.ScreeningInsideSelection},
                {Tuple.Create(States.OpeningUnderscore, TypesCharacters.Undercsore), States.Initial},
                {Tuple.Create(States.ScreeningInsideSelection, TypesCharacters.NotSpecial), States.OpeningUnderscore},
                {Tuple.Create(States.ScreeningInsideSelection, TypesCharacters.Slash), States.OpeningUnderscore},
                {Tuple.Create(States.ScreeningInsideSelection, TypesCharacters.Undercsore), States.OpeningUnderscore}
            };

        private readonly Dictionary<Tuple<States, TypesCharacters>, OperationDelegate> actions;
        public AutomateReplacement()
        {
            result = new StringBuilder();
            selectedWord = new StringBuilder();
            currentState = States.Initial;
            actions = new Dictionary<Tuple<States, TypesCharacters>, OperationDelegate>()
            {
                 {Tuple.Create(States.Initial,  TypesCharacters.Undercsore), AppendNewCharToResult},
                 {Tuple.Create(States.Initial, TypesCharacters.Slash), AppendNewCharToResult},
                 {Tuple.Create(States.Initial, TypesCharacters.NotSpecial), AppendNewCharToResult},
                 {Tuple.Create(States.Screening, TypesCharacters.NotSpecial), AppendNewCharToResult},
                 {Tuple.Create(States.Screening, TypesCharacters.Slash), AppendNewCharToResult},
                 {Tuple.Create(States.Screening, TypesCharacters.Undercsore), HandleScreeningUnderscore},
                 {Tuple.Create(States.OpeningUnderscore, TypesCharacters.NotSpecial), AppendCharToResultAndToSelectedWord},
                 {Tuple.Create(States.OpeningUnderscore, TypesCharacters.Slash), AppendCharToResultAndToSelectedWord},
                 {Tuple.Create(States.OpeningUnderscore, TypesCharacters.Undercsore), PushSelectedWordToResult},
                 {Tuple.Create(States.ScreeningInsideSelection, TypesCharacters.NotSpecial), AppendCharToResultAndToSelectedWord},
                 {Tuple.Create(States.ScreeningInsideSelection, TypesCharacters.Slash), AppendCharToResultAndToSelectedWord},
                 {Tuple.Create(States.ScreeningInsideSelection, TypesCharacters.Undercsore), HandleScreeningUnderscoreInsideSelection}
            };
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

        private void Reset()
        {

            currentState = States.Initial;
            result.Clear();
            selectedWord.Clear();
        }

        private void DoStep(char currentChar)
        {
            
            var typeCurrentChar = GetTypeChar(currentChar);
            var action = actions[Tuple.Create(currentState, typeCurrentChar)];
            action(currentChar);
            currentState = transictions[Tuple.Create(currentState, typeCurrentChar)];
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

        private void AppendNewCharToResult(char ch)
        {
            result.Append(ch);
        }

        private void AppendCharToResultAndToSelectedWord(char c)
        {
            result.Append(c);
            selectedWord.Append(c);
        }

        private void HandleScreeningUnderscore(char sreeningChar)
        {
            result.Remove(result.Length - 1, 1);
            result.Append(sreeningChar);
        }

        private void HandleScreeningUnderscoreInsideSelection(char sreeningChar)
        {
            selectedWord.Remove(selectedWord.Length - 1, 1);
            selectedWord.Append(sreeningChar);
            result.Remove(result.Length - 1, 1);
            result.Append(sreeningChar);
        }

        private void PushSelectedWordToResult(char ch)
        {
            result.Remove(result.Length - selectedWord.Length - 1, selectedWord.Length + 1);
            result.AppendFormat("<em>{0}</em>", selectedWord);
            selectedWord.Clear();
        }
    }
}
