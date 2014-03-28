/// This namespace contains Test.PowerPack extensions for the F# collection types
namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic


/// Hash tables, by default based on F# structural "hash" and (=) functions. 
/// The table may map a single key to multiple bindings.
[<Sealed>]
type HashMultiMap<'Key,'Value> =
    /// Create a new empty mutable HashMultiMap with the given key hash/equality functions
    new : comparer:IEqualityComparer<'Key> -> HashMultiMap<'Key,'Value>
    
    /// Create a new empty mutable HashMultiMap with an internal bucket array of the given approximate size
    /// and with the given key hash/equality functions
    new : size:int * comparer:IEqualityComparer<'Key> -> HashMultiMap<'Key,'Value>
    
    /// Build a map that contains the bindings of the given IEnumerable
    new : entries:seq<'Key * 'Value> * comparer:IEqualityComparer<'Key> -> HashMultiMap<'Key,'Value>

    /// Make a shallow copy of the collection
    member Copy    : unit    -> HashMultiMap<'Key,'Value>
    
    /// Add a binding for the element to the table
    member Add     : 'Key * 'Value -> unit
    
    /// Clear all elements from the collection
    member Clear   : unit    -> unit
    
    /// Test if the collection contains any bindings for the given element
    member ContainsKey: 'Key -> bool

    /// Remove the latest binding (if any) for the given element from the table
    member Remove : 'Key -> unit

    /// Replace the latest binding (if any) for the given element.
    member Replace : 'Key * 'Value -> unit

    /// Lookup or set the given element in the table. Set replaces all existing bindings for a value with a single
    /// bindings. Raise <c>KeyNotFoundException</c> if the element is not found.
    member Item : 'Key -> 'Value with get,set

    /// Lookup the given element in the table, returning the result as an Option
    member TryFind : 'Key      -> 'Value option
    
    /// Find all bindings for the given element in the table, if any
    member FindAll : 'Key      -> 'Value list

    /// Apply the given function to each element in the collection threading the accumulating parameter
    /// through the sequence of function applications
    member Fold    : ('Key -> 'Value -> 'State -> 'State) -> 'State -> 'State

    /// The total number of keys in the hash table
    member Count   : int

    ///Apply the given function to each binding in the hash table 
    member Iterate : ('Key -> 'Value -> unit) -> unit

    interface IDictionary<'Key, 'Value>         
    interface ICollection<KeyValuePair<'Key, 'Value>> 
    interface IEnumerable<KeyValuePair<'Key, 'Value>>         
    interface System.Collections.IEnumerable 
    
    [<System.Obsolete("This member has been redesigned. Use 'new HashMultiMap<_,_>(HashIdentity.Structural) instead to create a HashMultiMap using F# generic hashing and equality", true)>]
    new : unit -> HashMultiMap<'Key,'Value>

    [<System.Obsolete("This member has been redesigned. Use 'new HashMultiMap<_,_>(size, HashIdentity.Structural) instead to create a HashMultiMap using F# generic hashing and equality", true)>]
    new : size:int -> HashMultiMap<'Key,'Value>

    [<System.Obsolete("This member has been redesigned. Use 'new HashMultiMap<_,_>(entries, HashIdentity.Structural) instead to create a HashMultiMap using F# generic hashing and equality", true)>]
    new : entries:seq<'Key * 'Value> -> HashMultiMap<'Key,'Value>


    [<System.Obsolete("This member has been redesigned. Use 'new HashMultiMap<_,_>(HashIdentity.Structural) instead to create a HashMultiMap using F# generic hashing and equality", true)>]
    static member Create : unit -> HashMultiMap<'Key,'Value>

    [<System.Obsolete("This member has been redesigned. Use 'new HashMultiMap<_,_>(size, HashIdentity.Structural) instead to create a HashMultiMap using F# generic hashing and equality", true)>]
    static member Create : size:int -> HashMultiMap<'Key,'Value>

    [<System.Obsolete("This member has been redesigned. Use 'new HashMultiMap<_,_>(entries, HashIdentity.Structural) instead to create a HashMultiMap using F# generic hashing and equality", true)>]
    static member Create : entries:seq<'Key * 'Value> -> HashMultiMap<'Key,'Value>
