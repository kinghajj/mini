using System;

namespace Mini.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ini = new IniFile("test.ini");
            Console.WriteLine("Test\'s comment:\n\'{0}\'", ini["Test"].Comment);
            if(ini["Test"].Comment == String.Empty)
                Console.WriteLine("Comment is empty!");
            Console.WriteLine("*Drum roll* -- {0}", ini["Test"]["Hello"].Value);
            ini.SaveAs("test2.ini");
            /*var parser = new IniPatternMatcher(new StreamReader(File.Open("test.ini", FileMode.Open)));
            while(!parser.EndOfStream)
            {
                IniPatternKind kind = parser.GetNextPattern();
                if(kind != IniPatternKind.None)
                    Console.WriteLine(kind);
                switch(kind)
                {
                    case IniPatternKind.Comment:
                        Console.WriteLine("\tComment\t= \'{0}\'",
                            parser.LastComment);
                        break;
                    case IniPatternKind.Section:
                        Console.WriteLine("\tName\t= \'{0}\'\n\tComment\t= \'{1}\'",
                            parser.LastName, parser.LastComment);
                        break;
                    case IniPatternKind.Setting:
                        Console.WriteLine("\tName\t= \'{0}\'\n\tValue\t= \'{1}\'\n\tComment\t= \'{2}\'",
                            parser.LastName, parser.LastValue, parser.LastComment);
                        break;
                }
             }*/
        }
    }
}
