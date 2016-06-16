// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
// Build nuget package for the fsharp compiler
//=========================================================================================
open System.IO
open System.Diagnostics

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

    let usage = @"usage: BuildNuGets.fsx --version:<build-version> -nuspec:<nuspec-path> --binaries:<binaries-dir>"

    let Arguments = fsi.CommandLineArgs |> Seq.skip 1

    let GetArgumentFromCommandLine switchName defaultValue = 
        match Arguments |> Seq.filter(fun t -> t.StartsWith(switchName)) |> Seq.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> tryHead with
        | Some(file) -> if file.Length <> 0 then file else defaultValue
        | _ -> defaultValue

    let verbose     = GetArgumentFromCommandLine    "--verbosity:"  @"normal"
    let version     = GetArgumentFromCommandLine    "--version:"    @""
    let nuspec      = GetArgumentFromCommandLine    "--nuspec:"     @""
    let bindir      = GetArgumentFromCommandLine    "--bindir:"     @""
    let nuspecTitle = Path.GetFileNameWithoutExtension(nuspec)
    let layouts     = Path.Combine(Path.GetFullPath(bindir), "layouts", nuspecTitle)
    let output      = Path.Combine(Path.GetFullPath(bindir), "nuget")
    let isVerbose   = verbose = "verbose"

    let makeDirectory output =
        if not (Directory.Exists(output)) then Directory.CreateDirectory(output) |>ignore
        ()

    //=========================================================================================
    // Build Nuget Package
    //=========================================================================================
    let author =     @"Microsoft"
    let licenseUrl = @"https://github.com/Microsoft/visualfsharp/blob/master/License.txt"
    let projectUrl = @"https://github.com/Microsoft/visualfsharp"
    let tags =       @"Visual F# Compiler FSharp coreclr functional programming"

    let nugetArgs = sprintf "pack %s -BasePath \"%s\" -OutputDirectory \"%s\" -ExcludeEmptyDirectories -prop licenseUrl=\"%s\" -prop version=\"%s\" -prop authors=\"%s\" -prop projectURL=\"%s\" -prop tags=\"%s\" -Verbosity detailed"
                            nuspec
                            layouts
                            output
                            licenseUrl
                            version
                            author
                            projectUrl
                            tags

    let nugetExePath = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, @"..\..\.nuget\nuget.exe"))
    let executeProcess filename arguments =
        let processWriteMessage (chan:TextWriter) (message:string) =
            match message with
            | null -> ()
            | _ as m -> chan.WriteLine(m) |>ignore
        let info = new ProcessStartInfo()
        let p = new Process()
        printfn "%s %s" filename arguments
        info.Arguments <- arguments
        info.UseShellExecute <- false
        info.RedirectStandardOutput <- true
        info.RedirectStandardError <- true
        info.CreateNoWindow <- true
        info.FileName <- filename
        p.StartInfo <- info
        p.OutputDataReceived.Add(fun x -> processWriteMessage stdout x.Data)
        p.ErrorDataReceived.Add(fun x ->  processWriteMessage stderr x.Data)
        if p.Start() then
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()
            p.WaitForExit()
            p.ExitCode
        else
            0
    makeDirectory output
    exit (executeProcess nugetExePath nugetArgs) 
with _ -> 
    exit (1)
