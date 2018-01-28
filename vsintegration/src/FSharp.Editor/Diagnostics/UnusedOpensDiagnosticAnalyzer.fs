// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.Editor.Symbols

[<DiagnosticAnalyzer(FSharpConstants.FSharpLanguageName)>]
type internal UnusedOpensDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().FSharpProjectOptionsManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker

    static let userOpName = "UnusedOpensAnalyzer"
    static let Descriptor = 
        DiagnosticDescriptor(
            id = IDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId, 
            title = SR.RemoveUnusedOpens(),
            messageFormat = SR.UnusedOpens(),
            category = DiagnosticCategory.Style, 
            defaultSeverity = DiagnosticSeverity.Hidden, 
            isEnabledByDefault = true, 
            customTags = DiagnosticCustomTags.Unnecessary)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    override this.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    static member GetUnusedOpenRanges(document: Document, options, checker: FSharpChecker) : Async<Option<range list>> =
        asyncMaybe {
            do! Option.guard Settings.CodeFixes.UnusedOpens
            let! sourceText = document.GetTextAsync()
            let! _, _, checkResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true, userOpName = userOpName)
#if DEBUG
            let sw = Stopwatch.StartNew()
#endif
            let! unusedOpens = UnusedOpens.getUnusedOpens(checkResults, fun lineNumber -> sourceText.Lines.[Line.toZ lineNumber].ToString()) |> liftAsync
#if DEBUG
            Logging.Logging.logInfof "*** Got %d unused opens in %O" unusedOpens.Length sw.Elapsed
#endif
            return unusedOpens
        } 

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            do Trace.TraceInformation("{0:n3} (start) UnusedOpensAnalyzer", DateTime.Now.TimeOfDay.TotalSeconds)
            do! Async.Sleep DefaultTuning.UnusedOpensAnalyzerInitialDelay |> liftAsync // be less intrusive, give other work priority most of the time
            let! _parsingOptions, projectOptions = getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync()
            let checker = getChecker document
            let! unusedOpens = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges(document, projectOptions, checker)
            
            return 
                unusedOpens
                |> List.map (fun range ->
                      Diagnostic.Create(
                         Descriptor,
                         RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath)))
                |> Seq.toImmutableArray
        } 
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis