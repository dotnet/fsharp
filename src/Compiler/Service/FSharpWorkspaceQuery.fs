// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Code to handle quries to F# workspace
module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceQuery

open System
open System.Collections.Generic
open FSharp.Compiler.Diagnostics

open FSharp.Compiler.CodeAnalysis

open Internal.Utilities.DependencyGraph
open Internal.Utilities.Hashing
open Internal.Utilities.Library.Extras
open FSharpWorkspaceState
open Internal.Utilities.Library

#nowarn "57"

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpDiagnosticReport internal (diagnostics, resultId: string) =

    member _.Diagnostics = diagnostics

    /// The result ID of the diagnostics. This needs to be unique for each version of the document in order to be able to clear old diagnostics.
    member _.ResultId = resultId

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceQuery internal (depGraph: IThreadSafeDependencyGraph<_, _>, checker: FSharpChecker) =

    member internal _.Checker = checker

    member _.GetProjectSnapshot projectIdentifier =
        use _ =
            Activity.start "GetProjectSnapshot" [ Activity.Tags.project, projectIdentifier.ToString() |> (!!) ]

        try
            depGraph.GetProjectSnapshot projectIdentifier |> Some
        with :? KeyNotFoundException ->
            None

    member _.GetProjectSnapshotForFile(file: Uri) =
        use _ =
            Activity.start "GetProjectSnapshotForFile" [ Activity.Tags.fileName, file.LocalPath ]

        depGraph.GetProjectsContaining file.LocalPath

        // TODO: eventually we need to deal with choosing the appropriate project here
        // Hopefully we will be able to do it through receiving project context from LSP
        // Otherwise we have to keep track of which project/configuration is active
        |> Seq.tryHead // For now just get the first one

    member this.GetParseAndCheckResultsForFile(file: Uri) =
        async {

            use _ =
                Activity.start "GetParseAndCheckResultsForFile" [ Activity.Tags.fileName, file.LocalPath ]

            return!
                this.GetProjectSnapshotForFile file
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

    member this.GetCheckResultsForFile(file) =
        this.GetParseAndCheckResultsForFile file |> Async.map snd

    // TODO: split to parse and check diagnostics
    member this.GetDiagnosticsForFile(file: Uri) =
        use _ =
            Activity.start "GetDiagnosticsForFile" [ Activity.Tags.fileName, file.LocalPath ]

        let projectSnapshot = this.GetProjectSnapshotForFile file

        this.GetParseAndCheckResultsForFile file
        |> Async.map (fun results ->
            let diagnostics =
                match results with
                | _, Some checkResult -> checkResult.Diagnostics
                | Some parseResult, _ -> parseResult.Diagnostics
                | _ -> [||]

            let resultId =
                projectSnapshot
                |> Option.map (fun snapshot -> snapshot.ProjectSnapshot.FullVersion |> Md5Hasher.toString)
                |> Option.defaultValue "empty"

            FSharpDiagnosticReport(diagnostics, resultId))

    member this.GetSemanticClassification(file: Uri) =
        use _ =
            Activity.start "GetSemanticClassification" [ Activity.Tags.fileName, file.LocalPath ]

        this.GetProjectSnapshotForFile file
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
