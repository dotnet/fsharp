// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceQuery

open System
open System.Threading.Tasks

open Internal.Utilities.DependencyGraph

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceState.WorkspaceGraphTypes
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpDiagnosticReport =

    internal new: diagnostics: FSharpDiagnostic array * resultId: int -> FSharpDiagnosticReport

    member Diagnostics: FSharpDiagnostic array with get

    /// The result ID of the diagnostics. This needs to be unique for each version of the document in order to be able to clear old diagnostics.
    member ResultId: string with get

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceQuery =

    internal new:
        depGraph: IThreadSafeDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * checker: FSharpChecker ->
            FSharpWorkspaceQuery

    member GetDiagnosticsForFile: file: Uri -> Async<FSharpDiagnosticReport>

    member GetProjectSnapshot: projectIdentifier: FSharpProjectIdentifier -> FSharpProjectSnapshot option

    member GetProjectSnapshotForFile: file: Uri -> FSharpProjectSnapshot option

    member GetSemanticClassification: file: Uri -> Async<SemanticClassificationView option>

    member GetSource: file: Uri -> Task<ISourceTextNew option>
