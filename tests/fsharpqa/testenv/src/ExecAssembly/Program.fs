open System
open System.Security
open System.Security.Permissions

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