// replacements for classes which are entirely removed from the portable profile
// either calls out to hooks, or implements functionality locally for simple cases

namespace System

type Console() = 
    static member Write s = stdout.Write s
    static member WriteLine () = stdout.WriteLine ""
    static member WriteLine (f : obj, [<ParamArray>] args) = 
        match f with
        | null -> ()
        | _ -> stdout.WriteLine (System.String.Format(f.ToString(), args))

type Version = 
  {
    Major : int
    Minor : int
  }

type Environment() =
    static let mutable exitCode  = 0
    static member CurrentDirectory = Hooks.getCurrentDirectory.Invoke()
    static member ExitCode
        with get() = exitCode
        and set(v) = exitCode <- v
    static member GetCommandLineArgs() = ["aa"; "bb"]
    static member GetEnvironmentVariable varName = Hooks.getEnvironmentVariable.Invoke(varName)
    static member Version = { Major = Hooks.majorVersion.Invoke();  Minor = Hooks.minorVersion.Invoke() }