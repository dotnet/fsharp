using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Security.Policy;
using System.Reflection;
using System.Collections;
using System.Security.Permissions;
using System.Drawing.Printing;
using System.Net;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.IO;
using PTRunner;
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level2)]


/// <summary>
/// This class holds the data that was parsed from the command line
/// </summary>
internal class CommandLineData
{
    public CommandLineData()
    {
        programName = null;
        arguments = null;
        permission = null;
        fullTrustAssemblies = new List<Assembly>();
    }
    //Program name
    public string programName;
    //Arguments to the program
    public string[] arguments;
    //The permission set that will be granted to the progra
    public PermissionSet permission;
    //The full trust list
    public List<Assembly> fullTrustAssemblies;
}


class PartialTrustRunner
{

    /// <summary>
    /// Print the usage of the program
    /// </summary>
    public static void Usage()
    {
        string name = Process.GetCurrentProcess().ProcessName;
        string possibleVals = Enum.GetNames(typeof(PermSetNames)).Aggregate((workset, next) => workset + ", " + next);
        Console.WriteLine("Usage:");
        Console.WriteLine("{0} [-ps NamedPermissionSet]|[-xml ptrunner.xml] [-af fullTrustAssembly] {{program arguments}}\n", name);
        Console.WriteLine("Parameters:");
        Console.WriteLine("\t -ps : PermissionSet The named permission set in which the application could be run. This can be any one of {0}", possibleVals);
        Console.WriteLine("\n\t -xml : XmlFile File containing the permission set in which the application could be run. ");
        Console.WriteLine("\n\t -af AddToFullTrust One assembly that you want in the full trust list. Your full trust assembly should be signed. See \"Strong-Named Assemblies\" in msdn for more information.");
        Console.WriteLine("\n\t program arguments Command and arguments for the runned progam. Once you pass anything that doesn't have a \"-\" as a parameter we consider you are passing the name and the parameters for the program that will be run by this runner.");
        Console.WriteLine();
        Console.WriteLine("Example usage: PTRunner.exe -af Samples\\librarydemander.dll -ps LocalIntranet Samples\\demander.exe");
        Console.WriteLine("\t This runs Samples\\demander.exe and also makes sure Samples\\librarydemander.dll is loaded as FullTrust");
        Console.WriteLine();
        Console.WriteLine("{0} is a program designed to run an application in partial trust in an easy and simple way. Under the covers it starts an AppDomain with a permission set and a full trust list and uses this AppDomain to run your application in.", name);
        Console.WriteLine("If you are creating AppDomains or your application uses a host, you might not be able to use this program. This program is designed to offer an easy solution to those that want to run something in partial trust and don\'t care about the intricacies of this endevour");
        Console.WriteLine();
        Console.WriteLine("NOTE: Make sure you are passing any parameters for the runner before passing the name of the runned program");
        Console.WriteLine("NOTE: For the xml parameter, the file format is exactly what caspol -lp would print.The file samples\\perm.xml is a good example.");
    }

    /// <summary>
    /// Command line parser.
    /// </summary>
    /// <param name="args">The command line arguments</param>
    /// <returns>A structure with information about the run</returns>
    private static CommandLineData ParseCommandLine(string[] args)
    {
        try
        {
            int i = 0;
            CommandLineData ret = new CommandLineData();

            while (i < args.Length)
            {
                if (args[i].StartsWith("-"))
                {
                    switch (args[i])
                    {
                        //The partial trust PermissionSet
                        case "-ps":
                            PermSetNames permSet = (PermSetNames)Enum.Parse(typeof(PermSetNames), args[++i], true);
                            ret.permission = PTRunnerLib.GetStandardPermission(permSet);
                            break;
                        //Add full trust assembly
                        case "-af":
                            Assembly asm = Assembly.LoadFrom(args[++i]);
                            ret.fullTrustAssemblies.Add(asm);
                            break;
                        case "-xml":
                            StreamReader sr = new StreamReader(args[++i]);
                            SecurityElement elem = SecurityElement.FromString(sr.ReadToEnd());
                            ret.permission = new PermissionSet(PermissionState.None);
                            ret.permission.FromXml(elem);
                            break;
                        default:
                            Console.WriteLine("{0} - unknonw option", args[i]);
                            Usage();
                            return null;
                    }
                    ++i;
                }
                else break;
            }
            if (i < args.Length)
            {
                //This are the arguments for the program that will be run
                ret.programName = args[i++];
                int argsSize = args.Length - i;
                ret.arguments = new string[argsSize];
                if (argsSize > 0)
                    Array.Copy(args, i, ret.arguments, 0, argsSize);
                if (ret.permission == null)
                    ret.permission = PTRunnerLib.GetStandardPermission(PermSetNames.Execution);
                return ret;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(":RUNNER: Got exception while parsing command line: {0}", ex.Message);
        }
        Usage();
        return null;
    }

    /// <summary>
    /// The main function of the runner, obviously
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    [SecuritySafeCritical]
    static int Main(string[] args)
    {
        //Parse the command line
        CommandLineData commands = ParseCommandLine(args);
        if (commands == null)
            return 1;
        AssemblyRunner runner =null;
        try
        {
            //Create the sandbox
            runner = PTRunnerLib.GetPartialTrustInstance<AssemblyRunner>(commands.permission, commands.fullTrustAssemblies.ToArray());
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("ERROR: {0}", ex.Message);
            return -1;
        }
        //And we execute the assembly!!!
        return runner.ExecuteAssembly(commands.programName, commands.arguments);
    }

}

/// <summary>
/// The runner. This class will be instantiated on the sandboxed AppDomain. This will be the actual starter
/// </summary>
public class AssemblyRunner : MarshalByRefObject
{
    /// <summary>
    /// Print some preaty information about startup
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="arguments"></param>
    internal static void PrintStart(string assemblyName, string[] arguments)
    {
        string args = "";
        if (arguments.Length > 0)
            args = " " + arguments.Aggregate((workingSentence, next) => workingSentence + " " + next);
        Console.WriteLine(":RUNNER: start: \"{0}{1}\"", assemblyName, args);
    }
    /// <summary>
    /// Print some preaty information about shutdown
    /// </summary>
    /// <param name="ret"></param>
    internal static void PrintStop(int ret)
    {
        Console.WriteLine(":RUNNER: return: {0}", ret);
    }

