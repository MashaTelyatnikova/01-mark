using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorMarkdown
{
    public static class AutomateReplacement
    {
        public static string GetNewString(string line)
        {
            var result = "";
            var curWord = "";
            var state = 0;
            var pos = 0;
            while (pos != line.Length)
            {
                var ch = line[pos];
                switch (state)
                {
                    case 0:
                    {
                        if (ch != '_')
                        {
                            result += ch;
                            state = 0;
                        }
                        else
                        {
                            state = 1;
                        }
                        break;
                    }
                    case 1:
                    {
                        if (ch != '_')
                        {
                            curWord += ch;
                            state = 1;
                        }
                        else
                        {
                            state = 2;
                        }
                        break;
                    }
                    case 2:
                    {
                        result += string.Format("<em>{0}</em>", curWord);
                        curWord = "";
                        break;
                    }
                }
                pos++;
            }
            if (curWord.Length != 0)
            {
                result += "_" + curWord;
            }

            return result;
        }
    }
}
