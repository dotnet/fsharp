// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Layout the nuget package for the fsharp compiler
//=========================================================================================
open System.IO

try
    //=========================================================================================
    // Command line arguments
    //=========================================================================================

    // Try head was introduced in F# 4.0
    let tryHead (source : seq<_>) =
        let checkNonNull argName arg = 
            match box arg with 
            | null -> nullArg argName 
            | _ -> ()
        checkNonNull "source" source
        use e = source.GetEnumerator() 
        if (e.MoveNext()) then Some e.Current
        else None

    let usage = @"usage: layoutfcsnuget.fsx -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let Arguments = fsi.CommandLineArgs |> Seq.skip 1
    
    let GetArgumentFromCommandLine switchName defaultValue = 
        match Arguments |> Seq.filter(fun t -> t.StartsWith(switchName)) |> Seq.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> tryHead with
        | Some(file) -> if file.Length <> 0 then file else defaultValue
        | _ -> defaultValue

    let verbose     = GetArgumentFromCommandLine    "--verbosity:"  @"normal"
    let nuspec      = GetArgumentFromCommandLine    "--nuspec:"     @""
    let bindir      = GetArgumentFromCommandLine    "--bindir:"     @""
    let nuspecTitle = Path.GetFileNameWithoutExtension(nuspec)
    printfn ">>%s<<" bindir
    printfn ">>%s<<" nuspecTitle
    let layouts     = Path.Combine(Path.GetFullPath(bindir), "layouts", nuspecTitle)
    let isVerbose   = verbose = "verbose"

    //=========================================================================================
    // Layout nuget package
    //=========================================================================================
    let copyFile source dir =
        let dest = 
            if not (Directory.Exists(dir)) then Directory.CreateDirectory(dir) |>ignore
            let result = Path.Combine(dir, Path.GetFileName(source))
            result
        if isVerbose then
            printfn "source: %s" source
            printfn "dest:   %s" dest
        File.Copy(source, dest, true)

    let deleteDirectory (output) =
        if (Directory.Exists(output)) then Directory.Delete(output, true) |>ignore
        ()
    
    let makeDirectory (output) =
        if not (Directory.Exists(output)) then Directory.CreateDirectory(output) |>ignore
        ()

    let fsharpCompilerFiles =
        seq {
            yield Path.Combine(bindir, "fsc.exe")
            yield Path.Combine(bindir, "FSharp.Compiler.dll")
            yield Path.Combine(bindir, "default.win32manifest")
            yield Path.Combine(bindir, "fsi.exe")
            yield Path.Combine(bindir, "FSharp.Compiler.Interactive.Settings.dll")
        }
    
    //Clean intermediate directoriy
    deleteDirectory(layouts); makeDirectory(layouts)
    fsharpCompilerFiles |> Seq.iter(fun source -> copyFile source layouts)

with e -> printfn "Exception:  %s" e.Message
          printfn "Stacktrace: %s" e.StackTrace
          exit (1)
exit (0)
