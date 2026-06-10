namespace FSharp.Compiler.ComponentTests.HotReload

open Xunit
open FSharp.Compiler.TypedTreeDiff
open FSharp.Compiler.HotReload.DefinitionMap

module DefinitionMapTests =

    let private symbol path name stamp kind isSynthesized : SymbolId =
        { Path = path
          LogicalName = name
          Stamp = stamp
          Kind = kind
          MemberKind = None
          IsSynthesized = isSynthesized
          CompiledName = None
          TotalArgCount = None
          GenericArity = None
          ParameterTypeIdentities = None
          ReturnTypeIdentity = None }

    let private diffResult edits rude =
        { TypedTreeDiffResult.SemanticEdits = edits
          RudeEdits = rude }

    [<Fact>]
    let ``added edit surfaces in definition map`` () =
        let edit =
            { Symbol = symbol [ "Module" ] "AddedValue" 1L SymbolKind.Value false
              Kind = SemanticEditKind.Insert
              BaselineHash = None
              UpdatedHash = Some 42
              IsSynthesized = false
              ContainingEntity = None }

        let result = diffResult [ edit ] [] |> FSharpDefinitionMap.ofTypedTreeDiff

        let added = FSharpDefinitionMap.added result
        Assert.Single added |> ignore
        Assert.Equal("Module.AddedValue", (List.head added).QualifiedName)

    [<Fact>]
    let ``method body edit classified as update`` () =
        let edit =
            { Symbol = symbol [ "Module" ] "Method" 2L SymbolKind.Value false
              Kind = SemanticEditKind.MethodBody
              BaselineHash = Some 11
              UpdatedHash = Some 12
              IsSynthesized = false
              ContainingEntity = None }

        let result = diffResult [ edit ] [] |> FSharpDefinitionMap.ofTypedTreeDiff

        let updated = FSharpDefinitionMap.updated result
        Assert.Single updated |> ignore
        let (updateChange, kind) = List.head updated
        Assert.Equal("Module.Method", updateChange.Symbol.QualifiedName)
        Assert.Equal(SemanticEditKind.MethodBody, kind)
        let change =
            result.Changes
            |> List.find (fun change -> change.Symbol.LogicalName = "Method")
        Assert.Equal<int option>(Some 11, change.BaselineHash)
        Assert.Equal<int option>(Some 12, change.UpdatedHash)
        Assert.False(change.IsSynthesized)

    [<Fact>]
    let ``type definition edit captured as update`` () =
        let edit =
            { Symbol = symbol [ "Namespace" ] "Entity" 4L SymbolKind.Entity false
              Kind = SemanticEditKind.TypeDefinition
              BaselineHash = Some 5
              UpdatedHash = Some 6
              IsSynthesized = false
              ContainingEntity = None }

        let result = diffResult [ edit ] [] |> FSharpDefinitionMap.ofTypedTreeDiff

        let updated = FSharpDefinitionMap.updated result
        Assert.Single updated |> ignore
        let (updateChange, kind) = List.head updated
        Assert.Equal(SymbolKind.Entity, updateChange.Symbol.Kind)
        Assert.Equal(SemanticEditKind.TypeDefinition, kind)

    [<Fact>]
    let ``delete edit captured`` () =
        let edit =
            { Symbol = symbol [ "Module" ] "OldValue" 3L SymbolKind.Value false
              Kind = SemanticEditKind.Delete
              BaselineHash = Some 1
              UpdatedHash = None
              IsSynthesized = false
              ContainingEntity = None }

        let result = diffResult [ edit ] [] |> FSharpDefinitionMap.ofTypedTreeDiff
        let deleted = FSharpDefinitionMap.deleted result
        Assert.Single deleted |> ignore
        Assert.Equal("Module.OldValue", (List.head deleted).QualifiedName)

    [<Fact>]
    let ``rude edits are preserved`` () =
        let rude =
            { Symbol = Some(symbol [] "Type" 4L SymbolKind.Entity false)
              Kind = RudeEditKind.SignatureChange
              Message = "Signature changed" }

        let result = diffResult [] [ rude ] |> FSharpDefinitionMap.ofTypedTreeDiff
        Assert.Single result.RudeEdits |> ignore
        Assert.Equal("Signature changed", (List.head result.RudeEdits).Message)

    [<Fact>]
    let ``synthesized edits are surfaced`` () =
        let synthesizedEdit =
            { Symbol = symbol [ "Module" ] "closure@4" 5L SymbolKind.Value true
              Kind = SemanticEditKind.MethodBody
              BaselineHash = Some 1
              UpdatedHash = Some 2
              IsSynthesized = true
              ContainingEntity = None }

        let result = diffResult [ synthesizedEdit ] [] |> FSharpDefinitionMap.ofTypedTreeDiff

        let synthesized = FSharpDefinitionMap.synthesized result
        Assert.Single synthesized |> ignore
        Assert.True((List.head synthesized).IsSynthesized)

        Assert.Single(FSharpDefinitionMap.synthesizedUpdated result) |> ignore
