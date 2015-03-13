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

    public class CodeFile
    {
        public String FileName;
        public String LibraryName;
        public List<Namespace> Namespaces = new List<Namespace>();

        public Dictionary<string, Class> ClassesMap = new Dictionary<string, Class>();
        public List<Class> RootClasses = new List<Class>();

        public CodeFile CreatePInvokeCSharp()
        {
            var result = new CodeFile { LibraryName = LibraryName, FileName = FileName };
            foreach (var ns in Namespaces)
            {
                var resultNamespace = new Namespace
                {
                    Name = ns.Name
                };
                result.Namespaces.Add(resultNamespace);
                foreach (var e in ns.Enums)
                {
                    resultNamespace.Enums.Add(e);
                }
                foreach (var s in ns.Structs)
                {
                    resultNamespace.Structs.Add(s);
                }
                foreach (var c in ns.Classes)
                {
                    c.PInvokeMethods.Clear();
                    var newClass = new Class
                    {
                        Name = c.NormalizedName,
                        NormalizedName = c.NormalizedName
                    };
                    newClass.Fields.Add(new Field
                    {
                        Name = "ClassInstance",
                        TypeName = "IntPtr",
                    });
                    foreach (var method in c.Methods.Items)
                    {
                        var pInvoke = new Method
                        {
                            Name = string.Format("{0}_{1}_{2}", ns.Name, c.NormalizedName, method.Name),
                            ResulTypeName = method.ReturnLink?"IntPtr":method.ResulTypeName,
                            ReturnLink = false,
                            ReturnConst = false,
                            IsExtern = true,
                            IsStatic = true,
                            IsPrivate = true,
                        };
                        var pInvokeReflection = new Method
                        {
                            Name = method.Name,
                            ResulTypeName = method.ReturnClass == null ? method.ResulTypeName : method.ReturnClass.NormalizedName,
                            ReturnLink = false,
                            ReturnConst = false,
                            IsExtern = false,
                            IsStatic = false,
                            IsPrivate = false,
                        };
                        const string classInstance = "classInstance";
                        pInvoke.Parameters.Add(new MethodParameter
                        {
                            Name = classInstance,
                            TypeName = "IntPtr"
                        });
                        for (var i = 0; i < method.Parameters.Count; i++)
                        {
                            var parameter = method.Parameters[i];
                            var newParameter = new MethodParameter
                            {
                                IsConst = false,
                                IsLink = false,
                                TypeName = parameter.IsLink ? "IntPtr" : parameter.TypeName,
                                Name = parameter.Name
                            };
                            var resflectType = parameter.Class == null
                                ? parameter.TypeName
                                : parameter.Class.NormalizedName;
                            if ((resflectType == "void") && parameter.IsLink)
                            {
                                resflectType = "IntPtr";
                            }
                            if ((resflectType == "char") && parameter.IsLink)
                            {
                                resflectType = newParameter.TypeName = "string";
                            }
                            var reflectParameter = new MethodParameter
                            {
                                IsConst = false,
                                IsLink = false,
                                TypeName = resflectType,
                                Name = parameter.Name
                            };
                            pInvokeReflection.Parameters.Add(reflectParameter);
                            pInvoke.Parameters.Add(newParameter);
                        }

                        var parametersLine = "ClassInstance";
                        for (var i = 0; i < method.Parameters.Count; i++)
                        {
                            parametersLine += ", ";
                            var parameter = method.Parameters[i];
                            if (parameter.Class != null)
                                parametersLine += parameter.Name + ".ClassInstance";
                            else
                                parametersLine += parameter.Name;
                        }

                        var body = "";
                        if (pInvokeReflection.ResulTypeName != "void")
                        {
                            body += "return ";
                        }
                        if (method.ReturnClass != null)
                        {
                            body += string.Format("new {0}{{ ClassInstance = {1}({2}) }};",
                                method.ReturnClass.NormalizedName,
                                pInvoke.Name, parametersLine);
                        }
                        else
                        {
                            body += string.Format("{0}({1});", pInvoke.Name, parametersLine);
                        }

                        var firstChar = pInvokeReflection.Name.Remove(1, pInvokeReflection.Name.Length - 1);
                        var nextChars = pInvokeReflection.Name.Remove(0, 1);
                        pInvokeReflection.Name = firstChar.ToUpper() + nextChars;

                        pInvokeReflection.Body = body;

                        newClass.Methods.Add(pInvokeReflection);
                        newClass.Methods.Add(pInvoke);       
                    }
                    resultNamespace.Classes.Add(newClass);
                }
                var rootClass = new Class
                {
                    Name = Path.GetFileNameWithoutExtension(LibraryName),
                    NormalizedName = Path.GetFileNameWithoutExtension(LibraryName)
                };
                foreach (var method in ns.RootMethods)
                {
                    var pInvoke = new Method
                    {
                        Name = method.Name,
                        ResulTypeName = method.ReturnLink ? "IntPtr" : method.ResulTypeName,
                        ReturnLink = false,
                        ReturnConst = false,
                        IsExtern = true,
                        IsStatic = true,
                        IsPrivate = true,
                    };
                    var pInvokeReflection = new Method
                    {
                        Name = method.Name,
                        ResulTypeName = method.ReturnClass == null ? method.ResulTypeName : method.ReturnClass.NormalizedName,
                        ReturnLink = false,
                        ReturnConst = false,
                        IsExtern = false,
                        IsStatic = true,
                        IsPrivate = false,
                    };
                    for (var i = 0; i < method.Parameters.Count; i++)
                    {
                        var parameter = method.Parameters[i];
                        var newParameter = new MethodParameter
                        {
                            IsConst = false,
                            IsLink = false,
                            TypeName = parameter.IsLink ? "IntPtr" : parameter.TypeName,
                            Name = parameter.Name
                        };
                        var reflectParameter = new MethodParameter
                        {
                            IsConst = false,
                            IsLink = false,
                            TypeName = parameter.Class==null ? parameter.TypeName:parameter.Class.NormalizedName,
                            Name = parameter.Name
                        };
                        pInvoke.Parameters.Add(newParameter);
                        pInvokeReflection.Parameters.Add(reflectParameter);
                    }

                    var parametersLine = "";
                    for (var i = 0; i < method.Parameters.Count; i++)
                    {
                        var parameter = method.Parameters[i];
                        if (parameter.Class != null)
                            parametersLine += parameter.Name + ".ClassInstance";
                        else
                            parametersLine += parameter.Name;
                        if (i < (method.Parameters.Count - 1))
                            parametersLine += ", ";
                    }

                    var body = "";
                    if (pInvokeReflection.ResulTypeName != "void")
                    {
                        body += "return ";
                    }
                    if (method.ReturnClass != null)
                    {
                        body += string.Format("new {0}{{ ClassInstance = {1}({2}) }};",
                            method.ReturnClass.NormalizedName,
                            pInvoke.Name, parametersLine);
                    }
                    else
                    {
                        body += string.Format("{0}({1});", pInvoke.Name, parametersLine);
                    }
                    pInvokeReflection.Body = body;
                    var firstChar = pInvokeReflection.Name.Remove(1, pInvokeReflection.Name.Length - 1);
                    var nextChars = pInvokeReflection.Name.Remove(0, 1);
                    pInvokeReflection.Name = firstChar.ToUpper() + nextChars;


                    rootClass.Methods.Add(pInvokeReflection);
                    rootClass.Methods.Add(pInvoke);
                }
                resultNamespace.Classes.Add(rootClass);
            }
            return result;
        }

        public CodeFile CreatePInvokeCpp()
        {
            var result = new CodeFile
            {
                LibraryName = LibraryName,
                FileName = FileName
            };
            
            foreach (var ns in Namespaces)
            {
                var resultNamespace = new Namespace
                {
                    Name = ns.Name
                };
                result.Namespaces.Add(resultNamespace);
                foreach (var c in ns.Classes)
                {
                    c.PInvokeMethods.Clear();
                    foreach (var method in c.Methods.Items)
                    {
                        var pInvoke = new Method
                        {
                            Name = string.Format("{0}_{1}_{2}", ns.Name, c.NormalizedName, method.Name),
                            ResulTypeName = method.ResulTypeName,
                            ReturnLink = method.ReturnLink
                        };
                        const string classInstance = "classInstance";
                        pInvoke.Parameters.Add(new MethodParameter
                        {
                            Name = classInstance,
                            TypeName = c.Name + " *"
                        });
                        var parametersLine = "";
                        for (var i = 0; i < method.Parameters.Count; i++)
                        {
                            var parameter = method.Parameters[i];
                            pInvoke.Parameters.Add(parameter);
                            parametersLine += parameter.Name;
                            if (i < (method.Parameters.Count - 1)) 
                                parametersLine += ", ";
                        }
                        if (method.ResulTypeName != "void")
                            pInvoke.Body = "return ";
                        pInvoke.Body += string.Format("{0}->{1}({2});", classInstance, method.Name, parametersLine);
                        resultNamespace.RootMethods.Add(pInvoke);
                        c.PInvokeMethods.Add(pInvoke);
                    }
                }
            }
            return result;
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
    }

    public class FormatProvider
    {
        protected FormatPatterns _formatPatterns;

        public FormatProvider(FormatPatterns formatPatterns)
        {
            _formatPatterns = formatPatterns;
        }

        protected virtual void BeforeFillNamespace(Tabulator tabulator, Namespace value)
        {
        }


        public virtual void Format(CodeFile file, Tabulator tabulator)
        {
            foreach (var ns in file.Namespaces)
            {
                BeginCodeBlock(tabulator, _formatPatterns.PatternNamespace, ns.Name);
                BeforeFillNamespace(tabulator, ns);
                foreach (var item in ns.Enums)
                    Format(tabulator, item);
                foreach (var item in ns.Structs)
                    Format(tabulator, item);
                foreach (var item in ns.Classes)
                    Format(tabulator, item);
                foreach (var item in ns.RootMethods)
                    Format(tabulator, item);
                EndCodeBlock(tabulator);
            }
        }

        private void Format(Tabulator tabulator, Enum value)
        {
            BeginCodeBlock(tabulator, _formatPatterns.PatternEnum, value.Name);
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
            BeginCodeBlock(tabulator, _formatPatterns.PatternClass, value.Name);
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
                parameters += string.Format(_formatPatterns.PatternParameter, p.FullTypeName, p.Name);
                if (i < (value.Parameters.Count - 1))
                {
                    parameters += ", ";
                }
            }
            var pattern = value.IsPrivate ? _formatPatterns.PatternPrivateMethod : _formatPatterns.PatternMethod;
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
                value.IsPrivate ? _formatPatterns.PatternPrivateFiled : _formatPatterns.PatternFiled, 
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
            BeginCodeBlock(tabulator, _formatPatterns.PatternStruct, value.Name);
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

    public class FormatProviderCppPInvoke : FormatProvider
    {
        public FormatProviderCppPInvoke()
            : base(FormatPatterns.Cpp)
        {
        }

        public override void Format(CodeFile file, Tabulator tabulator)
        {
            tabulator.AppendFormatLine("#include \"{0}\"", file.FileName);
            tabulator.AppendLine("");
            base.Format(file, tabulator);
        }
    }

    public class FormatProviderCSharpPInvoke : FormatProvider
    {
        private CodeFile _file;
        public FormatProviderCSharpPInvoke()
            : base(FormatPatterns.CSharp)
        {
        }

        protected override void BeforeAppendStaticMethod(Tabulator tabulator, Method value)
        {
            tabulator.AppendLine("[DllImport(Version.Dll)]");

        }

        protected override void BeforeAppendStruct(Tabulator tabulator, Struct value)
        {
            tabulator.AppendLine("[StructLayout(LayoutKind.Sequential)]");
        }

        protected override void BeforeFillNamespace(Tabulator tabulator, Namespace value)
        {
            BeginCodeBlock(tabulator, _formatPatterns.PatternClass, "Version");
            tabulator.AppendFormatLine("public const string Dll = \"{0}\";", _file.LibraryName);
            EndCodeBlock(tabulator);
        }


        public override void Format(CodeFile file, Tabulator tabulator)
        {
            _file = file;
            tabulator.AppendFormatLine("using System;");
            tabulator.AppendFormatLine("using System.Runtime.InteropServices;");
            tabulator.AppendLine("");
            base.Format(file, tabulator);
        }
    }
}
