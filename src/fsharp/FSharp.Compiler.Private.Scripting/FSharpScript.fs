// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting

open System
open System.Threading
open FSharp.Compiler
open FSharp.Compiler.Interactive.Shell

[<RequireQualifiedAccess>]
type LangVersion =
    | V47
    | V50
    | Preview

type FSharpScript(?additionalArgs: string[], ?quiet: bool, ?langVersion: LangVersion) =

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
        "--noninteractive";
        "--targetprofile:" + computedProfile
        if quiet then "--quiet"
        match langVersion with
        | LangVersion.V47 -> "--langversion:4.7"
        | LangVersion.V50 -> "--langversion:5.0"
        | LangVersion.Preview -> "--langversion:preview"
        |]

    let argv = Array.append baseArgs additionalArgs

    let fsi = FsiEvaluationSession.Create (config, argv, stdin, stdout, stderr)

    member __.ValueBound = fsi.ValueBound

    member __.Fsi = fsi

    member __.Eval(code: string, ?cancellationToken: CancellationToken) =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        let ch, errors = fsi.EvalInteractionNonThrowing(code, cancellationToken)
        match ch with
        | Choice1Of2 v -> Ok(v), errors
        | Choice2Of2 ex -> Error(ex), errors

    /// Get the available completion items from the code at the specified location.
    ///
    /// <param name="text">The input text on which completions will be calculated</param>
    /// <param name="line">The 1-based line index</param>
    /// <param name="column">The 0-based column index</param>
    member __.GetCompletionItems(text: string, line: int, column: int) =
        async {
            let! parseResults, checkResults, _projectResults = fsi.ParseAndCheckInteraction(text)
            let lineText = text.Split('\n').[line - 1]
            let partialName = QuickParse.GetPartialLongNameEx(lineText, column - 1)
            let! declarationListInfos = checkResults.GetDeclarationListInfo(Some parseResults, line, lineText, partialName)
            return declarationListInfos.Items
        }

    interface IDisposable with
        member __.Dispose() =
            (fsi :> IDisposable).Dispose()
