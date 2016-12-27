using System.IO;

namespace CodeFormatter
{
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

    public class FormatProviderCppJni : FormatProvider
    {
        public FormatProviderCppJni()
            : base(FormatPatterns.Cpp)
        {
        }

        public override void Format(CodeFile file, Tabulator tabulator)
        {
            tabulator.AppendFormatLine("#include <jni.h>");
            tabulator.AppendFormatLine("#include \"{0}\"", Path.GetFileName(file.FileName));
            tabulator.AppendLine("");
            base.Format(file, tabulator);
        }
    }

    public class FormatProviderJavaJni : FormatProvider
    {
        public FormatProviderJavaJni()
            : base(FormatPatterns.Java)
        {
        }

        public override void Format(CodeFile file, Tabulator tabulator)
        {
            base.Format(file, tabulator);
        }
    }
}