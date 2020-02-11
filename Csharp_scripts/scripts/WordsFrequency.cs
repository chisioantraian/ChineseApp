using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;

namespace CSharp_scripts.scripts
{
    public static class WordsFrequency
    {
        private static string freqInput = @"C:\Users\chisi\Desktop\work\ChineseApp\FSharp_Library\wordfreq.utf8";
        private static string subtInput = @"C:\Users\chisi\Desktop\work\ChineseApp\Csharp_scripts\scripts\SUBTLEX.utf8";

        public static void Run()
        {
            foreach (string line in File.ReadLines(freqInput))
            {
                Console.WriteLine($"line: {line}");
            }
        }
    }
}
