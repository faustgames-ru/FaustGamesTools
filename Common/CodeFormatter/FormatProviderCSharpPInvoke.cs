namespace CodeFormatter
{
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
            BeginCodeBlock(tabulator, FormatPatterns.PatternClass, "Version");
            tabulator.AppendFormatLineWithoutTabs("#if __IOS__");
            tabulator.AppendFormatLine("public const string Dll = \"{0}\";", "__Internal");
            tabulator.AppendFormatLineWithoutTabs("#else");
            tabulator.AppendFormatLine("public const string Dll = \"{0}\";", _file.LibraryName);
            tabulator.AppendFormatLineWithoutTabs("#endif");
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