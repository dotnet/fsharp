module FSharpChecker.ProjectSnapshot

open Xunit
open System
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot


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

