// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Internal.Utilities.Library.Extras

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Threading.Tasks
open Internal.Utilities.Collections
open Internal.Utilities.Library

let debug = false

let verbose = false

let mutable progress = false

// Intended to be a general hook to control diagnostic output when tracking down bugs
let mutable tracking = false

let isEnvVarSet s =
    try not(isNull(Environment.GetEnvironmentVariable s)) with _ -> false

let GetEnvInteger e dflt = match Environment.GetEnvironmentVariable(e) with null -> dflt | t -> try int t with _ -> dflt

let dispose (x: IDisposable MaybeNull) =
    match x with
    | Null -> ()
    | NonNull x -> x.Dispose()

//-------------------------------------------------------------------------
// Library: bits
//------------------------------------------------------------------------

module Bits =
    let b0 n =  (n          &&& 0xFF)
    let b1 n =  ((n >>> 8)  &&& 0xFF)
    let b2 n =  ((n >>> 16) &&& 0xFF)
    let b3 n =  ((n >>> 24) &&& 0xFF)

    let rec pown32 n = if n = 0 then 0  else (pown32 (n-1) ||| (1  <<<  (n-1)))
    let rec pown64 n = if n = 0 then 0L else (pown64 (n-1) ||| (1L <<< (n-1)))
    let mask32 m n = (pown32 n) <<< m
    let mask64 m n = (pown64 n) <<< m

//-------------------------------------------------------------------------
// Library: Orders
//------------------------------------------------------------------------

module Bool =
    let order = LanguagePrimitives.FastGenericComparer<bool>

module Int32 =
    let order = LanguagePrimitives.FastGenericComparer<int>

module Int64 =
    let order = LanguagePrimitives.FastGenericComparer<int64>

module Pair =
    let order (compare1: IComparer<'T1>, compare2: IComparer<'T2>) =
        { new IComparer<struct ('T1 * 'T2)> with
             member _.Compare((a1, a2), (aa1, aa2)) =
                  let res1 = compare1.Compare (a1, aa1)
                  if res1 <> 0 then res1 else compare2.Compare (a2, aa2) }

type NameSet =  Zset<string>

module NameSet =
    let ofList l : NameSet = List.foldBack Zset.add l (Zset.empty String.order)

module NameMap =
    let domain m = Map.foldBack (fun x _ acc -> Zset.add x acc) m (Zset.empty String.order)
    let domainL m = Zset.elements (domain m)

//-------------------------------------------------------------------------
// Library
//------------------------------------------------------------------------

