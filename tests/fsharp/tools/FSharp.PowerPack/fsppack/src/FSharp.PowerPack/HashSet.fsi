namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic

/// Mutable hash sets based by default on F# structural "hash" and (=) functions. Implemented via a hash table and/or Dictionary.
[<Sealed>]
[<Obsolete("The HashSet<_> type from the F# Power Pack is now deprecated. Use the System.Collections.Generic.HashSet<_> type from System.Core.dll instead.")>]
type HashSet<'T>  =

    /// Create a new empty mutable hash set using the given key hash/equality functions 
    new : comparer:IEqualityComparer<'T> -> HashSet<'T>

    /// Create a new empty mutable hash set with an internal bucket array of the given approximate size
    /// and with the given key hash/equality functions 
    new : size:int * comparer:IEqualityComparer<'T> -> HashSet<'T>

    /// Create a new mutable hash set with the given elements and using the given key hash/equality functions 
    new : elements:seq<'T> * comparer:IEqualityComparer<'T>  -> HashSet<'T>
    
    /// Make a shallow copy of the set
    member Copy    : unit -> HashSet<'T>
    
    /// Add an element to the collection
    member Add     : 'T   -> unit
    
    /// Clear all elements from the set
    member Clear   : unit -> unit
    
    /// Test if the set contains the given element
    member Contains: 'T   -> bool
    
    /// Remove the given element from the set
    member Remove  : 'T   -> unit
    
    /// Apply the given function to the set threading the accumulating parameter
    /// through the sequence of function applications
    member Fold    : ('T -> 'State -> 'State) -> 'State -> 'State

    /// The total number of elements in the set
    member Count   : int

    /// Apply the given function to each binding in the hash table 
    member Iterate : ('T -> unit) -> unit

    interface IEnumerable<'T> 
    interface System.Collections.IEnumerable 

    [<System.Obsolete("This member has been redesigned. Use 'new HashSet<_>(HashIdentity.Structural) to create a HashSet using F# generic hashing and equality", true)>]
    new : unit -> HashSet<'T>

    [<System.Obsolete("This member has been redesigned. Use 'new HashSet<_>(size, HashIdentity.Structural) to create a HashSet using F# generic hashing and equality", true)>]
    new : size:int -> HashSet<'T>

    [<System.Obsolete("This member has been redesigned. Use 'new HashSet<_>(elements, HashIdentity.Structural) to create a HashSet using F# generic hashing and equality", true)>]
    new : elements:seq<'T> -> HashSet<'T>

