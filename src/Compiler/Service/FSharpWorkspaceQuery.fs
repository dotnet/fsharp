// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Code to handle quries to F# workspace
module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceQuery

open System
open System.Collections.Generic
open FSharp.Compiler.Diagnostics
open System.Threading

open FSharp.Compiler.CodeAnalysis

open Internal.Utilities.DependencyGraph
open Internal.Utilities.Library.Extras
open FSharpWorkspaceState
open Internal.Utilities.Library

#nowarn "57"

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpDiagnosticReport internal (diagnostics, resultId: int) =

    member _.Diagnostics = diagnostics

    /// The result ID of the diagnostics. This needs to be unique for each version of the document in order to be able to clear old diagnostics.
    member _.ResultId = resultId.ToString()

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceQuery internal (depGraph: IThreadSafeDependencyGraph<_, _>, checker: FSharpChecker) =

    let mutable resultIdCounter = 0

    // TODO: we might need something more sophisticated eventually
    // for now it's important that the result id is unique every time
    // in order to be able to clear previous diagnostics
    let getDiagnosticResultId () = Interlocked.Increment(&resultIdCounter)

    member internal _.Checker = checker

    member _.GetProjectSnapshot projectIdentifier =
        use _ =
            Activity.start "GetProjectSnapshot" [ Activity.Tags.project, projectIdentifier.ToString() |> (!!) ]

        try
            depGraph.GetProjectSnapshot projectIdentifier |> Some
        with :? KeyNotFoundException ->
            None

    /// Returns the project snapshot for a file. If `context` is provided, returns the snapshot whose
    /// identifier matches. Otherwise (or when no match is found among the projects containing the
    /// file) falls back to the first snapshot, matching the prior behavior. This fallback keeps
    /// behavior reasonable for clients that don't send a VS project context (e.g. non-VS clients
    /// or before the project-context dropdown selection arrives).
    member _.GetProjectSnapshotForFile(file: Uri, ?context: ProjectSnapshot.FSharpProjectIdentifier) =
        use _ =
            Activity.start "GetProjectSnapshotForFile" [ Activity.Tags.fileName, file.LocalPath ]

        let candidates = depGraph.GetProjectsContaining file.LocalPath

        match context with
        | Some ctx ->
            let matching = candidates |> Seq.tryFind (fun s -> s.Identifier = ctx)
            match matching with
            | Some _ -> matching
            | None -> candidates |> Seq.tryHead
        | None -> candidates |> Seq.tryHead

    member _.GetProjectSnapshotsForFile(file: Uri) =
        use _ =
            Activity.start "GetProjectSnapshotsForFile" [ Activity.Tags.fileName, file.LocalPath ]

        depGraph.GetProjectsContaining file.LocalPath
        |> Seq.toArray

    member this.GetParseAndCheckResultsForFile(file: Uri, ?context: ProjectSnapshot.FSharpProjectIdentifier) =
        async {

            use _ =
                Activity.start "GetParseAndCheckResultsForFile" [ Activity.Tags.fileName, file.LocalPath ]

            return!
                this.GetProjectSnapshotForFile(file, ?context = context)
                |> Option.map (fun snapshot ->
                    async {
                        let! parseResult, checkFileAnswer = checker.ParseAndCheckFileInProject(file.LocalPath, snapshot)

                        return
                            match checkFileAnswer with
                            | FSharpCheckFileAnswer.Succeeded result -> Some parseResult, Some result
                            | FSharpCheckFileAnswer.Aborted -> Some parseResult, None
                    })
                |> Option.defaultValue (async.Return(None, None))

        }

    member this.GetCheckResultsForFile(file, ?context: ProjectSnapshot.FSharpProjectIdentifier) =
        this.GetParseAndCheckResultsForFile(file, ?context = context) |> Async.map snd

    // TODO: split to parse and check diagnostics
    member this.GetDiagnosticsForFile(file: Uri, ?context: ProjectSnapshot.FSharpProjectIdentifier) =
        use _ =
            Activity.start "GetDiagnosticsForFile" [ Activity.Tags.fileName, file.LocalPath ]

        this.GetParseAndCheckResultsForFile(file, ?context = context)
        |> Async.map (fun results ->
            let diagnostics =
                match results with
                | _, Some checkResult -> checkResult.Diagnostics
                | Some parseResult, _ -> parseResult.Diagnostics
                | _ -> [||]

            FSharpDiagnosticReport(diagnostics, getDiagnosticResultId ()))

    member this.GetSemanticClassification(file: Uri, ?context: ProjectSnapshot.FSharpProjectIdentifier) =
        use _ =
            Activity.start "GetSemanticClassification" [ Activity.Tags.fileName, file.LocalPath ]

        this.GetProjectSnapshotForFile(file, ?context = context)
        |> Option.map (fun snapshot ->
            checker.GetBackgroundSemanticClassificationForFile(file.LocalPath, snapshot, "LSP Get semantic classification"))
        |> Option.defaultValue (async.Return None)

    member _.GetSource(file: Uri) =
        task {
            try
                let! source = depGraph.GetSourceFile(file.LocalPath).GetSource()
                return Some source
            with :? KeyNotFoundException ->
                return None
        }
