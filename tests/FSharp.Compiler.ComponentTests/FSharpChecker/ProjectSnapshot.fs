[<Xunit.Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module FSharpChecker.ProjectSnapshot

open System
open System.IO
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.IO
open Xunit

#nowarn "57"

let private projectOptions projectFileName references referencedProjects =
    {
        ProjectFileName = projectFileName
        ProjectId = None
        SourceFiles = [| Path.ChangeExtension(projectFileName, ".fs") |]
        OtherOptions = [| for path in references -> $"-r:{path}" |]
        ReferencedProjects = referencedProjects
        IsIncompleteTypeCheckEnvironment = false
        UseScriptResolutionRules = false
        LoadTime = DateTime.UtcNow
        UnresolvedReferences = None
        OriginalLoadReferences = []
        Stamp = None
    }

let private emptySource _ path =
    async { return FSharpFileSnapshot.CreateFromString(path, "") }

let private january = DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
let private february = DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc)

/// A file system whose every reference stat answers `stamp`, or fails the test when `stamp` is `ValueNone`.
let private fileSystemStamping (stamp: DateTime voption) =
    { new DefaultFileSystem() with
        override _.GetLastWriteTimeShim _ =
            match stamp with
            | ValueSome stamp -> stamp
            | ValueNone -> failwith "unexpected reference stat"
    }

let private withFileSystem (fileSystem: IFileSystem) (test: unit -> Task) : Task =
    task {
        let current = FileSystem

        try
            FileSystem <- fileSystem
            do! test ()
        finally
            FileSystem <- current
    }

let private mainWithLib () =
    let lib = projectOptions "Lib.fsproj" [ "LibRef.dll" ] [||]
    projectOptions "Main.fsproj" [ "MainRef.dll" ] [| FSharpReferencedProject.FSharpReference("Lib.dll", lib) |]

let private referencesOnDisk (snapshot: FSharpProjectSnapshot) =
    let libSnapshot =
        match snapshot.ReferencedProjects with
        | [ FSharpReferencedProjectSnapshot.FSharpReference(_, lib) ] -> lib
        | other -> failwith $"Expected one referenced project, got %A{other}"

    snapshot.ReferencesOnDisk, libSnapshot.ReferencesOnDisk

[<Fact>]
let ``FromOptionsWithReferenceStamps takes reference stamps from the host, including referenced projects`` () : Task =
    withFileSystem (fileSystemStamping ValueNone) (fun () ->
        task {
            let stamps = dict [ "MainRef.dll", january; "LibRef.dll", february ]

            let! snapshot =
                FSharpProjectSnapshot.FromOptionsWithReferenceStamps(mainWithLib (), emptySource, (fun path -> stamps[path]))

            let mainReferences, libReferences = referencesOnDisk snapshot

            Assert.Equal<ReferenceOnDisk list>([ { Path = "MainRef.dll"; LastModified = january } ], mainReferences)
            Assert.Equal<ReferenceOnDisk list>([ { Path = "LibRef.dll"; LastModified = february } ], libReferences)
        })

[<Fact>]
let ``FromOptions reads reference stamps from the file system current when the snapshot is built`` () : Task =
    withFileSystem (fileSystemStamping (ValueSome january)) (fun () ->
        task {
            let work = FSharpProjectSnapshot.FromOptions(mainWithLib (), emptySource)
            FileSystem <- fileSystemStamping (ValueSome february)

            let! snapshot = work
            let mainReferences, libReferences = referencesOnDisk snapshot

            Assert.Equal<ReferenceOnDisk list>([ { Path = "MainRef.dll"; LastModified = february } ], mainReferences)
            Assert.Equal<ReferenceOnDisk list>([ { Path = "LibRef.dll"; LastModified = february } ], libReferences)
        })
