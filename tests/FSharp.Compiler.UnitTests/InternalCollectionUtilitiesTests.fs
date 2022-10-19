module FSharp.Compiler.UnitTests.InternalCollectionUtilitiesTests

open System
open System.Text
open Xunit
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Compiler.Syntax
open Internal.Utilities.Library


let inputs = ["single"; "A_member1"; "A_member2"; "single2"; "single3"; "B_member1"; "B_member2"; "A_misplaced1"; "B_misplaced1"; "B_misplaced2"]
   

[<Fact>]
let `` Chunking with a None-returning selector creates one chunk per item `` () =
    inputs
    |> List.chunkConsequtiveElementsVia (fun x -> None)
    |> Assert.shouldBe [for i in inputs -> [i]]

[<Fact>]
let `` Chunking with a constant-returning selector creates one big chunk `` () =
    inputs
    |> List.chunkConsequtiveElementsVia (fun x -> Some 42)
    |> Assert.shouldBe [inputs]

[<Fact>]
let `` Chunking based on group name puts misplaced members into their own groups `` () =
    inputs
    |> List.chunkConsequtiveElementsVia (fun x -> Some x[0])
    |> Assert.shouldBe [["single"];["A_member1"; "A_member2"];["single2"; "single3"];["B_member1"; "B_member2"];["A_misplaced1"];["B_misplaced1"; "B_misplaced2"]]

[<Fact>]
let `` Chunking - if selector returns None, it always means its own chunk, even if there are 2 consequtive Nones `` () =
    inputs
    |> List.chunkConsequtiveElementsVia (fun x -> if x[0] = 's' then None else Some x[0])
    |> Assert.shouldBe [["single"];["A_member1"; "A_member2"];["single2"];["single3"];["B_member1"; "B_member2"];["A_misplaced1"];["B_misplaced1"; "B_misplaced2"]]