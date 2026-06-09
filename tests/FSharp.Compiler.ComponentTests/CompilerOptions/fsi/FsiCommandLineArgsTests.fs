// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsi

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Regression tests for https://github.com/dotnet/fsharp/issues/10819
/// fsi.CommandLineArgs must preserve every token after `--` byte-for-byte;
/// in particular abbreviated flags like -d / -r / -I that follow `--` must
/// NOT be colon-joined with their next argument.
module FsiCommandLineArgsTests =

    let private writeProbeScript () : string =
        // Prints one ARG=<value> line per element of fsi.CommandLineArgs so the
        // test can parse exact contents from stdout.
        let body = """
for a in fsi.CommandLineArgs do
    printfn "ARG=%s" a
"""
        let path =
            Path.Combine(
                Path.GetTempPath(),
                sprintf "fsi_cmdline_%s.fsx" (Guid.NewGuid().ToString("N")))
        File.WriteAllText(path, body)
        path

    let private parseArgsFromStdOut (stdout: string) : string list =
        stdout.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.choose (fun line ->
            if line.StartsWith("ARG=") then Some (line.Substring(4)) else None)
        |> Array.toList

    /// Run fsi <script> <extra args> and return the parsed fsi.CommandLineArgs
    /// (without index 0, which is the absolute script path and varies per run).
    let private runAndGetTail (extraArgs: string list) : string list =
        let scriptPath = writeProbeScript ()
        try
            let result = runFsiProcess (scriptPath :: extraArgs)
            Assert.True(
                result.ExitCode = 0,
                sprintf "fsi exited %d. stdout=%s stderr=%s"
                    result.ExitCode result.StdOut result.StdErr)
            match parseArgsFromStdOut result.StdOut with
            | [] ->
                failwithf "No ARG= lines in stdout. stdout=%s stderr=%s"
                    result.StdOut result.StdErr
            | _ :: tail -> tail
        finally
            try File.Delete(scriptPath) with _ -> ()

    // Primary regression theory for #10819: with `--`, abbreviated flags and
    // arbitrary user tokens must reach the script verbatim.
    [<Theory>]
    [<InlineData("-d,5",            "-d,5")>]
    [<InlineData("-r,5",            "-r,5")>]
    [<InlineData("-I,5",            "-I,5")>]
    [<InlineData("--foo,--bar=baz", "--foo,--bar=baz")>]
    [<InlineData("a:b,c:d",         "a:b,c:d")>]
    let ``fsi.CommandLineArgs preserves args after -- verbatim``
        (userArgsCsv: string) (expectedCsv: string) =
        let userArgs = userArgsCsv.Split(',') |> Array.toList
        let expected = expectedCsv.Split(',') |> Array.toList
        // Script appears before `--`; the script-arg tail follows `--`.
        let tail = runAndGetTail ("--" :: userArgs)
        Assert.Equal<string list>(expected, tail)

    // Baseline: -b is not an abbreviated flag, so it already round-trips through
    // `--` today without colon-joining. Locks in the only piece of current
    // behaviour that is correct, so the GREEN fix can't accidentally regress
    // unrelated tokens. NOTE: the `--` itself is still present in
    // fsi.CommandLineArgs on main; the GREEN sprint may also strip it, in
    // which case this baseline will need a matching update at that point.
    [<Fact>]
    let ``fsi.CommandLineArgs preserves non-abbreviated args after -- (baseline)`` () =
        let tail = runAndGetTail ["--"; "-b"; "5"]
        Assert.Equal<string list>(["--"; "-b"; "5"], tail)
