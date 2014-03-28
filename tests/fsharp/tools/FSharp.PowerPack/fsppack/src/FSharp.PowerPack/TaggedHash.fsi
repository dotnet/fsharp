namespace Microsoft.FSharp.Collections.Tagged

    open System
    open System.Collections.Generic

    /// HashMultiMap, but where a constraint tag tracks information about the hash/equality functions used
    /// for the hashing. When the tag is Tags.StructuralHash this is identical to HashMultiMap.
    [<Sealed>]
    type HashMultiMap<'Key,'Value,'HashTag> when 'HashTag :> IEqualityComparer<'Key> =
        /// Create a new empty mutable hash table with an internal bucket array of the given approximate size
        /// and with the given key hash/equality functions
        static member Create: 'HashTag * int             -> HashMultiMap<'Key,'Value,'HashTag>

        /// Make a shallow copy of the collection
        member Copy    : unit    -> HashMultiMap<'Key,'Value,'HashTag>

        /// Add a binding for the element to the table
        member Add     : 'Key * 'Value -> unit

        /// Clear all elements from the collection
        member Clear   : unit    -> unit

        /// Test if the collection contains any bindings for the given element
        [<System.Obsolete("This member has been renamed to ContainsKey")>]
        member Contains: 'Key      -> bool

        /// Test if the collection contains any bindings for the given element
        member ContainsKey: 'Key      -> bool

        /// Remove the latest binding (if any) for the given element from the table
        member Remove  : 'Key      -> unit

        /// Replace the latest binding (if any) for the given element.
        member Replace : 'Key * 'Value -> unit

        /// Lookup or set the given element in the table.  Raise <c>KeyNotFoundException</c> if the element is not found.
        member Item : 'Key -> 'Value with get,set

        /// Lookup the given element in the table, returning the result as an Option
        member TryFind : 'Key      -> 'Value option
        /// Find all bindings for the given element in the table, if any
        member FindAll : 'Key      -> 'Value list

        /// Apply the given function to each element in the collection threading the accumulating parameter
        /// through the sequence of function applications
        member Fold    : ('Key -> 'Value -> 'c -> 'c) -> 'c -> 'c

        /// The number of bindings in the hash table
        member Count   : int

        /// Apply the given function to each binding in the hash table 
        member Iterate : ('Key -> 'Value -> unit) -> unit

    type HashMultiMap<'Key,'Value> = HashMultiMap<'Key,'Value, IEqualityComparer<'Key>>    

    /// Mutable hash sets where a constraint tag tracks information about the hash/equality functions used
    /// for the hashing. When the tag is Tags.StructuralHash this is identical to HashSet.
    [<Sealed>]
    type HashSet<'T,'HashTag> when 'T : equality and 'HashTag :> IEqualityComparer<'T>  =
        /// Create a new empty mutable hash set with an internal bucket array of the given approximate size
        /// and with the given key hash/equality functions 
        static member Create: 'HashTag * int             -> HashSet<'T,'HashTag>

        /// Make a shallow copy of the set
        member Copy    : unit -> HashSet<'T,'HashTag>
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
        
        /// The number of elements in the set
        member Count   : int

        /// Apply the given function to each binding in the hash table 
        member Iterate : ('T -> unit) -> unit

        interface IEnumerable<'T> 
        interface System.Collections.IEnumerable 

    type HashSet<'T when 'T : equality> = HashSet<'T, IEqualityComparer<'T>>    
