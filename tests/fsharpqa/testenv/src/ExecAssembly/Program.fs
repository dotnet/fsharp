open System
open System.Security
open System.Security.Permissions

(*
   This program loads a specified .NET exe and runs it in a dedicated appdomain.
   Useful for 2 types of tests:
   - The exe is build against .NET 2
     - Use this to force it to run in .NET 4+ environment
   - The exe is built against an earlier version of FSharp.Core
     - Use this to force it to run with binding to latest FSharp.Core
     
  Usage: ExecAssembly <path to exe> [optional args for exe]
*)

[<EntryPoint>]
let main args =
    let setup = AppDomainSetup(ApplicationBase = Environment.CurrentDirectory)
    let name = "App domain with F# vLatest and .NET vLatest"
    let appDomain = AppDomain.CreateDomain(name, null, setup, PermissionSet(PermissionState.Unrestricted))

    let assemblyUnderTest = args.[0]
    let exeArgs = args.[1..]
    
    try
        exit (appDomain.ExecuteAssembly(assemblyUnderTest, exeArgs))
    with
    | :? BadImageFormatException as e ->
            printfn "%O" e
            printfn ""
            printfn "Is the loaded assembly built for the same platform (x86/x64) as ExecAssembly?"
            exit 1