﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFormatter;

namespace PInvokeTool
{
    class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            if (Directory.Exists("include"))
            {
                var headers = Directory.GetFiles("include", "*.h");
                foreach (var a in headers)
                {
                    Console.WriteLine("processing '"+a+"'");
                    var ext = Path.GetExtension(a);
                    if (ext == null) continue;
                    if (ext.ToLower() != ".h") continue;
                    var name = Path.GetFileNameWithoutExtension(a);
                    if (name == null) continue;
                    var dir = Path.GetDirectoryName(a);
                    if (dir == null) continue;

                    var input = File.ReadAllText(a);
                    var parser = new ParserCpp();
                    var result = parser.Parse(name, name + ".h", input);
                    result.Build();
                    var pInvoke = result.CreatePInvokeCpp();
                    var formatter = new FormatProviderCppPInvoke();
                    var tabulator = new Tabulator();
                    formatter.Format(pInvoke, tabulator);
                    Console.WriteLine("save '" + @"gen\" + name + "PInvoke.cpp" + "'");
                    File.WriteAllText(@"gen\" + name + "PInvoke.cpp", tabulator.Build());

                    var pInvokeCSharp = result.CreatePInvokeCSharp();
                    var formatterCSharp = new FormatProviderCSharpPInvoke();
                    var tabulatorCSharp = new Tabulator();
                    formatterCSharp.Format(pInvokeCSharp, tabulatorCSharp);
                    Console.WriteLine("save '" + @"gen\" + name + "PInvoke.cs" + "'");
                    File.WriteAllText(@"gen\" + name + "PInvoke.cs", tabulatorCSharp.Build());
                }
            }
#endif
            if (args.Length == 0)
            {
                // todo: display help
            }

            foreach (var a in args)
            {
                //if (!Directory.Exists(a)) continue;

                var ext = Path.GetExtension(a);
                if (ext == null) continue;
                if (ext.ToLower() != ".h") continue;
                var name = Path.GetFileNameWithoutExtension(a);
                if (name == null) continue;
                var dir = Path.GetDirectoryName(a);
                if (dir == null) continue;

                var arg = Path.Combine(dir, name);
                var input = File.ReadAllText(arg + ".h");
                var parser = new ParserCpp();
                var result = parser.Parse(arg, arg + ".h", input);
                result.Build();

                SaveGeneratedFile<FormatProviderCppPInvoke, Tabulator>(result.CreatePInvokeCpp(), arg + "PInvoke.cpp");
                SaveGeneratedFile<FormatProviderCppJni, Tabulator>(result.CreateJniCpp(), arg + "JniBridge.cpp");
                SaveGeneratedFile<FormatProviderCSharpPInvoke, Tabulator>(result.CreatePInvokeCSharp(), arg + "PInvoke.cs");
            }

        }

        public static void SaveGeneratedFile<TFormatProvider, TTabulator>(CodeFile file, string fileName)
            where TFormatProvider : FormatProvider, new() where TTabulator : Tabulator, new()
        {
            var formatter = new TFormatProvider();
            var tabulator = new TTabulator();
            formatter.Format(file, tabulator);
            File.WriteAllText(fileName, tabulator.Build());
        }
    }
}
