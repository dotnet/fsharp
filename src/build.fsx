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




// Set some environment variables which can be useful for debugging purposes;
// they either influence the build process or activate internal switches within the F# compiler / libraries.
#if DEBUG
setenv "FSHARP_verboseOptimizationInfo" Boolean.TrueString
setenv "FSHARP_verboseOptimizations" Boolean.TrueString
#else
unsetenv "FSHARP_verboseOptimizationInfo"
unsetenv "FSHARP_verboseOptimizations"
#endif

// Create a folder (if necessary) to hold the build logs.
let build_logs_folder = "build-logs"
if not <| Directory.Exists build_logs_folder then
    Directory.CreateDirectory build_logs_folder |> ignore
;;

// Set the verbosity level.
let build_verbosity = "detailed"

// MSBuild command.
let msbuild = "msbuild"

/// Builds a project using MSBuild.
let buildProject projectFilename targetFramework =
    match targetFramework with
    | None ->
        execf msbuild "%s /flp:Verbosity=%s;LogFile=%s\\%s.log"
            projectFilename build_verbosity build_logs_folder projectFilename
    | Some targetFramework ->
        execf msbuild "%s /p:TargetFramework=%s /flp:Verbosity=%s;LogFile=%s\\%s.log"
            projectFilename targetFramework build_verbosity build_logs_folder projectFilename
;;

// Build the proto compiler.
buildProject "fsharp-proto-build.proj" None

// Build FSharp.Core for various target frameworks, then build the normal F# compiler and other tools.
buildProject "fsharp-library-build.proj" <| Some "net40"
buildProject "fsharp-compiler-build.proj" <| Some "net40"
buildProject "fsharp-typeproviders-build.proj" <| Some "net40"
buildProject "fsharp-library-unittests-build.proj" <| Some "net40"
buildProject "fsharp-library-build.proj" <| Some "net20"
buildProject "fsharp-library-build.proj" <| Some "portable47"
buildProject "fsharp-library-build.proj" <| Some "portable7"
;;

