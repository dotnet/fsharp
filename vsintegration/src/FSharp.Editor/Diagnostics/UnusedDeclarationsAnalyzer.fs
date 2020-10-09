// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Diagnostics
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Host.Mef
open FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

[<Export(typeof<IFSharpUnusedDeclarationsDiagnosticAnalyzer>)>]
type internal UnusedDeclarationsAnalyzer [<ImportingConstructor>] () =
    
    static let userOpName = "UnusedDeclarationsAnalyzer"
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().FSharpProjectOptionsManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker
    

    interface IFSharpUnusedDeclarationsDiagnosticAnalyzer with

        member __.AnalyzeSemanticsAsync(descriptor, document, cancellationToken) =
            asyncMaybe {
                do! Option.guard document.FSharpOptions.CodeFixes.UnusedDeclarations

                do Trace.TraceInformation("{0:n3} (start) UnusedDeclarationsAnalyzer", DateTime.Now.TimeOfDay.TotalSeconds)
                do! Async.Sleep DefaultTuning.UnusedDeclarationsAnalyzerInitialDelay |> liftAsync // be less intrusive, give other work priority most of the time
                match! getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName) with
                | (_parsingOptions, projectOptions) ->
                    let! sourceText = document.GetTextAsync()
                    let checker = getChecker document
                    let! _, _, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName = userOpName)
                    let! unusedRanges = UnusedDeclarations.getUnusedDeclarations( checkResults, (isScriptFile document.FilePath)) |> liftAsync
                    return
                        unusedRanges
                        |> Seq.map (fun m -> Diagnostic.Create(descriptor, RoslynHelpers.RangeToLocation(m, sourceText, document.FilePath)))
                        |> Seq.toImmutableArray
            }
            |> Async.map (Option.defaultValue ImmutableArray.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
