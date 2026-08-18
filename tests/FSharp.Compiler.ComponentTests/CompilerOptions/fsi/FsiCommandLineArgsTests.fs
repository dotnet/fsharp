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
        // test can parse exact contents from stdout. A leading empty `printfn`
        // ensures the first ARG= line is never prefixed by an FSI `> ` prompt
        // (which only matters when the probe runs interactively via --use,
        // not when it is invoked as the script-file argument directly).
        let body = """
printfn ""
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
    // arbitrary user tokens must reach the script verbatim. The `--` separator
    // itself is included in fsi.CommandLineArgs (matching pre-fix behaviour);
    // the GREEN fix only stops PostProcessCompilerArgs from colon-joining the
    // abbreviated flags that follow it.
    [<Theory>]
    [<InlineData("-d,5",            "--,-d,5")>]
    [<InlineData("-r,5",            "--,-r,5")>]
    [<InlineData("-I,5",            "--,-I,5")>]
    [<InlineData("--foo,--bar=baz", "--,--foo,--bar=baz")>]
    [<InlineData("a:b,c:d",         "--,a:b,c:d")>]
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
    // unrelated tokens.
    [<Fact>]
    let ``fsi.CommandLineArgs preserves non-abbreviated args after -- (baseline)`` () =
        let tail = runAndGetTail ["--"; "-b"; "5"]
        Assert.Equal<string list>(["--"; "-b"; "5"], tail)

    // No-script regression for #10819: when `--` appears without a preceding
    // script-file argument, ParseCompilerOptions must see `--` so its
    // OptionRest recordExplicitArg handler fires. If the GREEN fix were to
    // strip `--` from the suffix, `-d` and `5` would instead be parsed as
    // compiler options (and `-d` would fail to bind without its joined
    // value), so this row locks the OptionRest path.
    [<Fact>]
    let ``fsi.CommandLineArgs preserves args after -- with no script (OptionRest path)`` () =
        let scriptPath = writeProbeScript ()
        try
            // --use:<script> + --exec loads the probe script then exits without
            // making the script path the first non-option arg, so the IsScript
            // OptionGeneral handler does NOT fire and the `--` token reaches
            // ParseCompilerOptions' OptionRest handler instead. --nologo
            // suppresses the banner so the FSI prompt prefix does not bleed
            // into the parsed ARG= lines.
            let result =
                runFsiProcess
                    [ "--nologo"
                      "--use:" + scriptPath
                      "--exec"
                      "--"
                      "-d"
                      "5" ]
            Assert.True(
                result.ExitCode = 0,
                sprintf "fsi exited %d. stdout=%s stderr=%s"
                    result.ExitCode result.StdOut result.StdErr)
            let args = parseArgsFromStdOut result.StdOut
            // With no script before `--`, OptionRest captured only the tokens
            // AFTER `--`; the `--` itself is consumed by ParseCompilerOptions.
            // args[0] is the fsi binary path; the tail must be exactly `-d 5`.
            match args with
            | [] ->
                failwithf "No ARG= lines in stdout. stdout=%s stderr=%s"
                    result.StdOut result.StdErr
            | _ :: tail ->
                Assert.Equal<string list>([ "-d"; "5" ], tail)
        finally
            try File.Delete(scriptPath) with _ -> ()
