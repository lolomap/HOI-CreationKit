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
            int maxDepth = 0;
            for (int i = 0; i < lexems.collection.Count; i++)
            {
                if (lexems.collection[i].Type == Lexem.LexemType.BlockSign && (char)lexems.collection[i].Value == '{')
                {
                    lexems.collection[i].AssignBlock(depth);
                    depth++;
                }
                else if (lexems.collection[i].Type == Lexem.LexemType.BlockSign && (char)lexems.collection[i].Value == '}')
                {
                    depth--;
                    lexems.collection[i].AssignBlock(depth);
                }
                else
                {
                    lexems.collection[i].AssignBlock(depth);
                }

                if (depth > maxDepth)
                {
                    maxDepth = depth;
                }
            }
            lexems.MaxDepth = maxDepth;

            lexems.DepthAssignedCollection.Add(0, new List<Lexem>());
            foreach (Lexem lexem in lexems.collection)
            {
                if (lexems.DepthAssignedCollection.ContainsKey(lexem.Block))
                {
                    lexems.DepthAssignedCollection[lexem.Block].Add(lexem);
                }
                else
                {
                    lexems.DepthAssignedCollection.Add(lexem.Block, new List<Lexem>() { lexem });
                }
            }
        }

        public void Run(LexemCollection lexems)
        {
            BlockAssign(lexems);
            AST ast = new AST();
            ast.GenerateSyntaxTree(lexems);
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

        private string Operators = "=><";
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
                        if (sym == '}')
                        {
                            Lexems.CloseBlockSignsIndexes.Add(Lexems.collection.Count - 1);
                        }
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
        private SyntaxObj GenerateBlock(List<Lexem> lexems)
        {
            List<SyntaxPair> res = new List<SyntaxPair>();

            for (int i = 0; i < lexems.Count - 2; i += 3)
            {
                if (lexems[i].Type == Lexem.LexemType.KeyWord || lexems[i].Type == Lexem.LexemType.Unknown)
                {
                    if (lexems[i + 1].Type == Lexem.LexemType.Operator)
                    {
                        if (lexems[i + 2].Type == Lexem.LexemType.Unknown || lexems[i+2].Type == Lexem.LexemType.Number)
                        {
                            SyntaxObj root = new SyntaxObj();
                            root.SetValue(lexems[i].Value);

                            SyntaxObj child = new SyntaxObj();
                            child.SetValue(lexems[i + 2].Value);

                            if ((char)lexems[i + 1].Value == '<')
                            {
                                res.Add(new SyntaxPair(root, child, SyntaxPair.OperatorType.Less));
                            }
                            else if ((char)lexems[i + 1].Value == '>')
                            {
                                res.Add(new SyntaxPair(root, child, SyntaxPair.OperatorType.More));
                            }
                            else
                            {
                                res.Add(new SyntaxPair(root, child));
                            }
                        }
                        else if (lexems[i + 2].Type == Lexem.LexemType.BlockSign)
                        {
                            SyntaxObj root = new SyntaxObj();
                            root.SetValue(lexems[i].Value);

                            SyntaxObj child = new SyntaxObj
                            {
                                isBlock = true
                            };
                            child.SetValue("~&$BLOCK$&~");

                            if ((char)lexems[i + 1].Value == '<')
                            {
                                res.Add(new SyntaxPair(root, child, SyntaxPair.OperatorType.Less));
                            }
                            else if ((char)lexems[i + 1].Value == '>')
                            {
                                res.Add(new SyntaxPair(root, child, SyntaxPair.OperatorType.More));
                            }
                            else
                            {
                                res.Add(new SyntaxPair(root, child));
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
                else
                {
                    throw new Exception("Unexpected lexem");
                }
            }

            SyntaxObj o = new SyntaxObj();
            if (res.Count == 1)
            {
                o.SetValue(res[0]);
            }
            else
            {
                o.SetValue(res);
            }

            return o;

        }

        //Return dictionary A: keys - depths, values - dictionary B. Dict B: keys - ids, values - blocks
        private Dictionary<int, Dictionary<int, SyntaxObj>> GenerateDictionaryTree(LexemCollection lexems)
        {
            //Tree with pointers to blocks instead of blocks. Keys are depths
            Dictionary<int, Dictionary<int, SyntaxObj>> prepareDict = new Dictionary<int, Dictionary<int, SyntaxObj>>();
            for (int i = 0; i < lexems.MaxDepth; i++)
            {
                //Generated for every depth. Keys are ids for blocks on same depth
                Dictionary<int, SyntaxObj> res = new Dictionary<int, SyntaxObj>();

                Dictionary<int, List<Lexem>> blocksInThisDepth = new Dictionary<int, List<Lexem>>();
                int curblock = 0; //for next loop
                bool blockLoad = true; //
                List<Lexem> oneBlock = new List<Lexem>();

                //Sort lexems for blocks on one depth
                foreach (Lexem lex in lexems.DepthAssignedCollection[i])
                {
                    if (lex.Type == Lexem.LexemType.BlockSign && (char)lex.Value == '{' && !blockLoad)
                    {
                        curblock++;
                    }
                    else if (lex.Type == Lexem.LexemType.BlockSign && (char)lex.Value == '}')
                    {
                        blocksInThisDepth.Add(curblock, oneBlock);
                        oneBlock = new List<Lexem>();
                        blockLoad = false;
                    }
                    else
                    {
                        oneBlock.Add(lex);
                    }
                }

                //Generate pairs from last lexems
                for (int j = 0; j < blocksInThisDepth.Keys.Count; j++)
                {
                    if (!res.ContainsKey(j))
                    {
                        res.Add(j, GenerateBlock(blocksInThisDepth[j]));
                    }
                    else
                    {
                        res[j] = GenerateBlock(blocksInThisDepth[j]);
                    }
                }

                prepareDict.Add(i, res);

            }

            return prepareDict;
        }

        public List<SyntaxPair> GenerateSyntaxTree(LexemCollection lexems)
        {
            List<SyntaxPair> KeyValueObjs = new List<SyntaxPair>();

            Dictionary<int, Dictionary<int, SyntaxObj>> preparedData = GenerateDictionaryTree(lexems);
            

            return null;
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
    }

    public class LexemCollection
    {
        public List<Lexem> collection = new List<Lexem>();
        public List<int> CloseBlockSignsIndexes = new List<int>();
        public Dictionary<int, List<Lexem>> DepthAssignedCollection = new Dictionary<int, List<Lexem>>();
        public int MaxDepth = 0;
    }

    public class SyntaxPair
    {
        public enum OperatorType
        {
            Equal,
            Less,
            More
        }

        public SyntaxObj Key;
        public SyntaxObj Value;
        public OperatorType Operator;

        public SyntaxPair(SyntaxObj key, SyntaxObj value, OperatorType oper = OperatorType.Equal)
        {
            Key = key;
            Value = value;
            Operator = oper;
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
