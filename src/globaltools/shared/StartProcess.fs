// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Implement a global command to execute the built in F# compiler.
// 
// CreateProcess Utilities
//

module Microsoft.FSharp.Dotnet.GlobalTools.Shared

open System
open System.Diagnostics
open System.IO

///
/// Execute dotnet.exe passing in the path to F# tool and the arguments
///
let startProcess path fullPathToApp args =

    let info = 
        ProcessStartInfo(
            Arguments = "\"" + fullPathToApp + "\" " + (args |> String.concat " "),
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = false,
            FileName = path)
    use p = new Process(StartInfo = info)
    if p.Start() then
        p.WaitForExit()
        p.ExitCode
    else
        1

///
/// Execute dotnet.exe argument and scrape the Output
///
let executeProcessScrapeOutput path arguments (scrapeOutput:(StreamReader -> string option) option) : string option =

    let executeTimeOut = 5000

    let info = 
        ProcessStartInfo(
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = false,
            CreateNoWindow = true,
            FileName = path)

    let stream = new MemoryStream()
    let streamWriter = new StreamWriter(stream)
    use p = new Process(StartInfo = info)
    p.OutputDataReceived.Add(fun x -> if not (isNull(x.Data)) then streamWriter.WriteLine(x.Data))

    let gotIt =
        if p.Start() then
            p.BeginOutputReadLine()
            if not (p.WaitForExit(executeTimeOut)) then false
            else p.ExitCode = 0
        else
            false

    streamWriter.Flush()

    if gotIt then
        if Option.isSome scrapeOutput then
            stream.Position <- 0L
            use rdr = new StreamReader(stream)
            let result = scrapeOutput.Value rdr
            result
        else
            None
    else
        None

///
/// Scrape the baseBath from dotnet.exe --info
///
let argumentBasePath = "--info"
let basePath = "Base Path:"
let rec scrapeOutputForBasePath (rdr:StreamReader) =

    if rdr.EndOfStream then
        None
    else 
        let v = rdr.ReadLine()
        if String.IsNullOrEmpty (v) || not (v.Contains(basePath)) then
            scrapeOutputForBasePath rdr
        else
            let index = v.IndexOf(basePath)
            if index > 0 then Some (v.Substring(index + basePath.Length).Trim())
            else Some v
 