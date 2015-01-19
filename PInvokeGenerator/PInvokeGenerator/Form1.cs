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

namespace PInvokeGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var input = File.ReadAllText(@"C:\_faust\FaustGamesTools\PInvokeGenerator\include\TestHeader.h");
            var parser = new ParserCpp();
            var result = parser.Parse(input);


        }
    }
}
