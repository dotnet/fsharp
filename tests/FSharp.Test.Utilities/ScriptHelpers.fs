// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.ScriptHelpers

open System
open System.IO
open System.Text
open System.Threading
open FSharp.Compiler
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Test

[<RequireQualifiedAccess>]
type LangVersion =
    | V47
    | V50
    | V60
    | V70
    | V80
    | V90
    | Preview
    | Latest
    | SupportsMl

type private EventedTextWriter() =
    inherit TextWriter()
    let sb = StringBuilder()
    let sw = new StringWriter()
    let lineWritten = Event<string>()
    member _.LineWritten = lineWritten.Publish
    override _.Encoding = Encoding.UTF8
    override _.Write(c: char) =
        if c = '\n' then
            let line =
                let v = sb.ToString()
                if v.EndsWith("\r") then v.Substring(0, v.Length - 1)
                else v
            sb.Clear() |> ignore
            sw.WriteLine line
            lineWritten.Trigger(line)
        else sb.Append(c) |> ignore
    override _.ToString() = sw.ToString()

type FSharpScript(?additionalArgs: string[], ?quiet: bool, ?langVersion: LangVersion, ?input: string) =

    let additionalArgs = defaultArg additionalArgs [||]
    let quiet = defaultArg quiet true
    let langVersion = defaultArg langVersion LangVersion.Preview
    let config = FsiEvaluationSession.GetDefaultConfiguration()

    let computedProfile =
        // If we are being executed on the desktop framework (we can tell because the assembly containing int is mscorlib) then profile must be mscorlib otherwise use netcore
        if typeof<int>.Assembly.GetName().Name = "mscorlib" then "mscorlib"
        else "netcore"

    let baseArgs = [|
        typeof<FSharpScript>.Assembly.Location;
        "--targetprofile:" + computedProfile
        if quiet then "--quiet"
        match langVersion with
        | LangVersion.V47 -> "--langversion:4.7"
        | LangVersion.V50 | LangVersion.SupportsMl -> "--langversion:5.0"
        | LangVersion.Preview -> "--langversion:preview"
        | LangVersion.Latest -> "--langversion:latest"
        | LangVersion.V60 -> "--langversion:6.0"
        | LangVersion.V70 -> "--langversion:7.0"
        | LangVersion.V80 -> "--langversion:8.0"
        | LangVersion.V90 -> "--langversion:9.0"
        |]

    let argv = Array.append baseArgs additionalArgs

    let inReader = new StringReader(defaultArg input "")
    let outWriter = new EventedTextWriter()
    let errorWriter = new EventedTextWriter()

    do
        ParallelConsole.localIn.Set inReader
        ParallelConsole.localOut.Set outWriter
        ParallelConsole.localError.Set errorWriter

    let fsi = FsiEvaluationSession.Create (config, argv, stdin, stdout, stderr)

    member _.ValueBound = fsi.ValueBound

    member _.Fsi = fsi

    member _.OutputProduced = outWriter.LineWritten

    member _.ErrorProduced = errorWriter.LineWritten

    member _.GetOutput() = ParallelConsole.OutText

    member _.GetErrorOutput() = ParallelConsole.ErrorText

    member this.Eval(code: string, ?cancellationToken: CancellationToken, ?desiredCulture: Globalization.CultureInfo) =
        let originalCulture = Thread.CurrentThread.CurrentCulture
        Thread.CurrentThread.CurrentCulture <- Option.defaultValue Globalization.CultureInfo.InvariantCulture desiredCulture

        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        let ch, errors = fsi.EvalInteractionNonThrowing(code, cancellationToken)

        Thread.CurrentThread.CurrentCulture <- originalCulture

        match ch with
        | Choice1Of2 v -> Ok(v), errors
        | Choice2Of2 ex -> Error(ex), errors

    /// Get the available completion items from the code at the specified location.
    ///
    /// <param name="text">The input text on which completions will be calculated</param>
    /// <param name="line">The 1-based line index</param>
    /// <param name="column">The 0-based column index</param>
    member _.GetCompletionItems(text: string, line: int, column: int) =
        async {
            let parseResults, checkResults, _projectResults = fsi.ParseAndCheckInteraction(text)
            let lineText = text.Split('\n').[line - 1]
            let partialName = QuickParse.GetPartialLongNameEx(lineText, column - 1)
            let declarationListInfos = checkResults.GetDeclarationListInfo(Some parseResults, line, lineText, partialName)
            return declarationListInfos.Items
        }

    interface IDisposable with
        member this.Dispose() =
            ((this.Fsi) :> IDisposable).Dispose()

[<AutoOpen>]
module TestHelpers =

    let getValue ((value: Result<FsiValue option, exn>), (errors: FSharpDiagnostic[])) =
        if errors.Length > 0 then
            failwith <| sprintf "Evaluation returned %d errors:\r\n\t%s" errors.Length (String.Join("\r\n\t", errors))
        match value with
        | Ok(value) -> value
        | Error ex -> raise ex

    let ignoreValue = getValue >> ignore
