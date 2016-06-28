// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Layout the nuget host package for the fsharp compiler
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

    let usage = @"usage: layoutfcsnhostnuget.fsx -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let Arguments = fsi.CommandLineArgs |> Seq.skip 1
    
    let GetArgumentFromCommandLine switchName defaultValue = 
        match Arguments |> Seq.filter(fun t -> t.StartsWith(switchName)) |> Seq.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> tryHead with
        | Some(file) -> if file.Length <> 0 then file else defaultValue
        | _ -> defaultValue

    let verbose     = GetArgumentFromCommandLine    "--verbosity:"  @"normal"
    let nuspec      = GetArgumentFromCommandLine    "--nuspec:"     @""
    let bindir      = GetArgumentFromCommandLine    "--bindir:"     @""
    let nuspecTitle = Path.GetFileNameWithoutExtension(nuspec)
    let layouts     = Path.Combine(Path.GetFullPath(bindir), "layouts", nuspecTitle)
    let isVerbose   = verbose = "verbose"

    //=========================================================================================
    // Layout nuget package --- nothing to do here except make the directory
    //=========================================================================================
    let deleteDirectory output =
        if (Directory.Exists(output)) then Directory.Delete(output, true) |>ignore
        ()
    
    let makeDirectory output =
        if not (Directory.Exists(output)) then Directory.CreateDirectory(output) |>ignore
        ()

    //Clean intermediate directory
    deleteDirectory layouts; makeDirectory layouts

with e -> printfn "Exception:  %s" e.Message
          printfn "Stacktrace: %s" e.StackTrace
          exit (1)
exit (0)
