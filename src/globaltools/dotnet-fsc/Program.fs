// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Implement a global command to execute the built in F# compiler.
//
// This is a simple bootstrapper that bootstraps fsc using the fsc.exe that is deployed with the dotnet cli.
//
// The program figures out the location of fsc.exe by calling dotnet.exe --version
//
// fsc is located in a subdirectory below the executing dotnet.exe 
//  at the location sdk/%sdkversion%/FSharp/fsc.exe
//
// All command line arguments are passed through to the fsc.exe

module Microsoft.FSharp.Dotnet.Fsc

open System
open System.Diagnostics
open System.IO
open Microsoft.FSharp.Dotnet.GlobalTools.Shared

[<EntryPoint>]
let main arguments =

    let dotnetExe = Process.GetCurrentProcess().MainModule.FileName
    Environment.SetEnvironmentVariable("DOTNET_MULTILEVEL_LOOKUP", "false")

    match executeProcessScrapeOutput dotnetExe argumentBasePath (Some scrapeOutputForBasePath) with
    | Some basePath ->
        let fullPathToFsc = Path.Combine(basePath, "FSharp", "fsc.exe")
        startProcess dotnetExe fullPathToFsc arguments

    | _ ->
        printfn "Unable to execute the command: %s %s" dotnetExe argumentBasePath
        1
 