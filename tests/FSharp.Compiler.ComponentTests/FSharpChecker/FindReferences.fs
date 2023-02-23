module FSharp.Compiler.ComponentTests.FSharpChecker.FindReferences

open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration

type Occurence = Definition | InType | Use

let deriveOccurence (su:FSharpSymbolUse) =
    if su.IsFromDefinition 
    then Definition
    elif su.IsFromType
    then InType
    elif su.IsFromUse
    then Use
    else failwith $"Unexpected type of occurence (for this test), symbolUse = {su}" 

/// https://github.com/dotnet/fsharp/issues/13199
let reproSourceCode = """
type MyType() = 
    member x.DoNothing(d:MyType) = ()

let a = MyType()
let b = new MyType()
a.DoNothing(b)
"""
let impFile() = { sourceFile "First" [] with ExtraSource = reproSourceCode }
let createProject() = SyntheticProject.Create(impFile())

[<Fact>]
let ``Finding usage of type via GetUsesOfSymbolInFile should also find it's constructors`` () =
    createProject().Workflow
        {        
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
             
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(7, 11, "type MyType() =", ["MyType"]).Value
                let references = 
                    typeCheckResult.GetUsesOfSymbolInFile(symbolUse.Symbol) 
                    |> Array.sortBy (fun su -> su.Range.StartLine)
                    |> Array.map (fun su -> su.Range.StartLine, su.Range.StartColumn, su.Range.EndColumn, deriveOccurence su)

                Assert.Equal<(int*int*int*Occurence)>(
                    [| 7,5,11,Definition
                       8,25,31,InType
                       10,8,14,Use
                       11,12,18,Use
                    |],references)  )           
        }


[<Fact>]
let ``Finding usage of type via FindReference should also find it's constructors`` () =
    createProject().Workflow
        {        
            placeCursor "First" 7 11 "type MyType() =" ["MyType"]     
            findAllReferencesInFile "First" (fun (ranges:list<FSharp.Compiler.Text.range>) ->
                let ranges = 
                    ranges 
                    |> List.sortBy (fun r -> r.StartLine)
                    |> List.map (fun r -> r.StartLine, r.StartColumn, r.EndColumn)
                    |> Array.ofSeq

                Assert.Equal<(int*int*int)>(
                    [| 7,5,11 // Typedef itself
                       8,25,31 // Usage within type
                       10,8,14 // "a= ..." constructor 
                       11,12,18 // "b= ..." constructor
                    |],ranges)  )    

        }

[<Fact>]
let ``Finding usage of type via FindReference works across files`` () =
    let secondFile = { sourceFile "Second" ["First"] with ExtraSource = """
open ModuleFirst
let secondA = MyType()
let secondB = new MyType()
secondA.DoNothing(secondB)
 """}
    let original = createProject()
    let project = {original with SourceFiles = original.SourceFiles @ [secondFile]}
    project.Workflow
        {        
            placeCursor "First" 7 11 "type MyType() =" ["MyType"]     
            findAllReferencesInFile "Second" (fun (ranges:list<FSharp.Compiler.Text.range>) ->
                let ranges = 
                    ranges 
                    |> List.sortBy (fun r -> r.StartLine)
                    |> List.map (fun r -> r.StartLine, r.StartColumn, r.EndColumn)
                    |> Array.ofSeq

                Assert.Equal<(int*int*int)>(
                    [| 9,14,20 // "secondA = ..." constructor 
                       10,18,24 // "secondB = ..." constructor
                    |],ranges)  )    

        }

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(true, false)>]
[<InlineData(false, true)>]
[<InlineData(false, false)>]
let ``Finding references in project`` (fastCheck, captureIdentifiersWhenParsing) =
    let size = 20

    let project =
        { SyntheticProject.Create() with
            SourceFiles = [
                sourceFile $"File%03d{0}" [] |> addSignatureFile
                for i in 1..size do
                    sourceFile $"File%03d{i}" [$"File%03d{i-1}"]
            ]
        }
        |> updateFile "File005" (addDependency "File000")
        |> updateFile "File010" (addDependency "File000")

    let checker = FSharpChecker.Create(
        enableBackgroundItemKeyStoreAndSemanticClassification = true,
        captureIdentifiersWhenParsing = captureIdentifiersWhenParsing)

    project.WorkflowWith checker {
        findAllReferencesToModuleFromFile "File000" fastCheck (expectNumberOfResults 5)
    }

[<Fact>]
let ``We find back-ticked identifiers`` () =
    SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "let ``foo bar`` x = x + 5" },
        { sourceFile "Second" [] with ExtraSource = "let foo x = ModuleFirst.``foo bar`` x" })
        .Workflow {
            placeCursor "Second" 6 35 "let foo x = ModuleFirst.``foo bar`` x" ["``foo bar``"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 4, 15
                "FileSecond.fs", 6, 12, 35
            ])
        }

[<Fact>]
let ``We find operators`` () =
    SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "let (++) x y = x - y" },
        { sourceFile "Second" [] with ExtraSource = """
open ModuleFirst
let foo x = x ++ 4""" })
        .Workflow {
            placeCursor "Second" 8 16 "let foo x = x ++ 4" ["++"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 7
                "FileSecond.fs", 8, 14, 16
            ])
        }

module Attributes =

    let project() = SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "type MyAttribute() = inherit System.Attribute()" },
        { sourceFile "Second" [] with ExtraSource = """
open ModuleFirst
[<My>]
let foo x = 4""" },
        { sourceFile "Third" [] with ExtraSource = """
open ModuleFirst
[<MyAttribute>]
let foo x = 5""" })

    [<Fact>]
    let ``We find attributes from definition`` () =
        project().Workflow {
            placeCursor "First" 6 16 "type MyAttribute() = inherit System.Attribute()" ["MyAttribute"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 16
                "FileSecond.fs", 8, 2, 4
                "FileThird.fs", 8, 2, 13
            ])
        }

    [<Fact>]
    let ``We find attributes from usage`` () =
        project().Workflow {
            placeCursor "Second" 8 4 "[<My>]" ["My"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 16
                "FileSecond.fs", 8, 2, 4
                "FileThird.fs", 8, 2, 13
            ])
        }

    [<Fact>]
    let ``We find attributes from usage with Attribute suffix`` () =
        project().Workflow {
            placeCursor "Third" 8 13 "[<MyAttribute>]" ["MyAttribute"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 16
                "FileSecond.fs", 8, 2, 4
                "FileThird.fs", 8, 2, 13
            ])
        }

[<Fact>]
let ``We find a type that has been aliased`` () =

    let project = SyntheticProject.Create("TypeAliasTest",
        { sourceFile "First" [] with
            ExtraSource = "type MyInt = int32\n" +
                          "let myNum = 7"
            SignatureFile = Custom ("module TypeAliasTest.ModuleFirst\n" +
                                    "type MyInt = int32\n" +
                                    "val myNum: MyInt") },
        { sourceFile "Second" [] with
            ExtraSource = "let goo x = ModuleFirst.myNum + x"})

    project.Workflow {
        placeCursor "First" "myNum"
        findAllReferences (expectToFind [
            "FileFirst.fs", 7, 4, 9
            "FileFirst.fsi", 3, 4, 9
            "FileSecond.fs", 6, 12, 29
        ])
    }
