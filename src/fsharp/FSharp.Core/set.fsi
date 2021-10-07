// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

/// <summary>Immutable sets based on binary trees, where elements are ordered by F# generic comparison. By default
/// comparison is the F# structural comparison function or uses implementations of the IComparable interface on element values.</summary>
///
/// <remarks>See the <see cref="T:Microsoft.FSharp.Collections.SetModule"/> module for further operations on sets.
///
/// All members of this class are thread-safe and may be used concurrently from multiple threads.</remarks>
[<Sealed>]
[<CompiledName("FSharpSet`1")>]
type Set<[<EqualityConditionalOn>]'T  when 'T : comparison> = 

    /// <summary>Create a set containing elements drawn from the given sequence.</summary>
    /// <param name="elements">The input sequence.</param>
    ///
    /// <returns>The result set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    new : elements:seq<'T> -> Set<'T> 

    /// <summary>A useful shortcut for Set.add. Note this operation produces a new set
    /// and does not mutate the original set. The new set will share many storage
    /// nodes with the original. See the Set module for further operations on sets.</summary>
    ///
    /// <param name="value">The value to add to the set.</param>
    ///
    /// <returns>The result set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member Add : value:'T -> Set<'T>
    
    /// <summary>A useful shortcut for Set.remove. Note this operation produces a new set
    /// and does not mutate the original set. The new set will share many storage
    /// nodes with the original. See the Set module for further operations on sets.</summary>
    ///
    /// <param name="value">The value to remove from the set.</param>
    ///
    /// <returns>The result set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member Remove : value:'T -> Set<'T>
    
    /// <summary>The number of elements in the set</summary>
    /// 
    /// <example-tbd></example-tbd>
    member Count : int
    
    /// <summary>A useful shortcut for Set.contains. See the Set module for further operations on sets.</summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns>True if the set contains <c>value</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member Contains : value:'T -> bool
    
    /// <summary>A useful shortcut for Set.isEmpty. See the Set module for further operations on sets.</summary>
    /// 
    /// <example-tbd></example-tbd>
    member IsEmpty  : bool

    /// <summary>Returns a new set with the elements of the second set removed from the first.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>A set containing elements of the first set that are not contained in the second set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    static member (-) : set1:Set<'T> * set2:Set<'T> -> Set<'T> 

    /// <summary>Compute the union of the two sets.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>The union of the two input sets.</returns>
    /// 
    /// <example-tbd></example-tbd>
    static member (+) : set1:Set<'T> * set2:Set<'T> -> Set<'T> 

    /// <summary>Evaluates to "true" if all elements of the first set are in the second.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a subset of <c>otherSet</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member IsSubsetOf: otherSet:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the first set are in the second, and at least 
    /// one element of the second is not in the first.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a proper subset of <c>otherSet</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member IsProperSubsetOf: otherSet:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a superset of <c>otherSet</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member IsSupersetOf: otherSet:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first, and at least 
    /// one element of the first is not in the second.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a proper superset of <c>otherSet</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    member IsProperSupersetOf: otherSet:Set<'T> -> bool

    /// <summary>Returns the lowest element in the set according to the ordering being used for the set.</summary>
    /// 
    /// <example-tbd></example-tbd>
    member MinimumElement: 'T

    /// <summary>Returns the highest element in the set according to the ordering being used for the set.</summary>
    /// 
    /// <example-tbd></example-tbd>
    member MaximumElement: 'T

    interface ICollection<'T> 
    interface IEnumerable<'T> 
    interface System.Collections.IEnumerable 
    interface System.IComparable
    interface IReadOnlyCollection<'T>
    override Equals : obj -> bool

namespace Microsoft.FSharp.Collections
    
open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

