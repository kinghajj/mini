using System;

namespace Mini.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ini = new IniFile("test.ini");
            ini.SaveAs("test2.ini");
        }
    }
}
