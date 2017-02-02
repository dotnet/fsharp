// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections
  
  /// Simple aging lookup table. When a member is accessed it's
  /// moved to the top of the list and when there are too many elements
  /// the least-recently-accessed element falls of the end.
  type internal AgedLookup<'TThreadToken, 'TKey,'TValue when 'TValue : not struct> = 
    new : keepStrongly:int
            * areSame:('TKey * 'TKey -> bool) 
            * ?requiredToKeep:('TValue -> bool)
            * ?onStrongDiscard : ('TValue -> unit) // this may only be set if keepTotal=keepStrongly, i.e. not weak entries
            * ?keepMax: int
            -> AgedLookup<'TThreadToken,'TKey,'TValue>
    /// Lookup the value without making it the most recent.
    /// Returns the original key value because the areSame function
    /// may have unified two different keys.
    member TryPeekKeyValue : 'TThreadToken * key:'TKey -> ('TKey*'TValue) option
    /// Lookup a value and make it the most recent.
    /// Returns the original key value because the areSame function
    /// may have unified two different keys.
    member TryGetKeyValue : 'TThreadToken * key: 'TKey -> ('TKey*'TValue) option    
    /// Lookup a value and make it the most recent. Return <c>None</c> if it wasn't there.
    member TryGet : 'TThreadToken * key:'TKey -> 'TValue option        
    /// Add an element to the collection. Make it the most recent.
    member Put : 'TThreadToken * 'TKey * 'TValue -> unit
    /// Remove the given value from the collection.
    member Remove : 'TThreadToken * key:'TKey -> unit
    /// Remove all elements.
    member Clear : 'TThreadToken -> unit
    /// Resize
    member Resize : 'TThreadToken * keepStrongly: int * ?keepMax : int -> unit
    
  /// Simple priority caching for a small number of key/value associations.
  /// This cache may age-out results that have been Set by the caller.
  /// Because of this, the caller must be able to tolerate values 
  /// that aren't what was originally passed to the Set function.         
  type internal MruCache<'TThreadToken, 'TKey,'TValue when 'TValue : not struct> =
    new : keepStrongly:int 
            * areSame:('TKey * 'TKey -> bool) 
            * ?isStillValid:('TKey * 'TValue -> bool)
            * ?areSameForSubsumption:('TKey * 'TKey -> bool) 
            * ?requiredToKeep:('TValue -> bool)
            * ?onDiscard:('TValue -> unit)
            * ?keepMax:int
            -> MruCache<'TThreadToken,'TKey,'TValue>
    /// Clear out the cache.
    member Clear : 'TThreadToken -> unit
    /// Get the value for the given key or <c>None</c> if not already available.
    member TryGetAny : 'TThreadToken * key:'TKey -> 'TValue option
    /// Get the value for the given key or None if not already available
    member TryGet : 'TThreadToken * key:'TKey -> 'TValue option
    /// Remove the given value from the mru cache.
    member Remove : 'TThreadToken * key:'TKey -> unit
    /// Set the given key. 
    member Set : 'TThreadToken * key:'TKey * value:'TValue -> unit
    /// Resize
    member Resize : 'TThreadToken * keepStrongly: int * ?keepMax : int -> unit

  [<Sealed>]
  type internal List = 
    /// Return a new list with one element for each unique 'TKey. Multiple 'TValues are flattened. 
    /// The original order of the first instance of 'TKey is preserved.
    static member groupByFirst : l:('TKey * 'TValue) list -> ('TKey * 'TValue list) list when 'TKey : equality
    /// Return each distinct item in the list using reference equality.
    static member referenceDistinct : 'T list -> 'T list when 'T : not struct
