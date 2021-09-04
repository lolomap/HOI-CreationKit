using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HOICK
{
    public class HOIParser
    {
        private LexicAnalizer la = new LexicAnalizer();
        private SyntaxAnalizer sa = new SyntaxAnalizer();

        public void Parse(string raw)
        {
            List<Lexem> L = la.Run(raw);
            sa.Run(L, la.BlockSignIndexes);
        }
    }

    public class SyntaxAnalizer
    {
        private void BlockAssign(List<Lexem> lexems, List<int> BSLs)
        {
            int depth = 0;
            for (int i = 0; i < lexems.Count; i++)
            {
                if (lexems[i].Type == Lexem.LexemType.BlockSign)
                {
                    depth++;

                    int lastBsl = BSLs[BSLs.Count - depth];
                    for (int j = i; j <= lastBsl; j++)
                    {
                        lexems[j].AssignBlock(depth);
                    }
                }
            }
        }

        public void Run(List<Lexem> lexems, List<int> BSLs)
        {
            BlockAssign(lexems, BSLs);
        }
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

        private string Operators = "=+-\"";
        private string BlockSigns = "{}";

        private string Spaces = " \n\r\t";

        private List<Lexem> Lexems = new List<Lexem>();
        public List<int> BlockSignIndexes = new List<int>();

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
                bool isBlockS = BlockSigns.Contains(sym);
                if (sym == ' ' || isOperator || isBlockS)
                {
                    if (lexem != string.Empty)
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
                    }

                    if (isOperator)
                    {
                        Lexems.Add(new Lexem(sym, Lexem.LexemType.Operator));
                    }
                    else if (isBlockS)
                    {
                        Lexems.Add(new Lexem(sym, Lexem.LexemType.BlockSign));
                        BlockSignIndexes.Add(Lexems.Count - 1);
                    }

                    continue;
                }
                lexem += sym;
            }
        }

        public List<Lexem> Run(string data)
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
            Operator,
            BlockSign
        }
        public LexemType Type { private set; get; }
        public object Value { private set; get; }
        public int Block { private set; get; }

        public void AssignBlock(int block)
        {
            Block = block;
        }

        public Lexem(object value, LexemType type = LexemType.Unknown)
        {
            Type = type;
            Block = 0;

            if (int.TryParse(value as string, out int i))
            {
                Value = i;
                Type = LexemType.Number;
            }
            else if (float.TryParse(value as string, out float f))
            {
                Value = f;
                Type = LexemType.Number;
            }
            else
            {
                Value = value;
            }
        }
    }
}
