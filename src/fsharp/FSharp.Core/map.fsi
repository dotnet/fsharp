// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

/// <summary>Immutable maps based on binary trees, where keys are ordered by F# generic comparison. By default
/// comparison is the F# structural comparison function or uses implementations of the IComparable interface on key values.</summary>
/// 
/// <remarks>See the <see cref="T:Microsoft.FSharp.Collections.MapModule"/> module for further operations on maps.
///
/// All members of this class are thread-safe and may be used concurrently from multiple threads.</remarks>
[<CompiledName("FSharpMap`2")>]
[<Sealed>]
type Map<[<EqualityConditionalOn>]'Key,[<EqualityConditionalOn;ComparisonConditionalOn>]'Value when 'Key : comparison> =
    /// <summary>Returns a new map with the binding added to the given map.
    /// If a binding with the given key already exists in the input map, the existing binding is replaced by the new binding in the result map.</summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="member-add-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.Add (3, "c") // evaluates to map [(1, "a"); (2, "b"); (3, "c")]
    /// sample.Add (2, "aa") // evaluates to map [(1, "a"); (2, "aa")]
    /// </code>
    /// </example>
    member Add: key:'Key * value:'Value -> Map<'Key,'Value>

    /// <summary>Returns a new map with the value stored under key changed according to f.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="f">The change function.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="member-change-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// let f x =
    ///     match x with 
    ///     | Some s -> Some (s + "z")
    ///     | None -> None
    /// 
    /// sample.Change (1, f) // evaluates to map [(1, "az"); (2, "b")]
    /// </code>
    /// </example>
    member Change: key:'Key * f:('Value option -> 'Value option) -> Map<'Key,'Value>

    /// <summary>Returns true if there are no bindings in the map.</summary>
    /// 
    /// <example id="member-isempty-1">
    /// <code lang="fsharp">
    /// let emptyMap: Map&lt;int, string> = Map.empty
    /// emptyMap.IsEmpty // evaluates to true
    /// 
    /// let notEmptyMap = Map [ (1, "a"); (2, "b") ]
    /// notEmptyMap.IsEmpty // evaluates to false
    /// </code>
    /// </example>
    member IsEmpty: bool

    /// <summary>Builds a map that contains the bindings of the given IEnumerable.</summary>
    ///
    /// <param name="elements">The input sequence of key/value pairs.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="new-1">
    /// <code lang="fsharp">
    /// Map [ (1, "a"); (2, "b") ] // evaluates to map [(1, "a"); (2, "b")]
    /// </code>
    /// </example>
    new: elements:seq<'Key * 'Value> -> Map<'Key,'Value>

    /// <summary>Tests if an element is in the domain of the map.</summary>
    ///
    /// <param name="key">The input key.</param>
    ///
    /// <returns>True if the map contains the given key.</returns>
    /// 
    /// <example id="member-containskey-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.ContainsKey 1 // evaluates to true
    /// sample.ContainsKey 3 // evaluates to false
    /// </code>
    /// </example>
    member ContainsKey: key:'Key -> bool

    /// <summary>The number of bindings in the map.</summary>
    /// 
    /// <example id="member-count-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.Count // evaluates to 2
    /// </code>
    /// </example>
    member Count: int

    /// <summary>Lookup an element in the map. Raise <c>KeyNotFoundException</c> if no binding
    /// exists in the map.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown when the key is not found.</exception>
    ///
    /// <returns>The value mapped to the key.</returns>
    /// 
    /// <example id="member-item-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.[1] // evaluates to "a"
    /// sample.[3] // throws KeyNotFoundException
    /// </code>
    /// </example>
    member Item : key:'Key -> 'Value with get

    /// <summary>Removes an element from the domain of the map. No exception is raised if the element is not present.</summary>
    ///
    /// <param name="key">The input key.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="member-remove-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.Remove 1 // evaluates to map [(2, "b")]
    /// sample.Remove 3 // equal to sample
    /// </code>
    /// </example>
    member Remove: key:'Key -> Map<'Key,'Value>

    /// <summary>Lookup an element in the map, returning a <c>Some</c> value if the element is in the domain 
    /// of the map and <c>None</c> if not.</summary>
    ///
    /// <param name="key">The input key.</param>
    ///
    /// <returns>The mapped value, or None if the key is not in the map.</returns>
    /// 
    /// <example id="member-tryfind-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.TryFind 1 // evaluates to Some "a"
    /// sample.TryFind 3 // evaluates to None
    /// </code>
    /// </example>
    member TryFind: key:'Key -> 'Value option

    /// <summary>Lookup an element in the map, assigning to <c>value</c> if the element is in the domain 
    /// of the map and returning <c>false</c> if not.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="value">A reference to the output value.</param>
    ///
    /// <returns><c>true</c> if the value is present, <c>false</c> if not.</returns>
    /// 
    /// <example id="member-trygetvalue-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.TryGetValue 1 // evaluates to (true, "a")
    /// sample.TryGetValue 3 // evaluates to (false, null)
    /// 
    /// let mutable x = ""
    /// sample.TryGetValue (1, &amp;x) // evaluates to true, x set to "a"
    /// 
    /// let mutable y = ""
    /// sample.TryGetValue (3, &amp;y) // evaluates to false, y unchanged
    /// </code>
    /// </example>
    member TryGetValue: key: 'Key * [<System.Runtime.InteropServices.Out>] value: byref<'Value> -> bool
    
    /// <summary>The keys in the map.
    /// The sequence will be ordered by the keys of the map.</summary>
    /// 
    /// <example id="member-keys-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.Keys // evaluates to seq [1; 2]
    /// </code>
    /// </example>
    member Keys : ICollection<'Key>

    /// <summary>All the values in the map, including the duplicates.
    /// The sequence will be ordered by the keys of the map.</summary>
    /// 
    /// <example id="member-values-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample.Values // evaluates to seq ["a"; "b"]
    /// </code>
    /// </example>
    member Values : ICollection<'Value>

    interface IDictionary<'Key, 'Value>         
    interface ICollection<KeyValuePair<'Key, 'Value>> 
    interface IEnumerable<KeyValuePair<'Key, 'Value>>         
    interface System.IComparable
    interface System.Collections.IEnumerable 
    interface IReadOnlyCollection<KeyValuePair<'Key,'Value>>
    interface IReadOnlyDictionary<'Key,'Value>
    override Equals : obj -> bool

