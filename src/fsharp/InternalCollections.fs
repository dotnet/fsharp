// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections
open System
open System.Collections.Generic

[<StructuralEquality; NoComparison>]
type internal ValueStrength<'T when 'T : not struct> =
   | Strong of 'T
#if FX_NO_GENERIC_WEAKREFERENCE
   | Weak of WeakReference
#else
   | Weak of WeakReference<'T>
#endif

type internal AgedLookup<'TKey,'TValue when 'TValue : not struct>(keepStrongly:int, areSame, ?requiredToKeep, ?onStrongDiscard, ?keepMax: int) =
    /// The list of items stored. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let mutable refs:('TKey*ValueStrength<'TValue>) list = [] 
    let mutable keepStrongly = keepStrongly

    // Only set a strong discard function if keepMax is explicitly set to keepStrongly, i.e. there are no weak entries in this lookup.
    do assert (onStrongDiscard.IsNone || Some keepStrongly = keepMax)
       
    let strongDiscard x = match onStrongDiscard with None -> () | Some f -> f x
        
    // The 75 here determines how long the list should be passed the end of strongly held
    // references. Some operations are O(N) and we don't want to let things get out of
    // hand.
    let keepMax = defaultArg keepMax 75 
    let mutable keepMax = max keepStrongly keepMax 
    let requiredToKeep = defaultArg requiredToKeep (fun _ -> false) 
    
    /// Look up a the given key, return <c>None</c> if not found.
    let TryPeekKeyValueImpl(data,key) = 
        let rec Lookup key = function 
            // Treat a list of key-value pairs as a lookup collection.
            // This function returns true if two keys are the same according to the predicate
            // function passed in.
            | []->None
            | (key',value)::t->
                if areSame(key,key') then Some(key',value) 
                else Lookup key t      
        Lookup key data    
        
    /// Determines whether a particular key exists.
    let Exists(data,key) = TryPeekKeyValueImpl(data,key).IsSome
        
    /// Set a particular key's value.
    let Add(data,key,value) = 
        data @ [key,value]   
        
    /// Promote a particular key value.
    let Promote (data, key, value) = 
        (data |> List.filter (fun (key',_)-> not (areSame(key,key')))) @ [ (key, value) ] 

    /// Remove a particular key value.
    let RemoveImpl (data, key) = 
        let discard,keep = data |> List.partition (fun (key',_)-> areSame(key,key'))
        keep, discard
        
    let TryGetKeyValueImpl(data,key) = 
        match TryPeekKeyValueImpl(data,key) with 
        | Some(key', value) as result ->
            // If the result existed, move it to the end of the list (more likely to keep it)
            result,Promote (data,key',value)
        | None -> None,data          
       
    /// Remove weak entries from the list that have been collected.
    let FilterAndHold() =
        [ for (key,value) in refs do
            match value with
            | Strong(value) -> yield (key,value)
            | Weak(weakReference) ->
#if FX_NO_GENERIC_WEAKREFERENCE
                match weakReference.Target with 
                | null -> assert onStrongDiscard.IsNone; ()
                | value -> yield key,(value:?>'TValue) ]
#else
                match weakReference.TryGetTarget () with
                | false, _ -> assert onStrongDiscard.IsNone; ()
                | true, value -> yield key, value ]
#endif
        
    let AssignWithStrength(newdata,discard1) = 
        let actualLength = List.length newdata
        let tossThreshold = max 0 (actualLength - keepMax) // Delete everything less than this threshold
        let weakThreshhold = max 0 (actualLength - keepStrongly) // Weaken everything less than this threshold
        
        let newdata = newdata|> List.mapi( fun n kv -> n,kv ) // Place the index.
        let newdata,discard2 = newdata |> List.partition (fun (n:int,v) -> n >= tossThreshold || requiredToKeep (snd v))
        let newdata = 
            newdata 
            |> List.map( fun (n:int,(k,v)) -> 
                let handle = 
                    if n<weakThreshhold && not (requiredToKeep v) then 
                        assert onStrongDiscard.IsNone; // it disappeared, we can't dispose 
#if FX_NO_GENERIC_WEAKREFERENCE
                        Weak(WeakReference(v)) 
#else
                        Weak(WeakReference<_>(v)) 
#endif
                    else 
                        Strong(v)
                k,handle )
        refs <- newdata
        discard1 |> List.iter (snd >> strongDiscard)
        discard2 |> List.iter (snd >> snd >> strongDiscard)
        
    member al.TryPeekKeyValue(key) = 
        // Returns the original key value as well since it may be different depending on equality test.
        let data = FilterAndHold()
        TryPeekKeyValueImpl(data,key)
        
    member al.TryGetKeyValue(key) = 
        let data = FilterAndHold()
        let result,newdata = TryGetKeyValueImpl(data,key)
        AssignWithStrength(newdata,[])
        result
    member al.TryGet(key) = 
        let data = FilterAndHold()
        let result,newdata = TryGetKeyValueImpl(data,key)
        AssignWithStrength(newdata,[])
        match result with
        | Some(_,value) -> Some(value)
        | None -> None
    member al.Put(key,value) = 
        let data = FilterAndHold()
        let data,discard = if Exists(data,key) then RemoveImpl (data,key) else data,[]
        let data = Add(data,key,value)
        AssignWithStrength(data,discard) // This will remove extras 

    member al.Remove(key) = 
        let data = FilterAndHold()
        let newdata,discard = RemoveImpl (data,key)
        AssignWithStrength(newdata,discard)

    member al.Clear() =
       let discards = FilterAndHold()
       AssignWithStrength([], discards)

    member al.Resize(newKeepStrongly, ?newKeepMax) =
       let newKeepMax = defaultArg newKeepMax 75 
       keepStrongly <- newKeepStrongly
       keepMax <- max newKeepStrongly newKeepMax
       do assert (onStrongDiscard.IsNone || keepStrongly = keepMax)
       let keep = FilterAndHold()
       AssignWithStrength(keep, [])

        

type internal MruCache<'TKey,'TValue when 'TValue : not struct>(keepStrongly, areSame, ?isStillValid : 'TKey*'TValue->bool, ?areSameForSubsumption, ?requiredToKeep, ?onStrongDiscard, ?keepMax) =
        
    /// Default behavior of <c>areSameForSubsumption</c> function is areSame.
    let areSameForSubsumption = defaultArg areSameForSubsumption areSame
        
    /// The list of items in the cache. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let cache = AgedLookup<'TKey,'TValue>(keepStrongly=keepStrongly,areSame=areSameForSubsumption,?onStrongDiscard=onStrongDiscard,?keepMax=keepMax,?requiredToKeep=requiredToKeep)
        
    /// Whether or not this result value is still valid.
    let isStillValid = defaultArg isStillValid (fun _ -> true)
        
    member bc.TryGetAny(key) = 
        match cache.TryPeekKeyValue(key) with
        | Some(key', value)->
            if areSame(key',key) then Some(value)
            else None
        | None -> None
       
    member bc.TryGet(key) = 
        match cache.TryGetKeyValue(key) with
        | Some(key', value) -> 
            if areSame(key', key) && isStillValid(key,value) then Some value
            else None
        | None -> None
           
    member bc.Set(key:'TKey,value:'TValue) = 
        cache.Put(key,value)
       
    member bc.Remove(key) = 
        cache.Remove(key)
       
    member bc.Clear() =
        cache.Clear()
        
    member bc.Resize(newKeepStrongly, ?newKeepMax) =
        cache.Resize(newKeepStrongly, ?newKeepMax=newKeepMax)
        
/// List helpers
[<Sealed>]
type internal List = 
    /// Return a new list with one element for each unique 'TKey. Multiple 'TValues are flattened. 
    /// The original order of the first instance of 'TKey is preserved.
    static member groupByFirst( l : ('TKey * 'TValue) list) : ('TKey * 'TValue list) list =
        let nextIndex = ref 0
        let result = System.Collections.Generic.List<'TKey * System.Collections.Generic.List<'TValue>>()
        let keyToIndex = Dictionary<'TKey,int>(HashIdentity.Structural)
        let indexOfKey(key) =
            match keyToIndex.TryGetValue(key) with
            | true, v -> v
            | false, _ -> 
                keyToIndex.Add(key,!nextIndex)
                nextIndex := !nextIndex + 1
                !nextIndex - 1
            
        for kv in l do 
            let index = indexOfKey(fst kv)
            if index>= result.Count then 
                let k,vs = fst kv,System.Collections.Generic.List<'TValue>()
                vs.Add(snd kv)
                result.Add(k,vs)
            else
                let _,vs = result.[index]
                vs.Add(snd kv)
        
        result |> Seq.map(fun (k,vs) -> k,vs |> List.ofSeq ) |> List.ofSeq

    /// Return each distinct item in the list using reference equality.
    static member referenceDistinct( l : 'T list) : 'T list when 'T : not struct =
        let set = System.Collections.Generic.Dictionary<'T,bool>(HashIdentity.Reference)
        l |> List.iter(fun i->set.Add(i,true))
        set |> Seq.map(fun kv->kv.Key) |> List.ofSeq