/// <summary>Contains operations for working with values of type <see cref="T:Microsoft.FSharp.Collections.Set`1"/>.</summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Set = 

    /// <summary>The empty set for the type 'T.</summary>
    /// 
    /// <example id="empty-1">
    /// <code lang="fsharp">
    /// Set.empty&lt;int&gt;
    /// </code>
    /// Evaluates to <c>set [ ]</c>.
    /// </example>
    [<GeneralizableValue>]
    [<CompiledName("Empty")>]
    val empty<'T> : Set<'T> when 'T : comparison

    /// <summary>The set containing the given element.</summary>
    ///
    /// <param name="value">The value for the set to contain.</param>
    ///
    /// <returns>The set containing <c>value</c>.</returns>
    /// 
    /// <example id="singleton-1">
    /// <code lang="fsharp">
    /// Set.singleton 7
    /// </code>
    /// Evaluates to <c>set [ 7 ]</c>.
    /// </example>
    [<CompiledName("Singleton")>]
    val singleton: value:'T -> Set<'T>

    /// <summary>Returns a new set with an element added to the set. No exception is raised if
    /// the set already contains the given element.</summary>
    ///
    /// <param name="value">The value to add.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>A new set containing <c>value</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Add")>]
    val add: value:'T -> set:Set<'T> -> Set<'T>

    /// <summary>Evaluates to "true" if the given element is in the given set.</summary>
    ///
    /// <param name="element">The element to test.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if <c>element</c> is in <c>set</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Contains")>]
    val contains: element:'T -> set:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the first set are in the second</summary>
    ///
    /// <param name="set1">The potential subset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a subset of <c>set2</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IsSubset")>]
    val isSubset: set1: Set<'T> -> set2:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the first set are in the second, and at least 
    /// one element of the second is not in the first.</summary>
    ///
    /// <param name="set1">The potential subset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a proper subset of <c>set2</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IsProperSubset")>]
    val isProperSubset: set1: Set<'T> -> set2:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first.</summary>
    ///
    /// <param name="set1">The potential superset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a superset of <c>set2</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IsSuperset")>]
    val isSuperset: set1: Set<'T> -> set2:Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first, and at least 
    /// one element of the first is not in the second.</summary>
    ///
    /// <param name="set1">The potential superset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a proper superset of <c>set2</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IsProperSuperset")>]
    val isProperSuperset: set1: Set<'T> -> set2:Set<'T> -> bool


    /// <summary>Returns the number of elements in the set. Same as <c>size</c>.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The number of elements in the set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Count")>]
    val count: set:Set<'T> -> int

    /// <summary>Tests if any element of the collection satisfies the given predicate.
    /// If the input function is <c>predicate</c> and the elements are <c>i0...iN</c> 
    /// then computes <c>p i0 or ... or p iN</c>.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if any element of <c>set</c> satisfies <c>predicate</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Exists")>]
    val exists: predicate:('T -> bool) -> set:Set<'T> -> bool

    /// <summary>Returns a new collection containing only the elements of the collection
    /// for which the given predicate returns True.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The set containing only the elements for which <c>predicate</c> returns true.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Filter")>]
    val filter: predicate:('T -> bool) -> set:Set<'T> -> Set<'T>

    /// <summary>Returns a new collection containing the results of applying the
    /// given function to each element of the input set.</summary>
    ///
    /// <param name="mapping">The function to transform elements of the input set.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>A set containing the transformed elements.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Map")>]
    val map: mapping:('T -> 'U) -> set:Set<'T> -> Set<'U>

    /// <summary>Applies the given accumulating function to all the elements of the set</summary>
    ///
    /// <param name="folder">The accumulating function.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The final state.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Fold")>]
    val fold<'T,'State> : folder:('State -> 'T -> 'State) -> state:'State -> set:Set<'T> -> 'State when 'T : comparison

    /// <summary>Applies the given accumulating function to all the elements of the set.</summary>
    ///
    /// <param name="folder">The accumulating function.</param>
    /// <param name="set">The input set.</param>
    /// <param name="state">The initial state.</param>
    ///
    /// <returns>The final state.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("FoldBack")>]
    val foldBack<'T,'State> : folder:('T -> 'State -> 'State) -> set:Set<'T> -> state:'State -> 'State when 'T : comparison

    /// <summary>Tests if all elements of the collection satisfy the given predicate.
    /// If the input function is <c>f</c> and the elements are <c>i0...iN</c> and "j0...jN"
    /// then computes <c>p i0 &amp;&amp; ... &amp;&amp; p iN</c>.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if all elements of <c>set</c> satisfy <c>predicate</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ForAll")>]
    val forall: predicate:('T -> bool) -> set:Set<'T> -> bool

    /// <summary>Computes the intersection of the two sets.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>The intersection of <c>set1</c> and <c>set2</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Intersect")>]
    val intersect: set1:Set<'T> -> set2:Set<'T> -> Set<'T>

    /// <summary>Computes the intersection of a sequence of sets. The sequence must be non-empty.</summary>
    ///
    /// <param name="sets">The sequence of sets to intersect.</param>
    ///
    /// <returns>The intersection of the input sets.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IntersectMany")>]
    val intersectMany: sets:seq<Set<'T>> -> Set<'T>

    /// <summary>Computes the union of the two sets.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>The union of <c>set1</c> and <c>set2</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Union")>]
    val union: set1:Set<'T> -> set2:Set<'T> -> Set<'T>

    /// <summary>Computes the union of a sequence of sets.</summary>
    ///
    /// <param name="sets">The sequence of sets to union.</param>
    ///
    /// <returns>The union of the input sets.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("UnionMany")>]
    val unionMany: sets:seq<Set<'T>> -> Set<'T>

    /// <summary>Returns "true" if the set is empty.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if <c>set</c> is empty.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IsEmpty")>]
    val isEmpty: set:Set<'T> -> bool

    /// <summary>Applies the given function to each element of the set, in order according
    /// to the comparison function.</summary>
    ///
    /// <param name="action">The function to apply to each element.</param>
    /// <param name="set">The input set.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Iterate")>]
    val iter: action:('T -> unit) -> set:Set<'T> -> unit

    /// <summary>Splits the set into two sets containing the elements for which the given predicate
    /// returns true and false respectively.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>A pair of sets with the first containing the elements for which <c>predicate</c> returns
    /// true and the second containing the elements for which <c>predicate</c> returns false.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Partition")>]
    val partition: predicate:('T -> bool) -> set:Set<'T> -> (Set<'T> * Set<'T>)

    /// <summary>Returns a new set with the given element removed. No exception is raised if 
    /// the set doesn't contain the given element.</summary>
    ///
    /// <param name="value">The element to remove.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The input set with <c>value</c> removed.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Remove")>]
    val remove: value: 'T -> set:Set<'T> -> Set<'T>

    /// <summary>Returns the lowest element in the set according to the ordering being used for the set.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The min value from the set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("MinElement")>]
    val minElement: set:Set<'T> -> 'T

    /// <summary>Returns the highest element in the set according to the ordering being used for the set.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The max value from the set.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("MaxElement")>]
    val maxElement: set:Set<'T> -> 'T

    /// <summary>Builds a set that contains the same elements as the given list.</summary>
    ///
    /// <param name="elements">The input list.</param>
    ///
    /// <returns>A set containing the elements form the input list.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("OfList")>]
    val ofList: elements:'T list -> Set<'T>

    /// <summary>Builds a list that contains the elements of the set in order.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>An ordered list of the elements of <c>set</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToList")>]
    val toList: set:Set<'T> -> 'T list

    /// <summary>Builds a set that contains the same elements as the given array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>A set containing the elements of <c>array</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("OfArray")>]
    val ofArray: array:'T[] -> Set<'T>

    /// <summary>Builds an array that contains the elements of the set in order.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>An ordered array of the elements of <c>set</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToArray")>]
    val toArray: set:Set<'T> -> 'T[]

    /// <summary>Returns an ordered view of the collection as an enumerable object.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>An ordered sequence of the elements of <c>set</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToSeq")>]
    val toSeq: set:Set<'T> -> seq<'T>

    /// <summary>Builds a new collection from the given enumerable object.</summary>
    ///
    /// <param name="elements">The input sequence.</param>
    ///
    /// <returns>The set containing <c>elements</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("OfSeq")>]
    val ofSeq: elements:seq<'T> -> Set<'T>

    /// <summary>Returns a new set with the elements of the second set removed from the first.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The set whose elements will be removed from <c>set1</c>.</param>
    ///
    /// <returns>The set with the elements of <c>set2</c> removed from <c>set1</c>.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Difference")>]
    val difference: set1:Set<'T> -> set2:Set<'T> -> Set<'T>
