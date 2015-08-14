using System.IO;

namespace CodeFormatter
{
    public class CodeFileBuilderPInvokeCCharp : CodeFileBuilder
    {
        public override CodeFile CreateFile(CodeFile source)
        {
            var result = new CodeFile { LibraryName = source.LibraryName, FileName = source.FileName };
            foreach (var ns in source.Namespaces)
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
                    Name = Path.GetFileNameWithoutExtension(source.LibraryName),
                    NormalizedName = Path.GetFileNameWithoutExtension(source.LibraryName)
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
                            TypeName = parameter.Class == null ? parameter.TypeName : parameter.Class.NormalizedName,
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
    }
}