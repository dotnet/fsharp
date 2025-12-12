// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections

open System.Collections
open System.Collections.Generic

/// Core difference list implementation
/// DList is a function that prepends elements to a list
/// This gives O(1) append when combining two DLists
type internal DList<'T> = DList of ('T list -> 'T list)

/// Cached difference list with lazy materialization for efficient iteration
/// Combines the O(1) append of DList with efficient iteration via lazy caching
[<Sealed>]
type internal CachedDList<'T> internal (dlist: DList<'T>, lazyList: Lazy<'T list>) =
    
    static let empty = CachedDList<'T>(DList id, lazy [])
    
    /// Create from a DList and a lazy materialized list
    internal new (dlist: DList<'T>) =
        let lazyList = lazy (
            let (DList f) = dlist
            f []
        )
        CachedDList(dlist, lazyList)
    
    /// Create from a list
    new (xs: 'T list) =
        let dlist = DList (fun tail -> xs @ tail)
        let lazyList = lazy xs
        CachedDList(dlist, lazyList)
    
    static member Empty = empty
    
    /// The total number of elements
    member _.Length = lazyList.Value.Length
    
    /// Append a single element (O(1))
    member _.AppendOne(y: 'T) =
        let (DList f) = dlist
        let newDList = DList (fun tail -> f (y :: tail))
        CachedDList(newDList)
    
    /// Append a sequence of elements
    member _.Append(ys: seq<'T>) =
        let ysList = List.ofSeq ys
        let (DList f) = dlist
        let newDList = DList (fun tail -> f (ysList @ tail))
        CachedDList(newDList)
    
    /// Convert to list (uses cached value if available)
    member _.ToList() = lazyList.Value
    
    /// For QueueList compatibility - returns materialized list
    member x.FirstElements : 'T list = x.ToList()
    
    /// For QueueList compatibility - returns empty list (no "last" concept in DList)
    member _.LastElements : 'T list = []
    
    /// Internal access to the DList for efficient append operations
    member internal _.InternalDList = dlist
    
    interface IEnumerable<'T> with
        member x.GetEnumerator() : IEnumerator<'T> =
            (lazyList.Value :> IEnumerable<'T>).GetEnumerator()
    
    interface IEnumerable with
        member x.GetEnumerator() : IEnumerator =
            (lazyList.Value :> IEnumerable).GetEnumerator()

module internal CachedDList =
    
    let empty<'T> : CachedDList<'T> = CachedDList<'T>.Empty
    
    let ofSeq (x: seq<'T>) = CachedDList(List.ofSeq x)
    
    let ofList (x: 'T list) = CachedDList(x)
    
    let toList (x: CachedDList<'T>) = x.ToList()
    
    let one (x: 'T) = CachedDList([x])
    
    let appendOne (x: CachedDList<'T>) (y: 'T) = x.AppendOne(y)
    
    /// Append two DLists - O(1) operation via function composition
    let append (x: CachedDList<'T>) (ys: CachedDList<'T>) =
        if x.Length = 0 then ys
        elif ys.Length = 0 then x
        else
            let (DList f) = x.InternalDList
            let (DList g) = ys.InternalDList
            // Compose the two functions: first apply g, then apply f
            let newDList = DList (f >> g)
            CachedDList(newDList)
    
    let iter (f: 'T -> unit) (x: CachedDList<'T>) =
        List.iter f (x.ToList())
    
    let map (f: 'T -> 'U) (x: CachedDList<'T>) =
        ofList (List.map f (x.ToList()))
    
    let exists (f: 'T -> bool) (x: CachedDList<'T>) =
        List.exists f (x.ToList())
    
    let forall (f: 'T -> bool) (x: CachedDList<'T>) =
        List.forall f (x.ToList())
    
    let filter (f: 'T -> bool) (x: CachedDList<'T>) =
        ofList (List.filter f (x.ToList()))
    
    let foldBack (f: 'T -> 'S -> 'S) (x: CachedDList<'T>) (acc: 'S) =
        List.foldBack f (x.ToList()) acc
    
    let tryFind (f: 'T -> bool) (x: CachedDList<'T>) =
        List.tryFind f (x.ToList())
