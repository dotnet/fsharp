
using System;
using System.Diagnostics;

class Maine
{
    public static int failures = 0;
    
    public static void Fail(string outputString)
    {
        failures++;
        Console.WriteLine(outputString + new StackTrace(true).ToString());
    }
    static int Main()
    {
        if (Lib.addToRef(3) != 9)
            Fail("Expected initialized value");
        if (Lib.addToRef(6) != 15)
            Fail("Expected initialized value (2)");
        if (Lib.addToRef2(3) != 10)
            Fail("Expected initialized value");
        if (Lib.addToRef2(6) != 16)
            Fail("Expected initialized value (2)");

        Console.WriteLine("failures = {0}", failures);
        
        return failures;
    }





}
