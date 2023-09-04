module FSharpChecker.ProjectSnapshot

open Xunit

open FSharp.Compiler.CodeAnalysis


[<Fact>]
let WithoutImplFilesThatHaveSignatures () =

    let snapshot: FSharpProjectSnapshot = {
        ProjectFileName = Unchecked.defaultof<_>
        ProjectId = Unchecked.defaultof<_>
        SourceFiles = [
            { FileName = "A.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "A.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "B.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "C.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "C.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
        ]
        ReferencesOnDisk = Unchecked.defaultof<_>
        OtherOptions = Unchecked.defaultof<_>
        ReferencedProjects = Unchecked.defaultof<_>
        IsIncompleteTypeCheckEnvironment = Unchecked.defaultof<_>
        UseScriptResolutionRules = Unchecked.defaultof<_>
        LoadTime = Unchecked.defaultof<_>
        UnresolvedReferences = Unchecked.defaultof<_>
        OriginalLoadReferences = Unchecked.defaultof<_>
        Stamp = Unchecked.defaultof<_>
    }

    let result = snapshot.WithoutImplFilesThatHaveSignatures

    let expected = [| "A.fsi"; "B.fs"; "C.fsi" |]

    Assert.Equal<string array>(expected, result.SourceFileNames |> List.toArray)

[<Fact>]
let WithoutImplFilesThatHaveSignaturesExceptLastOne () =

    let snapshot: FSharpProjectSnapshot = {
        ProjectFileName = Unchecked.defaultof<_>
        ProjectId = Unchecked.defaultof<_>
        SourceFiles = [
            { FileName = "A.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "A.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "B.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "C.fsi"; Version = "1"; GetSource = Unchecked.defaultof<_> }
            { FileName = "C.fs"; Version = "1"; GetSource = Unchecked.defaultof<_> }
        ]
        ReferencesOnDisk = Unchecked.defaultof<_>
        OtherOptions = Unchecked.defaultof<_>
        ReferencedProjects = Unchecked.defaultof<_>
        IsIncompleteTypeCheckEnvironment = Unchecked.defaultof<_>
        UseScriptResolutionRules = Unchecked.defaultof<_>
        LoadTime = Unchecked.defaultof<_>
        UnresolvedReferences = Unchecked.defaultof<_>
        OriginalLoadReferences = Unchecked.defaultof<_>
        Stamp = Unchecked.defaultof<_>
    }

    let result = snapshot.WithoutImplFilesThatHaveSignaturesExceptLastOne

    let expected = [| "A.fsi"; "B.fs"; "C.fsi"; "C.fs" |]

    Assert.Equal<string array>(expected, result.SourceFileNames |> List.toArray)
