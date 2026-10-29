module TypeChecks.TypeRelations

open Xunit
open FSharp.Compiler.Symbols
open FSharp.Test.Compiler
open FSharp.Test
open FSharp.Test.ScriptHelpers

[<Theory; FileInlineData("CrgpLibrary.fs")>]
let ``Unsolved type variables are not cached`` compilation =
    compilation
    |> getCompilation
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Struct tuple with more than seven elements implements the interfaces of its own type`` () =
    FSharp """
module M

open System

type T8 = (struct (int * int * int * int * int * int * int * int))
type T10 = (struct (int * int * int * int * int * int * int * int * int * int))

let eq8 (x: T8) = x :> IEquatable<T8>
let cmp8 (x: T8) = x :> IComparable<T8>
let eq10 (x: T10) = x :> IEquatable<T10>
let cmp10 (x: T10) = x :> IComparable<T10>
"""
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Interfaces from an assembly resolved by a later #r are reported on an entity obtained before it`` () =
    let missing =
        CSharp """namespace Missing { public interface IMarker {} }"""
        |> withName "Missing"

    let missingPath = (missing |> compile |> shouldSucceed).OutputPath.Value

    let libraryPath =
        CSharp """namespace Library { public class C : Missing.IMarker {} }"""
        |> withName "Library"
        |> withReferences [ missing ]
        |> compile
        |> shouldSucceed
        |> _.OutputPath.Value

    let declaredInterfaces (entity: FSharpEntity) =
        entity.DeclaredInterfaces |> Seq.map _.TypeDefinition.FullName |> List.ofSeq

    use script = new FSharpScript()
    script.Eval $"#r @\"{libraryPath}\"" |> ignoreValue
    let _, checkResults, _ = script.Fsi.ParseAndCheckInteraction "typeof<Library.C>"

    let entity =
        checkResults.GetAllUsesOfAllSymbolsInFile()
        |> Seq.pick (fun symbolUse ->
            match symbolUse.Symbol with
            | :? FSharpEntity as entity when entity.DisplayName = "C" -> Some entity
            | _ -> None)

    Assert.Empty(declaredInterfaces entity)

    script.Eval $"#r @\"{missingPath}\"" |> ignoreValue

    Assert.Equal<string list>([ "Missing.IMarker" ], declaredInterfaces entity)