type IntMap<'T> = Zmap<int, 'T>
module IntMap =
    let empty () = Zmap.empty Int32.order

    let add k v (t:IntMap<'T>) = Zmap.add k v t
    let find k (t:IntMap<'T>) = Zmap.find k t
    let tryFind k (t:IntMap<'T>) = Zmap.tryFind k t
    let remove k (t:IntMap<'T>) = Zmap.remove k t
    let mem k (t:IntMap<'T>) = Zmap.mem k t
    let iter f (t:IntMap<'T>) = Zmap.iter f t
    let map f (t:IntMap<'T>) = Zmap.map f t
    let fold f (t:IntMap<'T>) z = Zmap.fold f t z


//-------------------------------------------------------------------------
// Library: generalized association lists
//------------------------------------------------------------------------

module ListAssoc =

    /// Treat a list of key-value pairs as a lookup collection.
    /// This function looks up a value based on a match from the supplied
    /// predicate function.
    let rec find f x l =
      match l with
      | [] -> notFound()
      | (x2, y) :: t -> if f x x2 then y else find f x t

    /// Treat a list of key-value pairs as a lookup collection.
    /// This function looks up a value based on a match from the supplied
    /// predicate function and returns None if value does not exist.
    let rec tryFind (f:'key->'key->bool) (x:'key) (l:('key*'value) list) : 'value option =
        match l with
        | [] -> None
        | (x2, y) :: t -> if f x x2 then Some y else tryFind f x t

//-------------------------------------------------------------------------
// Library: lists as generalized sets
//------------------------------------------------------------------------

module ListSet =
    let inline contains f x l = List.exists (f x) l

    /// NOTE: O(n)!
    let insert f x l = if contains f x l then l else x :: l

    let unionFavourRight f l1 l2 =
        match l1, l2 with
        | _, [] -> l1
        | [], _ -> l2
        | _ -> List.foldBack (insert f) l1 l2 (* nb. foldBack to preserve natural orders *)

    /// NOTE: O(n)!
    let rec private findIndexAux eq x l n =
        match l with
        | [] -> notFound()
        | h :: t -> if eq h x then n else findIndexAux eq x t (n+1)

    /// NOTE: O(n)!
    let findIndex eq x l = findIndexAux eq x l 0

    let rec remove f x l =
        match l with
        | h :: t -> if f x h then t else h :: remove f x t
        | [] -> []

    /// NOTE: quadratic!
    let rec subtract f l1 l2 =
      match l2 with
      | h :: t -> subtract f (remove (fun y2 y1 -> f y1 y2) h l1) t
      | [] -> l1

    let isSubsetOf f l1 l2 = List.forall (fun x1 -> contains f x1 l2) l1

    /// nb. preserve orders here: f must be applied to elements of l1 then elements of l2
    let isSupersetOf f l1 l2 = List.forall (fun x2 -> contains (fun y2 y1 -> f y1 y2) x2 l1) l2

    let equals f l1 l2 = isSubsetOf f l1 l2 && isSupersetOf f l1 l2

    let unionFavourLeft f l1 l2 =
        match l1, l2 with
        | _, [] -> l1
        | [], _ -> l2
        | _ -> l1 @ (subtract f l2 l1)

    /// NOTE: not tail recursive!
    let rec intersect f l1 l2 =
        match l2 with
        | h :: t -> if contains f h l1 then h :: intersect f l1 t else intersect f l1 t
        | [] -> []

    /// Note: if duplicates appear, keep the ones toward the _front_ of the list
    let setify f l = List.fold (fun acc x -> insert f x acc) [] l |> List.rev

    let hasDuplicates f l =
        match l with
        | [] -> false
        | [_] -> false
        | [x; y] -> f x y
        | x :: rest ->
            let rec loop acc l =
                match l with
                | [] -> false
                | x :: rest ->
                    if contains f x acc then
                        true
                    else
                        loop (x :: acc) rest

            loop [x] rest

//-------------------------------------------------------------------------
// Library: pairs
//------------------------------------------------------------------------

let mapFoldFst f s (x, y) = let x2, s = f s x in (x2, y), s

let mapFoldSnd f s (x, y) = let y2, s = f s y in (x, y2), s

let pair a b = a, b

let p13 (x, _y, _z) = x

let p23 (_x, y, _z) = y

let p33 (_x, _y, z) = z

let p14 (x1, _x2, _x3, _x4) = x1

let p24 (_x1, x2, _x3, _x4) = x2

let p34 (_x1, _x2, x3, _x4) = x3

let p44 (_x1, _x2, _x3, x4) = x4

let p15 (x1, _x2, _x3, _x4, _x5) = x1

let p25 (_x1, x2, _x3, _x4, _x5) = x2

let p35 (_x1, _x2, x3, _x4, _x5) = x3

let p45 (_x1, _x2, _x3, x4, _x5) = x4

let p55 (_x1, _x2, _x3, _x4, x5) = x5

let map1Of2 f (a1, a2) = (f a1, a2)

let map2Of2 f (a1, a2) = (a1, f a2)

let map1Of3 f (a1, a2, a3) = (f a1, a2, a3)

let map2Of3 f (a1, a2, a3) = (a1, f a2, a3)

let map3Of3 f (a1, a2, a3) = (a1, a2, f a3)

let map3Of4 f (a1, a2, a3, a4) = (a1, a2, f a3, a4)

let map4Of4 f (a1, a2, a3, a4) = (a1, a2, a3, f a4)

let map5Of5 f (a1, a2, a3, a4, a5) = (a1, a2, a3, a4, f a5)

let map6Of6 f (a1, a2, a3, a4, a5, a6) = (a1, a2, a3, a4, a5, f a6)

let foldPair (f1, f2) acc (a1, a2) = f2 (f1 acc a1) a2

let fold1Of2 f1 acc (a1, _a2) = f1 acc a1

let foldTriple (f1, f2, f3) acc (a1, a2, a3) = f3 (f2 (f1 acc a1) a2) a3

let foldQuadruple (f1, f2, f3, f4) acc (a1, a2, a3, a4) = f4 (f3 (f2 (f1 acc a1) a2) a3) a4

let mapPair (f1, f2) (a1, a2) = (f1 a1, f2 a2)

let mapTriple (f1, f2, f3) (a1, a2, a3) = (f1 a1, f2 a2, f3 a3)

let mapQuadruple (f1, f2, f3, f4) (a1, a2, a3, a4) = (f1 a1, f2 a2, f3 a3, f4 a4)

//---------------------------------------------------------------------------
// Zmap rebinds
//-------------------------------------------------------------------------

module Zmap =
    let force k mp = match Zmap.tryFind k mp with Some x -> x | None -> failwith "Zmap.force: lookup failed"

    let mapKey key f mp =
      match f (Zmap.tryFind key mp) with
      | Some fx -> Zmap.add key fx mp
      | None -> Zmap.remove key mp

//---------------------------------------------------------------------------
// Zset
//-------------------------------------------------------------------------

module Zset =
    let ofList order xs = Zset.addList xs (Zset.empty order)

    // CLEANUP NOTE: move to Zset?
    let rec fixpoint f (s as s0) =
        let s = f s
        if Zset.equal s s0 then s0 (* fixed *)
        else fixpoint f s (* iterate *)

/// Buffer printing utility
let buildString f =
    let buf = StringBuilder 100
    f buf
    buf.ToString()

/// Writing to output stream via a string buffer.
let writeViaBuffer (os: TextWriter) f =
    let buf = StringBuilder 100
    f buf
    os.Write(buf.ToString())

type StringBuilder with

    member this.AppendString(value: string) = this.Append(value) |> ignore

//---------------------------------------------------------------------------
// Imperative Graphs
//---------------------------------------------------------------------------

type GraphNode<'Data, 'Id> = { nodeId: 'Id; nodeData: 'Data; mutable nodeNeighbours: GraphNode<'Data, 'Id> list }

type Graph<'Data, 'Id when 'Id : comparison and 'Id : equality>
         (nodeIdentity: 'Data -> 'Id,
          nodes: 'Data list,
          edges: ('Data * 'Data) list) =

    let edges = edges |> List.map (fun (v1, v2) -> nodeIdentity v1, nodeIdentity v2)
    let nodes = nodes |> List.map (fun d -> nodeIdentity d, { nodeId = nodeIdentity d; nodeData=d; nodeNeighbours=[] })
    let tab = Map.ofList nodes
    let nodes = List.map snd nodes
    do for node in nodes do
        node.nodeNeighbours <- edges |> List.filter (fun (x, _y) -> x = node.nodeId) |> List.map (fun (_, nodeId) -> tab[nodeId])

    member g.GetNodeData nodeId = tab[nodeId].nodeData

    member g.IterateCycles f =
        let rec trace path node =
            if List.exists (nodeIdentity >> (=) node.nodeId) path then f (List.rev path)
            else List.iter (trace (node.nodeData :: path)) node.nodeNeighbours
        List.iter (fun node -> trace [] node) nodes

//---------------------------------------------------------------------------
// In some cases we play games where we use 'null' as a more efficient representation
// in F#. The functions below are used to give initial values to mutable fields.
// This is an unsafe trick, as it relies on the fact that the type of values
// being placed into the slot never utilizes "null" as a representation. To be used with
// with care.
//----------------------------------------------------------------------------

type NonNullSlot<'T when 'T : not struct> = 'T
let nullableSlotEmpty() : NonNullSlot<'T> = Unchecked.defaultof<_>
let nullableSlotFull (x: 'T) : NonNullSlot<'T> = x

//---------------------------------------------------------------------------
// Caches, mainly for free variables
//---------------------------------------------------------------------------

type cache<'T when 'T : not struct> = { mutable cacheVal: NonNullSlot<'T> }
let newCache() = { cacheVal = nullableSlotEmpty() }

let inline cached cache ([<InlineIfLambda>] resF) =
    match box cache.cacheVal with
    | null ->
        let res = resF()
        cache.cacheVal <- nullableSlotFull res
        res
    | _ ->
        cache.cacheVal

let inline cacheOptByref (cache: byref<'T option>) ([<InlineIfLambda>] f) =
    match cache with
    | Some v -> v
    | None ->
       let res = f()
       cache <- Some res
       res

// REVIEW: this is only used because we want to mutate a record field,
// and because you cannot take a byref<_> of such a thing directly,
// we cannot use 'cacheOptByref'. If that is changed, this can be removed.
let inline cacheOptRef (cache: _ ref) ([<InlineIfLambda>] f) =
    match cache.Value with
    | Some v -> v
    | None ->
       let res = f()
       cache.Value <-  Some res
       res

let inline tryGetCacheValue cache =
    match box cache.cacheVal with
    | null -> ValueNone
    | _ -> ValueSome cache.cacheVal

#if DUMPER
type Dumper(x:obj) =
     [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
     member self.Dump = sprintf "%A" x
#endif

[<RequireQualifiedAccess>]
type MaybeLazy<'T> =
    | Strict of 'T
    | Lazy of InterruptibleLazy<'T>

    member this.Value: 'T =
        match this with
        | Strict x -> x
        | Lazy x -> x.Value

    member this.Force() : 'T =
        match this with
        | Strict x -> x
        | Lazy x -> x.Force()

let inline vsnd ((_, y): struct('T * 'T)) = y

/// Track a set of resources to cleanup
type DisposablesTracker() =

    let items = Stack<IDisposable>()

    /// Register some items to dispose
    member _.Register (i:#IDisposable MaybeNull) = 
        match box i with
        | null -> ()
        | _ -> items.Push (!!i)

    interface IDisposable with

        member _.Dispose() =
            let l = List.ofSeq items
            items.Clear()
            for i in l do
                try i.Dispose() with _ -> ()

/// Specialized parallel functions for an array.
/// Different from Array.Parallel as it will try to minimize the max degree of parallelism.
/// Will flatten aggregate exceptions that contain one exception.
[<RequireQualifiedAccess>]
module ArrayParallel =

    let inline iteri f (arr: 'T []) =
        let parallelOptions = ParallelOptions(MaxDegreeOfParallelism = max (min Environment.ProcessorCount arr.Length) 1)
        try
            Parallel.For(0, arr.Length, parallelOptions, fun i ->
                f i arr[i]
            ) |> ignore
        with
        | :? AggregateException as ex when ex.InnerExceptions.Count = 1 ->
            raise(ex.InnerExceptions[0])

    let inline iter f (arr: 'T []) =
        arr |> iteri (fun _ item -> f item)

    let inline mapi f (arr: 'T []) =
        let mapped = Array.zeroCreate arr.Length
        arr |> iteri (fun i item -> mapped[i] <- f i item)
        mapped

    let inline map f (arr: 'T []) =
        arr |> mapi (fun _ item -> f item)

[<RequireQualifiedAccess>]
module ListParallel =

    let map f (xs: 'T list) =
        xs
        |> List.toArray
        |> ArrayParallel.map f
        |> Array.toList

[<RequireQualifiedAccess>]
module Async =
    let map f a =
        async {
            let! a = a
            return f a
        }
