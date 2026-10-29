// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.IO
open System.Runtime.InteropServices
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.Internal.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Threading

open CancellableTasks

/// Keeps the data of a solution in the folder Visual Studio keeps the solution's own state in — `.vs\<solution>` —
/// so it goes away with the solution and nothing of it is left under the user's profile. A solution Visual Studio
/// gives no such folder keeps nothing. One directory per format version, so a newer layout starts empty.
[<Export(typeof<IFSharpPersistentStorageConfiguration>); Export; Shared>]
type internal FSharpPersistentStorageConfiguration
    [<ImportingConstructor>]
    ([<Import(AllowDefault = true)>] serviceProvider: SVsServiceProvider) =

    static let invalidPathChars = set [ yield! Path.GetInvalidPathChars(); '/' ]

    static let safeName (fullPath: string) =
        let fileName = Path.GetFileName fullPath

        let prefix =
            if fileName.Length > 20 then
                fileName.Substring(0, 20)
            else
                fileName

        match
            $"{prefix}-{FSharpChecksum.Create fullPath}"
            |> String.filter (invalidPathChars.Contains >> not)
        with
        | name when String.IsNullOrWhiteSpace name -> "None"
        | name -> name

    /// The state folder of the solution open in Visual Studio, not specific to its version: the data has a format
    /// version of its own. A solution not yet saved has only a temporary one, which does not outlive the session.
    static let stateFolderOf (solution: obj) =
        match solution with
        | :? IVsSolutionWorkingFolders as folders ->
            try
                match
                    folders.GetFolder(
                        uint32 __SolutionWorkingFolder.SlnWF_StatePersistence,
                        Guid.Empty,
                        fVersionSpecific = false,
                        fEnsureCreated = true
                    )
                with
                | false, folder when not (String.IsNullOrEmpty folder) -> ValueSome folder
                | _ -> ValueNone
            with :? COMException ->
                ValueNone
        | _ -> ValueNone

    /// Where the data of every solution goes instead of its state folder. Set by tests, which run without Visual
    /// Studio.
    member val CacheDirectory: string voption = ValueNone with get, set

    interface IFSharpPersistentStorageConfiguration with
        member this.TryGetStorageLocationAsync(solution, cancellationToken) =
            cancellableTask {
                match solution.FilePath with
                | null -> return ValueNone
                | path when not (Path.IsPathRooted path) -> return ValueNone
                | path ->
                    match this.CacheDirectory, serviceProvider with
                    | ValueSome directory, _ -> return ValueSome(Path.Combine(directory, safeName path, "v1"))
                    | ValueNone, null -> return ValueNone
                    | ValueNone, serviceProvider ->
                        do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken)
                        let stateFolder = stateFolderOf (serviceProvider.GetService typeof<SVsSolution>)
                        do! TaskScheduler.Default.SwitchTo()

                        return
                            stateFolder
                            |> ValueOption.map (fun folder -> Path.Combine(folder, "FSharp", "v1"))
            }
            |> CancellableTask.start cancellationToken

/// One file per document and name, named by a hash of the project's and the document's paths and names — never by
/// the document's id, which lasts one session. The file starts with the checksum it was written with.
type private FilePersistentStorage(directory: string) =

    static let magic = "FSPS"B

    static let header (checksum: FSharpChecksum) = Array.append magic (checksum.ToBytes())

    let pathOf (document: Document) name =
        let project = document.Project

        let key =
            FSharpChecksum.Create [ project.FilePath; project.Name; document.FilePath; document.Name; name ]

        Path.Combine(directory, BitConverter.ToString(key.ToBytes()).Replace("-", ""))

    interface IFSharpChecksummedPersistentStorage with
        member _.ReadStreamAsync(document, name, checksum, cancellationToken) =
            cancellationToken.ThrowIfCancellationRequested()
            let path = pathOf document name
            let expected = header checksum

            match File.Exists path with
            | false -> Task.FromResult null
            | true ->
                match File.ReadAllBytes path with
                | bytes when bytes.Length >= expected.Length && Array.sub bytes 0 expected.Length = expected ->
                    Task.FromResult(new MemoryStream(bytes, expected.Length, bytes.Length - expected.Length, false) :> Stream)
                | _ -> Task.FromResult null

        member _.WriteStreamAsync(document, name, stream, checksum, cancellationToken) =
            cancellationToken.ThrowIfCancellationRequested()
            Directory.CreateDirectory directory |> ignore
            let path = pathOf document name
            let written = $"{path}.{Guid.NewGuid():N}.tmp"

            try
                do
                    use file =
                        new FileStream(written, FileMode.CreateNew, FileAccess.Write, FileShare.None)

                    let header = header checksum
                    file.Write(header, 0, header.Length)
                    stream.CopyTo file

                // Another writer of the same entry that gets there first makes this one fail, and its data stands.
                if File.Exists path then
                    File.Replace(written, path, null)
                else
                    File.Move(written, path)

                Task.FromResult true
            finally
                File.Delete written

/// The storage of the solution it was last asked for, as Roslyn keeps one database open.
[<Export(typeof<IFSharpChecksummedPersistentStorageService>); Shared>]
type internal FSharpFilePersistentStorageService [<ImportingConstructor>] (configuration: IFSharpPersistentStorageConfiguration) =

    static let noStorage =
        { new IFSharpChecksummedPersistentStorage with
            member _.ReadStreamAsync(_, _, _, _) = Task.FromResult null
            member _.WriteStreamAsync(_, _, _, _, _) = Task.FromResult false
        }

    let mutable current = (null: string), noStorage

    interface IFSharpChecksummedPersistentStorageService with
        member _.GetStorageAsync(solution, cancellationToken) =
            match current with
            | solutionPath, storage when String.Equals(solutionPath, solution.FilePath, StringComparison.Ordinal) -> Task.FromResult storage
            | _ ->
                task {
                    let! location = configuration.TryGetStorageLocationAsync(solution, cancellationToken)

                    let storage =
                        match location with
                        | ValueSome directory -> FilePersistentStorage directory :> IFSharpChecksummedPersistentStorage
                        | ValueNone -> noStorage

                    current <- solution.FilePath, storage
                    return storage
                }
