using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFormatter;
using Test;

namespace PInvokeGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            /*
            var factory = TestDll.CreateTestFactory();
            var args = factory.CreateUpdateArgs();
            var sys = factory.CreateTestSystem();

            var time = args.GetElapsedTime();
            args.SetElapsedTime(0.1f);
            time = args.GetElapsedTime();
            sys.Update(args);
            */
            
            var input = File.ReadAllText(@"C:\_faust\FaustGamesTools\PInvokeGenerator\include\TestHeader.h");
            var parser = new ParserCpp();
            var result = parser.Parse("TestDll.dll", "TestHeader.h", input);
            result.Build();
            var pInvoke = result.CreatePInvokeCpp();
            var formatter = new FormatProviderCppPInvoke();
            var tabulator = new Tabulator();
            formatter.Format(pInvoke, tabulator);
            File.WriteAllText(@"C:\_faust\FaustGamesTools\PInvokeGenerator\gen\TestHeaderPInvoke.cpp", tabulator.Build());

            var pInvokeCSharp = result.CreatePInvokeCSharp();
            var formatterCSharp = new FormatProviderCSharpPInvoke();
            var tabulatorCSharp = new Tabulator();
            formatterCSharp.Format(pInvokeCSharp, tabulatorCSharp);
            File.WriteAllText(@"C:\_faust\FaustGamesTools\PInvokeGenerator\gen\TestHeaderPInvoke.cs", tabulatorCSharp.Build());
            
        }
    }
}
