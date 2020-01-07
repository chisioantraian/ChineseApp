using ConsoleApp1_csharp.scripts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using WpfApp2.Logic;
using WpfApp2.Models;
using static WpfApp2.Logic.Kangxi;

namespace ConsoleApp1
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Subtlex s = new Subtlex();
            s.Run();
            s.CreateDatabase();
        }
    }
}

