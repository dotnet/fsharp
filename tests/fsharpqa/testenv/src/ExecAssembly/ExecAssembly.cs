using System;
using System.Reflection;

class C
{
    static int Main(string[] args)
    {
        // Create a new App Domain
        var appdomain = AppDomain.CreateDomain("F# targeting NetFx 2.0");

        // Set the assembly to be loaded and executed. It will be a command line argument...
        var assemblyundertest = args[0];

        // Execute the assembly
        var rv = appdomain.ExecuteAssembly(assemblyundertest);

        // Return exit code to the automation harness
        return rv;
    }
}