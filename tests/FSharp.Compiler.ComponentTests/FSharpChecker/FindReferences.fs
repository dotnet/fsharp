module FSharp.Compiler.ComponentTests.FSharpChecker.FindReferences

open FSharp.Compiler.CodeAnalysis
open Xunit
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
let impFile = { sourceFile "First" [] with ExtraSource = reproSourceCode }
let createProject() = SyntheticProject.Create(impFile)

let plainCreation = sourceFile "First" []
let creationWithWith = { sourceFile "First" [] with ExtraSource = reproSourceCode }
let plainCreationChangedLater = { plainCreation with ExtraSource = reproSourceCode }
let creationviaFunc() = { sourceFile "First" [] with ExtraSource = reproSourceCode }

let sourceFileLocal   =
    { Id = "xx"
      PublicVersion = 1
      InternalVersion = 1
      DependsOn = []
      FunctionName = "f"
      SignatureFile = No
      HasErrors = false
      ExtraSource = ""
      EntryPoint = false }

let plainLocalCreation = sourceFileLocal  
let localCreationWithWith = { sourceFileLocal   with ExtraSource = reproSourceCode }
let localCreationChangedLater = { plainCreation with ExtraSource = reproSourceCode }


[<Fact>]
let ``What is happening with static init - plainCreation`` () =  
    Assert.NotNull(plainCreation)

[<Fact>]
let ``What is happening with static init - creationWithWith`` () =  
    Assert.NotNull(creationWithWith)

[<Fact>]
let ``What is happening with static init - plainCreationChangedLater`` () =  
    Assert.NotNull(plainCreationChangedLater)

[<Fact>]
let ``What is happening with static init - impFile`` () = 
    Assert.NotNull(impFile)

[<Fact>]
let ``What is happening with static init - creationviaFunc`` () = 
    Assert.NotNull(creationviaFunc())

[<Fact>]
let ``What is happening with static init - plainLocalCreation`` () =  
    Assert.NotNull(plainLocalCreation)

[<Fact>]
let ``What is happening with static init - localCreationWithWith`` () =  
    Assert.NotNull(localCreationWithWith)

[<Fact>]
let ``What is happening with static init - localCreationChangedLater`` () =  
    Assert.NotNull(localCreationChangedLater)

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
            findAllReferences "First" (fun (ranges:list<FSharp.Compiler.Text.range>) ->
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
            findAllReferences "Second" (fun (ranges:list<FSharp.Compiler.Text.range>) ->
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

