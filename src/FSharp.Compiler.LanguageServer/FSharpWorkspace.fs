module FSharp.Compiler.LanguageServer.Workspace

open FSharp.Compiler.Text

#nowarn "57"

open System
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

/// Holds a project snapshot and a queue of changes that will be applied to it when it's requested
///
/// The assumption is that this is faster than actually applying the changes to the snapshot immediately and that
/// we will be doing this on potentially every keystroke. But this should probably be measured at some point.
type SnapshotHolder(snapshot: FSharpProjectSnapshot, changedFiles: Set<string>, openFiles: Map<string, string>) =

    let applyFileChangesToSnapshot () =
        let files =
            changedFiles
            |> Seq.map (fun filePath ->
                match openFiles.TryFind filePath with
                | Some content ->
                    FSharpFileSnapshot.Create(
                        filePath,
                        DateTime.Now.Ticks.ToString(),
                        fun () -> content |> SourceTextNew.ofString |> Task.FromResult
                    )
                | None -> FSharpFileSnapshot.CreateFromFileSystem(filePath))
            |> Seq.toList

        snapshot.Replace files

    // We don't want to mutate the workspace by applying the changes when snapshot is requested because that would force the language
    // requests to be processed sequentially. So instead we keep the change application under lazy so it's still only computed if needed
    // and only once and workspace doesn't change.
    let appliedChanges =
        lazy SnapshotHolder(applyFileChangesToSnapshot (), Set.empty, openFiles)

    member private _.snapshot = snapshot
    member private _.changedFiles = changedFiles

    member private this.GetMostUpToDateInstance() =
        if appliedChanges.IsValueCreated then
            appliedChanges.Value
        else
            this

    member this.WithFileChanged(file, openFiles) =
        let previous = this.GetMostUpToDateInstance()
        SnapshotHolder(previous.snapshot, previous.changedFiles.Add file, openFiles)

    member this.WithoutFileChanged(file, openFiles) =
        let previous = this.GetMostUpToDateInstance()
        SnapshotHolder(previous.snapshot, previous.changedFiles.Remove file, openFiles)

    member _.GetSnapshot() = appliedChanges.Value.snapshot

    static member Of(snapshot: FSharpProjectSnapshot) =
        SnapshotHolder(snapshot, Set.empty, Map.empty)

type FSharpWorkspace
    private
    (
        projects: Map<FSharpProjectIdentifier, SnapshotHolder>,
        openFiles: Map<string, string>,
        fileMap: Map<string, Set<FSharpProjectIdentifier>>
    ) =

    let updateProjectsWithFile (file: Uri) f (projects: Map<FSharpProjectIdentifier, SnapshotHolder>) =
        fileMap
        |> Map.tryFind file.LocalPath
        |> Option.map (fun identifier ->
            (projects, identifier)
            ||> Seq.fold (fun projects identifier ->
                let snapshotHolder = projects[identifier]
                projects.Add(identifier, f snapshotHolder)))
        |> Option.defaultValue projects

    member _.Projects = projects
    member _.OpenFiles = openFiles
    member _.FileMap = fileMap

    member this.OpenFile(file: Uri, content: string) = this.ChangeFile(file, content)

    member _.CloseFile(file: Uri) =
        let openFiles = openFiles.Remove(file.LocalPath)

        FSharpWorkspace(
            projects =
                (projects
                 |> updateProjectsWithFile file _.WithoutFileChanged(file.LocalPath, openFiles)),
            openFiles = openFiles,
            fileMap = fileMap
        )

    member _.ChangeFile(file: Uri, content: string) =

        // TODO: should we assert that the file is open?

        let openFiles = openFiles.Add(file.LocalPath, content)

        FSharpWorkspace(
            projects =
                (projects
                 |> updateProjectsWithFile file _.WithFileChanged(file.LocalPath, openFiles)),
            openFiles = openFiles,
            fileMap = fileMap
        )

    member _.GetSnapshotForFile(file: Uri) =
        fileMap
        |> Map.tryFind file.LocalPath

        // TODO: eventually we need to deal with choosing the appropriate project here
        // Hopefully we will be able to do it through receiving project context from LSP
        // Otherwise we have to keep track of which project/configuration is active
        |> Option.bind Seq.tryHead

        |> Option.bind projects.TryFind
        |> Option.map _.GetSnapshot()

    static member Create(projects: FSharpProjectSnapshot seq) =
        FSharpWorkspace(
            projects = Map.ofSeq (projects |> Seq.map (fun p -> p.Identifier, SnapshotHolder.Of p)),
            openFiles = Map.empty,
            fileMap =
                (projects
                 |> Seq.collect (fun p ->
                     p.ProjectSnapshot.SourceFileNames
                     |> Seq.map (fun f -> Uri(f).LocalPath, p.Identifier))
                 |> Seq.groupBy fst
                 |> Seq.map (fun (f, ps) -> f, Set.ofSeq (ps |> Seq.map snd))
                 |> Map.ofSeq)
        )
