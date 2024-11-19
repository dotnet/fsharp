// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceQuery

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpDiagnosticReport =

    internal new: diagnostics: FSharp.Compiler.Diagnostics.FSharpDiagnostic array *
                  resultId: int -> FSharpDiagnosticReport

    member
      Diagnostics: FSharp.Compiler.Diagnostics.FSharpDiagnostic array with get

    /// The result ID of the diagnostics. This needs to be unique for each version of the document in order to be able to clear old diagnostics.
    member ResultId: string with get

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceQuery =

    internal new: depGraph: Internal.Utilities.DependencyGraph.IThreadSafeDependencyGraph<FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceState.WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                          FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceState.WorkspaceGraphTypes.WorkspaceNodeValue> *
                  checker: FSharp.Compiler.CodeAnalysis.FSharpChecker ->
                    FSharpWorkspaceQuery

    member
      GetDiagnosticsForFile: file: System.Uri -> Async<FSharpDiagnosticReport>

    member
      GetProjectSnapshot: projectIdentifier: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier ->
                            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot option

    member
      GetProjectSnapshotForFile: file: System.Uri ->
                                   FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot option

    member
      GetSemanticClassification: file: System.Uri ->
                                   Async<FSharp.Compiler.EditorServices.SemanticClassificationView option>

    member
      GetSource: file: System.Uri ->
                   System.Threading.Tasks.Task<FSharp.Compiler.Text.ISourceTextNew option>

