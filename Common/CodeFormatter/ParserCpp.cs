using System;
using System.Collections.Generic;

namespace CodeFormatter
{
    public class ParserCpp : Parser
    {
        public override CodeFile Parse(string code)
        {
            throw new NotImplementedException();
        }
    }
    public class ParseState
    {
        public string Text;
        public int Index;
        public ParseNode Root = new ParseNode();
        public ParseState(string text)
        {
            Text = text;
        }
    }

    public abstract class ParseRule
    {
        public bool Match(ParseState state)
        {
            var index = state.Index;
            if (OnMatch(state))
                return true;
            state.Index = index;
            return false;
        }

        protected abstract bool OnMatch(ParseState state);
    }

    public class ParseNode
    {
        public ParseNode Parent;
        public List<ParseNode> Childs = new List<ParseNode>();
        public ParseRule Rule;
    }
}