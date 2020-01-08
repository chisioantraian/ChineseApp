using ConsoleApp1_csharp.scripts;

namespace ConsoleApp1
{
    internal static class Program
    {
        public static void Main()
        {
            Subtlex s = new Subtlex();
            s.Run();
            s.CreateDatabase();
        }
    }
}

