﻿using System;
using System.Text;

namespace CodeFormatter
{
    public enum CodeType
    {
        Namespace,
        Interface,
        Class,
        Struct,
    }

    public class Namespace
    {
        public string Name;
        public Struct[] Structs;
        public Interface[] Interfaces;
        public Class[] Classes;
    }

    public enum TypePrimive
    {
        Void, 
        UInt8,
        UInt16,
        UInt32,
        UInt64,
        Int8,
        Int16,
        Int32,
        Int64,
        Single,
        Double,
        Boolean,
        String,
        Struct,
        Interface,
        Class,
    }

    public class Type
    {
        public TypePrimive Primive;
        public TypeDeclaration Declaration;
    }

    public class Field
    {
        public Type Type;
        public string Name;
    }

    public class MethodParameter
    {
        public Type Type;
        public string Name;
    }

    public class Method
    {
        public Type ResulType;
        public string Name;
        public MethodParameter[] Parameters;
    }

    public class Fields
    {
        public Field[] Items;
    }

    public class Methods
    {
        public Method[] Items;
    }

    public class TypeDeclaration
    {
        public string Name;
    }

    public class Class : TypeDeclaration
    {
        public Class Parent;
        public Interfaces Interfaces = new Interfaces();
        public Fields Fields = new Fields();
        public Methods Methods = new Methods();
    }
    public class Struct : TypeDeclaration
    {
        public Fields Fields = new Fields();
    }

    public class Interface : TypeDeclaration
    {
        public Interfaces Parents = new Interfaces();
        public Methods Methods = new Methods();

        public bool HasParents()
        {
            if (Parents == null) return false;
            if (Parents.Items == null) return false;
            if (Parents.Items.Length == 0) return false;
            return true;
        }
    }

    public class Interfaces
    {
        public Interface[] Items;
    }

    public class CodeFile
    {
        public Namespace[] Namespaces;
    }

    public abstract class Parser
    {
        public abstract CodeFile Parse();
    }

    public abstract class TextParser : Parser
    {
        protected TextParser(string text)
        {
        }
    }

    public class Formatter
    {
        private readonly FormatProvider _provider;
        public Formatter(FormatProvider provider)
        {
            _provider = provider;
        }

        public string Format(CodeFile file)
        {
            var tabulator = new Tabulator();
            _provider.Format(file, tabulator);
            return tabulator.Build();
        }
    }

    public abstract class FormatProvider
    {
        public abstract void Format(CodeFile file, Tabulator tabulator);
    }

    public class FormatProviderCSharp : FormatProvider
    {
        public const string PatternNamespace = "namespace {0}";
        public const string PatternInterface = "public interface {0}";
        public const string PatternClass = "public class {0}";
        public const string PatternStruct = "public struct {0}";
        public const string PatternMethod = "public {0} {1} ({2})";
        public const string PatternParameter = "{0} {1}";
        public const string PatternFiled = "public {0} {1};";
        public const string PatternNone = "{0}";
        
        public override void Format(CodeFile file, Tabulator tabulator)
        {
            foreach (var ns in file.Namespaces)
            {
                BeginCodeBlock(tabulator, PatternNamespace, ns.Name);
                foreach (var item in ns.Structs)
                    Format(tabulator, item);
                foreach (var item in ns.Interfaces)
                    Format(tabulator, item);
                foreach (var item in ns.Classes)
                    Format(tabulator, item);
                EndCodeBlock(tabulator);
            }
        }

        private void Format(Tabulator tabulator, Field value)
        {
            tabulator.AppendFormatLine(PatternFiled, value.Type, value.Name);
        }

        private void Format(Tabulator tabulator, Fields value)
        {
            foreach (var field in value.Items)
            {
                Format(tabulator, field);
            }
        }

        private void Format(Tabulator tabulator, Method value)
        {
            throw new NotImplementedException();
        }

        private void Format(Tabulator tabulator, Methods value)
        {
            foreach (var method in value.Items)
            {
                Format(tabulator, method);
            }
        }

        private void Format(Tabulator tabulator, Struct value)
        {
            BeginCodeBlock(tabulator, PatternStruct, value.Name);
            Format(tabulator, value.Fields);
            EndCodeBlock(tabulator);
        }
        private void Format(Tabulator tabulator, Interface value)
        {
            BeginInterface(tabulator, value);
            Format(tabulator, value.Methods);
            EndCodeBlock(tabulator);
        }

