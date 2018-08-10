﻿namespace Internal.Utilities.Collections

open System
open System.Collections.Generic

[<Struct; CustomComparison; CustomEquality>]
type SortKey<'Key, 'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> = {
    CompareObj : 'Key
}
with
    interface IComparable<SortKey<'Key, 'Comparer>> with
        member lhs.CompareTo (rhs:SortKey<'Key, 'Comparer>): int = 
            Unchecked.defaultof<'Comparer>.Compare(lhs.CompareObj, rhs.CompareObj)

    static member fail () = failwith "Invalid logic. No method other than IComparable<_>.CompareTo is valid for SortKey"
    override __.GetHashCode () = SortKey<'Key,'Comparer>.fail ()
    override __.Equals _ = SortKey<'Key,'Comparer>.fail ()

#if THIS_SHOULD_JUST_THROW_AN_EXCEPTION
    interface IComparable with member __.CompareTo _ = SortKey<'Key,'Comparer>.fail ()
#else
    // tests run with an old version of FSharp.Core that doesn't using the non-boxing IComparable
    interface IComparable with
        member lhs.CompareTo rhs =
            Unchecked.defaultof<'Comparer>.Compare(lhs.CompareObj, (rhs:?>SortKey<'Key,'Comparer>).CompareObj)
#endif

type zmap<'Key,'Comparer,'Value when 'Comparer :> IComparer<'Key> and 'Comparer : struct> = Map<SortKey<'Key,'Comparer>,'Value>

[<Sealed; AbstractClass>]
type Zmap<'Key,'Value>() =
    static member empty<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct>() : zmap<'Key,'Comparer,'Value> =
        Map.empty<SortKey<'Key,'Comparer>, 'Value>

    static member ofList<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> lst : zmap<'Key,'Comparer,'Value> =
        lst
        |> List.map (fun (k,v) -> {CompareObj=k},v)
        |> Map.ofList

    static member inline chooseL<'Comparer, 'U when 'Comparer :> IComparer<'Key> and 'Comparer : struct> f (m:zmap<'Key,'Comparer,'Value>) =
        Map.foldBack (fun k v (s:list<'U>) -> match f k.CompareObj v with None -> s | Some x -> x::s) m []

    static member inline tryFind<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:zmap<'Key,'Comparer,'Value>) =
        Map.tryFind {CompareObj=k} m

    static member inline mem<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:zmap<'Key,'Comparer,'Value>) =
        Map.containsKey {CompareObj=k} m

    static member inline memberOf<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (m:zmap<'Key,'Comparer,'Value>) (k:'Key) =
        Map.containsKey {CompareObj=k} m

    static member inline add<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (v:'Value) (m:zmap<'Key,'Comparer,'Value>) =
        Map.add {CompareObj=k} v m

    static member inline find<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:zmap<'Key,'Comparer,'Value>) =
        Map.find {CompareObj=k} m

    static member inline fold<'Comparer, 'State when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (folder:'Key->'Value->'State->'State) (m:zmap<'Key,'Comparer,'Value>) (state:'State) : 'State =
        Map.foldBack (fun {CompareObj=k} t s -> folder k t s) m state 

    static member inline remove<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:zmap<'Key,'Comparer,'Value>) =
        Map.remove {CompareObj=k} m

    static member inline keys<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (m:zmap<'Key,'Comparer,'Value>) =
        Map.foldBack (fun {CompareObj=k} _ s -> k::s) m []

    static member inline values<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (m:zmap<'Key,'Comparer,'Value>) =
        Map.foldBack (fun _ v s -> v::s) m []

    static member inline toList<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (m:zmap<'Key,'Comparer,'Value>) =
        Map.foldBack (fun {CompareObj=k} v acc -> (k,v) :: acc) m []

    static member inline iter<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (f:'Key->'Value->unit) (m:zmap<'Key,'Comparer,'Value>) =
        Map.iter (fun {CompareObj=k} v -> f k v) m

    static member foldMap<'Comparer, 'State, 'U when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (folder:'State->'Key->'Value->'State*'U) (initialState:'State) (initialMap:zmap<'Key,'Comparer,'Value>) : 'State * zmap<'Key,'Comparer,'U> =
        let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt folder
        let struct (finalState, finalMap) =
            (initialMap, struct (initialState, Zmap.empty<'Comparer> ()))
            ||> Map.foldBack (fun {CompareObj=k} v struct (acc, m) ->
                let acc', v' = f.Invoke (acc, k, v)
                let m' = Map.add {CompareObj=k} v' m
                struct (acc', m'))
        finalState, finalMap

