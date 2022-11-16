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
let b = new MyType() // alternative syntax
a.DoNothing(b)
"""
let impFile = { sourceFile "First" [] with ExtraSource = reproSourceCode }
let project = SyntheticProject.Create(impFile)

[<Fact>]
let ``Finding usage of type via GetUsesOfSymbolInFile should also find it's constructors`` () =
    project.Workflow
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
    project.Workflow
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
                       //10,8,14 // "a" constructor This is the bug, A should be reported but it is not
                       11,12,18 // "b" constructor
                    |],ranges)  )    

        }

