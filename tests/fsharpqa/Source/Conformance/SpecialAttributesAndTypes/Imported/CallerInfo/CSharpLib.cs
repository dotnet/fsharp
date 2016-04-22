using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CSharpLib
{
    public class CallerInfoTest
    {
        public static int LineNumber([CallerLineNumber] int line = 777)
        {
            return line;
        }
        
        public static string FilePath([CallerFilePath] string filePath = "dummy1")
        {
            return filePath;
        }
        
        public static Tuple<string, int> AllInfo(int normalArg, [CallerFilePath] string filePath = "dummy2", [CallerLineNumber] int line = 778)
        {
            return new Tuple<string, int>(filePath, line);
        }
    }

    public class MyCallerInfoAttribute : Attribute
    {
        public int LineNumber { get; set; }
        
        public MyCallerInfoAttribute([CallerLineNumber] int lineNumber = -1)
        {
            LineNumber = lineNumber;
        }
    }
}