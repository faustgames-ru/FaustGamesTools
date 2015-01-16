using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using testLib;

namespace test
{
    internal class Program
    {
        private const string Input = @"namespace Fooo
{
    class Foo 
    {
        virtual void init(int, float);
        virtual void _stdcall init1(int, float);
        virtual void _stdcall init2(int, float) = 0;
    };

    //[Factory]
    class Foo1 : Foo {};
    class Foo2 {};
}
";

        private static void Main(string[] args)
        {
            var lex = new CLexer(new AntlrInputStream(Input));
            var tokens = new CommonTokenStream(lex);
            var par = new CParser(tokens);
            var ctx = par.namespaceDeclaration();
            var child = ctx.GetChild<CParser.ClassDeclarationContext>(0);
            var walker = new ParseTreeWalker();
            walker.Walk(new ParseTreeListenerEmpty(), ctx);

            var c = par.compileUnit();
            var s = c.ToStringTree();
            /*
            MyPascalParser.myprogram_return X = g.myprogram();
            Console.WriteLine(X.Tree);  // Writes: nill
            Console.WriteLine(X.Start); // Writes: [@0,0:4='begin',<4>,1:0]
            Console.WriteLine(X.Stop);  // Writes: [@35,57:57='end',<19>,12:2]
             */
        }

        private class ParseTreeListenerEmpty : IParseTreeListener
        {
            public void VisitTerminal(ITerminalNode node)
            {
            }

            public void VisitErrorNode(IErrorNode node)
            {
            }

            public void EnterEveryRule(ParserRuleContext ctx)
            {
                if (ctx.RuleIndex == CParser.RULE_parameterName)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_parameterType)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_methodParameter)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_methodParameters)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_type)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_methodDeclaration)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_classExtends)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_classAttributeName)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_classAttribute)
                {
                }
                if (ctx.RuleIndex == CParser.RULE_classDeclaration)
                {
                }
            }

            public void ExitEveryRule(ParserRuleContext ctx)
            {
            }
        }

        private class ParseTreeListener : IParseTreeListener
        {
            public void VisitTerminal(ITerminalNode node)
            {
            }

            public void VisitErrorNode(IErrorNode node)
            {
            }

            public void EnterEveryRule(ParserRuleContext ctx)
            {
            }

            public void ExitEveryRule(ParserRuleContext ctx)
            {
            }
        }
    }
}
