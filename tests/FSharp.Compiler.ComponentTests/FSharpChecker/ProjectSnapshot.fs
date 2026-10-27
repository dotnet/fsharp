module FSharpChecker.ProjectSnapshot

open Xunit
open System
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

#nowarn 57

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``ConditionalDefines matches FSharpParsingOptions`` (isInteractive: bool) =
    let otherOptions =
        [ "--define:A"; "-d:B"; "/define:C"; "/d:D"; "--debug+"; "-o:out.dll"; "--define:E:F"; "--define:"; "-define:G"; "--d:H"; "define:I" ]

    let projectConfig =
        ProjectConfig("Dummy.fsproj", None, [], otherOptions, useScriptResolutionRules = isInteractive)

    let parsingOptions, _ =
        FSharpChecker.Create().GetParsingOptionsFromCommandLineArgs(otherOptions, isInteractive = isInteractive)

    Assert.Equal<string list>(parsingOptions.ConditionalDefines, projectConfig.ConditionalDefines)

[<Theory>]
[<InlineData("")>]
[<InlineData("--langversion:8.0")>]
[<InlineData("-d:X --langversion:preview")>]
[<InlineData("--langversion:9.0 /langversion:LATEST")>]
[<InlineData("--langversion:4.7")>]
[<InlineData("--langversion:foo")>]
[<InlineData("--langversion:")>]
[<InlineData("-langversion:9.0")>]
let ``LangVersionText matches FSharpParsingOptions`` (options: string) =
    let otherOptions = options.Split(' ', StringSplitOptions.RemoveEmptyEntries) |> Array.toList

    let projectConfig = ProjectConfig("Dummy.fsproj", None, [], otherOptions)

    let parsingOptions, _ =
        FSharpChecker.Create().GetParsingOptionsFromCommandLineArgs(otherOptions)

    Assert.Equal(parsingOptions.LangVersionText, projectConfig.LangVersionText)

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