/// <summary>Contains operations for working with values of type <see cref="T:Microsoft.FSharp.Collections.FSharpMap`2"/>.</summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Map = 

    /// <summary>Returns a new map with the binding added to the given map.
    /// If a binding with the given key already exists in the input map, the existing binding is replaced by the new binding in the result map.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="value">The input value.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="add-1">
    /// <code lang="fsharp">
    /// let input = Map [ (1, "a"); (2, "b") ]
    /// 
    /// input |> Map.add 3 "c" // evaluates to map [(1, "a"); (2, "b"); (3, "c")]
    /// input |> Map.add 2 "aa" // evaluates to map [(1, "a"); (2, "aa")]
    /// </code>
    /// </example>
    [<CompiledName("Add")>]
    val add: key:'Key -> value:'T -> table:Map<'Key,'T> -> Map<'Key,'T>

    /// <summary>Returns a new map with the value stored under key changed according to f.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="f">The change function.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="change-1">
    /// <code lang="fsharp">
    /// let input = Map [ (1, "a"); (2, "b") ]
    /// 
    /// input |> Map.change 1 (fun x ->
    ///     match x with
    ///     | Some s -> Some (s + "z")
    ///     | None -> None
    /// )
    /// evaluates to map [(1, "az"); (2, "b")]
    /// </code>
    /// </example>
    [<CompiledName("Change")>]
    val change: key:'Key -> f:('T option -> 'T option) -> table:Map<'Key,'T> -> Map<'Key,'T>

    /// <summary>Returns a new map made from the given bindings.</summary>
    ///
    /// <param name="elements">The input list of key/value pairs.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="oflist-1">
    /// <code lang="fsharp">
    /// let input = [ (1, "a"); (2, "b") ]
    ///
    /// input |> Map.ofList // evaluates to map [(1, "a"); (2, "b")]
    /// </code>
    /// </example>
    [<CompiledName("OfList")>]
    val ofList: elements:('Key * 'T) list -> Map<'Key,'T>

    /// <summary>Returns a new map made from the given bindings.</summary>
    ///
    /// <param name="elements">The input array of key/value pairs.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="ofarray-1">
    /// <code lang="fsharp">
    /// let input = [| (1, "a"); (2, "b") |]
    ///
    /// input |> Map.ofArray // evaluates to map [(1, "a"); (2, "b")]
    /// </code>
    /// </example>
    [<CompiledName("OfArray")>]
    val ofArray: elements:('Key * 'T)[] -> Map<'Key,'T>

    /// <summary>Returns a new map made from the given bindings.</summary>
    ///
    /// <param name="elements">The input sequence of key/value pairs.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="ofseq-1">
    /// <code lang="fsharp">
    /// let input = seq { (1, "a"); (2, "b") }
    ///
    /// input |> Map.ofSeq // evaluates to map [(1, "a"); (2, "b")]
    /// </code>
    /// </example>
    [<CompiledName("OfSeq")>]
    val ofSeq: elements:seq<'Key * 'T> -> Map<'Key,'T>

    /// <summary>Views the collection as an enumerable sequence of pairs.
    /// The sequence will be ordered by the keys of the map.</summary>
    ///
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The sequence of key/value pairs.</returns>
    /// 
    /// <example id="toseq-1">
    /// <code lang="fsharp">
    /// let input = Map [ (1, "a"); (2, "b") ]
    ///
    /// input |> Map.toSeq // evaluates to seq [(1, "a"); (2, "b")]
    /// </code>
    /// </example>
    [<CompiledName("ToSeq")>]
    val toSeq: table:Map<'Key,'T> -> seq<'Key * 'T> 

    /// <summary>Returns a list of all key-value pairs in the mapping.
    /// The list will be ordered by the keys of the map.</summary>
    ///
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The list of key/value pairs.</returns>
    /// 
    /// <example id="tolist-1">
    /// <code lang="fsharp">
    /// let input = Map [ (1, "a"); (2, "b") ]
    ///
    /// input |> Map.toList // evaluates to [(1, "a"); (2, "b")]
    /// </code>
    /// </example>
    [<CompiledName("ToList")>]
    val toList: table:Map<'Key,'T> -> ('Key * 'T) list 

    /// <summary>Returns an array of all key-value pairs in the mapping.
    /// The array will be ordered by the keys of the map.</summary>
    ///
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The array of key/value pairs.</returns>
    /// 
    /// <example id="toarray-1">
    /// <code lang="fsharp">
    /// let input = Map [ (1, "a"); (2, "b") ]
    ///
    /// input |> Map.toArray // evaluates to [|(1, "a"); (2, "b")|]
    /// </code>
    /// </example>
    [<CompiledName("ToArray")>]
    val toArray: table:Map<'Key,'T> -> ('Key * 'T)[]

    /// <summary>Is the map empty?</summary>
    ///
    /// <param name="table">The input map.</param>
    ///
    /// <returns>True if the map is empty.</returns>
    /// 
    /// <example id="isempty-1">
    /// <code lang="fsharp">
    /// let emptyMap = Map.empty&lt;int, string>
    /// emptyMap |> Map.isEmpty  // evaluates to true
    /// 
    /// let notEmptyMap = Map [ (1, "a"); (2, "b") ]
    /// emptyMap |> Map.isEmpty // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("IsEmpty")>]
    val isEmpty: table:Map<'Key,'T> -> bool

    /// <summary>The empty map.</summary>
    /// 
    /// <example id="empty-1">
    /// <code lang="fsharp">
    /// let emptyMap = Map.empty&lt;int, string>
    /// </code>
    /// </example>
    [<GeneralizableValueAttribute>]
    [<CompiledName("Empty")>]
    val empty<'Key,'T> : Map<'Key,'T> when 'Key : comparison

    /// <summary>Lookup an element in the map, raising <c>KeyNotFoundException</c> if no binding
    /// exists in the map.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="table">The input map.</param>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown when the key does not exist in the map.</exception>
    ///
    /// <returns>The value mapped to the given key.</returns>
    /// 
    /// <example id="find-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.find 1 // evaluates to "a"
    /// sample |> Map.find 3 // throws KeyNotFoundException
    /// </code>
    /// </example>
    [<CompiledName("Find")>]
    val find: key:'Key -> table:Map<'Key,'T> -> 'T

    /// <summary>Searches the map looking for the first element where the given function returns a <c>Some</c> value.</summary>
    ///
    /// <param name="chooser">The function to generate options from the key/value pairs.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The first result.</returns>
    /// 
    /// <example id="trypick-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b"); (10, "ccc"); (20, "ddd") ]
    /// 
    /// sample |> Map.tryPick (fun n s -> if n > 5 &amp;&amp; s.Length > 2 then Some s else None)
    /// </code>
    /// Evaluates to <c>Some "ccc"</c>.
    /// </example>
    ///
    /// <example id="trypick-2">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b"); (10, "ccc"); (20, "ddd") ]
    /// 
    /// sample |> Map.tryPick (fun n s -> if n > 5 &amp;&amp; s.Length > 4 then Some s else None)
    /// </code>
    /// Evaluates to <c>None</c>.
    /// </example>
    [<CompiledName("TryPick")>]
    val tryPick: chooser:('Key -> 'T -> 'U option) -> table:Map<'Key,'T> -> 'U option

    /// <summary>Searches the map looking for the first element where the given function returns a <c>Some</c> value.
    /// Raise <c>KeyNotFoundException</c> if no such element exists.</summary>
    ///
    /// <param name="chooser">The function to generate options from the key/value pairs.</param>
    /// <param name="table">The input map.</param>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown if no element returns a <c>Some</c>
    /// value when evaluated by the chooser function</exception>
    ///
    /// <returns>The first result.</returns>
    /// 
    /// <example id="pick-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b"); (10, "ccc"); (20, "ddd") ]
    /// 
    /// sample |> Map.pick (fun n s -> if n > 5 &amp;&amp; s.Length > 2 then Some s else None)
    /// </code>
    /// Evaluates to <c>"ccc"</c>
    /// </example>
    /// <example id="pick-2">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b"); (10, "ccc"); (20, "ddd") ]
    /// 
    /// sample |> Map.pick (fun n s -> if n > 5 &amp;&amp; s.Length > 4 then Some s else None)
    /// </code>
    /// Raises <c>KeyNotFoundException</c>
    /// </example>
    [<CompiledName("Pick")>]
    val pick: chooser:('Key -> 'T -> 'U option) -> table:Map<'Key,'T> -> 'U 

    /// <summary>Folds over the bindings in the map.</summary>
    ///
    /// <param name="folder">The function to update the state given the input key/value pairs.</param>
    /// <param name="table">The input map.</param>
    /// <param name="state">The initial state.</param>
    ///
    /// <returns>The final state value.</returns>
    /// 
    /// <example id="foldback-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// (sample, "initial") ||> Map.foldBack (fun n s state -> sprintf "%i %s %s" n s state)
    /// </code>
    /// Evaluates to <c>"1 a 2 b initial"</c>
    /// </example>
    [<CompiledName("FoldBack")>]
    val foldBack<'Key,'T,'State> : folder:('Key -> 'T -> 'State -> 'State) -> table:Map<'Key,'T> -> state:'State -> 'State when 'Key : comparison

    /// <summary>Folds over the bindings in the map </summary>
    ///
    /// <param name="folder">The function to update the state given the input key/value pairs.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The final state value.</returns>
    /// 
    /// <example id="fold-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// ("initial", sample) ||> Map.fold (fun state n s -> sprintf "%s %i %s" state n s)
    /// </code>
    /// Evaluates to <c>"initial 1 a 2 b"</c>.
    /// </example>
    [<CompiledName("Fold")>]
    val fold<'Key,'T,'State> : folder:('State -> 'Key -> 'T -> 'State) -> state:'State -> table:Map<'Key,'T> -> 'State when 'Key : comparison

    /// <summary>Applies the given function to each binding in the dictionary</summary>
    ///
    /// <param name="action">The function to apply to each key/value pair.</param>
    /// <param name="table">The input map.</param>
    /// 
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.iter (fun n s -> printf "%i %s " n s)
    /// </code>
    /// Prints <c>"1 a 2 b "</c>.
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action:('Key -> 'T -> unit) -> table:Map<'Key,'T> -> unit

    /// <summary>Returns true if the given predicate returns true for one of the
    /// bindings in the map.</summary>
    ///
    /// <param name="predicate">The function to test the input elements.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>True if the predicate returns true for one of the key/value pairs.</returns>
    /// 
    /// <example id="exists-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.exists (fun n s -> n = s.Length) // evaluates to true
    /// sample |> Map.exists (fun n s -> n &lt; s.Length) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Exists")>]
    val exists: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> bool

    /// <summary>Builds a new map containing only the bindings for which the given predicate returns 'true'.</summary>
    ///
    /// <param name="predicate">The function to test the key/value pairs.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The filtered map.</returns>
    /// 
    /// <example id="filter-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.filter (fun n s -> n = s.Length) // evaluates to map [(1, "a")]
    /// </code>
    /// </example>
    [<CompiledName("Filter")>]
    val filter: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> Map<'Key, 'T>

    /// <summary>Returns true if the given predicate returns true for all of the
    /// bindings in the map.</summary>
    ///
    /// <param name="predicate">The function to test the input elements.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>True if the predicate evaluates to true for all of the bindings in the map.</returns>
    /// 
    /// <example id="forall-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.forall (fun n s -> n >= s.Length) // evaluates to true
    /// sample |> Map.forall (fun n s -> n = s.Length)  // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("ForAll")>]
    val forall: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> bool

    /// <summary>Builds a new collection whose elements are the results of applying the given function
    /// to each of the elements of the collection. The key passed to the
    /// function indicates the key of element being transformed.</summary>
    ///
    /// <param name="mapping">The function to transform the key/value pairs.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The resulting map of keys and transformed values.</returns>
    /// 
    /// <example id="map-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.map (fun n s -> sprintf "%i %s" n s) // evaluates to map [(1, "1 a"); (2, "2 b")]
    /// </code>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping:('Key -> 'T -> 'U) -> table:Map<'Key,'T> -> Map<'Key,'U>

    /// <summary>Tests if an element is in the domain of the map.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>True if the map contains the key.</returns>
    /// 
    /// <example id="containskey-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.containsKey 1 // evaluates to true
    /// sample |> Map.containsKey 3 // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("ContainsKey")>]
    val containsKey: key:'Key -> table:Map<'Key,'T> -> bool

    /// <summary>Builds two new maps, one containing the bindings for which the given predicate returns 'true',
    /// and the other the remaining bindings.</summary>
    ///
    /// <param name="predicate">The function to test the input elements.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>A pair of maps in which the first contains the elements for which the predicate returned true
    /// and the second containing the elements for which the predicated returned false.</returns>
    /// 
    /// <example id="partition-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.partition (fun n s -> n = s.Length) // evaluates to (map [(1, "a")], map [(2, "b")])
    /// </code>
    /// </example>
    [<CompiledName("Partition")>]
    val partition: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> Map<'Key, 'T> * Map<'Key, 'T>

    /// <summary>Removes an element from the domain of the map. No exception is raised if the element is not present.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The resulting map.</returns>
    /// 
    /// <example id="remove-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.remove 1 // evaluates to map [(2, "b")]
    /// sample |> Map.remove 3 // equal to sample
    /// </code>
    /// </example>
    [<CompiledName("Remove")>]
    val remove: key:'Key -> table:Map<'Key,'T> -> Map<'Key,'T>

    /// <summary>Lookup an element in the map, returning a <c>Some</c> value if the element is in the domain 
    /// of the map and <c>None</c> if not.</summary>
    ///
    /// <param name="key">The input key.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The found <c>Some</c> value or <c>None</c>.</returns>
    /// 
    /// <example id="tryfind-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.tryFind 1 // evaluates to Some "a"
    /// sample |> Map.tryFind 3 // evaluates to None
    /// </code>
    /// </example>
    [<CompiledName("TryFind")>]
    val tryFind: key:'Key -> table:Map<'Key,'T> -> 'T option

    /// <summary>Evaluates the function on each mapping in the collection. Returns the key for the first mapping
    /// where the function returns 'true'. Raise <c>KeyNotFoundException</c> if no such element exists.</summary>
    ///
    /// <param name="predicate">The function to test the input elements.</param>
    /// <param name="table">The input map.</param>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown if the key does not exist in the map.</exception>
    ///
    /// <returns>The first key for which the predicate evaluates true.</returns>
    /// 
    /// <example id="findkey-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.findKey (fun n s -> n = s.Length) // evaluates to 1
    /// sample |> Map.findKey (fun n s -> n &lt; s.Length) // throws KeyNotFoundException
    /// </code>
    /// </example>
    [<CompiledName("FindKey")>]
    val findKey: predicate:('Key -> 'T -> bool) -> table:Map<'Key,'T> -> 'Key

    /// <summary>Returns the key of the first mapping in the collection that satisfies the given predicate. 
    /// Returns 'None' if no such element exists.</summary>
    ///
    /// <param name="predicate">The function to test the input elements.</param>
    /// <param name="table">The input map.</param>
    ///
    /// <returns>The first key for which the predicate returns true or None if the predicate evaluates to false for each key/value pair.</returns>
    /// 
    /// <example id="tryfindkey-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.tryFindKey (fun n s -> n = s.Length) // evaluates to Some 1
    /// sample |> Map.tryFindKey (fun n s -> n &lt; s.Length) // evaluates to None
    /// </code>
    /// </example>
    [<CompiledName("TryFindKey")>]
    val tryFindKey: predicate:('Key -> 'T -> bool) -> table:Map<'Key,'T> -> 'Key option

    /// <summary>The number of bindings in the map.</summary>
    /// 
    /// <example id="count-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.count // evaluates to 2
    /// </code>
    /// </example>
    [<CompiledName("Count")>]
    val count: table:Map<'Key,'T> -> int

    /// <summary>The keys in the map.
    /// The sequence will be ordered by the keys of the map.</summary>
    /// 
    /// <example id="keys-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.keys // evaluates to seq [1; 2]
    /// </code>
    /// </example>
    [<CompiledName("Keys")>]
    val keys: table: Map<'Key, 'T> -> ICollection<'Key>

    /// <summary>The values in the map, including the duplicates.
    /// The sequence will be ordered by the keys of the map.</summary>
    /// 
    /// <example id="values-1">
    /// <code lang="fsharp">
    /// let sample = Map [ (1, "a"); (2, "b") ]
    /// 
    /// sample |> Map.values // evaluates to seq ["a"; "b"]
    /// </code>
    /// </example>
    [<CompiledName("Values")>]
    val values: table: Map<'Key, 'T> -> ICollection<'T>
