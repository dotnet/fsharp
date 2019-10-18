// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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

type internal AgedLookup<'Token, 'Key, 'Value when 'Value : not struct>(keepStrongly:int, areSimilar, ?requiredToKeep, ?onStrongDiscard, ?keepMax: int) =
    /// The list of items stored. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let mutable refs:('Key*ValueStrength<'Value>) list = [] 
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
            | (similarKey,value) :: t->
                if areSimilar(key,similarKey) then Some(similarKey,value) 
                else Lookup key t      
        Lookup key data    
        
    /// Determines whether a particular key exists.
    let Exists(data,key) = TryPeekKeyValueImpl(data,key).IsSome
        
    /// Set a particular key's value.
    let Add(data,key,value) = 
        data @ [key,value]   
        
    /// Promote a particular key value.
    let Promote (data, key, value) = 
        (data |> List.filter (fun (similarKey,_)-> not (areSimilar(key,similarKey)))) @ [ (key, value) ] 

    /// Remove a particular key value.
    let RemoveImpl (data, key) = 
        let discard,keep = data |> List.partition (fun (similarKey,_)-> areSimilar(key,similarKey))
        keep, discard
        
    let TryGetKeyValueImpl(data,key) = 
        match TryPeekKeyValueImpl(data,key) with 
        | Some(similarKey, value) as result ->
            // If the result existed, move it to the end of the list (more likely to keep it)
            result,Promote (data,similarKey,value)
        | None -> None,data          
       
    /// Remove weak entries from the list that have been collected.
    let FilterAndHold(tok: 'Token) =
        ignore tok // reading 'refs' requires a token
        [ for (key,value) in refs do
            match value with
            | Strong(value) -> yield (key,value)
            | Weak(weakReference) ->
#if FX_NO_GENERIC_WEAKREFERENCE
                match weakReference.Target with 
                | null -> assert onStrongDiscard.IsNone; ()
                | value -> yield key,(value:?>'Value) ]
#else
                match weakReference.TryGetTarget () with
                | false, _ -> assert onStrongDiscard.IsNone; ()
                | true, value -> yield key, value ]
#endif
        
    let AssignWithStrength(tok,newdata,discard1) = 
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
        ignore tok // Updating refs requires tok
        refs <- newdata
        discard1 |> List.iter (snd >> strongDiscard)
        discard2 |> List.iter (snd >> snd >> strongDiscard)
        
    member al.TryPeekKeyValue(tok, key) = 
        // Returns the original key value as well since it may be different depending on equality test.
        let data = FilterAndHold(tok)
        TryPeekKeyValueImpl(data,key)
        
    member al.TryGetKeyValue(tok, key) = 
        let data = FilterAndHold(tok)
        let result,newdata = TryGetKeyValueImpl(data,key)
        AssignWithStrength(tok,newdata,[])
        result

    member al.TryGet(tok, key) = 
        let data = FilterAndHold(tok)
        let result,newdata = TryGetKeyValueImpl(data,key)
        AssignWithStrength(tok,newdata,[])
        match result with
        | Some(_,value) -> Some(value)
        | None -> None

    member al.Put(tok, key,value) = 
        let data = FilterAndHold(tok)
        let data,discard = if Exists(data,key) then RemoveImpl (data,key) else data,[]
        let data = Add(data,key,value)
        AssignWithStrength(tok,data,discard) // This will remove extras 

    member al.Remove(tok, key) = 
        let data = FilterAndHold(tok)
        let newdata,discard = RemoveImpl (data,key)
        AssignWithStrength(tok,newdata,discard)

    member al.Clear(tok) =
       let discards = FilterAndHold(tok)
       AssignWithStrength(tok,[], discards)

    member al.Resize(tok, newKeepStrongly, ?newKeepMax) =
       let newKeepMax = defaultArg newKeepMax 75 
       keepStrongly <- newKeepStrongly
       keepMax <- max newKeepStrongly newKeepMax
       do assert (onStrongDiscard.IsNone || keepStrongly = keepMax)
       let keep = FilterAndHold(tok)
       AssignWithStrength(tok,keep, [])

        

type internal MruCache<'Token, 'Key,'Value when 'Value : not struct>(keepStrongly, areSame, ?isStillValid : 'Key*'Value->bool, ?areSimilar, ?requiredToKeep, ?onStrongDiscard, ?keepMax) =
        
    /// Default behavior of <c>areSimilar</c> function is areSame.
    let areSimilar = defaultArg areSimilar areSame
        
    /// The list of items in the cache. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let cache = AgedLookup<'Token, 'Key,'Value>(keepStrongly=keepStrongly,areSimilar=areSimilar,?onStrongDiscard=onStrongDiscard,?keepMax=keepMax,?requiredToKeep=requiredToKeep)
        
    /// Whether or not this result value is still valid.
    let isStillValid = defaultArg isStillValid (fun _ -> true)
        
    member bc.ContainsSimilarKey(tok, key) = 
        match cache.TryPeekKeyValue(tok, key) with
        | Some(_similarKey, _value)-> true
        | None -> false
       
    member bc.TryGetAny(tok, key) = 
        match cache.TryPeekKeyValue(tok, key) with
        | Some(similarKey, value)->
            if areSame(similarKey,key) then Some(value)
            else None
        | None -> None
       
    member bc.TryGet(tok, key) = 
        match cache.TryGetKeyValue(tok, key) with
        | Some(similarKey, value) -> 
            if areSame(similarKey, key) && isStillValid(key,value) then Some value
            else None
        | None -> None
           
    member bc.Set(tok, key:'Key,value:'Value) = 
        cache.Put(tok, key,value)
       
    member bc.RemoveAnySimilar(tok, key) = 
        cache.Remove(tok, key)
       
    member bc.Clear(tok) =
        cache.Clear(tok)
        
    member bc.Resize(tok, newKeepStrongly, ?newKeepMax) =
        cache.Resize(tok, newKeepStrongly, ?newKeepMax=newKeepMax)
        
