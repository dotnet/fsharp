// ===========================================================================================================
// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, 
//               Version 2.0.  See License.txt in the project root for license information.
// ===========================================================================================================

open System
open System.Diagnostics
open System.IO;;

/// Case-insensitive string equality comparison.
let ciequals x y =
    StringComparer.OrdinalIgnoreCase.Equals (x, y)

/// Executes a command with the specified arguments.
let exec command args =
    //printfn "Starting process: %s %s" command args

    use proc = new Process ()
    proc.StartInfo <-
        let procStartInfo = System.Diagnostics.ProcessStartInfo (command, args)
        procStartInfo.RedirectStandardError <- true
        procStartInfo.RedirectStandardOutput <- true
        procStartInfo.UseShellExecute <- false
        procStartInfo

    // Capture output from the process and write it into the output stream of the fsi process.
    proc.OutputDataReceived
    |> Event.add (fun args ->
        System.Console.Out.WriteLine args.Data)
    proc.ErrorDataReceived
    |> Event.add (fun args ->
        System.Console.Error.WriteLine args.Data)

    if not <| proc.Start () then
        eprintfn "Unable to start the process: \"%s %s\"" command args
    proc.BeginOutputReadLine ()
    proc.BeginErrorReadLine ()
    proc.WaitForExit ()

/// Executes a command with the specified formatted arguments.
let inline execf command formattedArgs =
    Printf.ksprintf (exec command) formattedArgs
    
/// Gets the value of the environment variable with the given name.
let getenv name =
    match Environment.GetEnvironmentVariable name with
    | null -> None
    | value -> Some value

/// <summary>
/// Inserts or resets the environment variable <paramref name="name"/> in the current environment list.
/// </summary>
let setenv name value =
    Environment.SetEnvironmentVariable (name, value, EnvironmentVariableTarget.Process)

/// Deletes the variable with the given name from the environment.
let unsetenv name =
    Environment.SetEnvironmentVariable (name, null, EnvironmentVariableTarget.Process)
;;




/// Prints usage information to stdout then exits with a nonzero error code.
let printUsageAndExit () =
    eprintfn "Builds a few test tools using latest compiler and runtime"
    eprintfn "Usage:"
    eprintfn "   fsi BuildTestTools.fsx debug"
    eprintfn "   fsi BuildTestTools.fsx release"
    exit 1;;

let args =
    // Trim off the 0th argument, which is the command name (fsi or fsharpi).
    // Also trim off the 1st argument, which is the name of this script.
    Environment.GetCommandLineArgs().[2..]

// Are the args empty?
if Array.isEmpty args then
    printUsageAndExit ()

let build_config = args.[0]
if not <| ciequals build_config "debug" && not <| ciequals build_config "release" then
    printUsageAndExit ()
;;


execf "msbuild" "%s\\fsharpqa\\testenv\\src\\ILComparer\\ILComparer.fsproj /p:Configuration=%s /t:Build"
    Environment.CurrentDirectory build_config
execf "xcopy" "/Y %s\\fsharpqa\\testenv\\src\\ILComparer\\bin\\%s\\* %s\\fsharpqa\\testenv\\bin"
    Environment.CurrentDirectory build_config Environment.CurrentDirectory

execf "msbuild" "%s\\fsharpqa\\testenv\\src\\HostedCompilerServer\\HostedCompilerServer.fsproj /p:Configuration=%s /t:Build"
    Environment.CurrentDirectory build_config
execf "xcopy" "/Y %s\\fsharpqa\\testenv\\src\\HostedCompilerServer\\bin\\%s\\* %s\\fsharpqa\\testenv\\bin"
    Environment.CurrentDirectory build_config Environment.CurrentDirectory

if Directory.Exists <| sprintf "%s\..\%s\net40\bin" Environment.CurrentDirectory build_config then
    execf "xcopy" "xcopy /Y %s\\..\%s\\net40\\bin\\FSharp.Core.sigdata fsharpqa\\testenv\\bin"
        Environment.CurrentDirectory build_config
    execf "xcopy" "xcopy /Y %s\\..\\%s\\net40\\bin\\FSharp.Core.optdata fsharpqa\\testenv\\bin"
        Environment.CurrentDirectory build_config
;;
