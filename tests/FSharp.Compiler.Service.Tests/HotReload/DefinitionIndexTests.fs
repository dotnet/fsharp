namespace FSharp.Compiler.Service.Tests.HotReload

open System
open Xunit

module DefinitionIndexTests =

    let private tryGetExisting (baseline: (string * int) list) =
        let lookup = dict baseline
        fun item ->
            match lookup.TryGetValue item with
            | true, rowId -> Some rowId
            | _ -> None

    [<Fact>]
    let ``Add and add-existing track rows and additions`` () =
        let index =
            FSharp.Compiler.CodeGen.FSharpDefinitionIndex.DefinitionIndex(tryGetExisting [ "existing", 3 ], 4)

        let newRowId = index.Add("new")
        Assert.Equal(5, newRowId)

        index.AddExisting("existing")

        let rows = index.Rows
        Assert.Collection(
            rows,
            (fun struct (rowId, item, isAdded) ->
                Assert.Equal(3, rowId)
                Assert.Equal("existing", item)
                Assert.False(isAdded)),
            (fun struct (rowId, item, isAdded) ->
                Assert.Equal(5, rowId)
                Assert.Equal("new", item)
                Assert.True(isAdded)))

        let added = index.Added
        Assert.Collection(
            added,
            (fun struct (rowId, item) ->
                Assert.Equal(5, rowId)
                Assert.Equal("new", item)))

    [<Fact>]
    let ``Rows freeze the index and prevent further edits`` () =
        let index =
            FSharp.Compiler.CodeGen.FSharpDefinitionIndex.DefinitionIndex(tryGetExisting [], 0)

        index.Add("first") |> ignore

        index.Rows |> ignore

        Assert.Throws<InvalidOperationException>(fun () -> index.Add("second") |> ignore :> obj)
        |> ignore

    [<Fact>]
    let ``Contains and TryGetDefinition account for baseline entries`` () =
        let index =
            FSharp.Compiler.CodeGen.FSharpDefinitionIndex.DefinitionIndex(tryGetExisting [ "baseline", 2 ], 3)

        Assert.True(index.Contains("baseline"))
        Assert.False(index.IsAdded("baseline"))

        let newRow = index.Add("new")
        Assert.True(index.Contains("new"))
        Assert.True(index.IsAdded("new"))

        let baselineLookup =
            match index.TryGetDefinition 2 with
            | Some item -> item
            | None -> failwith "Expected baseline definition."

        let addedLookup =
            match index.TryGetDefinition newRow with
            | Some item -> item
            | None -> failwith "Expected added definition."

        Assert.Equal("baseline", baselineLookup)
        Assert.Equal("new", addedLookup)

    [<Fact>]
    let ``Missing definition raises`` () =
        let index =
            FSharp.Compiler.CodeGen.FSharpDefinitionIndex.DefinitionIndex(tryGetExisting [], 0)

        let ex =
            Assert.Throws<InvalidOperationException>(fun () -> index.GetRowId("missing") |> ignore :> obj)

        Assert.Contains("Row id not found", ex.Message)
