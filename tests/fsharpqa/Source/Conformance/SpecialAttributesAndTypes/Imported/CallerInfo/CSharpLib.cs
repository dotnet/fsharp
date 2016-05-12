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
		
		public static string MemberName([CallerMemberName] string memberName = "dummy1")
        {
            return memberName;
        }
        
        public static Tuple<string, int, string> AllInfo(int normalArg, [CallerFilePath] string filePath = "dummy2", [CallerLineNumber] int line = 778, [CallerMemberName] string memberName = "dummy3")
        {
            return new Tuple<string, int, string>(filePath, line, memberName);
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

    public class MyCallerMemberNameAttribute : Attribute
    {
        public string MemberName { get; set; }

        public MyCallerMemberNameAttribute([CallerMemberName] string member = "dflt")
        {
            MemberName = member;
        }
    }
}