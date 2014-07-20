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




/// Prints usage information to stdout then exits with a nonzero error code.
let printUsageAndExit () =
    eprintfn "GACs built binaries, adds required strong name verification skipping, and optionally NGens built binaries"
    eprintfn "Usage:"
    eprintfn "   update.cmd debug [-ngen]"
    eprintfn "   update.cmd release [-ngen]"
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

// Get paths to required .NET tools.
let bindir =
    sprintf "%s\\..\\%s\\net40\\bin" Environment.CurrentDirectory build_config

let ``Program Files (x86)`` =
    let specialFolder =
        if Environment.Is64BitOperatingSystem then
            Environment.SpecialFolder.ProgramFilesX86
        else
            Environment.SpecialFolder.ProgramFiles
    Environment.GetFolderPath specialFolder

let gacutil =
    sprintf "%s\\Microsoft SDKs\\Windows\\v8.0A\\bin\\NETFX 4.0 Tools\\gacutil.exe" ``Program Files (x86)``
let sn32 =
    sprintf "%s\\Microsoft SDKs\\Windows\\v8.0A\\bin\\NETFX 4.0 Tools\\sn.exe" ``Program Files (x86)``
let sn64 =
    sprintf "%s\\Microsoft SDKs\\Windows\\v8.0A\\bin\\NETFX 4.0 Tools\\x64\\sn.exe" ``Program Files (x86)``

let windir =
    Environment.GetFolderPath Environment.SpecialFolder.Windows
let ngen32 =
    sprintf "%s\\Microsoft.NET\\Framework\\v4.0.30319\\ngen.exe" windir
let ngen64 =
    sprintf "%s\\Microsoft.NET\\Framework64\\v4.0.30319\\ngen.exe" windir
;;

// Disable strong-name validation for F# binaries built from open source that are signed with the microsoft key
let fsharpAssemblyNamesWithToken =
    let microsoftPublicKeyToken = "b03f5f7f11d50a3a"

    let fsharpAssemblyNames =
        [|  "FSharp.Build";
            "FSharp.Core";
            "FSharp.Compiler";
            "FSharp.Compiler.Interactive.Settings";
            "FSharp.Compiler.Hosted";
            "FSharp.Compiler.Server.Shared";
            "FSharp.Editor";
            "FSharp.LanguageService";
            "FSharp.LanguageService.Base";
            "FSharp.LanguageService.Compiler";
            "FSharp.ProjectSystem.Base";
            "FSharp.ProjectSystem.FSharp";
            "FSharp.ProjectSystem.PropertyPages";
            "FSharp.VS.FSI"; |]

    fsharpAssemblyNames
    |> Array.map (fun assemblyName ->
        sprintf "%s,%s" assemblyName microsoftPublicKeyToken)
;;

for assemblyNameAndToken in fsharpAssemblyNamesWithToken do
    execf sn32 "-Vr %s" assemblyNameAndToken

// For 64-bit versions of Windows, we also need to run the 64-bit version of the strong-name tool.
if Environment.Is64BitOperatingSystem then
    for assemblyNameAndToken in fsharpAssemblyNamesWithToken do
        execf sn64 "-Vr %s" assemblyNameAndToken
;;

// Only GACing FSharp.Core for now
do execf gacutil "/if \"%s\FSharp.Core.dll\"" bindir;;

// NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll
if Array.length args > 1 && ciequals args.[1] "-ngen" then
    execf ngen32 "install \"%s\\fsc.exe\" /queue:1" bindir
    execf ngen32 "install \"%s\\fsi.exe\" /queue:1" bindir
    execf ngen32 "install \"%s\\FSharp.Build.dll\" /queue:1" bindir
    execf ngen32 "executeQueuedItems 1"

    if Environment.Is64BitOperatingSystem then
        execf ngen32 "install \"%s\\fsiAnyCpu.exe\" /queue:1" bindir
        execf ngen32 "install \"%s\\FSharp.Build.dll\" /queue:1" bindir
        execf ngen32 "executeQueuedItems 1"
;;
