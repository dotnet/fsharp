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

let private fileSystemAnswering (stamp: DateTime) =
    { new DefaultFileSystem() with
        override _.GetLastWriteTimeShim _ = stamp
    }

let private fileSystemRejectingStats =
    { new DefaultFileSystem() with
        override _.GetLastWriteTimeShim _ = failwith "unexpected reference stat"
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

let private assertReferencesOnDisk (snapshot: FSharpProjectSnapshot) mainStamp libStamp =
    let libSnapshot =
        match snapshot.ReferencedProjects with
        | [ FSharpReferencedProjectSnapshot.FSharpReference(_, lib) ] -> lib
        | other -> failwith $"Expected one referenced project, got %A{other}"

    Assert.Equal<ReferenceOnDisk list>([ { Path = "MainRef.dll"; LastModified = mainStamp } ], snapshot.ReferencesOnDisk)
    Assert.Equal<ReferenceOnDisk list>([ { Path = "LibRef.dll"; LastModified = libStamp } ], libSnapshot.ReferencesOnDisk)

[<Fact>]
let ``Reference stamps come from the host when it gives them and from the file system current at build time when not`` () : Task =
    withFileSystem fileSystemRejectingStats (fun () ->
        task {
            let stamps = dict [ "MainRef.dll", january; "LibRef.dll", february ]

            let! hosted =
                FSharpProjectSnapshot.FromOptionsWithReferenceStamps(mainWithLib (), emptySource, (fun path -> stamps[path]))

            assertReferencesOnDisk hosted january february

            FileSystem <- fileSystemAnswering january
            let work = FSharpProjectSnapshot.FromOptions(mainWithLib (), emptySource)
            FileSystem <- fileSystemAnswering february

            let! built = work

            assertReferencesOnDisk built february february
        })
