// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections
  
/// Simple aging lookup table. When a member is accessed it's
/// moved to the top of the list and when there are too many elements
/// the least-recently-accessed element falls of the end.  
///
///  - areSimilar: Keep at most once association for two similar keys (as given by areSimilar)
type internal AgedLookup<'Token, 'Key, 'Value when 'Value : not struct> = 
    new : keepStrongly:int
            * areSimilar:('Key * 'Key -> bool) 
            * ?requiredToKeep:('Value -> bool)
            * ?keepMax: int
            -> AgedLookup<'Token,'Key,'Value>

    /// Lookup the value without making it the most recent.
    /// Returns the original key value because the areSame function
    /// may have unified two different keys.
    member TryPeekKeyValue : 'Token * key:'Key -> ('Key*'Value) option

    /// Lookup a value and make it the most recent.
    /// Returns the original key value because the areSame function
    /// may have unified two different keys.
    member TryGetKeyValue : 'Token * key: 'Key -> ('Key*'Value) option    

    /// Lookup a value and make it the most recent. Return <c>None</c> if it wasn't there.
    member TryGet : 'Token * key:'Key -> 'Value option        

    /// Add an element to the collection. Make it the most recent.
    member Put : 'Token * 'Key * 'Value -> unit

    /// Remove the given value from the collection.
    member Remove : 'Token * key:'Key -> unit

    /// Remove all elements.
    member Clear : 'Token -> unit

    /// Resize
    member Resize : 'Token * newKeepStrongly: int * ?newKeepMax : int -> unit

    /// Keys
    member Keys : 'Token -> 'Key list

/// Simple priority caching for a small number of key/value associations.
/// This cache may age-out results that have been Set by the caller.
/// Because of this, the caller must be able to tolerate values 
/// that aren't what was originally passed to the Set function.     
///
/// Concurrency: This collection is thread-safe, though concurrent use may result in different
/// threads seeing different live sets of cached items. 
///
///  - areSimilar: Keep at most once association for two similar keys (as given by areSimilar)
type internal MruCache<'Token, 'Key,'Value when 'Value : not struct> =
    new : keepStrongly:int 
            * areSame:('Key * 'Key -> bool) 
            * ?isStillValid:('Key * 'Value -> bool)
            * ?areSimilar:('Key * 'Key -> bool) 
            * ?requiredToKeep:('Value -> bool)
            * ?keepMax:int
            -> MruCache<'Token,'Key,'Value>

    /// Clear out the cache.
    member Clear : 'Token -> unit

    /// Get the similar (subsumable) value for the given key or <c>None</c> if not already available.
    member ContainsSimilarKey : 'Token * key:'Key -> bool

    /// Get the value for the given key or <c>None</c> if not still valid.
    member TryGetAny : 'Token * key:'Key -> 'Value option

    /// Get the value for the given key or None, but only if entry is still valid
    member TryGet : 'Token * key:'Key -> 'Value option

    /// Get the value for the given key or <c>None</c> if not still valid. Skips `areSame` checking unless `areSimilar` is not provided.
    member TryGetSimilarAny : 'Token * key:'Key -> 'Value option

    /// Get the value for the given key or None, but only if entry is still valid. Skips `areSame` checking unless `areSimilar` is not provided.
    member TryGetSimilar : 'Token * key:'Key -> 'Value option

    /// Remove the given value from the mru cache.
    member RemoveAnySimilar : 'Token * key:'Key -> unit

    /// Set the given key. 
    member Set : 'Token * key:'Key * value:'Value -> unit

    /// Resize
    member Resize : 'Token * newKeepStrongly: int * ?newKeepMax : int -> unit

    /// Keys
    member Keys : 'Token -> 'Key list
