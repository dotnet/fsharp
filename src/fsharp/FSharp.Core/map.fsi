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
        member Add: key:'Key * value:'Value -> Map<'Key,'Value>

        /// <summary>Returns a new map with the value stored under key changed according to f.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <param name="f">The change function.</param>
        ///
        /// <returns>The resulting map.</returns>
        member Change: key:'Key * f:('Value option -> 'Value option) -> Map<'Key,'Value>

        /// <summary>Returns true if there are no bindings in the map.</summary>
        member IsEmpty: bool

        /// <summary>Builds a map that contains the bindings of the given IEnumerable.</summary>
        ///
        /// <param name="elements">The input sequence of key/value pairs.</param>
        ///
        /// <returns>The resulting map.</returns>
        new : elements:seq<'Key * 'Value> -> Map<'Key,'Value>

        /// <summary>Tests if an element is in the domain of the map.</summary>
        ///
        /// <param name="key">The input key.</param>
        ///
        /// <returns>True if the map contains the given key.</returns>
        member ContainsKey: key:'Key -> bool

        /// <summary>The number of bindings in the map.</summary>
        member Count: int

        /// <summary>Lookup an element in the map. Raise <c>KeyNotFoundException</c> if no binding
        /// exists in the map.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Thrown when the key is not found.</exception>
        ///
        /// <returns>The value mapped to the key.</returns>
        member Item : key:'Key -> 'Value with get

        /// <summary>Removes an element from the domain of the map. No exception is raised if the element is not present.</summary>
        ///
        /// <param name="key">The input key.</param>
        ///
        /// <returns>The resulting map.</returns>
        member Remove: key:'Key -> Map<'Key,'Value>

        /// <summary>Lookup an element in the map, returning a <c>Some</c> value if the element is in the domain 
        /// of the map and <c>None</c> if not.</summary>
        ///
        /// <param name="key">The input key.</param>
        ///
        /// <returns>The mapped value, or None if the key is not in the map.</returns>
        member TryFind: key:'Key -> 'Value option

        /// <summary>Lookup an element in the map, assigning to <c>value</c> if the element is in the domain 
        /// of the map and returning <c>false</c> if not.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <param name="value">A reference to the output value.</param>
        ///
        /// <returns><c>true</c> if the value is present, <c>false</c> if not.</returns>
        member TryGetValue: key: 'Key * [<System.Runtime.InteropServices.Out>] value: byref<'Value> -> bool
        
        /// <summary>The keys in the map.
        /// The sequence will be ordered by the keys of the map.</summary>
        member Keys : ICollection<'Key>

        /// <summary>All the values in the map, including the duplicates.
        /// The sequence will be ordered by the keys of the map.</summary>
        member Values : ICollection<'Value>

        interface IDictionary<'Key, 'Value>         
        interface ICollection<KeyValuePair<'Key, 'Value>> 
        interface IEnumerable<KeyValuePair<'Key, 'Value>>         
        interface System.IComparable
        interface System.Collections.IEnumerable 
        interface IReadOnlyCollection<KeyValuePair<'Key,'Value>>
        interface IReadOnlyDictionary<'Key,'Value>
        override Equals : obj -> bool

    /// <summary>Contains operations for working with values of type <see cref="T:Microsoft.FSharp.Collections.Map`2"/>.</summary>
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
        [<CompiledName("Add")>]
        val add: key:'Key -> value:'T -> table:Map<'Key,'T> -> Map<'Key,'T>

        /// <summary>Returns a new map with the value stored under key changed according to f.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <param name="f">The change function.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The resulting map.</returns>
        [<CompiledName("Change")>]
        val change: key:'Key -> f:('T option -> 'T option) -> table:Map<'Key,'T> -> Map<'Key,'T>

        /// <summary>Returns a new map made from the given bindings.</summary>
        ///
        /// <param name="elements">The input list of key/value pairs.</param>
        ///
        /// <returns>The resulting map.</returns>
        [<CompiledName("OfList")>]
        val ofList: elements:('Key * 'T) list -> Map<'Key,'T>

        /// <summary>Returns a new map made from the given bindings.</summary>
        ///
        /// <param name="elements">The input array of key/value pairs.</param>
        ///
        /// <returns>The resulting map.</returns>
        [<CompiledName("OfArray")>]
        val ofArray: elements:('Key * 'T)[] -> Map<'Key,'T>

        /// <summary>Returns a new map made from the given bindings.</summary>
        ///
        /// <param name="elements">The input sequence of key/value pairs.</param>
        ///
        /// <returns>The resulting map.</returns>
        [<CompiledName("OfSeq")>]
        val ofSeq: elements:seq<'Key * 'T> -> Map<'Key,'T>

        /// <summary>Views the collection as an enumerable sequence of pairs.
        /// The sequence will be ordered by the keys of the map.</summary>
        ///
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The sequence of key/value pairs.</returns>
        [<CompiledName("ToSeq")>]
        val toSeq: table:Map<'Key,'T> -> seq<'Key * 'T> 

        /// <summary>Returns a list of all key-value pairs in the mapping.
        /// The list will be ordered by the keys of the map.</summary>
        ///
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The list of key/value pairs.</returns>
        [<CompiledName("ToList")>]
        val toList: table:Map<'Key,'T> -> ('Key * 'T) list 

        /// <summary>Returns an array of all key-value pairs in the mapping.
        /// The array will be ordered by the keys of the map.</summary>
        ///
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The array of key/value pairs.</returns>
        [<CompiledName("ToArray")>]
        val toArray: table:Map<'Key,'T> -> ('Key * 'T)[]

        /// <summary>Is the map empty?</summary>
        ///
        /// <param name="table">The input map.</param>
        ///
        /// <returns>True if the map is empty.</returns>
        [<CompiledName("IsEmpty")>]
        val isEmpty: table:Map<'Key,'T> -> bool

        /// <summary>The empty map.</summary>
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
        [<CompiledName("Find")>]
        val find: key:'Key -> table:Map<'Key,'T> -> 'T

        /// <summary>Searches the map looking for the first element where the given function returns a <c>Some</c> value.</summary>
        ///
        /// <param name="chooser">The function to generate options from the key/value pairs.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The first result.</returns>
        [<CompiledName("TryPick")>]
        val tryPick: chooser:('Key -> 'T -> 'U option) -> table:Map<'Key,'T> -> 'U option

        /// <summary>Searches the map looking for the first element where the given function returns a <c>Some</c> value</summary>
        ///
        /// <param name="chooser">The function to generate options from the key/value pairs.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The first result.</returns>
        [<CompiledName("Pick")>]
        val pick: chooser:('Key -> 'T -> 'U option) -> table:Map<'Key,'T> -> 'U 

        /// <summary>Folds over the bindings in the map.</summary>
        ///
        /// <param name="folder">The function to update the state given the input key/value pairs.</param>
        /// <param name="table">The input map.</param>
        /// <param name="state">The initial state.</param>
        ///
        /// <returns>The final state value.</returns>
        [<CompiledName("FoldBack")>]
        val foldBack<'Key,'T,'State> : folder:('Key -> 'T -> 'State -> 'State) -> table:Map<'Key,'T> -> state:'State -> 'State when 'Key : comparison

        /// <summary>Folds over the bindings in the map </summary>
        ///
        /// <param name="folder">The function to update the state given the input key/value pairs.</param>
        /// <param name="state">The initial state.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The final state value.</returns>
        [<CompiledName("Fold")>]
        val fold<'Key,'T,'State> : folder:('State -> 'Key -> 'T -> 'State) -> state:'State -> table:Map<'Key,'T> -> 'State when 'Key : comparison

        /// <summary>Applies the given function to each binding in the dictionary</summary>
        ///
        /// <param name="action">The function to apply to each key/value pair.</param>
        /// <param name="table">The input map.</param>
        [<CompiledName("Iterate")>]
        val iter: action:('Key -> 'T -> unit) -> table:Map<'Key,'T> -> unit

        /// <summary>Returns true if the given predicate returns true for one of the
        /// bindings in the map.</summary>
        ///
        /// <param name="predicate">The function to test the input elements.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>True if the predicate returns true for one of the key/value pairs.</returns>
        [<CompiledName("Exists")>]
        val exists: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> bool

        /// <summary>Builds a new map containing only the bindings for which the given predicate returns 'true'.</summary>
        ///
        /// <param name="predicate">The function to test the key/value pairs.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The filtered map.</returns>
        [<CompiledName("Filter")>]
        val filter: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> Map<'Key, 'T>

        /// <summary>Returns true if the given predicate returns true for all of the
        /// bindings in the map.</summary>
        ///
        /// <param name="predicate">The function to test the input elements.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>True if the predicate evaluates to true for all of the bindings in the map.</returns>
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
        [<CompiledName("Map")>]
        val map: mapping:('Key -> 'T -> 'U) -> table:Map<'Key,'T> -> Map<'Key,'U>

        /// <summary>Tests if an element is in the domain of the map.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>True if the map contains the key.</returns>
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
        [<CompiledName("Partition")>]
        val partition: predicate:('Key -> 'T -> bool) -> table:Map<'Key, 'T> -> Map<'Key, 'T> * Map<'Key, 'T>

        /// <summary>Removes an element from the domain of the map. No exception is raised if the element is not present.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The resulting map.</returns>
        [<CompiledName("Remove")>]
        val remove: key:'Key -> table:Map<'Key,'T> -> Map<'Key,'T>

        /// <summary>Lookup an element in the map, returning a <c>Some</c> value if the element is in the domain 
        /// of the map and <c>None</c> if not.</summary>
        ///
        /// <param name="key">The input key.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The found <c>Some</c> value or <c>None</c>.</returns>
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
        [<CompiledName("FindKey")>]
        val findKey: predicate:('Key -> 'T -> bool) -> table:Map<'Key,'T> -> 'Key

        /// <summary>Returns the key of the first mapping in the collection that satisfies the given predicate. 
        /// Returns 'None' if no such element exists.</summary>
        ///
        /// <param name="predicate">The function to test the input elements.</param>
        /// <param name="table">The input map.</param>
        ///
        /// <returns>The first key for which the predicate returns true or None if the predicate evaluates to false for each key/value pair.</returns>
        [<CompiledName("TryFindKey")>]
        val tryFindKey: predicate:('Key -> 'T -> bool) -> table:Map<'Key,'T> -> 'Key option

        /// <summary>The number of bindings in the map.</summary>
        [<CompiledName("Count")>]
        val count: table:Map<'Key,'T> -> int

        /// <summary>The keys in the map.
        /// The sequence will be ordered by the keys of the map.</summary>
        [<CompiledName("Keys")>]
        val keys: table: Map<'Key, 'T> -> ICollection<'Key>

        /// <summary>The values in the map, including the duplicates.
        /// The sequence will be ordered by the keys of the map.</summary>
        [<CompiledName("Values")>]
        val values: table: Map<'Key, 'T> -> ICollection<'T>