        private void Format(Tabulator tabulator, Class value)
        {
        }

        private void AppendInterfaces(Tabulator tabulator, Interface[] items)
        {
            tabulator.Append(" : ");
            var high = items.Length - 1;
            for (var i = 0; i < items.Length; i++)
            {
                tabulator.Append(items[i].Name);
                if (i < high)
                    tabulator.Append(",");
            }
        }

        private void BeginInterface(Tabulator tabulator, Interface value)
        {
            tabulator.AppendFormat(PatternInterface, value.Name);
            if (value.HasParents())
            {
                var items = value.Parents.Items;
                AppendInterfaces(tabulator, items);
            }
            tabulator.AppendLine("");
            tabulator.AppendLine("{");
            tabulator.BeginBlock();
        }
        
        private void BeginCodeBlock(Tabulator tabulator, string format, params object[] parameters)
        {
            tabulator.AppendFormatLine(format, parameters);
            tabulator.AppendLine("{");
            tabulator.BeginBlock();
        }

        private void EndCodeBlock(Tabulator tabulator)
        {
            tabulator.EndBlock();
            tabulator.AppendLine("}");
            tabulator.AppendLine("");
        }

    }

    public class Tabulator
    {
        public Tabulator()
        {
            _builder.AppendLine("/* ============================================================== */");
            _builder.AppendLine("/* This file is automatically generated. Please do not modify it. */");
            _builder.AppendLine("/* ============================================================== */");
            _builder.AppendLine("");
        }

        public void BeginBlock()
        {
            _blocksCount++;
        }

        public void AppendLine(string text)
        {
            AppendTabs();
            _builder.AppendLine(text);
        }

        public void Append(string text)
        {
            AppendTabs();
            _builder.Append(text);
        }

        public void AppendFormatLine(string fromat, params object[] parameters)
        {
            AppendTabs();
            _builder.AppendLine(string.Format(fromat, parameters));
        }

        public void AppendFormat(string fromat, params object[] parameters)
        {
            AppendTabs();
            _builder.Append(string.Format(fromat, parameters));
        }

        public void EndBlock()
        {
            _blocksCount--;
        }

        private void AppendTabs()
        {
            for (var i = 0; i < _blocksCount; i++)
                _builder.Append("\t");
        }

        private readonly StringBuilder _builder = new StringBuilder();
        private int _blocksCount;

        public string Build()
        {
            return _builder.ToString();
        }
    }

    public abstract class CodeFormatProvider
    {
        public abstract string GetCodeLinePattern(CodeType type);
    }

    public class CodeFormatProviderCSharp: CodeFormatProvider
    {
        public const string PatternNamespace = "namespace {0}";
        public const string PatternInterface = "public interface {0}";
        public const string PatternClass = "public class {0}";
        public const string PatternStruct = "public struct {0}";
        public const string PatternMethod = "public {0} {1} ({2})";
        public const string PatternParameter = "{0} {1}";
        public const string PatternFiled = "public {0} {1}";
        public const string PatternNone = "{0}";
        public override string GetCodeLinePattern(CodeType type)
        {
            switch (type)
            {
                case CodeType.Namespace:
                    return PatternNamespace;
                case CodeType.Interface:
                    return PatternInterface;
                case CodeType.Class:
                    return PatternClass;
                case CodeType.Struct:
                    return PatternStruct;
                default:
                    return PatternNone;
            }
        }
    }

    public class CodeFormatProviderCpp : CodeFormatProvider
    {
        public const string PatternNamespace = "namespace {0}";
        public const string PatternInterface = "class {0}";
        public const string PatternClass = "class {0}";
        public const string PatternStruct = "struct {0}";
        public const string PatternMethod = "{0} {1} ({2})";
        public const string PatternParameter = "{0} {1}";
        public const string PatternFiled = "{0} {1}";
        public const string PatternNone = "{0}";
        public override string GetCodeLinePattern(CodeType type)
        {
            switch (type)
            {
                case CodeType.Namespace:
                    return PatternNamespace;
                case CodeType.Interface:
                    return PatternInterface;
                case CodeType.Class:
                    return PatternClass;
                case CodeType.Struct:
                    return PatternStruct;
                default:
                    return PatternNone;
            }
        }
    }
}