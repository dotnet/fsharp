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
    
    let getProjectInfoManager(document: Document) =
        document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager

    static let Descriptor = 
        DiagnosticDescriptor(IDEDiagnosticIds.RemoveQualificationDiagnosticId, "Simplify name", "", "", DiagnosticSeverity.Hidden, true, "", "", DiagnosticCustomTags.Unnecessary)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor

    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
        async {
            match getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some _options ->
                let! sourceText = document.GetTextAsync() |> Async.AwaitTask
                //let lines = sourceText.Lines
                //let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, Seq.toList options.OtherOptions)

                return 
                    [ 
                      let linePositionSpan = LinePositionSpan(LinePosition(0, 0), LinePosition(0, 10))
                      let textSpan = sourceText.Lines.GetTextSpan linePositionSpan
                      let location = Location.Create(document.FilePath, textSpan, linePositionSpan)
                      yield Diagnostic.Create(Descriptor, location) 
                    ]
                    .ToImmutableArray()

            | None -> return ImmutableArray.Empty
        } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    override this.AnalyzeSemanticsAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis