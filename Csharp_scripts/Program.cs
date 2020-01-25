using ConsoleApp1_csharp.scripts;

using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using NumeTestare;

namespace ConsoleApp1
{
    internal static class Program
    {
        public static void Main()
        {
            //Subtlex s = new Subtlex();
            //s.Run();
            var words = Testare.cautaTateCuvintele().ToList();
            words.ForEach(w => Console.WriteLine($"{w.Simplified} #"));
            //s.CreateDatabase();
        }

    }
}

