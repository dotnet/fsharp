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
    interface IComparable with member __.CompareTo _ = SortKey<'Key,'Comparer>.fail ()
    override __.GetHashCode () = SortKey<'Key,'Comparer>.fail ()
    override __.Equals _ = SortKey<'Key,'Comparer>.fail ()

[<Sealed; AbstractClass>]
type MapCustom<'Key,'Value>() =
    static member Empty<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct>() : Map<SortKey<'Key,'Comparer>,'Value> =
        Map.empty<SortKey<'Key,'Comparer>, 'Value>

    static member ofList<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> lst : Map<SortKey<'Key,'Comparer>,'Value> =
        lst
        |> List.map (fun (k,v) -> {CompareObj=k},v)
        |> Map.ofList

    static member inline chooseL<'Comparer, 'U when 'Comparer :> IComparer<'Key> and 'Comparer : struct> f (m:Map<SortKey<'Key,'Comparer>,'Value>) =
        Map.foldBack (fun k v (s:list<'U>) -> match f k.CompareObj v with None -> s | Some x -> x::s) m []

    static member inline tryFind<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:Map<SortKey<'Key,'Comparer>,'Value>) =
        Map.tryFind {CompareObj=k} m

    static member inline mem<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:Map<SortKey<'Key,'Comparer>,'Value>) =
        Map.containsKey {CompareObj=k} m

    static member inline add<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (v:'Value) (m:Map<SortKey<'Key,'Comparer>,'Value>) =
        Map.add {CompareObj=k} v m

    static member inline find<'Comparer when 'Comparer :> IComparer<'Key> and 'Comparer : struct> (k:'Key) (m:Map<SortKey<'Key,'Comparer>,'Value>) =
        Map.find {CompareObj=k} m