using System.Linq;

namespace CodeFormatter
{
    public class CodeFileBuilderJniJava : CodeFileBuilder
    {
        public override CodeFile CreateFile(CodeFile source)
        {
            var result = new CodeFile
            {
                LibraryName = source.LibraryName,
                FileName = source.FileName
            };

            foreach (var ns in source.Namespaces)
            {
                var resultNamespace = new Namespace
                {
                    Name = ns.Name
                };
                result.Namespaces.Add(resultNamespace);

            }
            return result;
        }
    }

    public class CodeFileBuilderJniCpp : CodeFileBuilder
    {
        public override CodeFile CreateFile(CodeFile source)
        {
            var result = new CodeFile
            {
                LibraryName = source.LibraryName,
                FileName = source.FileName
            };

            foreach (var ns in source.Namespaces)
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
                            Name = string.Format("{0}_{1}_{2}", ns.Name, c.Name, method.Name),
                            ResulTypeName = ReplaceTypeName(ns, method.ResulTypeName, method.ReturnLink),
                            ReturnLink = false
                        };
                        FillPInvokeMethod(ns, pInvoke, method, c);
                        resultNamespace.RootMethods.Add(pInvoke);
                        c.PInvokeMethods.Add(pInvoke);
                    }
                }

                foreach (var method in ns.RootMethods)
                {
                    var pInvoke = new Method
                    {
                        Name = string.Format("{0}_{1}", ns.Name, method.Name),
                        ResulTypeName = ReplaceTypeName(ns, method.ResulTypeName, method.ReturnLink),
                        ReturnLink = false
                    };
                    FillPInvokeMethod(ns, pInvoke, method, null);
                    resultNamespace.RootMethods.Add(pInvoke);
                }

            }
            return result;
        }

        public void FillPInvokeMethod(Namespace ns, Method pInvoke, Method method, Class c)
        {
            const string classInstance = "classInstance";
            pInvoke.Parameters.Add(new MethodParameter
            {
                Name = "env",
                TypeName = "JNIEnv",
                IsLink = true
            });
            pInvoke.Parameters.Add(new MethodParameter
            {
                Name = "object",
                TypeName = "jobject",
                IsLink = false
            });
            if (c != null)
            {
                pInvoke.Parameters.Add(new MethodParameter
                {
                    Name = classInstance,
                    TypeName = "jlong",
                    IsLink = false
                });
            }
            var parametersLine = "";
            var parametersConvert = "";
            var parametersReleaseConvert = "";
            for (var i = 0; i < method.Parameters.Count; i++)
            {
                var parameter = new MethodParameter(method.Parameters[i]);
                var originType = method.Parameters[i].FullTypeName;
                parameter.TypeName = ReplaceTypeName(ns, parameter.TypeName, parameter.IsLink);
                var pref = "";
                if (parameter.TypeName == "jstring")
                {
                    parametersConvert += string.Format("const char *str_{0} = env->GetStringUTFChars({0}, JNI_FALSE);",
                            parameter.Name);
                    parametersReleaseConvert += string.Format("env->ReleaseStringUTFChars({0}, str_{0});", parameter.Name);
                    pref = "str_";
                }
                parameter.IsLink = false;
                pInvoke.Parameters.Add(parameter);
                parametersLine += "(" + originType + ")" + pref + parameter.Name;
                if (i < (method.Parameters.Count - 1))
                    parametersLine += ", ";
            }
            pInvoke.Body = parametersConvert;
            if (method.ResulTypeName != "void")
            {
                if (string.IsNullOrEmpty(parametersConvert))
                {
                    pInvoke.Body += string.Format("return ({0})", pInvoke.ResulTypeName);
                }
                else
                {
                    pInvoke.Body += string.Format("{0} result = ({0})", pInvoke.ResulTypeName);
                }
            }
            if (c != null)
            {
                pInvoke.Body += string.Format("(({0} *){1})->{2}({3});", c.Name, classInstance, method.Name,
                    parametersLine);
            }
            else
            {
                pInvoke.Body += string.Format("{0}({1});", method.Name, parametersLine);
            }
            pInvoke.Body += parametersReleaseConvert;
            if (method.ResulTypeName != "void")
            {
                if (!string.IsNullOrEmpty(parametersConvert))
                {
                    pInvoke.Body += "return result;";
                }
            }
        }

        public string ReplaceTypeName(Namespace ns, string typeName, bool link)
        {
            var en = ns.Enums.FirstOrDefault(e => e.Name == typeName);
            if (en != null)
                return "jlong";
            if (link)
                return "jlong";
            switch (typeName)
            {
                case "String":
                    return "jstring";
                case "ushort":
                    return "jshort";
                case "uint":
                    return "jint";
                case "ulong":
                    return "jlong";
                case "IntPtr":
                    return "jlong";
                case "bool":
                    return "jboolean";
                case "byte":
                    return "jbyte";
                case "char":
                    return "jchar";
                case "short":
                    return "jshort";
                case "int":
                    return "jint";
                case "long":
                    return "jlong";
                case "float":
                    return "jfloat";
                case "double":
                    return "jdouble";
                default:
                    return typeName;
            }
        }
    }
}