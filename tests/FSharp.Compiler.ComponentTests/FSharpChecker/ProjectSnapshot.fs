module FSharpChecker.ProjectSnapshot

open System
open System.IO
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
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

[<Fact>]
let ``FromOptions takes reference stamps from the host, including referenced projects`` () : Task =
    task {
        let stamps =
            dict
                [
                    "MainRef.dll", DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    "LibRef.dll", DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc)
                ]

        let lib = projectOptions "Lib.fsproj" [ "LibRef.dll" ] [||]

        let main =
            projectOptions "Main.fsproj" [ "MainRef.dll" ] [| FSharpReferencedProject.FSharpReference("Lib.dll", lib) |]

        let! snapshot = FSharpProjectSnapshot.FromOptions(main, emptySource, getReferenceStamp = (fun path -> stamps[path]))

        let libSnapshot =
            match snapshot.ReferencedProjects with
            | [ FSharpReferencedProjectSnapshot.FSharpReference(_, lib) ] -> lib
            | other -> failwith $"Expected one referenced project, got %A{other}"

        Assert.Equal<ReferenceOnDisk list>(
            [ { Path = "MainRef.dll"; LastModified = stamps["MainRef.dll"] } ],
            snapshot.ReferencesOnDisk
        )

        Assert.Equal<ReferenceOnDisk list>(
            [ { Path = "LibRef.dll"; LastModified = stamps["LibRef.dll"] } ],
            libSnapshot.ReferencesOnDisk
        )
    }
