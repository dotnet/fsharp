// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.IO
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.SolutionCrawler

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

open Microsoft.VisualStudio.FSharp.LanguageService

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpProjectDiagnosticAnalyzer() =
    inherit ProjectDiagnosticAnalyzer()

    static member GetDiagnostics(options: FSharpProjectOptions) =
        let checkProjectResults = FSharpChecker.Instance.ParseAndCheckProject(options) |> Async.RunSynchronously
        (checkProjectResults.Errors |> Seq.choose(fun (error) ->
            if error.StartLineAlternate = 0 || error.EndLineAlternate = 0 then
                Some(CommonRoslynHelpers.ConvertError(error, Location.None))
            else
                // F# error line numbers are one-based. Errors that have a valid line number are reported by DocumentDiagnosticAnalyzer
                None
        )).ToImmutableArray()
        
    override this.SupportedDiagnostics with get() = CommonRoslynHelpers.SupportedDiagnostics()

    override this.AnalyzeProjectAsync(project: Project, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        let computation = async {
            match FSharpLanguageService.GetOptions(project.Id) with
            | Some(options) ->
                return FSharpProjectDiagnosticAnalyzer.GetDiagnostics(options)
            | None -> return ImmutableArray<Diagnostic>.Empty
        }

        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
             .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)
