// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Implement a global command to execute the built in F# interactive sesssion.
//
// This is a simple bootstrapper that bootstraps fsi using the fsi.exe that is deployed with the dotnet cli.
//
// The program figures out the location of fsi.exe by calling dotnet.exe --version
//
// fsi is located in a subdirectory below the executing dotnet.exe 
//  at the location sdk/%sdkversion%/FSharp/fsi.exe
//
// All command line arguments are passed through to the fsi.exe

module Microsoft.FSharp.Dotnet.Fsi

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
        let fullPathToFsi = Path.Combine(basePath, "FSharp", "fsi.exe")
        startProcess dotnetExe fullPathToFsi arguments

    | _ ->
        printfn "Unable to execute the command: %s %s" dotnetExe argumentBasePath
        1
 