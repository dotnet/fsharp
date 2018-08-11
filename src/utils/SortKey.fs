namespace Internal.Utilities.Collections

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

module Set =
    let diff a b =
        if Set.isEmpty a || Set.isEmpty b then a
        else Set.fold (fun a k -> Set.remove k a) a b

type zset<'Key,'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> = Set<SortKey<'Key,'Comparer>>

[<Sealed; AbstractClass>]
type Zset<'Key>() =
    static member empty<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct>() : zset<'Key,'Comparer> =
        Set.empty<SortKey<'Key,'Comparer>>

    static member inline isEmpty<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (s:zset<'Key,'Comparer>) =
        Set.isEmpty s

    static member ofList<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> lst : zset<'Key,'Comparer> =
        lst
        |> List.map (fun k -> {CompareObj=k})
        |> Set.ofList

    static member ofSeq<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> lst : zset<'Key,'Comparer> =
        lst
        |> Seq.map (fun k -> {CompareObj=k})
        |> Set.ofSeq

    static member inline contains<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (s:zset<'Key,'Comparer>) =
        Set.contains {CompareObj=k} s

    static member inline exists<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (f:'Key->bool) (s:zset<'Key,'Comparer>) =
        Set.exists (fun {CompareObj=k} -> f k) s

    static member inline add<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (s:zset<'Key,'Comparer>) =
        Set.add {CompareObj=k} s

    static member inline remove<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (s:zset<'Key,'Comparer>) =
        Set.remove {CompareObj=k} s

    static member inline forall<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (f:'Key->bool) (s:zset<'Key,'Comparer>) =
        Set.forall (fun {CompareObj=k} -> f k) s

    static member inline memberOf<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (s:zset<'Key,'Comparer>) (k:'Key) =
        Set.contains {CompareObj=k} s

    static member inline elements<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (s:zset<'Key,'Comparer>) =
        Set.foldBack (fun e l -> e.CompareObj::l) s []

    static member inline filter<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (f:'Key->bool) (s:zset<'Key,'Comparer>) =
        Set.filter (fun {CompareObj=k} -> f k) s

    static member inline union<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (set1:zset<'Key,'Comparer>) (set2:zset<'Key,'Comparer>) =
        Set.union set1 set2

    static member inline fold<'Comparer, 'State when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (folder:'Key->'State->'State) (s:zset<'Key,'Comparer>) (state:'State) : 'State =
        Set.fold (fun acc {CompareObj=k} -> folder k acc) state s

    static member inline addList<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (xs:list<'Key>) (s:zset<'Key,'Comparer>) =
        List.fold (fun acc x -> Set.add {CompareObj=x} acc) s xs
    
    static member inline inter<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (a:zset<'Key,'Comparer>) (b:zset<'Key,'Comparer>) =
        Set.intersect a b

    static member inline equal<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (a:zset<'Key,'Comparer>) (b:zset<'Key,'Comparer>) =
        if obj.ReferenceEquals (a,b) then true
        else
            let lhs = (a:>seq<_>).GetEnumerator ()
            let rhs = (b:>seq<_>).GetEnumerator ()
            let rec loop () =
                match lhs.MoveNext (), rhs.MoveNext () with
                | true, true when Unchecked.defaultof<'Comparer>.Compare (lhs.Current.CompareObj, rhs.Current.CompareObj) = 0 -> loop ()  
                | false, false -> true
                | _ -> false
            loop ()

    static member inline iter<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (f:'Key->unit) (m:zset<'Key,'Comparer>) =
        Set.iter (fun {CompareObj=k} -> f k) m

