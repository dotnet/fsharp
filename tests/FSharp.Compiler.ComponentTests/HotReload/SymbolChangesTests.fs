namespace FSharp.Compiler.ComponentTests.HotReload

open Xunit
open FSharp.Compiler.TypedTreeDiff
open FSharp.Compiler.HotReload.DefinitionMap
open FSharp.Compiler.HotReload.SymbolChanges

module SymbolChangesTests =

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

    let private diff edits rude =
        { TypedTreeDiffResult.SemanticEdits = edits
          RudeEdits = rude }

    [<Fact>]
    let ``synthesized updates are partitioned separately`` () =
        let synthesizedEdit : SemanticEdit =
            { Symbol = symbol [ "Module" ] "closure@4" 7L SymbolKind.Value true
              Kind = SemanticEditKind.MethodBody
              BaselineHash = Some 10
              UpdatedHash = Some 20
              IsSynthesized = true
              ContainingEntity = None }

        let regularEdit : SemanticEdit =
            { Symbol = symbol [ "Module" ] "Value" 8L SymbolKind.Value false
              Kind = SemanticEditKind.MethodBody
              BaselineHash = Some 3
              UpdatedHash = Some 4
              IsSynthesized = false
              ContainingEntity = None }

        let definitionMap = diff [ synthesizedEdit; regularEdit ] [] |> FSharpDefinitionMap.ofTypedTreeDiff
        let symbolChanges = FSharpSymbolChanges.ofDefinitionMap definitionMap

        let synthesizedDefinitionUpdates = FSharpDefinitionMap.synthesizedUpdated definitionMap
        Assert.Single synthesizedDefinitionUpdates |> ignore
        let (synthChange, synthKind) = List.head synthesizedDefinitionUpdates
        Assert.Equal(synthesizedEdit.Symbol.QualifiedName, synthChange.Symbol.QualifiedName)
        Assert.Equal(SemanticEditKind.MethodBody, synthKind)

        let synthesizedUpdated = FSharpSymbolChanges.synthesizedUpdated symbolChanges
        Assert.Single synthesizedUpdated |> ignore
        let (symbol, editKind) = List.head synthesizedUpdated
        Assert.True(symbol.IsSynthesized)
        Assert.Equal(SemanticEditKind.MethodBody, editKind)

        // Regular edits should still appear in the aggregated updated list.
        Assert.Contains(symbolChanges.Updated, fun change -> change.Symbol.QualifiedName = "Module.Value")
