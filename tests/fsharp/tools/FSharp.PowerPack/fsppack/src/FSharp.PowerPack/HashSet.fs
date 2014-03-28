namespace Microsoft.FSharp.Collections

open System
open System.Collections
open System.Collections.Generic

// HashSets are currently implemented using the .NET Dictionary type. 
[<Sealed>]
type HashSet<'T>(t: Dictionary<'T,int>) = 

    new (hasheq: IEqualityComparer<'T>) = 
        new HashSet<_>(new Dictionary<_,_>(hasheq))

    new (size:int,hasheq: IEqualityComparer<'T>) = 
        new HashSet<_>(new Dictionary<_,_>(size,hasheq))

    new (elements:seq<'T>, hasheq: IEqualityComparer<'T>) as t = 
        new HashSet<_>(new Dictionary<_,_>(hasheq)) 
        then 
           for x in elements do t.Add x

    new (size:int) = failwith "unreachable"; new HashSet<'T>(11)

    new () = failwith "unreachable"; new HashSet<'T>(11)

    new (seq:seq<'T>) = failwith "unreachable"; new HashSet<'T>(11)
        
    member x.Add(y)    = t.[y] <- 0

    member x.Clear() = t.Clear()

    member x.Copy() : HashSet<'T>  = 
        let t2 = new Dictionary<'T,int>(t.Count,t.Comparer) in 
        t |> Seq.iter (fun kvp -> t2.[kvp.Key] <- 0); 
        new HashSet<'T>(t2)

    member x.Fold f acc = 
        let mutable res = acc
        for kvp in t do
            res <- f kvp.Key res
        res

    member x.Iterate(f) =  t |> Seq.iter (fun kvp -> f kvp.Key)

    member x.Contains(y) = t.ContainsKey(y)
    member x.Remove(y) = t.Remove(y) |> ignore
    member x.Count = t.Count
    interface IEnumerable<'T> with
        member x.GetEnumerator() = t.Keys.GetEnumerator() :> IEnumerator<_>
    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = t.Keys.GetEnumerator()  :> IEnumerator 
