namespace CodeFormatter
{
    public class CodeFileBuilderPInvokeCpp : CodeFileBuilder
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
    }
}