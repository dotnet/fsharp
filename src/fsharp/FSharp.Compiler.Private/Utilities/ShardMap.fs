namespace Internal.Utilities.Collections

open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open System
open System.Threading
open System.Collections.Generic

[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
[<NoEquality; NoComparison>]
type private MapTree<'Key,'T> = 
    | MapEmpty
    | MapOne of 'Key * 'T
    | MapNode of 'Key * 'T * MapTree<'Key,'T> *  MapTree<'Key,'T> * int

type Opt<'T> =
    struct
        val Val: 'T
        val Exists : bool
    end
    new (value:'T,exists) = { Val = value ; Exists = exists }
    static member None with get() = Opt<'T>(Unchecked.defaultof<'T>,false)
    static member inline Some value = Opt<'T>(value,true)


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private MapTree = 

    let rec sizeAux acc m = 
        match m with  
        | MapEmpty -> acc       
        | MapOne _ -> acc + 1
        | MapNode(_,_,l,r,_) -> sizeAux (sizeAux (acc+1) l) r 

    let size x = sizeAux 0 x

    let empty = MapEmpty 

    let height  = function
      | MapEmpty -> 0      
      | MapOne _ -> 1
      | MapNode(_,_,_,_,h) -> h

    let isEmpty m = 
        match m with 
        | MapEmpty -> true
        | _ -> false

    let mk l k v r = 
        match l,r with 
        | MapEmpty,MapEmpty -> MapOne(k,v)
        | _ -> 
            let hl = height l 
            let hr = height r 
            let m = if hl < hr then hr else hl 
            MapNode(k,v,l,r,m+1)

    let rebalance t1 k v t2 =
        let t1h = height t1 
        let t2h = height t2 
        if  t2h > t1h + 2 then (* right is heavier than left *)
            match t2 with 
            | MapNode(t2k,t2v,t2l,t2r,_) -> 
               (* one of the nodes must have height > height t1 + 1 *)
               if height t2l > t1h + 1 then  (* balance left: combination *)
                 match t2l with 
                 | MapNode(t2lk,t2lv,t2ll,t2lr,_) ->
                    mk (mk t1 k v t2ll) t2lk t2lv (mk t2lr t2k t2v t2r) 
                 | _ -> failwith "rebalance"
               else (* rotate left *)
                 mk (mk t1 k v t2l) t2k t2v t2r
            | _ -> failwith "rebalance"
        else
            if  t1h > t2h + 2 then (* left is heavier than right *)
              match t1 with 
              | MapNode(t1k,t1v,t1l,t1r,_) -> 
                (* one of the nodes must have height > height t2 + 1 *)
                  if height t1r > t2h + 1 then 
                  (* balance right: combination *)
                    match t1r with 
                    | MapNode(t1rk,t1rv,t1rl,t1rr,_) ->
                        mk (mk t1l t1k t1v t1rl) t1rk t1rv (mk t1rr k v t2)
                    | _ -> failwith "rebalance"
                  else
                    mk t1l t1k t1v (mk t1r k v t2)
              | _ -> failwith "rebalance"
            else mk t1 k v t2

    let rec add (comparer: IComparer<'Value>) k v m = 
        match m with 
        | MapEmpty -> MapOne(k,v)       
        | MapOne(k2,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0   then MapNode (k,v,MapEmpty,m,2)
            elif c = 0 then MapOne(k,v)
            else            MapNode (k,v,m,MapEmpty,2)
        | MapNode(k2,v2,l,r,h) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then rebalance (add comparer k v l) k2 v2 r
            elif c = 0 then MapNode(k,v,l,r,h)
            else rebalance l k2 v2 (add comparer k v r) 

    let rec find (comparer: IComparer<'Value>) k m = 
        match m with 
        | MapEmpty -> raise (KeyNotFoundException())
        | MapOne(k2,v2) -> 
            let c = comparer.Compare(k,k2) 
            if c = 0 then v2
            else raise (KeyNotFoundException())
        | MapNode(k2,v2,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then find comparer k l
            elif c = 0 then v2
            else find comparer k r

    let rec tryFind (comparer: IComparer<'Value>) k m = 
        match m with 
        | MapEmpty -> None
        | MapOne(k2,v2) -> 
            let c = comparer.Compare(k,k2) 
            if c = 0 then Some v2
            else None
        | MapNode(k2,v2,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then tryFind comparer k l
            elif c = 0 then Some v2
            else tryFind comparer k r

    let rec tryGetValue (comparer: IComparer<'Value>) k (out:byref<'V>) m = 
        match m with 
        | MapEmpty -> false
        | MapOne(k2,v2) -> 
            let c = comparer.Compare(k,k2) 
            if c = 0 then out <- v2 ; true
            else false
        | MapNode(k2,v2,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then tryGetValue comparer k &out l
            elif c = 0 then out <- v2 ; true
            else tryGetValue comparer k &out r

    let rec tryFindOpt (comparer: IComparer<'Value>) k m = 
        match m with 
        | MapEmpty -> Opt.None
        | MapOne(k2,v2) -> 
            let c = comparer.Compare(k,k2) 
            if c = 0 then Opt.Some v2
            else Opt.None
        | MapNode(k2,v2,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then tryFindOpt comparer k l
            elif c = 0 then Opt.Some v2
            else tryFindOpt comparer k r

    let partition1 (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) k v (acc1,acc2) = 
        if f.Invoke(k, v) then (add comparer k v acc1,acc2) else (acc1,add comparer k v acc2) 
    
    let rec partitionAux (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) s acc = 
        match s with 
        | MapEmpty -> acc
        | MapOne(k,v) -> partition1 comparer f k v acc
        | MapNode(k,v,l,r,_) -> 
            let acc = partitionAux comparer f r acc 
            let acc = partition1 comparer f k v acc
            partitionAux comparer f l acc

    let partition (comparer: IComparer<'Value>) f s = partitionAux comparer (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) s (empty,empty)

    let filter1 (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) k v acc = if f.Invoke(k, v) then add comparer k v acc else acc 

    let rec filterAux (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) s acc = 
        match s with 
        | MapEmpty -> acc
        | MapOne(k,v) -> filter1 comparer f k v acc
        | MapNode(k,v,l,r,_) ->
            let acc = filterAux comparer f l acc
            let acc = filter1 comparer f k v acc
            filterAux comparer f r acc

    let filter (comparer: IComparer<'Value>) f s = filterAux comparer (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) s empty

    let rec spliceOutSuccessor m = 
        match m with 
        | MapEmpty -> failwith "internal error: Map.spliceOutSuccessor"
        | MapOne(k2,v2) -> k2,v2,MapEmpty
        | MapNode(k2,v2,l,r,_) ->
            match l with 
            | MapEmpty -> k2,v2,r
            | _ -> let k3,v3,l' = spliceOutSuccessor l in k3,v3,mk l' k2 v2 r

    let rec remove (comparer: IComparer<'Value>) k m = 
        match m with 
        | MapEmpty -> empty
        | MapOne(k2,_) -> 
            let c = comparer.Compare(k,k2) 
            if c = 0 then MapEmpty else m
        | MapNode(k2,v2,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then rebalance (remove comparer k l) k2 v2 r
            elif c = 0 then 
              match l,r with 
              | MapEmpty,_ -> r
              | _,MapEmpty -> l
              | _ -> 
                  let sk,sv,r' = spliceOutSuccessor r 
                  mk l sk sv r'
            else rebalance l k2 v2 (remove comparer k r) 

    let rec removeDecr (comparer: IComparer<'Value>) k (count:int ref) m = 
        match m with 
        | MapEmpty -> empty
        | MapOne(k2,_) -> 
            let c = comparer.Compare(k,k2) 
            if c = 0 then Interlocked.Decrement(count) |> ignore ; MapEmpty else m
        | MapNode(k2,v2,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then rebalance (removeDecr comparer k count l) k2 v2 r
            elif c = 0 then
              Interlocked.Decrement(count) |> ignore            
              match l,r with 
              | MapEmpty,_ -> r
              | _,MapEmpty -> l
              | _ -> 
                  let sk,sv,r' = spliceOutSuccessor r 
                  mk l sk sv r'
            else rebalance l k2 v2 (removeDecr comparer k count r) 

    let rec mem (comparer: IComparer<'Value>) k m = 
        match m with 
        | MapEmpty -> false
        | MapOne(k2,_) -> (comparer.Compare(k,k2) = 0)
        | MapNode(k2,_,l,r,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then mem comparer k l
            else (c = 0 || mem comparer k r)

    let rec iterOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m =
        match m with 
        | MapEmpty -> ()
        | MapOne(k2,v2) -> f.Invoke(k2, v2)
        | MapNode(k2,v2,l,r,_) -> iterOpt f l; f.Invoke(k2, v2); iterOpt f r

    let iter f m = iterOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

    let rec tryPickOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m =
        match m with 
        | MapEmpty -> None
        | MapOne(k2,v2) -> f.Invoke(k2, v2) 
        | MapNode(k2,v2,l,r,_) -> 
            match tryPickOpt f l with 
            | Some _ as res -> res 
            | None -> 
            match f.Invoke(k2, v2) with 
            | Some _ as res -> res 
            | None -> 
            tryPickOpt f r

    let tryPick f m = tryPickOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

    let rec existsOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
        match m with 
        | MapEmpty -> false
        | MapOne(k2,v2) -> f.Invoke(k2, v2)
        | MapNode(k2,v2,l,r,_) -> existsOpt f l || f.Invoke(k2, v2) || existsOpt f r

    let exists f m = existsOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

    let rec forallOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
        match m with 
        | MapEmpty -> true
        | MapOne(k2,v2) -> f.Invoke(k2, v2)
        | MapNode(k2,v2,l,r,_) -> forallOpt f l && f.Invoke(k2, v2) && forallOpt f r

    let forall f m = forallOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

    let rec map f m = 
        match m with 
        | MapEmpty -> empty
        | MapOne(k,v) -> MapOne(k,f v)
        | MapNode(k,v,l,r,h) -> 
            let l2 = map f l 
            let v2 = f v 
            let r2 = map f r 
            MapNode(k,v2,l2, r2,h)

    let rec mapiOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
        match m with
        | MapEmpty -> empty
        | MapOne(k,v) -> MapOne(k, f.Invoke(k, v))
        | MapNode(k,v,l,r,h) -> 
            let l2 = mapiOpt f l 
            let v2 = f.Invoke(k, v) 
            let r2 = mapiOpt f r 
            MapNode(k,v2, l2, r2,h)

    let mapi f m = mapiOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

    let rec foldBackOpt (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x = 
        match m with 
        | MapEmpty -> x
        | MapOne(k,v) -> f.Invoke(k,v,x)
        | MapNode(k,v,l,r,_) -> 
            let x = foldBackOpt f r x
            let x = f.Invoke(k,v,x)
            foldBackOpt f l x

    let foldBack f m x = foldBackOpt (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) m x

    let rec foldOpt (f:OptimizedClosures.FSharpFunc<_,_,_,_>) x m  = 
        match m with 
        | MapEmpty -> x
        | MapOne(k,v) -> f.Invoke(x,k,v)
        | MapNode(k,v,l,r,_) -> 
            let x = foldOpt f x l
            let x = f.Invoke(x,k,v)
            foldOpt f x r

    let fold f x m = foldOpt (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) x m

    let foldSectionOpt (comparer: IComparer<'Value>) lo hi (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x =
        let rec foldFromTo (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x = 
            match m with 
            | MapEmpty -> x
            | MapOne(k,v) ->
                let cLoKey = comparer.Compare(lo,k)
                let cKeyHi = comparer.Compare(k,hi)
                let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke(k, v, x) else x
                x
            | MapNode(k,v,l,r,_) ->
                let cLoKey = comparer.Compare(lo,k)
                let cKeyHi = comparer.Compare(k,hi)
                let x = if cLoKey < 0                 then foldFromTo f l x else x
                let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke(k, v, x) else x
                let x = if cKeyHi < 0                 then foldFromTo f r x else x
                x
       
        if comparer.Compare(lo,hi) = 1 then x else foldFromTo f m x

    let foldSection (comparer: IComparer<'Value>) lo hi f m x =
        foldSectionOpt comparer lo hi (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) m x

    let toList m = 
        let rec loop m acc = 
            match m with 
            | MapEmpty -> acc
            | MapOne(k,v) -> (k,v)::acc
            | MapNode(k,v,l,r,_) -> loop l ((k,v)::loop r acc)
        loop m []
    let toArray m = m |> toList |> Array.ofList
    let ofList comparer l = List.fold (fun acc (k,v) -> add comparer k v acc) empty l

    let rec mkFromEnumerator comparer acc (e : IEnumerator<_>) = 
        if e.MoveNext() then 
            let (x,y) = e.Current 
            mkFromEnumerator comparer (add comparer x y acc) e
        else acc
      
    let ofArray comparer (arr : array<_>) =
        let mutable res = empty
        for (x,y) in arr do
            res <- add comparer x y res 
        res

    let ofSeq comparer (c : seq<'Key * 'T>) =
        match c with 
        | :? array<'Key * 'T> as xs -> ofArray comparer xs
        | :? list<'Key * 'T> as xs -> ofList comparer xs
        | _ -> 
            use ie = c.GetEnumerator()
            mkFromEnumerator comparer empty ie 
      
    let copyToArray s (arr: _[]) i =
        let j = ref i 
        s |> iter (fun x y -> arr.[!j] <- KeyValuePair(x,y); j := !j + 1)

    let rec addAndIncr (comparer: IComparer<'Value>) k v (iref:int ref) m = 
        match m with 
        | MapEmpty -> Interlocked.Increment(iref) |> ignore ; MapOne(k,v)       
        | MapOne(k2,_) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0   then Interlocked.Increment(iref) |> ignore ; MapNode (k,v,MapEmpty,m,2)
            elif c = 0 then MapOne(k,v)
            else            Interlocked.Increment(iref) |> ignore ; MapNode (k,v,m,MapEmpty,2)
        | MapNode(k2,v2,l,r,h) -> 
            let c = comparer.Compare(k,k2) 
            if c < 0 then rebalance (addAndIncr comparer k v iref l) k2 v2 r
            elif c = 0 then MapNode(k,v,l,r,h)
            else rebalance l k2 v2 (addAndIncr comparer k v iref r) 
    


////////////////////////////
////////////////////////
///Shard Map
////////////////////
///////////////////////////
type private Shard<'K,'V> = MapTree<'K,'V> []
type private Bucket<'K,'V> = Shard<'K,'V> []

type private MutateHead<'V>(head) =
    let mutable head : 'V list = [head]
    member __.Add(v:'V) = head <- v :: head
    member __.Head with get() = head




module private util =

    open NonStructuralComparison

    [<Literal>] 
    let ShardSize = 16
    //let private empty = Array.zeroCreate<Map<'K,'V>>(ShardSize)

    // Helper Functions
    ////////////////////////////

    //let calcBitMaskDepth itemCount = int(Math.Ceiling(Math.Log(float itemCount) / Math.Log(float 2))) //todo:make private
    let inline calcBitMaskDepth itemCount =
        let rec go s d =
            if s = 0 then if d < 5 then 5 else d
            else go (s >>> 1) (d + 1)
        go itemCount 0
   
    let inline bucketSizeFromBitDepth (i:int) = 2 <<< (i - 5) // todo 4 is shard size 2^n
    let inline requiredBucketSize itemCount = bucketSizeFromBitDepth (calcBitMaskDepth itemCount)
    let inline calcSubBitMask (bitDepth:int) = ~~~(-1 <<< (bitDepth))

    ///prvides index in bucket of shard
    let inline bucketIndex (keyHash:int,subBitMask:int) = (keyHash &&& subBitMask) >>> 4// todo: improve substring bitmask calc

    ///let inline private bucketIndexOld (keyHash:int,bitdepth:int) = (keyHash &&& (~~~(-1 <<< (bitdepth)))) >>> 4// todo: improve substring bitmask calc

    ///provides sub index in shards
    let inline shardIndex (keyHash:int) = keyHash &&& 0b1111
    let inline isNull v = Object.ReferenceEquals(null,v)

    let inline getIndexes(key,bucketBitMask) = let hk  = key.GetHashCode() in bucketIndex(hk,bucketBitMask) , shardIndex(hk)
    let inline higherRange (index:int,bitdepth:int) = (index ||| (1 <<< (bitdepth - 4)))

    let inline newShardEmpty () = Array.zeroCreate<MapTree<'K,'V>>(ShardSize)
    let inline newBucketEmpty bucketSize = Array.zeroCreate<Shard<'K,'V>>(bucketSize)

    let inline eachMapInBucketClose (bucket:Bucket<'K,'V>,f:int -> int -> MapTree<'K,'V> -> unit,bucketCloseFn:int -> unit) =
        let fo = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
    
        Tasks.Parallel.For(0, bucket.Length, fun bi ->
            for si in 0 .. ShardSize - 1 do
                let s = bucket.[bi]
                if not( isNull s) then
                    let m = s.[si]
                    if not( isNull m) then
                        fo.Invoke(bi,si,m)
            bucketCloseFn bi
        ) |> ignore

    let inline eachMapInBucket (bucket:Bucket<'K,'V>,f:int -> int -> MapTree<'K,'V> -> unit) =
        eachMapInBucketClose (bucket,f,ignore)

    let getOrCreateShard (bucket:Bucket<'K,'V>,index:int) =
        let mutable shrd = bucket.[index]
        if isNull shrd then
            shrd <- newShardEmpty ()
            bucket.[index] <- shrd
        shrd        

    /////////////////////////////
    /// Constructor helper


    let init<'T,'K,'V when 'K : comparison>(counter:int, items : 'T seq,mapFn: 'T -> KeyValuePair<'K,'V>) =
        let size = if counter < ShardSize then ShardSize else counter
        let comparer = LanguagePrimitives.FastGenericComparer<'K>
        let bitdepth = (calcBitMaskDepth size)
        let bucketSize = bucketSizeFromBitDepth (bitdepth)
        let bucketBitMask = calcSubBitMask bitdepth
        let newBucket = Array.zeroCreate<Shard<'K,'V>>(bucketSize)

        items
        |> Seq.iter (fun item ->
            let kvp = mapFn item
            // let kh = kvp.Key.GetHashCode()
            // let bi = bucketIndex(kh,bucketBitMask)
            // let si = shardIndex(kh)
            let bi, si = getIndexes(kvp.Key,bucketBitMask)
            let shrd = getOrCreateShard(newBucket,bi)
            let m = shrd.[si]
            shrd.[si] <- if isNull m then MapOne (kvp.Key,kvp.Value) else MapTree.add comparer kvp.Key kvp.Value m
            )
        (counter,newBucket)

open util
open MapTree
open System.Threading.Tasks

    /// Shard Map
    ////////////////////////////

type ShardMap<'K,'V  when 'K : equality and 'K : comparison >(icount:int, nBucket:Shard<'K,'V> []) =
    
    let empty = newShardEmpty ()

    //let mutable subMapHead = ihead
    let comparer = LanguagePrimitives.FastGenericComparer<'K>

    let mutable bucket = nBucket
    let countRef = ref icount

    //Lock variables
    ///////////////////////////////////
    let mutable resizing = false // lightweight single op read var for checking state
    let resizeLock = obj()  // lock for when internal bucket array needs to resize


    //let calcBitMaskDepth itemCount = int(Math.Ceiling(Math.Log(float itemCount) / Math.Log(float 2)))

    let mutable capacity = (bucket.Length * ShardSize) - 1
    let mutable bitMaskDepth = (util.calcBitMaskDepth capacity)
    let mutable bucketBitMask = calcSubBitMask bitMaskDepth    

    /// Internal Funtions
    /////////////////////////////////////////////////
    
    let newShardCopy oary = 
        let nary = newShardEmpty ()
        Array.Copy(oary,nary,util.ShardSize)
        nary

    let mapList () =
        let mutable result = []
        for bi in 0 .. bucket.Length - 1 do
            for si in 0 .. ShardSize - 1 do
                if not(util. isNull bucket.[bi].[si]) then
                    result <- bucket.[bi].[si] :: result
        result                                 

    let getMap (key:'K) =
        let kh = key.GetHashCode()
        bucket.[util.bucketIndex(kh,bucketBitMask)].[util.shardIndex kh]

    let item (key:'K) =
        let m = getMap key
        if isNull m then
            raise <| KeyNotFoundException(sprintf "Key:%A , does not exist in the dictionary" key)
        else
            MapTree.find comparer key m
    
    let tryFind (key:'K) =
        let m = getMap key
        if isNull m then
            None
        else
            MapTree.tryFind comparer key m

    let tryGetValue (key:'K,out:byref<'V>) =
        let m = getMap key
        if isNull m then
            false
        else
            MapTree.tryGetValue comparer key &out m

    let tryFindOpt (key:'K) =
        let m = getMap key
        if isNull m then
            Opt.None
        else
            MapTree.tryFindOpt comparer key m

    let resize newBucketSize =
        let capacityLimit = (newBucketSize * ShardSize) - 1
        let bmd = calcBitMaskDepth capacityLimit
        let newBucketBitMask = calcSubBitMask bmd
        let target = Array.zeroCreate<Shard<'K,'V>> (newBucketSize)
        //printfn "resizing: capacity:%i bitMaskDepth:%i buckBitMask:%i newBucketSize:%i" capacityLimit bmd newBucketBitMask newBucketSize
        eachMapInBucket(bucket,fun _ si sm ->
            MapTree.iter (fun k v -> // for each key in source
                let kh = k.GetHashCode()
                let tbi = bucketIndex(kh,newBucketBitMask)
                let tshrd = getOrCreateShard(target,tbi)
                let tm = tshrd.[si]

                if isNull tm then
                    tshrd.[si] <- MapOne(k,v)
                else                                             
                    tshrd.[si] <- MapTree.add comparer k v tm
            ) sm
        )


        for i2 in 0 .. target.Length - 1 do
            if util. isNull target.[i2] then target.[i2] <- empty
            
        //now update internal state
        lock bucket.SyncRoot (fun () -> bucket <- target ) // ??? needed with resize lock already in place?
        bitMaskDepth <- bmd
        bucketBitMask <- newBucketBitMask
        capacity <- capacityLimit - 1

    let grow () =

        //printfn "started resize ()"
        let isize = bucket.Length
        let nsize = isize * 2
        resize nsize
        
            
    let add(k:'K,v:'V) =
        // let kh = k.GetHashCode()
        // let bi = util.bucketIndex(kh,bucketBitMask)
        // let si = util.shardIndex kh
        let bi, si = getIndexes(k,bucketBitMask)
        let shrd = newShardCopy bucket.[bi]

        lock bucket.SyncRoot (fun () -> bucket.[bi] <- shrd)

        let m = shrd.[si]
        if isNull m then
            countRef := Interlocked.Increment(countRef)
            let nm = MapOne (k,v)
            shrd.[si] <- nm
        else
            if not(MapTree.mem comparer k m) then 
                countRef := Interlocked.Increment(countRef)
            let nm = MapTree.add comparer k v m
            shrd.[si] <- nm

    let remove(k:'K) =
        // let kh = k.GetHashCode()
        // let bi = util.bucketIndex(kh,bucketBitMask)
        // let si = util.shardIndex kh
        let bi, si = getIndexes(k,bucketBitMask)
        let shrd = newShardCopy bucket.[bi]
        
        lock bucket.SyncRoot (fun () -> bucket.[bi] <- shrd )

        let m = shrd.[si]
        if isNull m |> not then
            shrd.[si] <- MapTree.removeDecr comparer k countRef m
 

    let exists (existsFn:'K -> 'V  -> bool) = 
        let fnOpt = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(existsFn)

        let rec hop(bi,si) =
            let next(bi,si) = 
                if   si + 1 < ShardSize     then hop(bi    ,si + 1)
                elif bi + 1 < bucket.Length then hop(bi + 1,0     )
                else false
            let m = bucket.[bi].[si]
            if isNull m then
                next(bi,si)
            else
                if MapTree.existsOpt fnOpt m then
                    true
                else
                    next(bi,si)
        hop(0,0)

    let existsPar (existsFn:'K -> 'V  -> bool) = // ERROR OUT OF BOUNDS EXCEPTION ISSUE
        let fnOpt = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(existsFn)
        let mutable found = false
        let rec go(shrd:Shard<'K,'V>,si:int,pls:ParallelLoopState) =
            if not found then
                if isNull shrd.[si] then
                    if si + 1 < ShardSize then
                        go(shrd,si + 1,pls)
                else
                    if MapTree.existsOpt fnOpt shrd.[si] then
                        found <- true
                        pls.Break()
                    else
                        if si + 1 < ShardSize then
                            go(shrd,si + 1,pls)
        let rec buck(pos,final,pls:ParallelLoopState) =
            if pos < final then
                go(bucket.[pos],0,pls)
                if not found then
                    buck(pos+1,final,pls)

        // Tasks.Parallel.For()
        // Tasks.Parallel.ForEach(bucket,Action<Shard<_,_>,ParallelLoopState>(fun shrd pls ->
        //     go(shrd,0,pls)
        // )) |> ignore

        let batchsize = if bucket.Length = 2 then 2 else bucket.Length >>> 1 //default 4 batches
        Tasks.Parallel.For(0,2,fun wi (pls:ParallelLoopState) ->
            buck(wi * batchsize,(wi + 1) * batchsize,pls)
        ) |> ignore

        // Tasks.Parallel.For(0,bucket.Length,fun bi ->
        //     go(bucket.[bi],0)
        // ) |> ignore
        
        found
    let tryFindInRange (fn:'V -> bool) =
        let rec hop(bi,si) =
            let next(bi,si) = 
                if   si + 1 < ShardSize     then hop(bi,si + 1)
                elif bi + 1 < bucket.Length then hop(bi + 1,0)
                else None               
            let rec go ml =
                match ml with
                | [] -> next(bi,si)
                | m :: t ->
                    match m with 
                    | MapEmpty ->           go t
                    | MapOne(_,v) ->        if fn v then Some v else go t
                    | MapNode(_,v,l,r,_) -> if fn v then Some v else go (l :: r :: t)                
            let m = bucket.[bi].[si]
            if isNull m then next(bi,si)
            else go [m]      
        hop(0,0)


    /////////////////////////////////////////////////////////////
    /// Constructor operation to ensure no null array references
    /////////////////////////////////////////////////////////////
        
    do  // prevent any out of index errors on non-set shards
        for bi in 0.. bucket.Length - 1 do
        if isNull bucket.[bi] then
            bucket.[bi] <- empty


    /////////////////////////////////////////////////////////////
    /// private methods for internal access
    /////////////////////////////////////////////////////////////
    static member private transpose (fn:MapTree<'K,'V> -> MapTree<'K,'T>) (itemCount:int) (bucket:Shard<'K,'V> []) =
        let nBucket = Array.zeroCreate<Shard<'K,'T>>(bucket.Length)
        
        eachMapInBucket(bucket, fun bi si m -> 
            let s = getOrCreateShard (nBucket,bi)
            s.[si] <- fn m
        )
        ShardMap<'K,'T>(itemCount,nBucket)

    member __.addListThis(items:('K * 'V) list) = // presumes already grown bucket
        let newShards = Array.init bucket.Length (fun _ -> false)
        for (k,v) in items do
            // let kh = k.GetHashCode()
            // let bi = util.bucketIndex(kh,bucketBitMask)
            // let si = util.shardIndex kh
            let bi, si = getIndexes(k,bucketBitMask)
            
            let shrd = 
                if not newShards.[bi] then
                    newShards.[bi] <- true
                    let shrd = newShardCopy bucket.[bi]
                    lock bucket.SyncRoot (fun () -> bucket.[bi] <- shrd)
                    shrd
                else
                    bucket.[bi] // mutate if owned by call           

            let m = shrd.[si]
            if isNull m then
                countRef := Interlocked.Increment(countRef)
                let nm = MapOne (k,v)
                shrd.[si] <- nm
            else
                shrd.[si] <- MapTree.addAndIncr comparer k v countRef m 

    member __.Count = !countRef

    member __.BucketSize = bucket.Length

    member private __.resize newBucketSize = resize newBucketSize

    member private __.addNoLock(k,v) = add(k,v)

    /////////////////////////////////////////////////////////////
    /// Public methods for External access
    /////////////////////////////////////////////////////////////

    member this.AddThis(k:'K,v:'V) =     

        lock resizeLock (fun () -> 
            if !countRef > capacity then grow ()
            add(k,v)
            )
        this

        // if resizing then
        //     lock resizeLock (fun () -> add(k,v))
        // else
        //     if !countRef > capacity then
        //     // base array needs resizing
        //         resizing <- true
        //         lock resizeLock grow
        //         //End of Lock
        //         resizing <- false
        //     add(k,v)
        //this

    member __.RemoveThis(k:'K) =
        lock resizeLock (fun () -> remove(k))
        
        // if resizing then
        //     lock resizeLock (fun () -> remove(k))
        // else
        //     remove(k)
        // this

    member __.Copy() =        
        let newbucket = Array.zeroCreate<Shard<'K,'V>>(bucket.Length)
        Array.Copy(bucket,newbucket,bucket.Length)
        ShardMap<'K,'V>(!countRef,newbucket)
                
    member x.Add(k:'K,v:'V) =
        let newShardMap = x.Copy()
        newShardMap.addNoLock(k,v)
        newShardMap

    member x.AddList(items:('K * 'V) list) =
        let itemLen = List.length items
        let newShardMap = x.Copy()
        let rbs = requiredBucketSize (!countRef + itemLen)
        if rbs > capacity then newShardMap.resize rbs
        for (k,v) in items do
            newShardMap.addNoLock(k,v)
        newShardMap

    member x.AddMappedList (mapFn: 'T -> ('K * 'V)) (items:'T list) =
        let newShardMap = x.Copy()
        for item in items do
            newShardMap.addNoLock(mapFn item)
        newShardMap

    member x.Remove(k:'K) =
        let newShardMap = x.Copy()
        newShardMap.RemoveThis(k)
        newShardMap

    member x.RemoveList(items:'K list) =
        let newShardMap = x.Copy()
        for item in items do
            newShardMap.RemoveThis(item) |> ignore
        newShardMap
        
    member __.Item(key:'K) =
        if resizing then
            lock resizeLock (fun () -> item key)
        else
            item key

    member __.ContainsKey(key:'K) =
        let kh = key.GetHashCode()
        let m = bucket.[util.bucketIndex(kh,bucketBitMask)].[util.shardIndex kh]
        //printfn "?| looking for key:'%A' [%i][%i] in map{%A}" key bi si m
        if isNull m then
            false
        else
            MapTree.mem comparer key m

    member __.TryFind(key:'K) =
        if resizing then
            lock resizeLock (fun () -> tryFind key)
        else
            tryFind key

    member __.TryGetValue(key:'K,result:byref<'V>) = 
        tryGetValue(key,&result)

    member __.TryFindOpt(key:'K) = tryFindOpt(key)

    member __.Iter(fn:'K -> 'V -> unit) =
        let iopt = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(fn)
        eachMapInBucket(bucket,fun _ _ m ->
            MapTree.iterOpt iopt m
        )
    //member __.Fold (foldFn:'S -> 'K -> 'V  -> 'S) (istate:'S) = mapFold foldFn istate

    member __.Fold (foldFn:'S -> 'K -> 'V  -> 'S) (istate:'S) =
        let foldOpt = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(foldFn)

        let rec gmp(m,acc) = 
            match m with
            | MapEmpty -> acc
            | MapOne (k,v) -> foldOpt.Invoke(acc,k,v)
            | MapNode(k,v,l,r,_) ->
                gmp(r,
                    gmp(l,
                        foldOpt.Invoke(acc,k,v)))
        
        let mutable state = istate 
        for bi in 0 .. bucket.Length - 1 do
            for si in 0 .. ShardSize - 1 do
                if isNull bucket.[bi].[si] then ()
                else
                    state <- gmp(bucket.[bi].[si],state)
        state

    member __.FoldReduce (foldFn:'S -> 'K -> 'V  -> 'S) (aggrFn:'S -> 'S -> 'S) (istate:'S) =
        let foldOpt = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(foldFn)
        let aggrOpt = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(aggrFn)
        let results = Array.zeroCreate<'S>(bucket.Length)

        Tasks.Parallel.For(0,bucket.Length,fun bi ->
            let mutable lstate = istate
            let shrd = bucket.[bi]
            for si in 0 .. ShardSize - 1 do
                let m = shrd.[si]
                if not(util.isNull m) then
                    lstate <- MapTree.foldOpt foldOpt lstate m
            results.[bi] <- lstate
        ) |> ignore
        let mutable tstate = istate
        for bi in 0 .. bucket.Length - 1 do
            tstate <- aggrOpt.Invoke(tstate,results.[bi])
        tstate

    //[<Obsolete("Folds in same random order as fold but matches foldback fn signature of accumulator state last")>]
    member __.FoldBack (foldFn:'K -> 'V  -> 'S ->'S) (istate:'S) =
        let foldOpt = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(foldFn)

        let rec gmp(m,acc) = 
            match m with
            | MapEmpty -> acc
            | MapOne (k,v) -> foldOpt.Invoke(k,v,acc)
            | MapNode(k,v,l,r,_) ->
                gmp(r,
                    gmp(l,
                        foldOpt.Invoke(k,v,acc)))
        
        let mutable state = istate 
        for bi in 0 .. bucket.Length - 1 do
            for si in 0 .. ShardSize - 1 do
                if not(util.isNull bucket.[bi].[si]) then 
                    state <- gmp(bucket.[bi].[si],state)
        state
    
    member __.Partition (predicate:'K -> 'V -> bool) =
        let predOpt = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(predicate)
        let bt , ct = Array.zeroCreate<Shard<'K,'V>>(bucket.Length) , ref 0
        let bf , cf = Array.zeroCreate<Shard<'K,'V>>(bucket.Length) , ref 0
        
        Tasks.Parallel.For(0,bucket.Length,fun bi ->
            let shrd = bucket.[bi]
            for si in 0 .. ShardSize - 1 do
                let m = shrd.[si]
                if not(util.isNull m) then
                    MapTree.iter (fun k v ->
                        let mapResult(b0:Bucket<'K,'V>,cRef:int ref) = 
                            Interlocked.Increment(cRef) |> ignore
                            let s0 = getOrCreateShard(b0 , bi)
                            let m0 = s0.[si]
                            s0.[si] <-
                                if isNull m0 then
                                    MapOne(k,v)
                                else
                                    MapTree.add comparer k v m0
                        if predOpt.Invoke(k,v) then
                            mapResult(bt,ct)
                        else
                            mapResult(bf,cf)
                    ) m
            // after shard map filled, fill in empties
            if isNull bt then bt.[bi] <- empty
            if isNull bf then bf.[bi] <- empty
                            
        ) |> ignore
        
        ShardMap<'K,'V>(!ct,bt) , ShardMap<'K,'V>(!cf,bf)
        
    member __.Map  (mapFn:'V -> 'T)       : ShardMap<'K,'T> = ShardMap.transpose (MapTree.map mapFn) !countRef bucket
    member __.Mapi (mapFn:'K -> 'V -> 'T) : ShardMap<'K,'T> = ShardMap.transpose (MapTree.mapi mapFn) !countRef bucket
    
////////////////
    member __.toSeq () =

        let mutable stack = mapList ()
        let mutable current = Unchecked.defaultof<KeyValuePair<_,_>>

        { new IEnumerator<_> with 
                member self.Current = current
            interface System.Collections.IEnumerator with
                    member self.Current = box self.Current
                    member self.MoveNext() = 
                        let rec go =                                     
                            function
                            | [] -> false
                            | MapEmpty :: rest -> go rest
                            | MapOne (k,v) :: rest -> 
                                current <- new KeyValuePair<_,_>(k,v)
                                stack <- rest
                                true                   
                            | (MapNode(k,v,l,r,_)) :: rest ->             
                                current <- new KeyValuePair<_,_>(k,v)
                                stack <- l :: r :: rest
                                true

                        go stack

                    member self.Reset() = stack <- mapList ()
            interface System.IDisposable with 
                    member self.Dispose() = stack <- [] }
                      

////////////////



    member __.PrintLayout () =
        let mutable rowCount = 0
        let mutable tmapCount = 0
        let mutable rmapCount = 0
        let columnCount = Array.zeroCreate<int>(bucket.Length)
        printfn "Printing Layout:"
        for i in 0 .. bucket.Length - 1 do
            rowCount <- 0
            rmapCount <- 0

            printf "%4i {" i
            for j in 0 .. ShardSize - 1 do
                let m = bucket.[i].[j]
                if isNull m then
                    printf " ___ |"
                else
                    tmapCount <- tmapCount + 1
                    rmapCount <- rmapCount + 1
                    columnCount.[i] <- columnCount.[i] + (MapTree.size m)
                    rowCount <- rowCount + (MapTree.size m)
                    printf " %3i |" (MapTree.size m)
            printfn "} = %5i[%6i]" rmapCount rowCount
        
        printf "Total{" 
        for j in 0 .. ShardSize - 1 do
            printf " %3i |" columnCount.[j]
        printfn "} = %5i[%6i]" tmapCount !countRef            
    

    interface IEnumerable<KeyValuePair<'K, 'V>> with
        member s.GetEnumerator() = s.toSeq()

    interface System.Collections.IEnumerable with
        override s.GetEnumerator() = (s.toSeq() :> System.Collections.IEnumerator)

    member private __.getBucket () = bucket

    member __.Exists (fn:'K -> 'V -> bool) = exists fn
    member __.ExistsPar (fn:'K -> 'V -> bool) = existsPar fn

    member __.TryFindInRange fn = tryFindInRange fn

    member __.Merge (map:ShardMap<'K,'V>) : ShardMap<'K,'V> =
        
        let target, additions, tCount = // target will be a copy of the bigger of the two maps
            if map.BucketSize > bucket.Length then
                let target = Array.zeroCreate<Shard<'K,'V>>(map.BucketSize)
                Array.Copy(map.getBucket(),target,map.BucketSize)
                target, bucket, ref (map.Count)
            else
                let target = Array.zeroCreate<Shard<'K,'V>>(bucket.Length)
                Array.Copy(bucket,target,bucket.Length)
                target, map.getBucket() , ref (!countRef)
                
        let trgtBitDepth = calcBitMaskDepth ((target.Length * ShardSize) - 1)
        let bucketBitMask = calcSubBitMask trgtBitDepth
        
        let mapFn = 
            if additions.Length = target.Length then
                // Split into two types of mapping, one-to-one, and small map expansion
                ////////////////////////////////////////////////////////////////////////
                fun bi si sm ->
                    let tshrd = target.[bi]
                    let tm = tshrd.[si]
                    // Buckets same size so simple to one-to-one map
                    if isNull tm then
                        Interlocked.Add(tCount,MapTree.size sm) |> ignore
                        tshrd.[si] <- sm
                    else
                        tshrd.[si] <-
                            MapTree.fold (fun acc k v -> // for each key in source
                                MapTree.addAndIncr comparer k v tCount acc // lookup doubled so might be worth extending Maptree to return both tree & `newAddition' bool
                            ) tm sm
            else
                fun _ si sm ->
                    // Bucket lower order so shard needs to be remapped to higher order
                    ///////////////////////////////////////////////////////////////////////
                    MapTree.iter (fun k v -> // for each key in source
                        let kh = k.GetHashCode()
                        let tbi = bucketIndex(kh,bucketBitMask)
                        let tshrd = target.[tbi]
                        let tm = tshrd.[si]

                        if isNull tm then
                            Interlocked.Increment(tCount) |> ignore
                            tshrd.[si] <- MapOne(k,v)
                        else                                             
                            tshrd.[si] <- MapTree.addAndIncr comparer k v tCount tm
                    ) sm


        eachMapInBucket(additions , mapFn)
        // Step through maps to union
        ShardMap<'K,'V>(!tCount,target)


    /////////////////////////////////
    /// Static Methods
    /////////////////////////////////

    static member inline Empty with get () = ShardMap<'K,'V>(0)

    static member Collect (collectFn: 'T -> #seq<'V>) (keyFn:'V -> 'K) (collection:seq<'T>) =
        
        let initSize = Seq.length collection //:HACK
        let size = if initSize < ShardSize then ShardSize else initSize
        let comparer = LanguagePrimitives.FastGenericComparer<'K>
        let bitdepth = calcBitMaskDepth size
        let bucketSize = bucketSizeFromBitDepth (bitdepth)
        let bucketBitMask = calcSubBitMask bitdepth
        let newBucket = Array.zeroCreate<Shard<'K,'V>>(bucketSize)
        let countRef = ref 0

        for a in collection do
            for item in collectFn a do
                let k =  keyFn item 
                let kh = k.GetHashCode()
                let bi = bucketIndex(kh,bucketBitMask)
                let shrd = getOrCreateShard(newBucket,bi)
                let si = shardIndex(kh)
                let m = shrd.[si]
                shrd.[si] <- 
                    if isNull m then 
                        Interlocked.Increment(countRef) |> ignore
                        MapOne (k,item) 
                    else 
                        MapTree.addAndIncr comparer k item countRef m

        let smap = ShardMap<'K,'V>(!countRef,newBucket)
        let optimalBucketSize = !countRef |> calcBitMaskDepth |> bucketSizeFromBitDepth
        if optimalBucketSize > newBucket.Length then
            smap.resize optimalBucketSize
        smap

    static member LayerAdditive (addf:'T list -> 'U -> 'T list) (sm2:ShardMap<'K,'U>) (sm1:ShardMap<'K,'T list>) : ShardMap<'K,'T list> =
        let comparer = LanguagePrimitives.FastGenericComparer<'K>
        let tCount = ref sm1.Count        
        
        if sm2.BucketSize > sm1.BucketSize then sm1.resize sm2.BucketSize
        let target = sm1.getBucket()
        let bmd = calcBitMaskDepth ((target.Length * ShardSize) - 1 )
        let bucketBitMask = calcSubBitMask bmd

        let mapFn : int -> int -> MapTree<'K,'U> -> unit = 
            if sm2.BucketSize = sm1.BucketSize then
                // straight map
                fun bi si m2 ->
                    target.[bi].[si] <- 
                        MapTree.foldBack (fun k v acc -> 
                            MapTree.add comparer k (addf (match MapTree.tryFind comparer k acc with | Some ls -> ls | None -> Interlocked.Increment(tCount) |> ignore ;[]) v) acc ) m2 target.[bi].[si]

            elif sm2.BucketSize < sm1.BucketSize then
                // sm1 bigger so sm2 needs to be projected upwards onto it
                fun _ si m2 ->
                    MapTree.iter (fun k v ->
                        let tbi =  bucketIndex(k.GetHashCode(),bucketBitMask)
                        let tshrd = target.[tbi]
                        let tm = tshrd.[si]

                        if isNull tm then
                            Interlocked.Increment(tCount) |> ignore
                            tshrd.[si] <- MapOne(k,addf [] v)
                        else                                             
                            tshrd.[si] <- 
                                MapTree.add comparer k (addf (match MapTree.tryFind comparer k tm with | Some ls -> ls | None -> Interlocked.Increment(tCount) |> ignore ; []) v) tshrd.[si]
                    ) m2
            else
                failwithf "Bucket resizing of LayerAdditive method failed"
                    //target.[bi].[si] <- MapTree.foldBack (fun k v acc -> MapTree.add k (addf (match MapTree.tryFind k acc with | Some ls -> ls | None -> []) v) acc ) m2 b1.[bi].[si]

        eachMapInBucket(sm2.getBucket(),mapFn)
        ShardMap<'K,'T list>(!tCount,target)

    static member LayerList (f:'V -> 'K ) (ls:'V list) : ShardMap<'K,'V list> =
        let comparer = LanguagePrimitives.FastGenericComparer<'K>
        let tCount = ref 0
        let listlen = List.length ls
        if listlen = 0 then
            ShardMap<'K,'V list>()
        else
            let countEstimate = listlen / 3
            let bitdepth = (calcBitMaskDepth countEstimate)
            let bucketSize = bucketSizeFromBitDepth (bitdepth)
            let bucketBitMask = calcSubBitMask bitdepth
            //printfn "LayerList: len:%i | est:%i | bitDepth:%i | bucketSize:%i" listlen countEstimate bitdepth bucketSize
            let target = newBucketEmpty bucketSize
            for bi in 0 .. bucketSize - 1 do
                target.[bi] <- newShardEmpty()

            let rec go =
                function
                | [] -> ()
                | h :: t -> 
                    let k = f h
                    let bi, si = getIndexes(k,bucketBitMask)
                    // let kh = k.GetHashCode()
                    // let bi = util.bucketIndex(kh,bucketBitMask)
                    // let si = util.shardIndex(kh)
                    let shrd = target.[bi]
                    let m = shrd.[si]
                    if isNull m then
                        Interlocked.Increment(tCount) |> ignore
                        shrd.[si] <- MapOne(k,[h])
                    else
                        match MapTree.tryFind comparer k m with
                        | Some v -> 
                            shrd.[si] <- MapTree.add comparer k (h :: v) m
                        | None ->
                            Interlocked.Increment(tCount) |> ignore
                            shrd.[si] <- MapTree.add comparer k [h] m
                    go t
        
            go ls
            
            let result = ShardMap<'K,'T list>(!tCount,target)
            //let optSize = !tCount |> calcBitMaskDepth  |> pow2 
            //if bucketSize < optSize then
            //    result.resize optSize
            result

    //////////////////////////
    /////////////////////////////////////
    static member Union (unionf:seq<'V> -> 'T) (maps:ShardMap<'K,'V> seq) : ShardMap<'K,'T> =        
        let comparer = LanguagePrimitives.FastGenericComparer<'K>

        let processMaps (unionf:seq<'V> -> 'T,sources:ShardMap<'K,'V> seq) =
            //let mutable target = Unchecked.defaultof<Bucket<'K,MutateHead<'V>>>
            let enum = sources.GetEnumerator()
            let tCount = ref 0 

            let rec go(source:Bucket<'K,'V>,target:Bucket<'K,MutateHead<'V>>) =
                if source.Length = target.Length then
                    Tasks.Parallel.For(0,source.Length,fun bi ->
                        let sshrd = source.[bi]
                        
                        let mutable tshrd = target.[bi]// target.[bi] << target shard depends on bitdepth

                        for si in 0 .. ShardSize - 1 do
                            let sm = sshrd.[si]
                            if isNull sm |> not then
                                let mutable tm = tshrd.[si] //<< target shard depends on bitdepth
                                if isNull tm then
                                    tCount := Interlocked.Add(tCount,MapTree.size sm)
                                    tshrd.[si] <- MapTree.map (fun v -> MutateHead<_>(v)) sm
                                else
                                    tshrd.[si] <-
                                        MapTree.fold (fun acc k v -> // for each key in source
                                            match MapTree.tryFind comparer k acc with // try find in acc target
                                            | Some mh -> 
                                                mh.Add v
                                                acc
                                            | None -> 
                                                tCount := Interlocked.Increment(tCount)
                                                MapTree.add comparer k (MutateHead<'V>(v)) acc
                                        ) tm sm
                    ) |> ignore    
                if enum.MoveNext() then
                    go(enum.Current.getBucket(),target)
                else
                    //end of list so map value lists to new dictionary with provided unionf
                    enum.Dispose()
                    ShardMap.transpose (MapTree.map (fun (mh:MutateHead<'V>) -> unionf mh.Head )) !tCount target
            
            // start of enumeration (first shard used to create target interim map)
            if enum.MoveNext() then
                let ibucket = enum.Current.getBucket()
                let target = Array.zeroCreate<Shard<'K,MutateHead<'V>>>(ibucket.Length)
                for bi in 0 .. ibucket.Length - 1 do
                    target.[bi] <- Array.zeroCreate<MapTree<'K,MutateHead<'V>>>(util.ShardSize)
                go(ibucket,target)
            else
                enum.Dispose()
                ShardMap<'K,'T>(0,[])
    
        processMaps(unionf,maps)  //<<HACK to get intellisense to work

            
    static member UnionSingle (unionf:seq<'V> -> 'T) (maps:ShardMap<'K,'V> seq) : ShardMap<'K,'T> =        
        let comparer = LanguagePrimitives.FastGenericComparer<'K>

        let processMaps (unionf:seq<'V> -> 'T,sources:ShardMap<'K,'V> seq) =
            //let mutable target = Unchecked.defaultof<Bucket<'K,MutateHead<'V>>>
            let enum = sources.GetEnumerator()
            let tCount = ref 0 

            let rec go(source:Bucket<'K,'V>,target:Bucket<'K,MutateHead<'V>>) =
                 
                for bi in 0 .. source.Length - 1 do
                        let sshrd = source.[bi]
                        
                        let mutable tshrd = target.[bi]// target.[bi] << target shard depends on bitdepth

                        for si in 0 .. ShardSize - 1 do
                            let sm = sshrd.[si]
                            if isNull sm |> not then
                                let mutable tm = tshrd.[si] //<< target shard depends on bitdepth
                                if isNull tm then
                                    tCount := Interlocked.Add(tCount,MapTree.size sm)
                                    tshrd.[si] <- MapTree.map (fun v -> MutateHead<_>(v)) sm
                                else
                                    tshrd.[si] <-
                                        MapTree.fold (fun acc k v -> // for each key in source
                                            match MapTree.tryFind comparer k acc with // try find in acc target
                                            | Some mh -> 
                                                mh.Add v
                                                acc
                                            | None -> 
                                                tCount := Interlocked.Increment(tCount)
                                                MapTree.add comparer k (MutateHead<'V>(v)) acc
                                        ) tm sm  

                if enum.MoveNext() then
                    go(enum.Current.getBucket(),target)
                else
                    //end of list so map value lists to new dictionary with provided unionf
                    enum.Dispose()
                    //let tshard = ShardMap<_,_>(!tCount,target)
                    let fbucket = Array.zeroCreate<Shard<'K,'T>>(target.Length)
                    for fi in 0 .. fbucket.Length - 1 do
                        fbucket.[fi] <- Array.zeroCreate<MapTree<'K,'T>>(util.ShardSize)

                    for bi in 0 .. target.Length - 1 do
                        
                        let tshrd = target.[bi]// target.[bi] << target shard depends on bitdepth
                        let fshrd = fbucket.[bi]
                        for si in 0 .. ShardSize - 1 do
                            let tm = tshrd.[si]
                            if isNull tm |> not then
                                fshrd.[si] <- MapTree.map (fun (mh:MutateHead<'V>) -> unionf mh.Head ) tm
                    ShardMap(!tCount,fbucket)
            
            // start of enumeration (first shard used to create target interim map)
            if enum.MoveNext() then
                let ibucket = enum.Current.getBucket()
                let target = Array.zeroCreate<Shard<'K,MutateHead<'V>>>(ibucket.Length)
                for bi in 0 .. ibucket.Length - 1 do
                    target.[bi] <- Array.zeroCreate<MapTree<'K,MutateHead<'V>>>(util.ShardSize)
                go(enum.Current.getBucket(),target)
            else
                enum.Dispose()
                ShardMap<'K,'T>(0,[])

            
        
        processMaps(unionf,maps)  //<<HACK to get intellisense to work


        
    static member UnionParallel (unionf:'V list -> 'T) (maps:ShardMap<'K,'V> list) : ShardMap<'K,'T> =
        let comparer = LanguagePrimitives.FastGenericComparer<'K>
        let tCount = ref 0 

        let threadBuckets(sources:Bucket<'K,'V> list,target:Bucket<'K,MutateHead<'V>>) =
            let fBucket = Array.zeroCreate<Shard<'K,'T>>(target.Length)
            let trgtBitDepth = calcBitMaskDepth ((target.Length * ShardSize) - 1)
            let bucketBitMask = calcSubBitMask trgtBitDepth

            let fEmpty = newShardEmpty()

            let mapSameSize =
                fun bi si sm ->
                    let tshrd = getOrCreateShard(target,bi)
                    let tm = tshrd.[si]
                    // Buckets same size so simple to one-to-one map
                    if isNull tm then
                        Interlocked.Add(tCount,MapTree.size sm) |> ignore
                        tshrd.[si] <- MapTree.map (fun v -> MutateHead<_>(v)) sm
                    else
                        tshrd.[si] <-
                            MapTree.fold (fun acc k v -> // for each key in source
                                match MapTree.tryFind comparer k acc with // try find in acc target
                                | Some mh -> 
                                    mh.Add v
                                    acc
                                | None -> 
                                    Interlocked.Increment(tCount) |> ignore
                                    MapTree.add comparer k (MutateHead<'V>(v)) acc
                            ) tm sm

            let mapUpSize =
                fun _ si sm ->
                    MapTree.iter (fun k v -> // for each key in source
                        let kh = k.GetHashCode()
                        let tbi = bucketIndex(kh,bucketBitMask)
                        let tshrd = getOrCreateShard(target,tbi)
                        let tm = tshrd.[si]
                        if isNull tm then
                            Interlocked.Increment(tCount) |> ignore
                            tshrd.[si] <- MapOne(k,MutateHead<'V>(v))
                        else                                             
                            match MapTree.tryFind comparer k tm with // try find in acc target
                            | Some mh ->
                                mh.Add v
                            | None -> 
                                Interlocked.Increment(tCount) |> ignore
                                tshrd.[si] <-
                                    MapTree.add comparer k (MutateHead<'V>(v)) tm
                    ) sm  

            let rec go (ls:Bucket<'K,'V> list) = 
                match ls with
                | [] ->
                    // Apply unionF
                    Tasks.Parallel.For(0,target.Length,fun bi ->
                        
                        // mapping of final shard from target to final
                        let tshrd = target.[bi]
                        if isNull tshrd then
                            fBucket.[bi] <- fEmpty
                        else
                            fBucket.[bi] <- Array.zeroCreate<_>(util.ShardSize)
                            let fshrd = fBucket.[bi]
                            for si in 0 .. ShardSize - 1 do
                                let tm = tshrd.[si]
                                if isNull tm |> not then   
                                    fshrd.[si] <- MapTree.map (fun (mh:MutateHead<'V>) -> unionf mh.Head) tm          
                    ) |> ignore
                | h :: t -> 
                    let mapFn = if h.Length = target.Length then mapSameSize else mapUpSize
                    // Step through maps to union
                    eachMapInBucket(h, mapFn)
                    // rec go to next bucket on list
                    go(t)
            // begin processing list
            go(sources)

            ShardMap<'K,'T>(!tCount,fBucket)
        
        // start of enumeration (first shard used to create target interim map)
        let maxBucket, buckets = maps |> List.fold (fun (mb,bl) m -> (if m.BucketSize > mb then m.BucketSize else mb) , m.getBucket() :: bl ) (0,[])
        let target = Array.zeroCreate<Shard<'K,MutateHead<'V>>>(maxBucket)
        threadBuckets(buckets ,target)
            
    static member private init<'T>(counter:int, items : 'T seq,mapFn: 'T -> KeyValuePair<'K,'V>) =
        let size = if counter < ShardSize then ShardSize else counter
        let comparer = LanguagePrimitives.FastGenericComparer<'K>
        let bitdepth = calcBitMaskDepth size
        let bucketSize = bucketSizeFromBitDepth bitdepth
        let bucketBitMask = calcSubBitMask bitdepth
        let newBucket = Array.zeroCreate<Shard<'K,'V>>(bucketSize)

        items
        |> Seq.iter (fun item ->
            let kvp =  mapFn item
            let bi, si = getIndexes(kvp.Key,bucketBitMask)
            // let kh =   kvp.Key.GetHashCode()
            // let bi =   util.bucketIndex(kh,bucketBitMask)
            // let si =   util.shardIndex(kh)
            let shrd = util.getOrCreateShard(newBucket,bi)
            let m =    shrd.[si]
            shrd.[si] <- if isNull m then MapOne (kvp.Key,kvp.Value) else MapTree.add comparer kvp.Key kvp.Value m
            )
        
        ShardMap<'K,'V>(counter,newBucket)

    static member OfKeyedList (keyFn:'V -> 'K) (items:'V list) = ShardMap<'K,'V>.init(List.length items,items,fun v -> KeyValuePair<'K,'V>(keyFn v,v))
        
    static member OfList (items:('K * 'V) list) = ShardMap<'K,'V>(List.length items,items)
        
    static member OfSeq (keyFn:'V -> 'K) (keyValueTuples:'V seq) = 
        let mutable counter = 0
        let mutable items = []
        keyValueTuples |> Seq.iter (fun kvp -> 
            counter <- counter + 1
            items <- kvp :: items
        )
        ShardMap<'K,'V>.init<_>(counter,items,fun v -> KeyValuePair<'K,'V>(keyFn v,v))

    static member OfArray(keyValueTuples:('K * 'V) []) = 
        ShardMap<'K,'V>.init<_>(keyValueTuples.Length,keyValueTuples,fun (k,v) -> KeyValuePair<'K,'V>(k,v))

    static member OfListMapped  (fn:'T -> KeyValuePair<'K,'V>)  (items:'T list) = ShardMap<'K,'V>.init<_>(items.Length,items,fn)
    static member fold f s (m:ShardMap<_,_>) = m.Fold f s

    ////////////////////////////////////
    /// Contructors
    ///////////////////////////////////
    
    new(capacity:int) =
        let size = if capacity < ShardSize then ShardSize else capacity
        let bitdepth = calcBitMaskDepth size
        let bucketSize = bucketSizeFromBitDepth bitdepth
        let newBucket = Array.zeroCreate<Shard<'K,'V>>(bucketSize)
        ShardMap<_,_>(0,newBucket)

    new(count:int,kvps:('K * 'V) list) = let count,bucket = init<_,'K,'V>(count, kvps ,fun (k,v) -> KeyValuePair<'K,'V>(k,v)) in ShardMap<'K,'V>(count,bucket)
    new(kvps:('K * 'V) list) = ShardMap<'K,'V>(List.length kvps,kvps)

    new(kvps:('K * 'V) [])   = let count,bucket = init<_,'K,'V>(kvps.Length, kvps ,fun (k,v) -> KeyValuePair<'K,'V>(k,v)) in ShardMap<'K,'V>(count,bucket)

    new(count: int, kvps:('K * 'V) seq)  = 
        let count,bucket = init<_,'K,'V>(count, kvps ,fun (k,v) -> KeyValuePair<'K,'V>(k,v)) in ShardMap<'K,'V>(count,bucket)
    new(kvps:('K * 'V) seq)  = 
        let mutable counter = 0
        let mutable items = []
        kvps |> Seq.iter (fun kvp -> 
            counter <- counter + 1
            items <- kvp :: items
        )
        ShardMap<'K,'V>(counter,kvps)
    new() = ShardMap<_,_>(0)
