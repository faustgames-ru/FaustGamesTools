using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using GrammarCpp;

namespace CodeFormatter
{
    public class ParserCpp : Parser
    {
        public override CodeFile Parse(string code)
        {
            var lex = new CppClassesLexer(new AntlrInputStream(code));
            var tokens = new CommonTokenStream(lex);
            var par = new CppClassesParser(tokens);
            var ctx = par.namespaceDeclaration();
            var walker = new ParseTreeWalker();
            var listener = new ParseTreeListener();
            walker.Walk(listener, ctx);
            return listener.GetResult();
        }
    }

    internal class ParseArgs
    {
        public ParseTreeListener Listener;
        public ParserRuleContext Context;
    }

    internal abstract class ParseState
    {
        public abstract void EnterRule(ParseArgs e);
    }

    internal class ParseStateFile : ParseState
    {
        public CodeFile File = new CodeFile();
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_namespaceDeclaration:
                    e.Listener.StateNamespace.Namespace = new Namespace();
                    File.Namespaces.Add(e.Listener.StateNamespace.Namespace);
                    e.Listener.PushState(e.Listener.StateNamespace, CppClassesParser.RULE_namespaceDeclaration);
                    break;
            }
        }
    }

    internal class ParseStateNamespace : ParseState
    {
        public Namespace Namespace;
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_namespaceName:
                    Namespace.Name = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_methodDeclaration:
                    var method = new Method();
                    Namespace.RootMethods.Add(method);
                    e.Listener.StateMethod.Method = method;
                    e.Listener.PushState(e.Listener.StateMethod, CppClassesParser.RULE_methodDeclaration);
                    break;
                case CppClassesParser.RULE_structDeclaration:
                    var structItem = new Struct();
                    Namespace.Structs.Add(structItem);
                    e.Listener.StateStruct.Struct = structItem;
                    e.Listener.PushState(e.Listener.StateStruct, CppClassesParser.RULE_structDeclaration);
                    break;
                case CppClassesParser.RULE_classDeclaration:
                    var classItem = new Class();
                    Namespace.Classes.Add(classItem);
                    e.Listener.StateClass.Class = classItem;
                    e.Listener.PushState(e.Listener.StateClass, CppClassesParser.RULE_classDeclaration);
                    break;
            }
        }
    }

    internal class ParseStateStruct : ParseState
    {
        public Struct Struct;
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_structDeclarationName:
                    Struct.Name = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_fieldDeclaration:
                    var field = new Field();
                    Struct.Fields.Add(field);
                    e.Listener.StateField.Field = field;
                    e.Listener.PushState(e.Listener.StateField, CppClassesParser.RULE_fieldDeclaration);
                    break;
            }
        }
    }

    internal class ParseStateClass : ParseState
    {
        public Class Class;
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_classDeclarationName:
                    Class.Name = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_className:
                    Class.Extends.Add(e.Context.GetText());
                    break;
                case CppClassesParser.RULE_methodDeclaration:
                    var method = new Method();
                    Class.Methods.Add(method);
                    e.Listener.StateMethod.Method = method;
                    e.Listener.PushState(e.Listener.StateMethod, CppClassesParser.RULE_methodDeclaration);
                    break;
            }
        }
    }

    internal class ParseStateField : ParseState
    {
        public Field Field;
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_fieldType:
                    Field.TypeName = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_fieldName:
                    Field.Name = e.Context.GetText();
                    break;
            }
        }
    }

    internal class ParseStateMethod : ParseState
    {
        public Method Method;
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_returnType:
                    Method.ResulTypeName = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_methodPure:
                    Method.IsPure = true;
                    break;
                case CppClassesParser.RULE_methodName:
                    Method.Name = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_methodParameter:
                    var parameter = new MethodParameter();
                    Method.Parameters.Add(parameter);
                    e.Listener.StateMethodParameter.Parameter = parameter;
                    e.Listener.PushState(e.Listener.StateMethodParameter, CppClassesParser.RULE_methodParameter);
                    break;
            }
        }
    }

    internal class ParseStateMethodParameter : ParseState
    {
        public MethodParameter Parameter;
        public override void EnterRule(ParseArgs e)
        {
            switch (e.Context.RuleIndex)
            {
                case CppClassesParser.RULE_parameterType:
                    Parameter.TypeName = e.Context.GetText();
                    break;
                case CppClassesParser.RULE_parameterName:
                    Parameter.Name = e.Context.GetText();
                    break;
            }
        }
    }

    internal class ParseTreeListener : IParseTreeListener
    {
        public ParseStateFile StateFile = new ParseStateFile();
        public ParseStateNamespace StateNamespace = new ParseStateNamespace();
        public ParseStateClass StateClass = new ParseStateClass();
        public ParseStateStruct StateStruct = new ParseStateStruct();
        public ParseStateMethod StateMethod = new ParseStateMethod();
        public ParseStateField StateField = new ParseStateField();
        public ParseStateMethodParameter StateMethodParameter = new ParseStateMethodParameter();

        readonly ParseArgs _args = new ParseArgs();
        public ParseTreeListener()
        {
            _args.Listener = this;
            _stack.Push(new StateContainer
            {
                State = StateFile,
                ExitRule = -1
            });
        }

        public CodeFile GetResult()
        {
            return StateFile.File;
        }

        public void VisitTerminal(ITerminalNode node)
        {
        }

        public void VisitErrorNode(IErrorNode node)
        {
        }

        public void EnterEveryRule(ParserRuleContext context)
        {
            if (_stack.Count == 0) return;
            var state = _stack.Peek();
            _args.Context = context;
            state.State.EnterRule(_args);
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            var state = _stack.Peek();
            if (ctx.RuleIndex == state.ExitRule)
                _stack.Pop();
        }

        public void PushState(ParseState parseState, int exitRule)
        {
            _stack.Push(new StateContainer
            {
                ExitRule = exitRule,
                State = parseState
            });
        }

        private readonly Stack<StateContainer> _stack = new Stack<StateContainer>();
    }

    internal class StateContainer
    {
        public ParseState State;
        public int ExitRule;
    }
}