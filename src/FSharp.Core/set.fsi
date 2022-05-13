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
type Set<[<EqualityConditionalOn>] 'T when 'T: comparison> =

    /// <summary>Create a set containing elements drawn from the given sequence.</summary>
    /// <param name="elements">The input sequence.</param>
    ///
    /// <returns>The result set.</returns>
    ///
    /// <example id="set-new">
    /// <code lang="fsharp">
    /// let sequenceOfNumbers = seq { 1 .. 3 }
    /// let numbersInSet = Set(sequenceOfNumbers)
    /// printfn $"The set is {numbersInSet}"
    /// </code>
    /// </example>
    /// Creates a new Set containing the elements of the given sequence. <c> set [1; 2; 3]</c>
    new: elements: seq<'T> -> Set<'T>

    /// <summary>A useful shortcut for Set.add. Note this operation produces a new set
    /// and does not mutate the original set. The new set will share many storage
    /// nodes with the original. See the Set module for further operations on sets.</summary>
    ///
    /// <param name="value">The value to add to the set.</param>
    ///
    /// <returns>The result set.</returns>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(1).Add(2)
    /// printfn $"The new set is: {set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The new set is: set [1; 2]</c>
    /// </example>
    member Add: value: 'T -> Set<'T>

    /// <summary>A useful shortcut for Set.remove. Note this operation produces a new set
    /// and does not mutate the original set. The new set will share many storage
    /// nodes with the original. See the Set module for further operations on sets.</summary>
    ///
    /// <param name="value">The value to remove from the set.</param>
    ///
    /// <returns>The result set.</returns>
    ///
    /// <example id="set-remove">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(1).Add(2)
    /// printfn $"The new set is: {set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The new set is: set [2]</c>
    /// </example>
    member Remove: value: 'T -> Set<'T>

    /// <summary>The number of elements in the set</summary>
    ///
    /// <example id="set-count">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(1).Add(2)
    /// printfn $"The new set is: {set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set has 3 elements</c>
    /// </example>
    member Count: int

    /// <summary>A useful shortcut for Set.contains. See the Set module for further operations on sets.</summary>
    ///
    /// <param name="value">The value to check.</param>
    ///
    /// <returns>True if the set contains <c>value</c>.</returns>
    ///
    /// <example id="set-contains">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(2).Add(3)
    /// printfn $"Does the set contain 1? {set.Contains(1)}"
    /// </code>
    /// The sample evaluates to the following output: <c>Does the set contain 1? false</c>
    /// </example>
    member Contains: value: 'T -> bool

    /// <summary>A useful shortcut for Set.isEmpty. See the Set module for further operations on sets.</summary>
    ///
    /// <example id="set-isempty">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(2).Add(3)
    /// printfn $"Is the set empty? {set.IsEmpty}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is the set empty? false</c>
    /// </example>
    member IsEmpty: bool

    /// <summary>Returns a new set with the elements of the second set removed from the first.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>A set containing elements of the first set that are not contained in the second set.</returns>
    ///
    /// <example id="set-subtract">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(2).Add(3).Add(4)
    /// printfn $"The new set is: {set1 - set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>The new set is: set [1]</c>
    /// </example>
    static member (-): set1: Set<'T> * set2: Set<'T> -> Set<'T>

    /// <summary>Compute the union of the two sets.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>The union of the two input sets.</returns>
    ///
    /// <example id="set-add">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(2).Add(3).Add(4)
    /// printfn $"Output is %A" {set1 + set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>The new set is: set [1; 2; 3; 4]</c>
    /// </example>
    static member (+): set1: Set<'T> * set2: Set<'T> -> Set<'T>

    /// <summary>Evaluates to "true" if all elements of the first set are in the second.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a subset of <c>otherSet</c>.</returns>
    ///
    /// <example id="set-issubsetof">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a subset of {set2}? {Set.isSubset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a subset of set [1; 2; 3; 4]? true</c>
    /// </example>
    member IsSubsetOf: otherSet: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the first set are in the second, and at least
    /// one element of the second is not in the first.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a proper subset of <c>otherSet</c>.</returns>
    ///
    /// <example id="set-ispropersubsetof">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a proper superset of {set2}? {Set.isProperSuperset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a proper subset of set [1; 2; 3; 4]? true</c>
    /// </example>
    member IsProperSubsetOf: otherSet: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a superset of <c>otherSet</c>.</returns>
    ///
    /// <example id="set-issupersetof">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a superset of {set2}? {Set.isSuperset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a superset of set [1; 2; 3; 4]? false</c>
    /// </example>
    member IsSupersetOf: otherSet: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first, and at least
    /// one element of the first is not in the second.</summary>
    ///
    /// <param name="otherSet">The set to test against.</param>
    ///
    /// <returns>True if this set is a proper superset of <c>otherSet</c>.</returns>
    ///
    /// <example id="set-ispropersupersetof">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a proper superset of {set2}? {Set.isProperSuperset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a proper superset of set [1; 2; 3; 4]? false</c>
    /// </example>
    member IsProperSupersetOf: otherSet: Set<'T> -> bool

    /// <summary>Returns the lowest element in the set according to the ordering being used for the set.</summary>
    ///
    /// <example id="set-minimumelement">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"MinimumElement: {set.MinimumElement}"
    /// </code>
    /// The sample evaluates to the following output: <c>MinimumElement: 1</c>
    /// </example>
    member MinimumElement: 'T

    /// <summary>Returns the highest element in the set according to the ordering being used for the set.</summary>
    ///
    /// <example id="set-maximumelement">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"MaximumElement: {set.MaximumElement}"
    /// </code>
    /// The sample evaluates to the following output: <c>MaximumElement: 3</c>
    /// </example>
    member MaximumElement: 'T

    interface ICollection<'T>
    interface IEnumerable<'T>
    interface System.Collections.IEnumerable
    interface System.IComparable
    interface IReadOnlyCollection<'T>
    override Equals: obj -> bool

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

