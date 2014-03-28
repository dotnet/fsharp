// Dev11:210820, ensuring that the C# attributes to get line numbers and directories work correctly across F# boundaries

using System;

namespace ClassLibrary1
{
    public class Class1
    {
        public void TraceMessage(string message = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int line = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string file = "")
        {
            var s = String.Format("{0}:{1} - {2}", file, line, message);
            Console.WriteLine(s);
        }
        public void DoStuff()
        {
            TraceMessage("called DoStuff");
        }
    }
}