    public AssemblyRunner()
    {
    }

    /// <summary>
    /// Execute the client assembly. Arguments are passed to main
    /// </summary>
    /// <param name="assemblyName">The name of the assembly</param>
    /// <param name="arguments">Arguments to the main method</param>
    /// <returns>The return of the application</returns>
    public int ExecuteAssembly(string assemblyName, string[] arguments)
    {
        //Because there are a total of four possible signatures for main, we need a special indirection level which will hide that variety.
        //The reason we are not using dirrectly MethodInfo.Invoke is that for calling that a ReflectionPermission is required
        //which would taint the sandbox. So off to making the logic a touch more complex just to allow the client to get what he expects
        MainDelegates.MainWrapper main = MainDelegates.MainWrapper.ConstructMainWrapper(assemblyName);
        PrintStart(assemblyName, arguments);
        int ret = 0;
        ret = main.Invoke(arguments);
        PrintStop(ret);
        return ret;
    }
}

namespace MainDelegates
{
    //MSDN says there are only void/int return values, and non/string[] for parameter. That makes a total of 4 possible delegates

    /// <summary>
    /// This is the base class for each of the wrapper. These wrappers will be an abstraction over the 4 possible delegates
    /// </summary>
    internal abstract class MainWrapper
    {
        public abstract int Invoke(string[] args);
        /// <summary>
        /// This method is in charge of creating the propper main-delegate wrapper. It loads the assembly, gets the entry point
        /// and make a decision about which wrapper to create
        /// </summary>
        /// <param name="assemblyName">The assembly name</param>
        /// <returns>One of the four different main-delegate wrappers</returns>
        [SecuritySafeCritical]
        public static MainWrapper ConstructMainWrapper(string assemblyName)
        {
            (new PermissionSet(PermissionState.Unrestricted)).Assert();
            Assembly a = Assembly.LoadFrom(assemblyName);
            MethodInfo mi = a.EntryPoint;

            if (mi.ReturnType == typeof(void))
            {
                //No parameter, void return
                if (mi.GetParameters().Length == 0)
                    return new MainVoidEmpty(mi);
                else
                    //Parameters but void return
                    return new MainVoidArgs(mi);
            }
            else 
            {
                //Int return but no parameters
                if (mi.GetParameters().Length == 0)
                    return new MainIntEmpty(mi);
                else
                    //int return and string[] as parameters
                    return new MainIntFull(mi);
            }

        }
    }
    /// <summary>
    /// This wrapper treats the int Main(string[]args) signature
    /// </summary>
    internal class MainIntFull : MainWrapper
    {
        delegate int MainDelegateIntArg(string[] args);
        MainDelegateIntArg _dlg;
        public MainIntFull(MethodInfo mi)
        {

            _dlg = (MainDelegateIntArg)Delegate.CreateDelegate(typeof(MainDelegateIntArg), mi);
        }
        public override int Invoke(string[] args)
        {
            return _dlg(args);
        }
    }
    /// <summary>
    /// This wrapper treats the void Main(string[]args) signature
    /// </summary>
    internal class MainVoidArgs : MainWrapper
    {
        delegate void MainDelegateVoidArg(string[] args);
        MainDelegateVoidArg _dlg;
        public MainVoidArgs(MethodInfo mi)
        {
            _dlg = (MainDelegateVoidArg)Delegate.CreateDelegate(typeof(MainDelegateVoidArg), mi);
        }
        public override int Invoke(string[] args)
        {
            _dlg(args);
            return 0;
        }
    }
    /// <summary>
    /// This wrapper treats the int Main() signature
    /// </summary>
    internal class MainIntEmpty : MainWrapper
    {
        delegate int MainDelegateIntNoArg();
        MainDelegateIntNoArg _dlg;
        public MainIntEmpty(MethodInfo mi)
        {
            _dlg = (MainDelegateIntNoArg)Delegate.CreateDelegate(typeof(MainDelegateIntNoArg), mi);
        }
        public override int Invoke(string[] args)
        {
            return _dlg();
        }
    }
    /// <summary>
    /// This wrapper treats the int Main() signature
    /// </summary>
    internal class MainVoidEmpty : MainWrapper
    {
        delegate void MainDelegateVoidNoArg();
        MainDelegateVoidNoArg _dlg;
        public MainVoidEmpty(MethodInfo mi)
        {
            _dlg = (MainDelegateVoidNoArg)Delegate.CreateDelegate(typeof(MainDelegateVoidNoArg), mi);
        }
        public override int Invoke(string[] args)
        {
            _dlg();
            return 0;
        }
    }
}