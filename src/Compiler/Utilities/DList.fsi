// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections

/// Difference list with O(1) append. Optimized for append-heavy workloads where two DLists are frequently combined.
/// Provides lazy materialization for iteration operations.
[<Sealed>]
type internal CachedDList<'T> =

    interface System.Collections.IEnumerable

    interface System.Collections.Generic.IEnumerable<'T>

    /// Create from a list
    new: xs: 'T list -> CachedDList<'T>

    /// Append a single element (O(1))
    member AppendOne: y: 'T -> CachedDList<'T>

    /// Append a sequence of elements
    member Append: ys: seq<'T> -> CachedDList<'T>

    /// Convert to list (forces materialization if not already cached)
    member ToList: unit -> 'T list

    /// Get first elements (for compatibility)
    member FirstElements: 'T list

    /// Get last elements (for compatibility)
    member LastElements: 'T list

    /// Get the length of the list
    member Length: int

    /// Empty DList
    static member Empty: CachedDList<'T>

module internal CachedDList =

    /// Empty DList
    val empty<'T> : CachedDList<'T>

    /// Create from a sequence
    val ofSeq: x: seq<'a> -> CachedDList<'a>

    /// Create from a list
    val ofList: x: 'a list -> CachedDList<'a>

    /// Convert to list
    val toList: x: CachedDList<'a> -> 'a list

    /// Create a DList with one element
    val one: x: 'a -> CachedDList<'a>

    /// Append a single element
    val appendOne: x: CachedDList<'a> -> y: 'a -> CachedDList<'a>

    /// Append two DLists (O(1) operation)
    val append: x: CachedDList<'a> -> ys: CachedDList<'a> -> CachedDList<'a>

    /// Iterate over elements
    val iter: f: ('a -> unit) -> x: CachedDList<'a> -> unit

    /// Map over elements
    val map: f: ('a -> 'b) -> x: CachedDList<'a> -> CachedDList<'b>

    /// Check if any element satisfies predicate
    val exists: f: ('a -> bool) -> x: CachedDList<'a> -> bool

    /// Check if all elements satisfy predicate
    val forall: f: ('a -> bool) -> x: CachedDList<'a> -> bool

    /// Filter elements
    val filter: f: ('a -> bool) -> x: CachedDList<'a> -> CachedDList<'a>

    /// Fold back over elements
    val foldBack: f: ('a -> 'b -> 'b) -> x: CachedDList<'a> -> acc: 'b -> 'b

    /// Try to find an element
    val tryFind: f: ('a -> bool) -> x: CachedDList<'a> -> 'a option
