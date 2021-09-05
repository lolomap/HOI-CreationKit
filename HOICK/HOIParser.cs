using System;
using System.Collections.Generic;
using System.IO;
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
            LexemCollection L = la.Run(raw);
            sa.Run(L);
        }
    }

    public class SyntaxAnalizer
    {
        private void BlockAssign(LexemCollection lexems)
        {
            int depth = 0;
            for (int i = 0; i < lexems.collection.Count; i++)
            {
                if (lexems.collection[i].Type == Lexem.LexemType.BlockSign)
                {
                    depth++;

                    int lastBsl = lexems.BlockSignIndexes[lexems.BlockSignIndexes.Count - depth];
                    for (int j = i; j <= lastBsl; j++)
                    {
                        lexems.collection[j].AssignBlock(depth);
                        
                        if (lexems.DepthAssignedCollection.ContainsKey(depth))
                        {
                            lexems.DepthAssignedCollection[depth].Add(lexems.collection[j]);
                        }
                        else
                        {
                            lexems.DepthAssignedCollection.Add(depth, new List<Lexem>() { lexems.collection[j] });
                        }
                    }
                }
            }
            lexems.MaxDepth = depth;
        }

        public void Run(LexemCollection lexems)
        {
            BlockAssign(lexems);
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

        private LexemCollection LexemsWrite(string data)
        {
            LexemCollection Lexems = new LexemCollection();
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
                            Lexems.collection.Add(new Lexem(lexem, Lexem.LexemType.KeyWord));
                        }
                        else
                        {
                            Lexems.collection.Add(new Lexem(lexem));
                        }
                        lexem = "";
                    }

                    if (isOperator)
                    {
                        Lexems.collection.Add(new Lexem(sym, Lexem.LexemType.Operator));
                    }
                    else if (isBlockS)
                    {
                        Lexems.collection.Add(new Lexem(sym, Lexem.LexemType.BlockSign));
                        Lexems.BlockSignIndexes.Add(Lexems.collection.Count - 1);
                    }

                    continue;
                }
                lexem += sym;
            }

            return Lexems;
        }

        public LexemCollection Run(string data)
        {
            data = StyleIgnor(data);

            LexemCollection Lexems = LexemsWrite(data);

            return Lexems;
        }
    }
    
    public class AST
    {
        List<Tuple<SyntaxObj, SyntaxObj>> KeyValueObjs = new List<Tuple<SyntaxObj, SyntaxObj>>();

        private object GeneratePairOrBlock(List<Lexem> lexems)
        {
            List<Tuple<SyntaxObj, SyntaxObj>> res = new List<Tuple<SyntaxObj, SyntaxObj>>();

            for (int i = 0; i < lexems.Count; i += 3)
            {
                if (lexems[i].Type == Lexem.LexemType.KeyWord || lexems[i].Type == Lexem.LexemType.Unknown)
                {
                    if (lexems[i + 1].Type == Lexem.LexemType.Operator)
                    {
                        if (lexems[i + 2].Type == Lexem.LexemType.Unknown)
                        {
                            SyntaxObj root = new SyntaxObj();
                            root.SetValue(lexems[i].Value);

                            SyntaxObj child = new SyntaxObj();
                            child.SetValue(lexems[i + 2].Value);

                            res.Add(new Tuple<SyntaxObj, SyntaxObj>(root, child));
                        }
                        else
                        {
                            throw new Exception("Unexpected lexem");
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected lexem");
                    }
                }
                else
                {
                    throw new Exception("Unexpected lexem");
                }
            }

            if (res.Count == 1)
            {
                return res[0];
            }
            else
            {
                SyntaxObj o = new SyntaxObj();
                o.SetValue(res);
                return o;
            }

        }

        public List<Tuple<SyntaxObj, SyntaxObj>> GenerateTree(LexemCollection lexems)
        {
            
        }

        public void LogTree()
        {
            using (StreamWriter f = new StreamWriter("/logs/tree.txt"))
            {

            }
        }
    }
    
    public class SyntaxObj
    {
        public bool isBlock = false;
        private object value;

        public object GetValue()
        {
            return value;
        }
        public void SetValue(object val)
        {
            value = val;
        }
        public void SetValue(List<string> vals)
        {
            value = vals;
        }
    }

    public class LexemCollection
    {
        public List<Lexem> collection = new List<Lexem>();
        public List<int> BlockSignIndexes = new List<int>();
        public Dictionary<int, List<Lexem>> DepthAssignedCollection = new Dictionary<int, List<Lexem>>();
        public int MaxDepth = 0;
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