/// <summary>Contains operations for working with values of type <see cref="T:Microsoft.FSharp.Collections.FSharpSet`1"/>.</summary>
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
    val empty<'T> : Set<'T> when 'T: comparison

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
    val singleton: value: 'T -> Set<'T>

    /// <summary>Returns a new set with an element added to the set. No exception is raised if
    /// the set already contains the given element.</summary>
    ///
    /// <param name="value">The value to add.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>A new set containing <c>value</c>.</returns>
    ///
    /// <example id="set-add">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(1).Add(2)
    /// printfn $"The new set is: {set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The new set is: set [1; 2]</c>
    /// </example>
    [<CompiledName("Add")>]
    val add: value: 'T -> set: Set<'T> -> Set<'T>

    /// <summary>Evaluates to "true" if the given element is in the given set.</summary>
    ///
    /// <param name="element">The element to test.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if <c>element</c> is in <c>set</c>.</returns>
    ///
    /// <example id="set-contains">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(2).Add(3)
    /// printfn $"Does the set contain 1? {set.Contains(1))}"
    /// </code>
    /// The sample evaluates to the following output: <c>Does the set contain 1? false</c>
    /// </example>
    [<CompiledName("Contains")>]
    val contains: element: 'T -> set: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the first set are in the second</summary>
    ///
    /// <param name="set1">The potential subset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a subset of <c>set2</c>.</returns>
    ///
    /// <example id="set-issubset">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a subset of {set2}? {Set.isSubset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a subset of set [1; 2; 3; 4]? true</c>
    /// </example>
    [<CompiledName("IsSubset")>]
    val isSubset: set1: Set<'T> -> set2: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the first set are in the second, and at least
    /// one element of the second is not in the first.</summary>
    ///
    /// <param name="set1">The potential subset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a proper subset of <c>set2</c>.</returns>
    ///
    /// <example id="set-ispropersubset">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a proper subset of {set2}? {Set.isProperSubset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a proper subset of set [1; 2; 3; 4]? true</c>
    /// </example>
    [<CompiledName("IsProperSubset")>]
    val isProperSubset: set1: Set<'T> -> set2: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first.</summary>
    ///
    /// <param name="set1">The potential superset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a superset of <c>set2</c>.</returns>
    ///
    /// <example id="set-issuperset">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a superset of {set2}? {Set.isSuperset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a superset of set [1; 2; 3; 4]? false</c>
    /// </example>
    [<CompiledName("IsSuperset")>]
    val isSuperset: set1: Set<'T> -> set2: Set<'T> -> bool

    /// <summary>Evaluates to "true" if all elements of the second set are in the first, and at least
    /// one element of the first is not in the second.</summary>
    ///
    /// <param name="set1">The potential superset.</param>
    /// <param name="set2">The set to test against.</param>
    ///
    /// <returns>True if <c>set1</c> is a proper superset of <c>set2</c>.</returns>
    ///
    /// <example id="set-ispropersuperset">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"Is {set1} a proper superset of {set2}? {Set.isProperSuperset set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is set [1; 2; 3] a proper superset of set [1; 2; 3; 4]? false</c>
    /// </example>
    [<CompiledName("IsProperSuperset")>]
    val isProperSuperset: set1: Set<'T> -> set2: Set<'T> -> bool


    /// <summary>Returns the number of elements in the set. Same as <c>size</c>.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The number of elements in the set.</returns>
    ///
    /// <example id="set-count">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The set has {set.Count} elements"
    /// </code>
    /// The sample evaluates to the following output: <c>The set has 3 elements</c>
    /// </example>
    [<CompiledName("Count")>]
    val count: set: Set<'T> -> int

    /// <summary>Tests if any element of the collection satisfies the given predicate.
    /// If the input function is <c>predicate</c> and the elements are <c>i0...iN</c>
    /// then computes <c>p i0 or ... or p iN</c>.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if any element of <c>set</c> satisfies <c>predicate</c>.</returns>
    ///
    /// <example id="set-exists">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"Does the set contain 1? {Set.exists (fun x -> x = 1) set}"
    /// </code>
    /// The sample evaluates to the following output: <c>Does the set contain 1? true</c>
    /// </example>
    [<CompiledName("Exists")>]
    val exists: predicate: ('T -> bool) -> set: Set<'T> -> bool

    /// <summary>Returns a new collection containing only the elements of the collection
    /// for which the given predicate returns True.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The set containing only the elements for which <c>predicate</c> returns true.</returns>
    ///
    /// <example id="set-filter">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"The set with even numbers is {Set.filter (fun x -> x % 2 = 0) set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set with even numbers is set [2; 4]</c>
    /// </example>
    [<CompiledName("Filter")>]
    val filter: predicate: ('T -> bool) -> set: Set<'T> -> Set<'T>

    /// <summary>Returns a new collection containing the results of applying the
    /// given function to each element of the input set.</summary>
    ///
    /// <param name="mapping">The function to transform elements of the input set.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>A set containing the transformed elements.</returns>
    ///
    /// <example id="set-map">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The set with doubled values is {Set.map (fun x -> x * 2) set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set with doubled values is set [2; 4; 6]</c>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> set: Set<'T> -> Set<'U>

    /// <summary>Applies the given accumulating function to all the elements of the set</summary>
    ///
    /// <param name="folder">The accumulating function.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The final state.</returns>
    ///
    /// <example id="set-fold">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The sum of the set is {Set.fold (+) 0 set}"
    /// printfn $"The product of the set is {Set.fold (*) 1 set}"
    /// printfn $"The reverse of the set is {Set.fold (fun x y -> y :: x) [] set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The sum of the set is 6
    /// The product of the set is 6
    /// The reverse of the set is [3; 2; 1]</c>
    /// </example>
    [<CompiledName("Fold")>]
    val fold<'T, 'State> : folder: ('State -> 'T -> 'State) -> state: 'State -> set: Set<'T> -> 'State
        when 'T: comparison

    /// <summary>Applies the given accumulating function to all the elements of the set.</summary>
    ///
    /// <param name="folder">The accumulating function.</param>
    /// <param name="set">The input set.</param>
    /// <param name="state">The initial state.</param>
    ///
    /// <returns>The final state.</returns>
    ///
    /// <example id="set-foldback">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The sum of the set is {Set.foldBack (+) set 0}"
    /// printfn $"The set is {Set.foldBack (fun x acc -> x :: acc) set []}"
    /// </code>
    /// The sample evaluates to the following output: <c>The sum of the set is 6
    /// The set is [1; 2; 3]</c>
    /// </example>
    [<CompiledName("FoldBack")>]
    val foldBack<'T, 'State> : folder: ('T -> 'State -> 'State) -> set: Set<'T> -> state: 'State -> 'State
        when 'T: comparison

    /// <summary>Tests if all elements of the collection satisfy the given predicate.
    /// If the input function is <c>f</c> and the elements are <c>i0...iN</c> and "j0...jN"
    /// then computes <c>p i0 &amp;&amp; ... &amp;&amp; p iN</c>.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if all elements of <c>set</c> satisfy <c>predicate</c>.</returns>
    ///
    /// <example id="set-forall">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"Does the set contain even numbers? {Set.forall (fun x -> x % 2 = 0) set}"
    /// </code>
    /// The sample evaluates to the following output: <c>Does the set contain even numbers? false</c>
    /// </example>
    [<CompiledName("ForAll")>]
    val forall: predicate: ('T -> bool) -> set: Set<'T> -> bool

    /// <summary>Computes the intersection of the two sets.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>The intersection of <c>set1</c> and <c>set2</c>.</returns>
    ///
    /// <example id="set-intersect">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(2).Add(3).Add(4)
    /// printfn $"The intersection of {set1} and {set2} is {Set.intersect set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>The intersection of set [1; 2; 3] and set [2; 3; 4] is set [2; 3]</c>
    /// </example>
    [<CompiledName("Intersect")>]
    val intersect: set1: Set<'T> -> set2: Set<'T> -> Set<'T>

    /// <summary>Computes the intersection of a sequence of sets. The sequence must be non-empty.</summary>
    ///
    /// <param name="sets">The sequence of sets to intersect.</param>
    ///
    /// <returns>The intersection of the input sets.</returns>
    ///
    /// <example id="set-intersectmany">
    /// <code lang="fsharp">
    /// let headersByFile = seq{
    /// yield [ "id"; "name"; "date"; "color" ]
    /// yield [ "id"; "age"; "date" ]
    /// yield [ "id"; "sex"; "date"; "animal" ]
    /// }
    /// headersByFile
    /// |> Seq.map Set.ofList
    /// |> Set.intersectMany
    /// |> printfn "The intersection of %A is %A" headersByFile
    /// </code>
    /// The sample evaluates to the following output: <c>The intersection of seq
    /// [["id"; "name"; "date"; "color"]; ["id"; "age"; "date"];
    /// ["id"; "sex"; "date"; "animal"]] is set ["date"; "id"]</c>
    /// </example>
    [<CompiledName("IntersectMany")>]
    val intersectMany: sets: seq<Set<'T>> -> Set<'T>

    /// <summary>Computes the union of the two sets.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The second input set.</param>
    ///
    /// <returns>The union of <c>set1</c> and <c>set2</c>.</returns>
    ///
    /// <example id="set-union">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(2).Add(3).Add(4)
    /// printfn $"The union of {set1} and {set2} is {(Set.union set1 set2)}"
    /// </code>
    /// The sample evaluates to the following output: <c>The union of set [1; 2; 3] and set [2; 3; 4] is set [1; 2; 3; 4]</c>
    /// </example>
    [<CompiledName("Union")>]
    val union: set1: Set<'T> -> set2: Set<'T> -> Set<'T>

    /// <summary>Computes the union of a sequence of sets.</summary>
    ///
    /// <param name="sets">The sequence of sets to union.</param>
    ///
    /// <returns>The union of the input sets.</returns>
    ///
    /// <example id="set-unionmany">
    /// <code lang="fsharp">
    /// let headersByFile = seq{
    /// yield [ "id"; "name"; "date"; "color" ]
    /// yield [ "id"; "age"; "date" ]
    /// yield [ "id"; "sex"; "date"; "animal" ]
    /// }
    /// headersByFile
    /// |> Seq.map Set.ofList
    /// |> Set.intersectMany
    /// |> printfn "The intersection of %A is %A" headersByFile
    /// </code>
    /// The sample evaluates to the following output: <c>The union of seq
    /// [["id"; "name"; "date"; "color"]; ["id"; "age"; "date"];
    /// ["id"; "sex"; "date"; "animal"]] is set ["age"; "animal"; "color"; "date"; "id"; "name"; "sex"]</c>
    /// </example>
    [<CompiledName("UnionMany")>]
    val unionMany: sets: seq<Set<'T>> -> Set<'T>

    /// <summary>Returns "true" if the set is empty.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>True if <c>set</c> is empty.</returns>
    ///
    /// <example id="set-isempty">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(2).Add(3)
    /// printfn $"Is the set empty? {set.IsEmpty}"
    /// </code>
    /// The sample evaluates to the following output: <c>Is the set empty? false</c>
    /// </example>
    [<CompiledName("IsEmpty")>]
    val isEmpty: set: Set<'T> -> bool

    /// <summary>Applies the given function to each element of the set, in order according
    /// to the comparison function.</summary>
    ///
    /// <param name="action">The function to apply to each element.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <example id="set-iter">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// Set.iter (fun x -> printfn $"The set contains {x}") set
    /// </code>
    /// The sample evaluates to the following output: <c>
    /// The set contains 1
    /// The set contains 2
    /// The set contains 3</c>
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action: ('T -> unit) -> set: Set<'T> -> unit

    /// <summary>Splits the set into two sets containing the elements for which the given predicate
    /// returns true and false respectively.</summary>
    ///
    /// <param name="predicate">The function to test set elements.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>A pair of sets with the first containing the elements for which <c>predicate</c> returns
    /// true and the second containing the elements for which <c>predicate</c> returns false.</returns>
    ///
    /// <example id="set-partition">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3).Add(4)
    /// printfn $"The set with even numbers is {Set.partition (fun x -> x % 2 = 0) set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The partitioned sets are: (set [2; 4], set [1; 3])</c>
    /// </example>
    [<CompiledName("Partition")>]
    val partition: predicate: ('T -> bool) -> set: Set<'T> -> (Set<'T> * Set<'T>)

    /// <summary>Returns a new set with the given element removed. No exception is raised if
    /// the set doesn't contain the given element.</summary>
    ///
    /// <param name="value">The element to remove.</param>
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The input set with <c>value</c> removed.</returns>
    ///
    /// <example id="set-remove">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The set without 1 is {Set.remove 1 set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set without 1 is set [2; 3]</c>
    /// </example>
    [<CompiledName("Remove")>]
    val remove: value: 'T -> set: Set<'T> -> Set<'T>

    /// <summary>Returns the lowest element in the set according to the ordering being used for the set.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The min value from the set.</returns>
    ///
    /// <example id="set-minelement">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The min element of {set} is {Set.minElement set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The min element of set [1; 2; 3] is 1</c>
    /// </example>
    [<CompiledName("MinElement")>]
    val minElement: set: Set<'T> -> 'T

    /// <summary>Returns the highest element in the set according to the ordering being used for the set.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>The max value from the set.</returns>
    ///
    /// <example id="set-maxelement">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// printfn $"The min element of {set} is {Set.minElement set}"
    /// </code>
    /// The sample evaluates to the following output: <c>The max element of set [1; 2; 3] is 3</c>
    /// </example>
    [<CompiledName("MaxElement")>]
    val maxElement: set: Set<'T> -> 'T

    /// <summary>Builds a set that contains the same elements as the given list.</summary>
    ///
    /// <param name="elements">The input list.</param>
    ///
    /// <returns>A set containing the elements form the input list.</returns>
    ///
    /// <example id="set-oflist">
    /// <code lang="fsharp">
    /// let set = Set.ofList [1, 2, 3]
    /// printfn $"The set is {set} and type is {set.GetType().Name}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set is set [(1, 2, 3)] and type is "FSharpSet`1"</c>
    /// </example>
    [<CompiledName("OfList")>]
    val ofList: elements: 'T list -> Set<'T>

    /// <summary>Builds a list that contains the elements of the set in order.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>An ordered list of the elements of <c>set</c>.</returns>
    ///
    /// <example id="set-tolist">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// let list = Set.toList set
    /// printfn $"The set is {list} and type is {list.GetType().Name}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set is [1; 2; 3] and type is "FSharpList`1"</c>
    /// </example>
    [<CompiledName("ToList")>]
    val toList: set: Set<'T> -> 'T list

    /// <summary>Builds a set that contains the same elements as the given array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>A set containing the elements of <c>array</c>.</returns>
    ///
    /// <example id="set-remove">
    /// <code lang="fsharp">
    /// let set = Set.ofArray [|1, 2, 3|]
    /// printfn $"The set is {set} and type is {set.GetType().Name}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set is set [(1, 2, 3)] and type is "FSharpSet`1"</c>
    /// </example>
    [<CompiledName("OfArray")>]
    val ofArray: array: 'T[] -> Set<'T>

    /// <summary>Builds an array that contains the elements of the set in order.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>An ordered array of the elements of <c>set</c>.</returns>
    ///
    /// <example id="set-toarray">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// let array = Set.toArray set
    /// printfn$ "The set is {set} and type is {array.GetType().Name}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set is [|1; 2; 3|] and type is System.Int32[]</c>
    /// </example>
    [<CompiledName("ToArray")>]
    val toArray: set: Set<'T> -> 'T[]

    /// <summary>Returns an ordered view of the collection as an enumerable object.</summary>
    ///
    /// <param name="set">The input set.</param>
    ///
    /// <returns>An ordered sequence of the elements of <c>set</c>.</returns>
    ///
    /// <example id="set-toseq">
    /// <code lang="fsharp">
    /// let set = Set.empty.Add(1).Add(2).Add(3)
    /// let seq = Set.toSeq set
    /// printfn $"The set is {set} and type is {seq.GetType().Name}"
    /// </code>
    /// The sample evaluates to the following output: <c>he set is set [1; 2; 3] and type is Microsoft.FSharp.Collections.FSharpSet`1[System.Int32]</c>
    /// </example>
    [<CompiledName("ToSeq")>]
    val toSeq: set: Set<'T> -> seq<'T>

    /// <summary>Builds a new collection from the given enumerable object.</summary>
    ///
    /// <param name="elements">The input sequence.</param>
    ///
    /// <returns>The set containing <c>elements</c>.</returns>
    ///
    /// <example id="set-ofseq">
    /// <code lang="fsharp">
    /// let set = Set.ofSeq [1, 2, 3]
    /// printfn $"The set is {set} and type is {set.GetType().Name}"
    /// </code>
    /// The sample evaluates to the following output: <c>The set is set [(1, 2, 3)] and type is "FSharpSet`1"</c>
    /// </example>
    [<CompiledName("OfSeq")>]
    val ofSeq: elements: seq<'T> -> Set<'T>

    /// <summary>Returns a new set with the elements of the second set removed from the first.</summary>
    ///
    /// <param name="set1">The first input set.</param>
    /// <param name="set2">The set whose elements will be removed from <c>set1</c>.</param>
    ///
    /// <returns>The set with the elements of <c>set2</c> removed from <c>set1</c>.</returns>
    ///
    /// <example id="set-difference">
    /// <code lang="fsharp">
    /// let set1 = Set.empty.Add(1).Add(2).Add(3)
    /// let set2 = Set.empty.Add(2).Add(3).Add(4)
    /// printfn $"The difference of {set1} and {set2} is {Set.difference set1 set2}"
    /// </code>
    /// The sample evaluates to the following output: <c>The difference of set [1; 2; 3] and set [2; 3; 4] is set [1]</c>
    /// </example>
    [<CompiledName("Difference")>]
    val difference: set1: Set<'T> -> set2: Set<'T> -> Set<'T>
