using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
        public List<Struct> Structs = new List<Struct>();
        public List<Class> Classes = new List<Class>();
        public List<Enum> Enums = new List<Enum>();
        public List<Method> RootMethods = new List<Method>();

        public override string ToString()
        {
            return Name;
        }
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
        public string TypeName;
        public string Name;
        public bool IsPrivate;
        public override string ToString()
        {
            return string.Format("{0} {1}", TypeName, Name);
        }
    }

    public class EnumItem
    {
        public string Name;
        public string Value;
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
    }

    public class Enum
    {
        public string Name;
        public List<EnumItem> Items = new List<EnumItem>();
        public override string ToString()
        {
            return Name;
        }
    }

    public class MethodParameter
    {
        public MethodParameter()
        {
        }

        public MethodParameter(MethodParameter p)
        {
            TypeName = p.TypeName;
            Name = p.Name;
            IsConst = p.IsConst;
            IsLink = p.IsLink;
        }
        public string FullTypeName
        {
            get { return (IsConst ? "const " : "") + TypeName + (IsLink ? " *":""); }
        }

        public Class Class;

        public string TypeName;
        public string Name;
        public bool IsConst;
        public bool IsLink;

        public override string ToString()
        {
            return string.Format("{0} {1}", TypeName, Name);
        }
    }

    public class Method
    {
        public string ResulTypeFullName
        {
            get { return (ReturnConst ? "const " : "") + ResulTypeName + (ReturnLink ? " *" : ""); }
        }

        public bool IsPrivate;
        public bool IsExtern;
        public string ResulTypeName;
        public bool ReturnLink;
        public bool ReturnConst;
        public Class ReturnClass; 
        public string Name;
        public string Body;
        public List<MethodParameter> Parameters = new List<MethodParameter>();
        public bool IsPure = false;
        public bool IsStatic = false;

        public override string ToString()
        {
            return string.Format(IsPure ? "abstract {0} {1} (..)" : "{0} {1} (..)", ResulTypeName, Name);
        }
    }

    public class Fields
    {
        public List<Field> Items = new List<Field>();

        public void Add(Field field)
        {
            Items.Add(field);
        }
    }

    public class Methods
    {
        public List<Method> Items = new List<Method>();
        public int Count 
        {
            get { return Items.Count; }
        }

        public void Add(Method method)
        {
            Items.Add(method);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }

    public class TypeDeclaration
    {
        public string Name;
    }

    public class Class : TypeDeclaration
    {
        public List<string> Extends = new List<string>();
        public List<Class> Childs = new List<Class>();
        public List<Class> Parents = new List<Class>();
        public Fields Fields = new Fields();
        public Methods Methods = new Methods();
        public Methods PInvokeMethods = new Methods();

        public string NormalizedName;

        public override string ToString()
        {
            return Name;
        }

    }

    public class Struct : TypeDeclaration
    {
        public Fields Fields = new Fields();

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract class CodeFileBuilder
    {
        public static CodeFileBuilder PInvokeCCharp = new CodeFileBuilderPInvokeCCharp();
        public static CodeFileBuilder PInvokeCpp = new CodeFileBuilderPInvokeCpp();
        public static CodeFileBuilder JniCpp = new CodeFileBuilderJniCpp();
        public abstract CodeFile CreateFile(CodeFile source);
    }

    public class CodeFile
    {
        public String FileName;
        public String LibraryName;
        public List<Namespace> Namespaces = new List<Namespace>();

        public Dictionary<string, Class> ClassesMap = new Dictionary<string, Class>();
        public List<Class> RootClasses = new List<Class>();

        public CodeFile CreatePInvokeCSharp()
        {
            return CodeFileBuilder.PInvokeCCharp.CreateFile(this);
        }

        public CodeFile CreatePInvokeCpp()
        {
            return CodeFileBuilder.PInvokeCpp.CreateFile(this);
        }

        public CodeFile CreateJniCpp()
        {
            return CodeFileBuilder.JniCpp.CreateFile(this);
        }

        public void Build()
        {
            ClassesMap.Clear();
            var errors = new List<string>();
            foreach (var ns in Namespaces)
            {
                foreach (var c in ns.Classes)
                {
                    if (c.Name.StartsWith("I") || c.Name.StartsWith("C"))
                    {
                        c.NormalizedName = c.Name.Remove(0, 1);
                    }
                    if (ClassesMap.ContainsKey(c.Name))
                    {
                        errors.Add(string.Format("Class '{0}' already defined", c.Name));
                    }
                    else
                    {
                        ClassesMap.Add(c.Name, c);
                    }
                }
            }
            foreach (var ns in Namespaces)
            {
                foreach (var method in ns.RootMethods)
                {
                    if (ClassesMap.ContainsKey(method.ResulTypeName))
                        method.ReturnClass = ClassesMap[method.ResulTypeName];
                    foreach (var p in method.Parameters)
                    {
                        if (ClassesMap.ContainsKey(p.TypeName))
                            p.Class = ClassesMap[p.TypeName];
                    }
                }

                foreach (var c in ns.Classes)
                {
                    foreach (var extend in c.Extends)
                    {
                        if (!ClassesMap.ContainsKey(extend))
                        {
                            errors.Add(string.Format("Unable not find parent class '{0}'", extend));
                        }
                        else
                        {
                            var extendClass = ClassesMap[extend];
                            extendClass.Childs.Add(c);
                            c.Parents.Add(extendClass);
                        }
                    }
                    if (c.Parents.Count == 0)
                        RootClasses.Add(c);
                    foreach (var method in c.Methods.Items)
                    {
                        if (ClassesMap.ContainsKey(method.ResulTypeName))
                            method.ReturnClass = ClassesMap[method.ResulTypeName];
                        foreach (var p in method.Parameters)
                        {
                            if (ClassesMap.ContainsKey(p.TypeName))
                                p.Class = ClassesMap[p.TypeName];
                        }
                    }
                }
            }
        }
    }

    public abstract class Parser
    {
        public abstract CodeFile Parse(string libraryName, string fileName, string code);
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

    public class FormatPatterns
    {
        public string PatternNamespace = "ns {0}";
        public string PatternClass = "class {0}";
        public string PatternEnum = "enum {0}";
        public string PatternStruct = "struct {0}";
        public string PatternMethod = "{0} {1} ({2})";
        public string PatternPrivateMethod = "_ {0} {1} ({2})";
        public string PatternParameter = "{0} {1}";
        public string PatternFiled = "{0} {1};";
        public string PatternPrivateFiled = "_ {0} {1};";
        public string PatternNone = "{0}";

        private FormatPatterns()
        {
        }

        public static FormatPatterns CSharp = new FormatPatterns
        {
            PatternNamespace = "namespace {0}",
            PatternClass = "public class {0}",
            PatternEnum = "public enum {0}",
            PatternStruct = "public struct {0}",
            PatternMethod = "public {0} {1} ({2})",
            PatternPrivateMethod = "private {0} {1} ({2})",
            PatternParameter = "{0} {1}",
            PatternFiled = "public {0} {1};",
            PatternPrivateFiled = "private {0} {1};",
            PatternNone = "{0}",
        };

        public static FormatPatterns Cpp = new FormatPatterns
        {
            PatternNamespace = "namespace {0}",
            PatternClass = "class {0}",
            PatternEnum = "enum {0}",
            PatternStruct = "struct {0}",
            PatternMethod = "extern \"C\" DLLEXPORT {0} API_CALL {1} ({2})",
            PatternPrivateMethod = "extern \"C\" DLLEXPORT {0} API_CALL {1} ({2})",
            PatternParameter = "{0} {1}",
            PatternFiled = "{0} {1};",
            PatternPrivateFiled = "{0} {1};",
            PatternNone = "{0}",
        };

        public static FormatPatterns Java = new FormatPatterns
        {
            PatternNamespace = "public class {0}",
            PatternClass = "public class {0}",
            PatternEnum = "public enum {0}",
            PatternStruct = "public struct {0}",
            PatternMethod = "public {0} {1} ({2})",
            PatternPrivateMethod = "private {0} {1} ({2})",
            PatternParameter = "{0} {1}",
            PatternFiled = "public {0} {1};",
            PatternPrivateFiled = "private {0} {1};",
            PatternNone = "{0}",
        };
    }

    public class FormatProvider
    {
        protected FormatPatterns FormatPatterns;

        public FormatProvider(FormatPatterns formatPatterns)
        {
            FormatPatterns = formatPatterns;
        }

        protected virtual void BeforeFillNamespace(Tabulator tabulator, Namespace value)
        {
        }


        public virtual void Format(CodeFile file, Tabulator tabulator)
        {
            foreach (var ns in file.Namespaces)
            {
                if (!string.IsNullOrEmpty(ns.Name))
                    BeginCodeBlock(tabulator, FormatPatterns.PatternNamespace, ns.Name);
                
                BeforeFillNamespace(tabulator, ns);
                
                foreach (var item in ns.Enums)
                    Format(tabulator, item);
                foreach (var item in ns.Structs)
                    Format(tabulator, item);
                foreach (var item in ns.Classes)
                    Format(tabulator, item);
                foreach (var item in ns.RootMethods)
                    Format(tabulator, item);
                
                if (!string.IsNullOrEmpty(ns.Name))
                    EndCodeBlock(tabulator);
            }
        }

        private void Format(Tabulator tabulator, Enum value)
        {
            BeginCodeBlock(tabulator, FormatPatterns.PatternEnum, value.Name);
            Format(tabulator, value.Items);
            EndCodeBlock(tabulator);
        }

        private void Format(Tabulator tabulator, List<EnumItem> items)
        {
            foreach (var item in items)
            {
                Format(tabulator, item);
            }
        }

        private void Format(Tabulator tabulator, EnumItem item)
        {
            tabulator.AppendFormatLine("{0} = {1},", item.Name, item.Value);
        }

        private void Format(Tabulator tabulator, Class value)
        {
            BeginCodeBlock(tabulator, FormatPatterns.PatternClass, value.Name);
            Format(tabulator, value.Fields);
            Format(tabulator, value.Methods);
            EndCodeBlock(tabulator);

        }

        private void Format(Tabulator tabulator, Methods methods)
        {
            foreach (var method in methods.Items)
            {
                Format(tabulator, method);
            }
        }

        protected virtual void BeforeAppendStaticMethod(Tabulator tabulator, Method value)
        {
        }

        private void Format(Tabulator tabulator, Method value)
        {
            var parameters = "";
            for (int i = 0; i < value.Parameters.Count; i++)
            {
                var p = value.Parameters[i];
                parameters += string.Format(FormatPatterns.PatternParameter, p.FullTypeName, p.Name);
                if (i < (value.Parameters.Count - 1))
                {
                    parameters += ", ";
                }
            }
            var pattern = value.IsPrivate ? FormatPatterns.PatternPrivateMethod : FormatPatterns.PatternMethod;
            if (value.Body != null)
            {
                BeginCodeBlock(tabulator, (value.IsStatic ? "static " : "") + (value.IsExtern ? "extern " : "") + pattern,
                    value.ResulTypeFullName, value.Name, parameters);
                if (value.Body != null)
                    tabulator.AppendLine(value.Body);
                EndCodeBlock(tabulator);
            }
            else
            {
                BeforeAppendStaticMethod(tabulator, value);
                tabulator.AppendFormatLine((value.IsStatic ? "static " : "") + (value.IsExtern ? "extern " : "") + pattern + ";", value.ResulTypeFullName, value.Name, parameters);
            }
        }

        protected void Format(Tabulator tabulator, Field value)
        {
            tabulator.AppendFormatLine(
                value.IsPrivate ? FormatPatterns.PatternPrivateFiled : FormatPatterns.PatternFiled, 
                value.TypeName,
                value.Name);
        }

        protected void Format(Tabulator tabulator, Fields value)
        {
            foreach (var field in value.Items)
            {
                Format(tabulator, field);
            }
        }

        protected virtual void BeforeAppendStruct(Tabulator tabulator, Struct value)
        {
        }

        private void Format(Tabulator tabulator, Struct value)
        {
            BeforeAppendStruct(tabulator, value);
            BeginCodeBlock(tabulator, FormatPatterns.PatternStruct, value.Name);
            Format(tabulator, value.Fields);
            EndCodeBlock(tabulator);
        }

        protected void BeginCodeBlock(Tabulator tabulator, string format, params object[] parameters)
        {
            tabulator.AppendFormatLine(format, parameters);
            tabulator.AppendLine("{");
            tabulator.BeginBlock();
        }

        protected void EndCodeBlock(Tabulator tabulator)
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

        public void AppendFormatLineWithoutTabs(string fromat, params object[] parameters)
        {
            _builder.AppendLine(string.Format(fromat, parameters));
        }
        public void AppendLineWithoutTabs(string line)
        {
            _builder.AppendLine(line);
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
}
