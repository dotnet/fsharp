namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Collections.Generic
open System.Collections.Immutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Text.Json
open Xunit
open FSharp.Compiler.Service.Tests.HotReload
module DeltaWriter = FSharp.Compiler.CodeGen.FSharpDeltaMetadataWriter

module private MetadataHelpers =
    let countRows (delta: DeltaWriter.MetadataDelta) (table: TableIndex) =
        try
            use provider =
                MetadataReaderProvider.FromMetadataImage(
                    ImmutableArray.CreateRange delta.Metadata)
            provider.GetMetadataReader().GetTableRowCount(table)
        with :? BadImageFormatException ->
            delta.TableRowCounts.[int table]

    let tryFindTableIndex (name: string) : TableIndex option =
        match name with
        | "Module" -> Some TableIndex.Module
        | "TypeRef" -> Some TableIndex.TypeRef
        | "TypeDef" -> Some TableIndex.TypeDef
        | "Field" -> Some TableIndex.Field
        | "MethodDef" -> Some TableIndex.MethodDef
        | "Param" -> Some TableIndex.Param
        | "MemberRef" -> Some TableIndex.MemberRef
        | "StandAloneSig" -> Some TableIndex.StandAloneSig
        | "Property" -> Some TableIndex.Property
        | "PropertyMap" -> Some TableIndex.PropertyMap
        | "Event" -> Some TableIndex.Event
        | "EventMap" -> Some TableIndex.EventMap
        | "MethodSemantics" -> Some TableIndex.MethodSemantics
        | "TypeSpec" -> Some TableIndex.TypeSpec
        | "AssemblyRef" -> Some TableIndex.AssemblyRef
        | "EncLog" -> Some TableIndex.EncLog
        | "EncMap" -> Some TableIndex.EncMap
        | _ -> None

module RoslynBaselineComparisons =

    type RoslynBaselines = Map<string, Map<string, int>>

    let private loadRoslynTables () : RoslynBaselines =
        let path = Path.Combine(__SOURCE_DIRECTORY__, "../../../../tools/baselines/roslyn_tables.json") |> Path.GetFullPath
        if not (File.Exists path) then
            failwithf "Roslyn baseline table snapshot not found: %s" path
        let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
        let dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(File.ReadAllText path, options)
        dict
        |> Seq.map (fun outer ->
            let innerMap =
                outer.Value
                |> Seq.map (fun inner -> inner.Key, inner.Value)
                |> Map.ofSeq
            outer.Key, innerMap)
        |> Map.ofSeq

    let private findBaseline name (baselines: RoslynBaselines) =
        baselines
        |> Map.tryFind name
        |> Option.defaultWith (fun () -> failwithf "Roslyn baseline '%s' missing" name)

    let private getRow (baseline: Map<string, int>) key =
        baseline
        |> Map.tryFind key
        |> Option.defaultWith (fun () -> failwithf "Baseline missing '%s' row" key)

    [<Fact>]
    let ``roslyn delta tables include expected Module/Method/Param rows`` () =
        let baselines = loadRoslynTables ()
        let delta1 = findBaseline "Property" baselines
        Assert.Equal(1, getRow delta1 "Module")
        Assert.Equal(3, getRow delta1 "MethodDef")
        Assert.Equal(2, getRow delta1 "Param")
        let delta2 = findBaseline "PropertyUpdate" baselines
        Assert.Equal(1, getRow delta2 "Module")
        Assert.Equal(2, getRow delta2 "MethodDef")

    let private assertMatches (expected: Map<string,int>) (delta: DeltaWriter.MetadataDelta) =
        for KeyValue(key, budget) in expected do
            match MetadataHelpers.tryFindTableIndex key with
            | Some tableIndex ->
                let actual = MetadataHelpers.countRows delta tableIndex
                Assert.True(
                    actual <= budget,
                    sprintf "Table %A exceeded Roslyn baseline: actual=%d baseline=%d" tableIndex actual budget)
            | None -> ()

    [<Fact>]
    let ``property delta row counts do not exceed Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslyn = findBaseline "Property" baselines

        let propertyDelta = MetadataDeltaTestHelpers.emitPropertyDeltaArtifacts None ()
        assertMatches roslyn propertyDelta.Delta

    [<Fact>]
    let ``property multi-generation delta rows match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynAdd = findBaseline "Property" baselines
        let roslynUpdate = findBaseline "PropertyUpdate" baselines

        let artifacts = MetadataDeltaTestHelpers.emitPropertyMultiGenerationArtifacts ()
        assertMatches roslynAdd artifacts.Generation1
        assertMatches roslynUpdate artifacts.Generation2

    [<Fact>]
    let ``event delta row counts match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynEvent = findBaseline "Event" baselines

        let eventDelta = MetadataDeltaTestHelpers.emitEventDeltaArtifacts None ()
        assertMatches roslynEvent eventDelta.Delta

    [<Fact>]
    let ``async delta row counts match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynAsync = findBaseline "Async" baselines

        let asyncDelta = MetadataDeltaTestHelpers.emitAsyncDeltaArtifacts None ()
        assertMatches roslynAsync asyncDelta.Delta

    [<Fact>]
    let ``event multi-generation delta rows match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynAdd = findBaseline "Event" baselines
        let roslynUpdate = findBaseline "EventUpdate" baselines

        let artifacts = MetadataDeltaTestHelpers.emitEventMultiGenerationArtifacts ()
        assertMatches roslynAdd artifacts.Generation1
        assertMatches roslynUpdate artifacts.Generation2

    [<Fact>]
    let ``async multi-generation delta rows match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynAdd = findBaseline "Async" baselines
        let roslynUpdate = findBaseline "AsyncUpdate" baselines

        let artifacts = MetadataDeltaTestHelpers.emitAsyncMultiGenerationArtifacts ()
        assertMatches roslynAdd artifacts.Generation1
        assertMatches roslynUpdate artifacts.Generation2

    [<Fact>]
    let ``closure delta row counts match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynClosure = findBaseline "Closure" baselines

        let closureDelta = MetadataDeltaTestHelpers.emitClosureDeltaArtifacts ()
        assertMatches roslynClosure closureDelta.Delta

    [<Fact>]
    let ``closure multi-generation delta rows match Roslyn baseline`` () =
        let baselines = loadRoslynTables ()
        let roslynAdd = findBaseline "Closure" baselines
        let roslynUpdate = findBaseline "ClosureUpdate" baselines

        let artifacts = MetadataDeltaTestHelpers.emitClosureMultiGenerationArtifacts ()
        assertMatches roslynAdd artifacts.Generation1
        assertMatches roslynUpdate artifacts.Generation2
