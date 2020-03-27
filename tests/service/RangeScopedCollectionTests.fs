module FSharp.Compiler.Service.Tests.RangeScopedCollectionTest

open System.IO
open FSharp.Compiler
open FSharp.Compiler.Range
open FsUnit
open NUnit.Framework

type NumberedItems() =
    let mutable itemsCount = 0

    member val RequestedRangeNumbers = ResizeArray()

    member x.Create(m: range) =
        itemsCount <- itemsCount + 1
        NumberedItem(itemsCount, m, x)

    member x.Create() =
        x.Create(rangeStartup)

and NumberedItem(number: int, m: range, items) =
    member val Number = number

    override x.ToString() =
        x.Number.ToString() + ": " + m.ToShortString()

    interface IRangeOwner with
        member x.Range =
            items.RequestedRangeNumbers.Add(number)
            m


type RangeScopedCollection<'T when 'T :> IRangeOwner> with
    member x.WithScope(m, f) =
        use scope = x.CreateScope(m)
        f ()

    member x.WithScope(f) =
        x.WithScope(rangeStartup, f)


let mkRange s e =
    mkRange unknownFileName s e

let mkSimplePos c =
    mkPos 0 c

let mkSimpleRange (s, e) =
    let s = mkSimplePos s
    let e = mkSimplePos e
    mkRange s e


[<Test>]
let ``Filter - End pos 01`` () =
    let expectedPos = mkSimplePos 3
    let expectedRange = mkRange expectedPos expectedPos

    let nestedScopeRange = mkSimpleRange (1, 5)
    let innerNestedScopeRange = mkSimpleRange (1, 2)
    let otherNestedScopeRange = mkSimpleRange (4, 5)

    let items = NumberedItems()
    let rsc = RangeScopedCollection<_>()

    rsc.AddItem(items.Create(expectedRange))
    rsc.AddItem(items.Create())

    rsc.WithScope(nestedScopeRange, fun _ ->
        rsc.AddItem(items.Create())
        rsc.AddItem(items.Create(expectedRange))

        rsc.WithScope(innerNestedScopeRange, fun _ ->
            rsc.AddItem(items.Create(innerNestedScopeRange))))

    rsc.WithScope(otherNestedScopeRange, fun _ ->
        rsc.AddItem(items.Create(otherNestedScopeRange)))

    rsc.AddItem(items.Create(expectedRange))

    RangeScopedCollection.itemsByEndPos expectedPos rsc
    |> Seq.map (fun o -> o.Number)
    |> Array.ofSeq
    |> shouldEqual [| 1; 7; 4 |]

    items.RequestedRangeNumbers
    |> Array.ofSeq
    |> shouldEqual [| 1; 2; 7; 3; 4 |]


let dumpCollection (rsc: RangeScopedCollection<_>) =
    let writer = new StringWriter()

    let rec loop indent (rsc: RangeScopedCollection<_>) =
        for item in rsc.Items do
            writer.WriteLine(indent + item.ToString())

        for range, rsc in rsc.NestedScopes do
            writer.WriteLine()
            writer.WriteLine(indent + "Nested scope: " + range.ToShortString())
            loop (indent + "  ") rsc

    loop "" rsc
    writer.ToString()

let (|Trim|) (s: string) =
    s.Trim()

let assertDumpEquals rsc (Trim expected) =
    let (Trim dump) = dumpCollection rsc
    dump |> shouldEqual expected


[<Test>]
let ``Scopes 01`` () =
    let items = NumberedItems()
    let rsc = RangeScopedCollection<_>()

    rsc.AddItem(items.Create())
    rsc.WithScope(fun _ ->
        rsc.AddItem(items.Create()))
    rsc.AddItem(items.Create())

    assertDumpEquals rsc """
1: (1,0--1,0)
3: (1,0--1,0)

Nested scope: (1,0--1,0)
  2: (1,0--1,0)
"""


[<Test>]
let ``Scopes 02 - Nested`` () =
    let items = NumberedItems()
    let rsc = RangeScopedCollection<_>()

    rsc.AddItem(items.Create())
    rsc.WithScope(fun _ ->
        rsc.AddItem(items.Create())
        rsc.WithScope(fun _ ->
            rsc.AddItem(items.Create()))
        rsc.AddItem(items.Create()))
    rsc.AddItem(items.Create())

    assertDumpEquals rsc """
1: (1,0--1,0)
5: (1,0--1,0)

Nested scope: (1,0--1,0)
  2: (1,0--1,0)
  4: (1,0--1,0)

  Nested scope: (1,0--1,0)
    3: (1,0--1,0)
"""
