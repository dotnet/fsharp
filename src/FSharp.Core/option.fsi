// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections


/// <summary>Contains operations for working with options.</summary>
///
/// <category>Options</category>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Option =
    /// <summary>Returns true if the option is not None.</summary>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>True if the option is not None.</returns>
    ///
    /// <example id="isSome-1">
    /// <code lang="fsharp">
    /// None |> Option.isSome // evaluates to false
    /// Some 42 |> Option.isSome // evaluates to true
    /// </code>
    /// </example>
    [<CompiledName("IsSome")>]
    val inline isSome: option: 'T option -> bool

    /// <summary>Returns true if the option is None.</summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>True if the option is None.</returns>
    ///
    /// <example id="isNone-1">
    /// <code lang="fsharp">
    /// None |> Option.isNone // evaluates to true
    /// Some 42 |> Option.isNone // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("IsNone")>]
    val inline isNone: option: 'T option -> bool

    /// <summary>Gets the value of the option if the option is <c>Some</c>, otherwise returns the specified default value.</summary>
    ///
    /// <param name="value">The specified default value.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The option if the option is Some, else the default value.</returns>
    ///
    /// <remarks>Identical to the built-in <see cref="defaultArg"/> operator, except with the arguments swapped.</remarks>
    ///
    /// <example id="defaultValue-1">
    /// <code lang="fsharp">
    /// (99, None) ||> Option.defaultValue // evaluates to 99
    /// (99, Some 42) ||> Option.defaultValue // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("DefaultValue")>]
    val defaultValue: value: 'T -> option: 'T option -> 'T

    /// <summary>Gets the value of the option if the option is <c>Some</c>, otherwise evaluates <paramref name="defThunk"/> and returns the result.</summary>
    ///
    /// <param name="defThunk">A thunk that provides a default value when evaluated.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The option if the option is Some, else the result of evaluating <paramref name="defThunk"/>.</returns>
    /// <remarks><paramref name="defThunk"/> is not evaluated unless <paramref name="option"/> is <c>None</c>.</remarks>
    ///
    /// <example id="defaultWith-1">
    /// <code lang="fsharp">
    /// None |> Option.defaultWith (fun () -> 99) // evaluates to 99
    /// Some 42 |> Option.defaultWith (fun () -> 99) // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("DefaultWith")>]
    val defaultWith: defThunk: (unit -> 'T) -> option: 'T option -> 'T

    /// <summary>Returns <paramref name="option"/> if it is <c>Some</c>, otherwise returns <paramref name="ifNone"/>.</summary>
    ///
    /// <param name="ifNone">The value to use if <paramref name="option"/> is <c>None</c>.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The option if the option is Some, else the alternate option.</returns>
    ///
    /// <example id="orElse-1">
    /// <code lang="fsharp">
    /// ((None: int Option), None) ||> Option.orElse // evaluates to None
    /// (Some 99, None) ||> Option.orElse // evaluates to Some 99
    /// (None, Some 42) ||> Option.orElse // evaluates to Some 42
    /// (Some 99, Some 42) ||> Option.orElse // evaluates to Some 42
    /// </code>
    /// </example>
    [<CompiledName("OrElse")>]
    val orElse: ifNone: 'T option -> option: 'T option -> 'T option

    /// <summary>Returns <paramref name="option"/> if it is <c>Some</c>, otherwise evaluates <paramref name="ifNoneThunk"/> and returns the result.</summary>
    ///
    /// <param name="ifNoneThunk">A thunk that provides an alternate option when evaluated.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The option if the option is Some, else the result of evaluating <paramref name="ifNoneThunk"/>.</returns>
    /// <remarks><paramref name="ifNoneThunk"/> is not evaluated unless <paramref name="option"/> is <c>None</c>.</remarks>
    ///
    /// <example id="orElseWith-1">
    /// <code lang="fsharp">
    /// (None: int Option) |> Option.orElseWith (fun () -> None) // evaluates to None
    /// None |> Option.orElseWith (fun () -> (Some 99)) // evaluates to Some 99
    /// Some 42 |> Option.orElseWith (fun () -> None) // evaluates to Some 42
    /// Some 42 |> Option.orElseWith (fun () -> (Some 99)) // evaluates to Some 42
    /// </code>
    /// </example>
    [<CompiledName("OrElseWith")>]
    val orElseWith: ifNoneThunk: (unit -> 'T option) -> option: 'T option -> 'T option

    /// <summary>Gets the value associated with the option.</summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The value within the option.</returns>
    ///
    /// <exception href="System.ArgumentException">Thrown when the option is None.</exception>
    ///
    /// <example id="get-1">
    /// <code lang="fsharp">
    /// Some 42 |> Option.get // evaluates to 42
    /// (None: int option) |> Option.get // throws exception!
    /// </code>
    /// </example>
    [<CompiledName("GetValue")>]
    val get: option: 'T option -> 'T

    /// <summary><c>count inp</c> evaluates to <c>match inp with None -> 0 | Some _ -> 1</c>.</summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>A zero if the option is None, a one otherwise.</returns>
    ///
    /// <example id="count-1">
    /// <code lang="fsharp">
    /// None |> Option.count // evaluates to 0
    /// Some 99 |> Option.count // evaluates to 1
    /// </code>
    /// </example>
    [<CompiledName("Count")>]
    val count: option: 'T option -> int

    /// <summary><c>fold f s inp</c> evaluates to <c>match inp with None -> s | Some x -> f s x</c>.</summary>
    ///
    /// <param name="folder">A function to update the state data when given a value from an option.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The original state if the option is None, otherwise it returns the updated state with the folder
    /// and the option value.</returns>
    ///
    /// <example id="fold-1">
    /// <code lang="fsharp">
    /// (0, None) ||> Option.fold (fun accum x -> accum + x * 2) // evaluates to 0
    /// (0, Some 1) ||> Option.fold (fun accum x -> accum + x * 2) // evaluates to 2
    /// (10, Some 1) ||> Option.fold (fun accum x -> accum + x * 2) // evaluates to 12
    /// </code>
    /// </example>
    [<CompiledName("Fold")>]
    val fold<'T, 'State> : folder: ('State -> 'T -> 'State) -> state: 'State -> option: 'T option -> 'State

    /// <summary><c>fold f inp s</c> evaluates to <c>match inp with None -> s | Some x -> f x s</c>.</summary>
    ///
    /// <param name="folder">A function to update the state data when given a value from an option.</param>
    /// <param name="option">The input option.</param>
    /// <param name="state">The initial state.</param>
    ///
    /// <returns>The original state if the option is None, otherwise it returns the updated state with the folder
    /// and the option value.</returns>
    ///
    /// <example id="foldBack-1">
    /// <code lang="fsharp">
    /// (None, 0) ||> Option.foldBack (fun x accum -> accum + x * 2) // evaluates to 0
    /// (Some 1, 0) ||> Option.foldBack (fun x accum -> accum + x * 2) // evaluates to 2
    /// (Some 1, 10) ||> Option.foldBack (fun x accum -> accum + x * 2) // evaluates to 12
    /// </code>
    /// </example>
    [<CompiledName("FoldBack")>]
    val foldBack<'T, 'State> : folder: ('T -> 'State -> 'State) -> option: 'T option -> state: 'State -> 'State

    /// <summary><c>exists p inp</c> evaluates to <c>match inp with None -> false | Some x -> p x</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates to a boolean when given a value from the option type.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>False if the option is None, otherwise it returns the result of applying the predicate
    /// to the option value.</returns>
    ///
    /// <example id="exists-1">
    /// <code lang="fsharp">
    /// None |> Option.exists (fun x -> x >= 5) // evaluates to false
    /// Some 42 |> Option.exists (fun x -> x >= 5) // evaluates to true
    /// Some 4 |> Option.exists (fun x -> x >= 5) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Exists")>]
    val exists: predicate: ('T -> bool) -> option: 'T option -> bool

    /// <summary><c>forall p inp</c> evaluates to <c>match inp with None -> true | Some x -> p x</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates to a boolean when given a value from the option type.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>True if the option is None, otherwise it returns the result of applying the predicate
    /// to the option value.</returns>
    ///
    /// <example id="forall-1">
    /// <code lang="fsharp">
    /// None |> Option.forall (fun x -> x >= 5) // evaluates to true
    /// Some 42 |> Option.forall (fun x -> x >= 5) // evaluates to true
    /// Some 4 |> Option.forall (fun x -> x >= 5) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("ForAll")>]
    val forall: predicate: ('T -> bool) -> option: 'T option -> bool

    /// <summary>Evaluates to true if <paramref name="option"/> is <c>Some</c> and its value is equal to <paramref name="value"/>.</summary>
    ///
    /// <param name="value">The value to test for equality.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>True if the option is <c>Some</c> and contains a value equal to <paramref name="value"/>, otherwise false.</returns>
    ///
    /// <example id="contains-1">
    /// <code lang="fsharp">
    /// (99, None) ||> Option.contains // evaluates to false
    /// (99, Some 99) ||> Option.contains // evaluates to true
    /// (99, Some 100) ||> Option.contains // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Contains")>]
    val inline contains: value: 'T -> option: 'T option -> bool when 'T: equality

    /// <summary><c>iter f inp</c> executes <c>match inp with None -> () | Some x -> f x</c>.</summary>
    ///
    /// <param name="action">A function to apply to the option value.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// None |> Option.iter (printfn "%s") // does nothing
    /// Some "Hello world" |> Option.iter (printfn "%s") // prints "Hello world"
    /// </code>
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action: ('T -> unit) -> option: 'T option -> unit

    /// <summary><c>map f inp</c> evaluates to <c>match inp with None -> None | Some x -> Some (f x)</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the option value.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>An option of the input value after applying the mapping function, or None if the input is None.</returns>
    ///
    /// <example id="map-1">
    /// <code lang="fsharp">
    /// None |> Option.map (fun x -> x * 2) // evaluates to None
    /// Some 42 |> Option.map (fun x -> x * 2) // evaluates to Some 84
    /// </code>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> option: 'T option -> 'U option

    /// <summary><c>map f option1 option2</c> evaluates to <c>match option1, option2 with Some x, Some y -> Some (f x y) | _ -> None</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the option values.</param>
    /// <param name="option1">The first option.</param>
    /// <param name="option2">The second option.</param>
    ///
    /// <returns>An option of the input values after applying the mapping function, or None if either input is None.</returns>
    ///
    /// <example id="map2-1">
    /// <code lang="fsharp">
    /// (None, None) ||> Option.map2 (fun x y -> x + y) // evaluates to None
    /// (Some 5, None) ||> Option.map2 (fun x y -> x + y) // evaluates to None
    /// (None, Some 10) ||> Option.map2 (fun x y -> x + y) // evaluates to None
    /// (Some 5, Some 10) ||> Option.map2 (fun x y -> x + y) // evaluates to Some 15
    /// </code>
    /// </example>
    [<CompiledName("Map2")>]
    val map2: mapping: ('T1 -> 'T2 -> 'U) -> option1: 'T1 option -> option2: 'T2 option -> 'U option

    /// <summary><c>map f option1 option2 option3</c> evaluates to <c>match option1, option2, option3 with Some x, Some y, Some z -> Some (f x y z) | _ -> None</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the option values.</param>
    /// <param name="option1">The first option.</param>
    /// <param name="option2">The second option.</param>
    /// <param name="option3">The third option.</param>
    ///
    /// <returns>An option of the input values after applying the mapping function, or None if any input is None.</returns>
    ///
    /// <example id="map3-1">
    /// <code lang="fsharp">
    /// (None, None, None) |||> Option.map3 (fun x y z -> x + y + z) // evaluates to None
    /// (Some 100, None, None) |||> Option.map3 (fun x y z -> x + y + z) // evaluates to None
    /// (None, Some 100, None) |||> Option.map3 (fun x y z -> x + y + z) // evaluates to None
    /// (None, None, Some 100) |||> Option.map3 (fun x y z -> x + y + z) // evaluates to None
    /// (Some 5, Some 100, Some 10) |||> Option.map3 (fun x y z -> x + y + z) // evaluates to Some 115
    /// </code>
    /// </example>
    [<CompiledName("Map3")>]
    val map3:
        mapping: ('T1 -> 'T2 -> 'T3 -> 'U) ->
        option1: 'T1 option ->
        option2: 'T2 option ->
        option3: 'T3 option ->
            'U option

    /// <summary><c>bind f inp</c> evaluates to <c>match inp with None -> None | Some x -> f x</c></summary>
    ///
    /// <param name="binder">A function that takes the value of type T from an option and transforms it into
    /// an option containing a value of type U.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>An option of the output type of the binder.</returns>
    ///
    /// <example id="bind-1">
    /// <code lang="fsharp">
    /// let tryParse (input: string) =
    ///     match System.Int32.TryParse input with
    ///     | true, v -> Some v
    ///     | false, _ -> None
    /// None |> Option.bind tryParse // evaluates to None
    /// Some "42" |> Option.bind tryParse // evaluates to Some 42
    /// Some "Forty-two" |> Option.bind tryParse // evaluates to None
    /// </code>
    /// </example>
    [<CompiledName("Bind")>]
    val bind: binder: ('T -> 'U option) -> option: 'T option -> 'U option

    /// <summary><c>flatten inp</c> evaluates to <c>match inp with None -> None | Some x -> x</c></summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The input value if the value is Some; otherwise, None.</returns>
    ///
    /// <remarks><c>flatten</c> is equivalent to <c>bind id</c>.</remarks>
    ///
    /// <example id="flatten-1">
    /// <code lang="fsharp">
    /// (None: int option option) |> Option.flatten // evaluates to None
    /// (Some ((None: int option))) |> Option.flatten // evaluates to None
    /// (Some (Some 42)) |> Option.flatten // evaluates to Some 42
    /// </code>
    /// </example>
    [<CompiledName("Flatten")>]
    val flatten: option: 'T option option -> 'T option

    /// <summary><c>filter f inp</c> evaluates to <c>match inp with None -> None | Some x -> if f x then Some x else None</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates whether the value contained in the option should remain, or be filtered out.</param>
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The input if the predicate evaluates to true; otherwise, None.</returns>
    ///
    /// <example id="filter-1">
    /// <code lang="fsharp">
    /// None |> Option.filter (fun x -> x >= 5) // evaluates to None
    /// Some 42 |> Option.filter (fun x -> x >= 5) // evaluates to Some 42
    /// Some 4 |> Option.filter (fun x -> x >= 5) // evaluates to None
    /// </code>
    /// </example>
    [<CompiledName("Filter")>]
    val filter: predicate: ('T -> bool) -> option: 'T option -> 'T option

    /// <summary>Convert the option to an array of length 0 or 1.</summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The result array.</returns>
    ///
    /// <example id="toArray-1">
    /// <code lang="fsharp">
    /// (None: int option) |> Option.toArray // evaluates to [||]
    /// Some 42 |> Option.toArray // evaluates to [|42|]
    /// </code>
    /// </example>
    [<CompiledName("ToArray")>]
    val toArray: option: 'T option -> 'T[]

    /// <summary>Convert the option to a list of length 0 or 1.</summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The result list.</returns>
    ///
    /// <example id="toList-1">
    /// <code lang="fsharp">
    /// (None: int option) |> Option.toList // evaluates to []
    /// Some 42 |> Option.toList // evaluates to [42]
    /// </code>
    /// </example>
    [<CompiledName("ToList")>]
    val toList: option: 'T option -> 'T list

    /// <summary>Convert the option to a Nullable value.</summary>
    ///
    /// <param name="option">The input option.</param>
    ///
    /// <returns>The result value.</returns>
    ///
    /// <example id="toNullable-1">
    /// <code lang="fsharp">
    /// (None: int option) |> Option.toNullable // evaluates to new System.Nullable&lt;int&gt;()
    /// Some 42 |> Option.toNullable // evaluates to new System.Nullable(42)
    /// </code>
    /// </example>
    [<CompiledName("ToNullable")>]
    val toNullable: option: 'T option -> Nullable<'T>

    /// <summary>Convert a Nullable value to an option.</summary>
    ///
    /// <param name="value">The input nullable value.</param>
    ///
    /// <returns>The result option.</returns>
    ///
    /// <example id="ofNullable-1">
    /// <code lang="fsharp">
    /// System.Nullable&lt;int&gt;() |> Option.ofNullable // evaluates to None
    /// System.Nullable(42) |> Option.ofNullable // evaluates to Some 42
    /// </code>
    /// </example>
    [<CompiledName("OfNullable")>]
    val ofNullable: value: Nullable<'T> -> 'T option

    /// <summary>Convert a potentially null value to an option.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The result option.</returns>
    ///
    /// <example id="ofObj-1">
    /// <code lang="fsharp">
    /// (null: string) |> Option.ofObj // evaluates to None
    /// "not a null string" |> Option.ofObj // evaluates to (Some "not a null string")
    /// </code>
    /// </example>
    [<CompiledName("OfObj")>]
    val ofObj: value: 'T -> 'T option when 'T: null

    /// <summary>Convert an option to a potentially null value.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The result value, which is null if the input was None.</returns>
    ///
    /// <example id="toObj-1">
    /// <code lang="fsharp">
    /// (None: string option) |> Option.toObj // evaluates to null
    /// Some "not a null string" |> Option.toObj // evaluates to "not a null string"
    /// </code>
    /// </example>
    [<CompiledName("ToObj")>]
    val toObj: value: 'T option -> 'T when 'T: null

/// <summary>Contains operations for working with value options.</summary>
///
/// <category>Options</category>
module ValueOption =
    /// <summary>Returns true if the value option is not ValueNone.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>True if the value option is not ValueNone.</returns>
    ///
    /// <example id="isSome-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.isSome // evaluates to false
    /// ValueSome 42 |> ValueOption.isSome // evaluates to true
    /// </code>
    /// </example>
    [<CompiledName("IsSome")>]
    val inline isSome: voption: 'T voption -> bool

    /// <summary>Returns true if the value option is ValueNone.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>True if the voption is ValueNone.</returns>
    ///
    /// <example id="isNone-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.isNone // evaluates to true
    /// ValueSome 42 |> ValueOption.isNone // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("IsNone")>]
    val inline isNone: voption: 'T voption -> bool

    /// <summary>Gets the value of the value option if the option is <c>ValueSome</c>, otherwise returns the specified default value.</summary>
    ///
    /// <param name="value">The specified default value.</param>
    /// <param name="voption">The input voption.</param>
    ///
    /// <returns>The voption if the voption is ValueSome, else the default value.</returns>
    /// <remarks>Identical to the built-in <see cref="defaultArg"/> operator, except with the arguments swapped.</remarks>
    ///
    /// <example id="defaultValue-1">
    /// <code lang="fsharp">
    /// (99, ValueNone) ||> ValueOption.defaultValue // evaluates to 99
    /// (99, ValueSome 42) ||> ValueOption.defaultValue // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("DefaultValue")>]
    val defaultValue: value: 'T -> voption: 'T voption -> 'T

    /// <summary>Gets the value of the voption if the voption is <c>ValueSome</c>, otherwise evaluates <paramref name="defThunk"/> and returns the result.</summary>
    ///
    /// <param name="defThunk">A thunk that provides a default value when evaluated.</param>
    /// <param name="voption">The input voption.</param>
    ///
    /// <returns>The voption if the voption is ValueSome, else the result of evaluating <paramref name="defThunk"/>.</returns>
    /// <remarks><paramref name="defThunk"/> is not evaluated unless <paramref name="voption"/> is <c>ValueNone</c>.</remarks>
    ///
    /// <example id="defaultWith-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.defaultWith (fun () -> 99) // evaluates to 99
    /// ValueSome 42 |> ValueOption.defaultWith (fun () -> 99) // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("DefaultWith")>]
    val defaultWith: defThunk: (unit -> 'T) -> voption: 'T voption -> 'T

    /// <summary>Returns <paramref name="voption"/> if it is <c>Some</c>, otherwise returns <paramref name="ifNone"/>.</summary>
    ///
    /// <param name="ifNone">The value to use if <paramref name="voption"/> is <c>None</c>.</param>
    /// <param name="voption">The input option.</param>
    ///
    /// <returns>The option if the option is Some, else the alternate option.</returns>
    ///
    /// <example id="orElse-1">
    /// <code lang="fsharp">
    /// ((ValueNone: int ValueOption), ValueNone) ||> ValueOption.orElse // evaluates to ValueNone
    /// (ValueSome 99, ValueNone) ||> ValueOption.orElse // evaluates to ValueSome 99
    /// (ValueNone, ValueSome 42) ||> ValueOption.orElse // evaluates to ValueSome 42
    /// (ValueSome 99, ValueSome 42) ||> ValueOption.orElse // evaluates to ValueSome 42
    /// </code>
    /// </example>
    [<CompiledName("OrElse")>]
    val orElse: ifNone: 'T voption -> voption: 'T voption -> 'T voption

    /// <summary>Returns <paramref name="voption"/> if it is <c>Some</c>, otherwise evaluates <paramref name="ifNoneThunk"/> and returns the result.</summary>
    ///
    /// <param name="ifNoneThunk">A thunk that provides an alternate value option when evaluated.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The voption if the voption is ValueSome, else the result of evaluating <paramref name="ifNoneThunk"/>.</returns>
    /// <remarks><paramref name="ifNoneThunk"/> is not evaluated unless <paramref name="voption"/> is <c>ValueNone</c>.</remarks>
    ///
    /// <example id="orElseWith-1">
    /// <code lang="fsharp">
    /// (ValueNone: int ValueOption) |> ValueOption.orElseWith (fun () -> ValueNone) // evaluates to ValueNone
    /// ValueNone |> ValueOption.orElseWith (fun () -> (ValueSome 99)) // evaluates to ValueSome 99
    /// ValueSome 42 |> ValueOption.orElseWith (fun () -> ValueNone) // evaluates to ValueSome 42
    /// ValueSome 42 |> ValueOption.orElseWith (fun () -> (ValueSome 99)) // evaluates to ValueSome 42
    /// </code>
    /// </example>
    [<CompiledName("OrElseWith")>]
    val orElseWith: ifNoneThunk: (unit -> 'T voption) -> voption: 'T voption -> 'T voption

    /// <summary>Gets the value associated with the option.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The value within the option.</returns>
    /// <exception href="System.ArgumentException">Thrown when the option is ValueNone.</exception>
    ///
    /// <example id="get-1">
    /// <code lang="fsharp">
    /// ValueSome 42 |> ValueOption.get // evaluates to 42
    /// (ValueNone: int ValueOption) |> ValueOption.get // throws exception!
    /// </code>
    /// </example>
    [<CompiledName("GetValue")>]
    val get: voption: 'T voption -> 'T

    /// <summary><c>count inp</c> evaluates to <c>match inp with ValueNone -> 0 | ValueSome _ -> 1</c>.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>A zero if the option is ValueNone, a one otherwise.</returns>
    ///
    /// <example id="count-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.count // evaluates to 0
    /// ValueSome 99 |> ValueOption.count // evaluates to 1
    /// </code>
    /// </example>
    [<CompiledName("Count")>]
    val count: voption: 'T voption -> int

    /// <summary><c>fold f s inp</c> evaluates to <c>match inp with ValueNone -> s | ValueSome x -> f s x</c>.</summary>
    ///
    /// <param name="folder">A function to update the state data when given a value from a value option.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The original state if the option is ValueNone, otherwise it returns the updated state with the folder
    /// and the voption value.</returns>
    ///
    /// <example id="fold-1">
    /// <code lang="fsharp">
    /// (0, ValueNone) ||> ValueOption.fold (fun accum x -> accum + x * 2) // evaluates to 0
    /// (0, ValueSome 1) ||> ValueOption.fold (fun accum x -> accum + x * 2) // evaluates to 2
    /// (10, ValueSome 1) ||> ValueOption.fold (fun accum x -> accum + x * 2) // evaluates to 12
    /// </code>
    /// </example>
    [<CompiledName("Fold")>]
    val fold<'T, 'State> : folder: ('State -> 'T -> 'State) -> state: 'State -> voption: 'T voption -> 'State

    /// <summary><c>fold f inp s</c> evaluates to <c>match inp with ValueNone -> s | ValueSome x -> f x s</c>.</summary>
    ///
    /// <param name="folder">A function to update the state data when given a value from a value option.</param>
    /// <param name="voption">The input value option.</param>
    /// <param name="state">The initial state.</param>
    ///
    /// <returns>The original state if the option is ValueNone, otherwise it returns the updated state with the folder
    /// and the voption value.</returns>
    ///
    /// <example id="foldBack-1">
    /// <code lang="fsharp">
    /// (ValueNone, 0) ||> ValueOption.foldBack (fun x accum -> accum + x * 2) // evaluates to 0
    /// (ValueSome 1, 0) ||> ValueOption.foldBack (fun x accum -> accum + x * 2) // evaluates to 2
    /// (ValueSome 1, 10) ||> ValueOption.foldBack (fun x accum -> accum + x * 2) // evaluates to 12
    /// </code>
    /// </example>
    [<CompiledName("FoldBack")>]
    val foldBack<'T, 'State> : folder: ('T -> 'State -> 'State) -> voption: 'T voption -> state: 'State -> 'State

    /// <summary><c>exists p inp</c> evaluates to <c>match inp with ValueNone -> false | ValueSome x -> p x</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates to a boolean when given a value from the option type.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>False if the option is ValueNone, otherwise it returns the result of applying the predicate
    /// to the option value.</returns>
    ///
    /// <example id="exists-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.exists (fun x -> x >= 5) // evaluates to false
    /// ValueSome 42 |> ValueOption.exists (fun x -> x >= 5) // evaluates to true
    /// ValueSome 4 |> ValueOption.exists (fun x -> x >= 5) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Exists")>]
    val exists: predicate: ('T -> bool) -> voption: 'T voption -> bool

    /// <summary><c>forall p inp</c> evaluates to <c>match inp with ValueNone -> true | ValueSome x -> p x</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates to a boolean when given a value from the value option type.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>True if the option is None, otherwise it returns the result of applying the predicate
    /// to the option value.</returns>
    ///
    /// <example id="forall-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.forall (fun x -> x >= 5) // evaluates to true
    /// ValueSome 42 |> ValueOption.forall (fun x -> x >= 5) // evaluates to true
    /// ValueSome 4 |> ValueOption.forall (fun x -> x >= 5) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("ForAll")>]
    val forall: predicate: ('T -> bool) -> voption: 'T voption -> bool

    /// <summary>Evaluates to true if <paramref name="voption"/> is <c>ValueSome</c> and its value is equal to <paramref name="value"/>.</summary>
    ///
    /// <param name="value">The value to test for equality.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>True if the option is <c>ValueSome</c> and contains a value equal to <paramref name="value"/>, otherwise false.</returns>
    ///
    /// <example id="contains-1">
    /// <code lang="fsharp">
    /// (99, ValueNone) ||> ValueOption.contains // evaluates to false
    /// (99, ValueSome 99) ||> ValueOption.contains // evaluates to true
    /// (99, ValueSome 100) ||> ValueOption.contains // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Contains")>]
    val inline contains: value: 'T -> voption: 'T voption -> bool when 'T: equality

    /// <summary><c>iter f inp</c> executes <c>match inp with ValueNone -> () | ValueSome x -> f x</c>.</summary>
    ///
    /// <param name="action">A function to apply to the voption value.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.iter (printfn "%s") // does nothing
    /// ValueSome "Hello world" |> ValueOption.iter (printfn "%s") // prints "Hello world"
    /// </code>
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action: ('T -> unit) -> voption: 'T voption -> unit

    /// <summary><c>map f inp</c> evaluates to <c>match inp with ValueNone -> ValueNone | ValueSome x -> ValueSome (f x)</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the voption value.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>A value option of the input value after applying the mapping function, or ValueNone if the input is ValueNone.</returns>
    ///
    /// <example id="map-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.map (fun x -> x * 2) // evaluates to ValueNone
    /// ValueSome 42 |> ValueOption.map (fun x -> x * 2) // evaluates to ValueSome 84
    /// </code>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> voption: 'T voption -> 'U voption

    /// <summary><c>map f voption1 voption2</c> evaluates to <c>match voption1, voption2 with ValueSome x, ValueSome y -> ValueSome (f x y) | _ -> ValueNone</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the voption values.</param>
    /// <param name="voption1">The first value option.</param>
    /// <param name="voption2">The second value option.</param>
    ///
    /// <returns>A value option of the input values after applying the mapping function, or ValueNone if either input is ValueNone.</returns>
    ///
    /// <example id="map2-1">
    /// <code lang="fsharp">
    /// (ValueNone, ValueNone) ||> ValueOption.map2 (fun x y -> x + y) // evaluates to ValueNone
    /// (ValueSome 5, ValueNone) ||> ValueOption.map2 (fun x y -> x + y) // evaluates to ValueNone
    /// (ValueNone, ValueSome 10) ||> ValueOption.map2 (fun x y -> x + y) // evaluates to ValueNone
    /// (ValueSome 5, ValueSome 10) ||> ValueOption.map2 (fun x y -> x + y) // evaluates to ValueSome 15
    /// </code>
    /// </example>
    [<CompiledName("Map2")>]
    val map2: mapping: ('T1 -> 'T2 -> 'U) -> voption1: 'T1 voption -> voption2: 'T2 voption -> 'U voption

    /// <summary><c>map f voption1 voption2 voption3</c> evaluates to <c>match voption1, voption2, voption3 with ValueSome x, ValueSome y, ValueSome z -> ValueSome (f x y z) | _ -> ValueNone</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the value option values.</param>
    /// <param name="voption1">The first value option.</param>
    /// <param name="voption2">The second value option.</param>
    /// <param name="voption3">The third value option.</param>
    ///
    /// <returns>A value option of the input values after applying the mapping function, or ValueNone if any input is ValueNone.</returns>
    ///
    /// <example id="map3-1">
    /// <code lang="fsharp">
    /// (ValueNone, ValueNone, ValueNone) |||> ValueOption.map3 (fun x y z -> x + y + z) // evaluates to ValueNone
    /// (ValueSome 100, ValueNone, ValueNone) |||> ValueOption.map3 (fun x y z -> x + y + z) // evaluates to ValueNone
    /// (ValueNone, ValueSome 100, ValueNone) |||> ValueOption.map3 (fun x y z -> x + y + z) // evaluates to ValueNone
    /// (ValueNone, ValueNone, ValueSome 100) |||> ValueOption.map3 (fun x y z -> x + y + z) // evaluates to ValueNone
    /// (ValueSome 5, ValueSome 100, ValueSome 10) |||> ValueOption.map3 (fun x y z -> x + y + z) // evaluates to ValueSome 115
    /// </code>
    /// </example>
    [<CompiledName("Map3")>]
    val map3:
        mapping: ('T1 -> 'T2 -> 'T3 -> 'U) ->
        voption1: 'T1 voption ->
        voption2: 'T2 voption ->
        voption3: 'T3 voption ->
            'U voption

    /// <summary><c>bind f inp</c> evaluates to <c>match inp with ValueNone -> ValueNone | ValueSome x -> f x</c></summary>
    ///
    /// <param name="binder">A function that takes the value of type T from a value option and transforms it into
    /// a value option containing a value of type U.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>An option of the output type of the binder.</returns>
    ///
    /// <example id="bind-1">
    /// <code lang="fsharp">
    /// let tryParse input =
    ///     match System.Int32.TryParse (input: string) with
    ///     | true, v -> ValueSome v
    ///     | false, _ -> ValueNone
    /// ValueNone |> ValueOption.bind tryParse // evaluates to ValueNone
    /// ValueSome "42" |> ValueOption.bind tryParse // evaluates to ValueSome 42
    /// ValueSome "Forty-two" |> ValueOption.bind tryParse // evaluates to ValueNone
    /// </code>
    /// </example>
    [<CompiledName("Bind")>]
    val bind: binder: ('T -> 'U voption) -> voption: 'T voption -> 'U voption

    /// <summary><c>flatten inp</c> evaluates to <c>match inp with ValueNone -> ValueNone | ValueSome x -> x</c></summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The input value if the value is Some; otherwise, ValueNone.</returns>
    /// <remarks><c>flatten</c> is equivalent to <c>bind id</c>.</remarks>
    ///
    /// <example id="flatten-1">
    /// <code lang="fsharp">
    /// (ValueNone: int ValueOption ValueOption) |> ValueOption.flatten // evaluates to ValueNone
    /// (ValueSome ((ValueNone: int ValueOption))) |> ValueOption.flatten // evaluates to ValueNone
    /// (ValueSome (ValueSome 42)) |> ValueOption.flatten // evaluates to ValueSome 42
    /// </code>
    /// </example>
    [<CompiledName("Flatten")>]
    val flatten: voption: 'T voption voption -> 'T voption

    /// <summary><c>filter f inp</c> evaluates to <c>match inp with ValueNone -> ValueNone | ValueSome x -> if f x then ValueSome x else ValueNone</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates whether the value contained in the value option should remain, or be filtered out.</param>
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The input if the predicate evaluates to true; otherwise, ValueNone.</returns>
    ///
    /// <example id="filter-1">
    /// <code lang="fsharp">
    /// ValueNone |> ValueOption.filter (fun x -> x >= 5) // evaluates to ValueNone
    /// ValueSome 42 |> ValueOption.filter (fun x -> x >= 5) // evaluates to ValueSome 42
    /// ValueSome 4 |> ValueOption.filter (fun x -> x >= 5) // evaluates to ValueNone
    /// </code>
    /// </example>
    [<CompiledName("Filter")>]
    val filter: predicate: ('T -> bool) -> voption: 'T voption -> 'T voption

    /// <summary>Convert the value option to an array of length 0 or 1.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The result array.</returns>
    ///
    /// <example id="toArray-1">
    /// <code lang="fsharp">
    /// (ValueNone: int ValueOption) |> ValueOption.toArray // evaluates to [||]
    /// ValueSome 42 |> ValueOption.toArray // evaluates to [|42|]
    /// </code>
    /// </example>
    [<CompiledName("ToArray")>]
    val toArray: voption: 'T voption -> 'T[]

    /// <summary>Convert the value option to a list of length 0 or 1.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The result list.</returns>
    ///
    /// <example id="toList-1">
    /// <code lang="fsharp">
    /// (ValueNone: int ValueOption) |> ValueOption.toList // evaluates to []
    /// ValueSome 42 |> ValueOption.toList // evaluates to [42]
    /// </code>
    /// </example>
    [<CompiledName("ToList")>]
    val toList: voption: 'T voption -> 'T list

    /// <summary>Convert the value option to a Nullable value.</summary>
    ///
    /// <param name="voption">The input value option.</param>
    ///
    /// <returns>The result value.</returns>
    ///
    /// <example id="toNullable-1">
    /// <code lang="fsharp">
    /// (ValueNone: int ValueOption) |> ValueOption.toNullable // evaluates to new System.Nullable&lt;int&gt;()
    /// ValueSome 42 |> ValueOption.toNullable // evaluates to new System.Nullable(42)
    /// </code>
    /// </example>
    [<CompiledName("ToNullable")>]
    val toNullable: voption: 'T voption -> Nullable<'T>

    /// <summary>Convert a Nullable value to a value option.</summary>
    ///
    /// <param name="value">The input nullable value.</param>
    ///
    /// <returns>The result value option.</returns>
    ///
    /// <example id="ofNullable-1">
    /// <code lang="fsharp">
    /// System.Nullable&lt;int&gt;() |> ValueOption.ofNullable // evaluates to ValueNone
    /// System.Nullable(42) |> ValueOption.ofNullable // evaluates to ValueSome 42
    /// </code>
    /// </example>
    [<CompiledName("OfNullable")>]
    val ofNullable: value: Nullable<'T> -> 'T voption

    /// <summary>Convert a potentially null value to a value option.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The result value option.</returns>
    ///
    /// <example id="ofObj-1">
    /// <code lang="fsharp">
    /// (null: string) |> ValueOption.ofObj // evaluates to ValueNone
    /// "not a null string" |> ValueOption.ofObj // evaluates to (ValueSome "not a null string")
    /// </code>
    /// </example>
    [<CompiledName("OfObj")>]
    val ofObj: value: 'T -> 'T voption when 'T: null

    /// <summary>Convert an option to a potentially null value.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The result value, which is null if the input was ValueNone.</returns>
    ///
    /// <example id="toObj-1">
    /// <code lang="fsharp">
    /// (ValueNone: string ValueOption) |> ValueOption.toObj // evaluates to null
    /// ValueSome "not a null string" |> ValueOption.toObj // evaluates to "not a null string"
    /// </code>
    /// </example>
    [<CompiledName("ToObj")>]
    val toObj: value: 'T voption -> 'T when 'T: null
