// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler.SourceCodeServices

type private LineHash = int

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal RemoveQualificationDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker

    static let Descriptor = 
        DiagnosticDescriptor(IDEDiagnosticIds.RemoveQualificationDiagnosticId, "Simplify name", "", "", DiagnosticSeverity.Hidden, true, "", "", DiagnosticCustomTags.Unnecessary)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor

    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            match getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some options ->
                let! sourceText = document.GetTextAsync()
                let checker = getChecker document
                let! _, checkResults = checker.ParseAndCheckDocument(document, options, sourceText)
                let! symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                return 
                    [ for symbolUse in symbolUses do
                        yield Diagnostic.Create(Descriptor, CommonRoslynHelpers.RangeToLocation(symbolUse.RangeAlternate, sourceText, document.FilePath))
                    ]
            | None -> return []
        } 
        |> Async.map (fun xs -> (xs |> Option.defaultValue []).ToImmutableArray())
        |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    override this.AnalyzeSemanticsAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis