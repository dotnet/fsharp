module StructSeqCollectToList

open System.Collections
open System.Collections.Generic

[<Struct>]
type StructSeq(items: int[]) =
    interface IEnumerable<int> with
        member _.GetEnumerator() : IEnumerator<int> = (items :> IEnumerable<int>).GetEnumerator()
    interface IEnumerable with
        member _.GetEnumerator() : IEnumerator = items.GetEnumerator()

let collectToList (xs: StructSeq list) = xs |> Seq.collect id |> List.ofSeq
