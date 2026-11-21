#nowarn FS0057

module FSharpChecker.ProjectSnapshot

open Xunit
open System
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

let private createSnapshot projectFileName referencedProjects =
    FSharpProjectSnapshot.Create(
        projectFileName = projectFileName,
        outputFileName = None,
        projectId = None,
        sourceFiles = [| FSharpFileSnapshot.CreateFromString("A.fs", "module A") |],
        referencesOnDisk = [],
        otherOptions = [],
        referencedProjects = referencedProjects,
        isIncompleteTypeCheckEnvironment = false,
        useScriptResolutionRules = false,
        loadTime = DateTime(1234, 5, 6),
        unresolvedReferences = None,
        originalLoadReferences = [],
        stamp = None
    )

let private ilModuleReference outputFile =
    FSharpReferencedProjectSnapshot.ILModuleReference(outputFile, (fun () -> DateTime(1234, 5, 6)), (fun () -> failwith "Not read"))

let private libA = createSnapshot "LibA.fsproj" [||]
let private libB = createSnapshot "LibB.fsproj" [||]

let private referencingProject () =
    createSnapshot
        "App.fsproj"
        [|
            FSharpReferencedProjectSnapshot.FSharpReference("LibA.dll", libA)
            FSharpReferencedProjectSnapshot.FSharpReference("LibB.dll", libB)
            ilModuleReference "LibC.dll"
        |]

[<Fact>]
let ``Replace referenced projects replaces references with the same output file`` () =
    let snapshot = referencingProject ()
    let newLibA = createSnapshot "LibA.fsproj" [||]
    let newLibC = ilModuleReference "LibC.dll"

    let result =
        snapshot.Replace([| FSharpReferencedProjectSnapshot.FSharpReference("LibA.dll", newLibA); newLibC |])

    match result.ReferencedProjects with
    | [| FSharpReferencedProjectSnapshot.FSharpReference("LibA.dll", a)
         FSharpReferencedProjectSnapshot.FSharpReference("LibB.dll", b)
         c |] ->
        Assert.Same(newLibA, a)
        Assert.Same(libB, b)
        Assert.Same(newLibC, c)
    | references -> failwith $"Unexpected references: %A{references}"

    Assert.Same(snapshot.ProjectConfig, result.ProjectConfig)
    Assert.Same(snapshot.SourceFiles, result.SourceFiles)

[<Fact>]
let ``Replace referenced projects can change the kind of a reference`` () =
    let snapshot = referencingProject ()
    let newLibB = ilModuleReference "LibB.dll"

    let result = snapshot.Replace([| newLibB |])

    Assert.Same(snapshot.ReferencedProjects[0], result.ReferencedProjects[0])
    Assert.Same(newLibB, result.ReferencedProjects[1])
    Assert.Same(snapshot.ReferencedProjects[2], result.ReferencedProjects[2])

[<Fact>]
let ``Replace referenced projects ignores references with other output files`` () =
    let snapshot = referencingProject ()

    let result =
        snapshot.Replace([| FSharpReferencedProjectSnapshot.FSharpReference("Other.dll", libA) |])

    Assert.Same(snapshot.ReferencedProjects, result.ReferencedProjects)

// TODO: restore tests

//[<Fact>]
//let WithoutImplFilesThatHaveSignatures () =

//    let snapshot = FSharpProjectSnapshot.Create(
//        projectFileName = "Dummy.fsproj",
//        projectId = None,
//        sourceFiles = [
//            { FileName = "A.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "A.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "B.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "C.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "C.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//        ],
//        referencesOnDisk = [],
//        otherOptions = [],
//        referencedProjects = [],
//        isIncompleteTypeCheckEnvironment = true,
//        useScriptResolutionRules = false,
//        loadTime = DateTime(1234, 5, 6),
//        unresolvedReferences = None,
//        originalLoadReferences = [],
//        stamp = None
//    )

//    let result = snapshot.WithoutImplFilesThatHaveSignatures

//    let expected = [| "A.fsi"; "B.fs"; "C.fsi" |]

//    Assert.Equal<string array>(expected, result.SourceFileNames |> List.toArray)

//    Assert.Equal<byte array>(result.FullVersion, snapshot.SignatureVersion)

//[<Fact>]
//let WithoutImplFilesThatHaveSignaturesExceptLastOne () =

//    let snapshot = FSharpProjectSnapshot.Create(
//        projectFileName = "Dummy.fsproj",
//        projectId = None,
//        sourceFiles = [
//            { FileName = "A.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "A.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "B.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "C.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "C.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//        ],
//        referencesOnDisk = [],
//        otherOptions = [],
//        referencedProjects = [],
//        isIncompleteTypeCheckEnvironment = true,
//        useScriptResolutionRules = false,
//        loadTime = DateTime(1234, 5, 6),
//        unresolvedReferences = None,
//        originalLoadReferences = [],
//        stamp = None
//    )

//    let result = snapshot.WithoutImplFilesThatHaveSignaturesExceptLastOne

//    let expected = [| "A.fsi"; "B.fs"; "C.fsi"; "C.fs" |]

//    Assert.Equal<string array>(expected, result.SourceFileNames |> List.toArray)

//    Assert.Equal<byte array>(result.FullVersion, snapshot.LastFileVersion)


//[<Fact>]
//let WithoutImplFilesThatHaveSignaturesExceptLastOne_2 () =

//    let snapshot = FSharpProjectSnapshot.Create(
//        projectFileName = "Dummy.fsproj",
//        projectId = None,
//        sourceFiles = [
//            { FileName = "A.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "B.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//            { FileName = "C.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
//        ],
//        referencesOnDisk = [],
//        otherOptions = [],
//        referencedProjects = [],
//        isIncompleteTypeCheckEnvironment = true,
//        useScriptResolutionRules = false,
//        loadTime = DateTime(1234, 5, 6),
//        unresolvedReferences = None,
//        originalLoadReferences = [],
//        stamp = None
//    )

//    let result = snapshot.WithoutImplFilesThatHaveSignaturesExceptLastOne

//    let expected = [| "A.fs"; "B.fs"; "C.fs" |]

//    Assert.Equal<string array>(expected, result.SourceFileNames |> List.toArray)

//    Assert.Equal<byte array>(result.FullVersion, snapshot.LastFileVersion)

