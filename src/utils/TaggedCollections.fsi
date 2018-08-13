// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// This namespace contains FSharp.PowerPack extensions for the F# collection types
namespace Internal.Utilities.Collections.Tagged

    open System
    open System.Collections.Generic

    /// Immutable sets based on binary trees, default tag

    /// Immutable sets where a constraint tag carries information about the class of key-comparer being used.  
    [<Sealed>]
    type internal Set<'T,'ComparerTag> when 'ComparerTag :> IComparer<'T> =

        /// A useful shortcut for <c>Set.add</c>.  Note this operation produces a new set
        /// and does not mutate the original set.  The new set will share many storage
        /// nodes with the original.  See the <c>Set</c> module for further operations on sets.
        member Add : 'T -> Set<'T,'ComparerTag>
        
        /// A useful shortcut for <c>Set.remove</c>.  Note this operation produces a new set
        /// and does not mutate the original set.  The new set will share many storage
        /// nodes with the original.  See the <c>Set</c> module for further operations on sets.
        member Remove : 'T -> Set<'T,'ComparerTag>
        
        /// Return the number of elements in the set.
        member Count : int
        
        /// A useful shortcut for <c>Set.contains</c>.  See the <c>Set</c> module for further operations on sets.
        member Contains : 'T -> bool
        
        /// A useful shortcut for <c>Set.isEmpty</c>.  See the <c>Set</c> module for further operations on sets.
        member IsEmpty  : bool

        /// Apply the given function to each binding in the collection.
        member Iterate : ('T -> unit) -> unit

        /// Apply the given accumulating function to all the elements of the set.
        member Fold    : ('T -> 'State -> 'State) -> 'State -> 'State

        /// Build two new sets, one containing the elements for which the given predicate returns True,
        /// and another with the remaining elements.
        member Partition: predicate:('T -> bool) -> Set<'T,'ComparerTag> * Set<'T,'ComparerTag>

        /// Return a new collection containing only the elements of the collection
        /// for which the given predicate returns True.
        member Filter: predicate:('T -> bool) -> Set<'T,'ComparerTag> 

        /// Test if any element of the collection satisfies the given predicate.
        /// If the input function is f and the elements are i0...iN then computes 
        /// p i0 or ... or p iN.
        member Exists: predicate:('T -> bool) -> bool

        /// Test if all elements of the collection satisfy the given predicate.
        /// If the input function is f and the elements are i0...iN and j0...jN then 
        /// computes p i0 && ... && p iN.
        member ForAll: predicate:('T -> bool) -> bool

        /// A set based on the given comparer containing the given initial elements.
        static member Create: 'ComparerTag * seq<'T> -> Set<'T,'ComparerTag> 
        
        /// The empty set based on the given comparer.
        static member Empty: 'ComparerTag -> Set<'T,'ComparerTag> 
        
        /// A singleton set based on the given comparison operator.
        static member Singleton: 'ComparerTag * 'T -> Set<'T,'ComparerTag> 
        
        /// Compares two sets and returns True if they are equal or False otherwise.
        static member Equality : Set<'T,'ComparerTag> * Set<'T,'ComparerTag> -> bool
        
        /// Compares a and b and returns 1 if a > b, -1 if b < a and 0 if a = b.       
        static member Compare : a:Set<'T,'ComparerTag> * b:Set<'T,'ComparerTag> -> int

        /// Return a new set with the elements of the second set removed from the first.
        static member (-) : Set<'T,'ComparerTag> * Set<'T,'ComparerTag> -> Set<'T,'ComparerTag> 

        /// Compute the union of the two sets.
        static member (+) : Set<'T,'ComparerTag> * Set<'T,'ComparerTag> -> Set<'T,'ComparerTag> 

        /// Compute the intersection of the two sets.
        static member Intersection : Set<'T,'ComparerTag> * Set<'T,'ComparerTag> -> Set<'T,'ComparerTag> 

        /// Compute the union of the two sets.
        static member Union : Set<'T,'ComparerTag> * Set<'T,'ComparerTag> -> Set<'T,'ComparerTag>

        /// Return a new set with the elements of the second set removed from the first.
        static member Difference: Set<'T,'ComparerTag> * Set<'T,'ComparerTag> -> Set<'T,'ComparerTag> 

        /// The number of elements in the set.
        member Choose : 'T 

        /// Returns the lowest element in the set according to the ordering being used for the set.
        member MinimumElement: 'T

        /// Returns the highest element in the set according to the ordering being used for the set.
        member MaximumElement: 'T

        /// Evaluates to True if all elements of the second set are in the first.
        member IsSubsetOf: Set<'T,'ComparerTag> -> bool

        /// Evaluates to True if all elements of the first set are in the second.
        member IsSupersetOf: Set<'T,'ComparerTag> -> bool

        /// The elements of the set as a list.
        member ToList : unit -> 'T list
        
        /// The elements of the set as an array.
        member ToArray: unit -> 'T array 

        interface ICollection<'T> 

        interface IEnumerable<'T> 

        interface System.Collections.IEnumerable

        interface System.IComparable

        override Equals : obj -> bool

    type internal Set<'T> = Set<'T, IComparer<'T>>