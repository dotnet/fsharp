// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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

open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Range

#if PROJECT_ANALYSIS
// Project-wide error analysis.  We don't enable this because ParseAndCheckProject checks projects against the versions of the files
// saves to the file system. This is different to the versions of the files active in the editor.  This results in out-of-sync error
// messages while files are being edited

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpProjectDiagnosticAnalyzer() =
    inherit ProjectDiagnosticAnalyzer()

    static member GetDiagnostics(options: FSharpProjectOptions) = async {
        let! checkProjectResults = FSharpLanguageService.Checker.ParseAndCheckProject(options) 
        let results = 
          checkProjectResults.Errors 
          |> Seq.choose(fun (error) ->
            if error.StartLineAlternate = 0 || error.EndLineAlternate = 0 then
                Some(CommonRoslynHelpers.ConvertError(error, Location.None))
            else
                // F# error line numbers are one-based. Errors that have a valid line number are reported by DocumentDiagnosticAnalyzer
                None
             )
          |> Seq.toImmutableArray
        return results
      }
        
    override this.SupportedDiagnostics = CommonRoslynHelpers.SupportedDiagnostics()

    override this.AnalyzeProjectAsync(project: Project, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        async {
            match FSharpLanguageService.GetOptionsForProject(project.Id) with
            | Some options -> return! FSharpProjectDiagnosticAnalyzer.GetDiagnostics(options)
            | None -> return ImmutableArray<Diagnostic>.Empty
        } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken
#endif
