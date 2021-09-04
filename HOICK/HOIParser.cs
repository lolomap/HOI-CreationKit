using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HOICK
{
    public class HOIParser
    {
    }

    public class LexicAnalizer
    {
        private List<string> KeyWords = new List<string>()
        {
            "focus_tree",
            "id",
            "country",
            "factor",
            "modifier",
            "add",
            "tag",
            "default",
            "reset_on_civil_war",
            "shared_focus",
            "continuous_focus_position",
            "initial_show_position",
            "x",
            "y",
            "offset",
            "cost",
            "allow_branch",
            "relative_position_id",
            "prerequisite",
            "focus",
            "mutually_exclusive",
            "available",
            "bypass",
            "historical_ai",
            "cancel",
            "available_if_capitulated",
            "cancel_if_invalid",
            "continue_if_invalid",
            "will_lead_to_war_with",
            "search_filters",
            "select_effect",
            "completion_reward",
            "complete_tooltip",
            "ai_will_do"
        };

        private string Operators = "=+-\"{}";

        private string Spaces = " \n\r\t";

        private List<Lexem> Lexems = new List<Lexem>();

        private string StyleIgnor(string data)
        {
            string res = data;

            while (res.Contains('#'))
            {
                int start = res.IndexOf('#');
                int end = start + res.Substring(start).IndexOf('\n');

                res = res.Substring(0, start - 1) + res.Substring(end + 1);
            }

            foreach(char sym in Spaces)
            {
                res = res.Replace(sym, ' ');
            }
            foreach(char sym in Operators)
            {
                res = res.Replace(sym.ToString(), " " + sym + " ");
            }
            res = Regex.Replace(res, @"([ ]){2,}", @"$1");

            return res;
        }

        private void LexemsWrite(string data)
        {
            string lexem = "";
            foreach(char sym in data)
            {
                bool isOperator = Operators.Contains(sym);
                if ((sym == ' ' || isOperator) && lexem != string.Empty)
                {
                    if (KeyWords.Contains(lexem))
                    {
                        Lexems.Add(new Lexem(lexem, Lexem.LexemType.KeyWord));
                    }
                    else
                    {
                        Lexems.Add(new Lexem(lexem));
                    }
                    lexem = "";

                    if (isOperator)
                    {
                        Lexems.Add(new Lexem(sym, Lexem.LexemType.Operator));
                    }

                    continue;
                }
                lexem += sym;
            }
        }

        public List<Lexem> Parse(string data)
        {
            data = StyleIgnor(data);
            LexemsWrite(data);

            return Lexems;
        }
    }

    public class Lexem
    {
        public enum LexemType
        {
            Unknown,
            Number,
            KeyWord,
            Operator
        }
        public LexemType type { private set; get; }
        public object Value { private set; get; }

        public Lexem(object value, LexemType type = LexemType.Unknown)
        {
            this.type = type;

            if (int.TryParse(value as string, out int i))
            {
                Value = i;
                this.type = LexemType.Number;
            }
            else if (float.TryParse(value as string, out float f))
            {
                Value = f;
                this.type = LexemType.Number;
            }
            else
            {
                Value = value;
            }
        }
    }
}
