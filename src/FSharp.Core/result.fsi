// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

open Microsoft.FSharp.Collections

/// <summary>Contains operations for working with values of type <see cref="T:Microsoft.FSharp.Core.FSharpResult`2"/>.</summary>
///
/// <category>Choices and Results</category>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =

    /// <summary><c>map f inp</c> evaluates to <c>match inp with Error e -> Error e | Ok x -> Ok (f x)</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the OK result value.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>A result of the input value after applying the mapping function, or Error if the input is Error.</returns>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Ok 1 |> Result.map (fun x -> "perfect") // evaluates to Ok "perfect"
    ///
    /// Error "message" |> Result.map (fun x -> "perfect") // evaluates to Error "message"
    /// </code>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> result: Result<'T, 'TError> -> Result<'U, 'TError>

    /// <summary><c>map f inp</c> evaluates to <c>match inp with Error x -> Error (f x) | Ok v -> Ok v</c>.</summary>
    ///
    /// <param name="mapping">A function to apply to the Error result value.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>A result of the error value after applying the mapping function, or Ok if the input is Ok.</returns>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// Ok 1 |> Result.mapError (fun x -> "bar") // evaluates to Ok 1
    ///
    /// Error "foo" |> Result.mapError (fun x -> "bar") // evaluates to Error "bar"
    /// </code>
    /// </example>
    [<CompiledName("MapError")>]
    val mapError: mapping: ('TError -> 'U) -> result: Result<'T, 'TError> -> Result<'T, 'U>

    /// <summary><c>bind f inp</c> evaluates to <c>match inp with Error e -> Error e | Ok x -> f x</c></summary>
    ///
    /// <param name="binder">A function that takes the value of type T from a result and transforms it into
    /// a result containing a value of type U.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>A result of the output type of the binder.</returns>
    ///
    /// <example>
    /// <code lang="fsharp">
    /// let tryParse (input: string) =
    ///     match System.Int32.TryParse input with
    ///     | true, v -> Ok v
    ///     | false, _ -> Error "couldn't parse"
    ///
    /// Error "message" |> Result.bind tryParse // evaluates to Error "message"
    ///
    /// Ok "42" |> Result.bind tryParse // evaluates to Ok 42
    ///
    /// Ok "Forty-two" |> Result.bind tryParse // evaluates to Error "couldn't parse"
    /// </code>
    /// </example>
    [<CompiledName("Bind")>]
    val bind: binder: ('T -> Result<'U, 'TError>) -> result: Result<'T, 'TError> -> Result<'U, 'TError>

    /// <summary>Returns true if the result is Ok.</summary>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>True if the result is OK.</returns>
    ///
    /// <example id="isOk-1">
    /// <code lang="fsharp">
    /// Ok 42 |> Result.isOk // evaluates to true
    /// Error 42 |> Result.isOk // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("IsOk")>]
    val inline isOk: result: Result<'T, 'Error> -> bool

    /// <summary>Returns true if the result is Error.</summary>
    ///
    /// <param name="result">The input result.</param>
    ///
    /// <returns>True if the result is Error.</returns>
    ///
    /// <example id="isError-1">
    /// <code lang="fsharp">
    /// Ok 42 |> Result.isError // evaluates to false
    /// Error 42 |> Result.isError // evaluates to true
    /// </code>
    /// </example>
    [<CompiledName("IsError")>]
    val inline isError: result: Result<'T, 'Error> -> bool

    /// <summary>Gets the value of the result if the result is <c>Ok</c>, otherwise returns the specified default value.</summary>
    ///
    /// <param name="value">The specified default value.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The result if the result is Ok, else the default value.</returns>
    ///
    /// <example id="defaultValue-1">
    /// <code lang="fsharp">
    /// Result.defaultValue 2 (Error 3) // evaluates to 2
    /// Result.defaultValue 2 (Ok 1) // evaluates to 1
    /// </code>
    /// </example>
    [<CompiledName("DefaultValue")>]
    val defaultValue: value: 'T -> result: Result<'T, 'Error> -> 'T

    /// <summary>Gets the value of the result if the result is <c>Ok</c>, otherwise evaluates <paramref name="defThunk"/> and returns the result.</summary>
    ///
    /// <param name="defThunk">A thunk that provides a default value when evaluated.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The result if the result is Ok, else the result of evaluating <paramref name="defThunk"/>.</returns>
    /// <remarks><paramref name="defThunk"/> is not evaluated unless <paramref name="result"/> is <c>Error</c>.</remarks>
    ///
    /// <example id="defaultWith-1">
    /// <code lang="fsharp">
    /// Ok 1 |> Result.defaultWith (fun error -> 99) // evaluates to 1
    /// Error 2 |> Result.defaultWith (fun error -> 99) // evaluates to 99
    /// </code>
    /// </example>
    [<CompiledName("DefaultWith")>]
    val defaultWith: defThunk: ('Error -> 'T) -> result: Result<'T, 'Error> -> 'T

    /// <summary><c>count inp</c> evaluates to <c>match inp with Error _ -> 0 | Ok _ -> 1</c>.</summary>
    ///
    /// <param name="result">The input result.</param>
    ///
    /// <returns>A zero if the result is Error, a one otherwise.</returns>
    ///
    /// <example id="count-1">
    /// <code lang="fsharp">
    /// Error 99 |> Result.count // evaluates to 0
    /// Ok 99 |> Result.count // evaluates to 1
    /// </code>
    /// </example>
    [<CompiledName("Count")>]
    val count: result: Result<'T, 'Error> -> int

    /// <summary><c>fold f s inp</c> evaluates to <c>match inp with Error _ -> s | Ok x -> f s x</c>.</summary>
    ///
    /// <param name="folder">A function to update the state data when given a value from an result.</param>
    /// <param name="state">The initial state.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The original state if the result is Error, otherwise it returns the updated state with the folder
    /// and the result value.</returns>
    ///
    /// <example id="fold-1">
    /// <code lang="fsharp">
    /// (0, Error 2) ||> Result.fold (fun accum x -> accum + x * 2) // evaluates to 0
    /// (0, Ok 1) ||> Result.fold (fun accum x -> accum + x * 2) // evaluates to 2
    /// (10, Ok 1) ||> Result.fold (fun accum x -> accum + x * 2) // evaluates to 12
    /// </code>
    /// </example>
    [<CompiledName("Fold")>]
    val fold<'T, 'Error, 'State> :
        folder: ('State -> 'T -> 'State) -> state: 'State -> result: Result<'T, 'Error> -> 'State

    /// <summary><c>fold f inp s</c> evaluates to <c>match inp with Error _ -> s | Ok x -> f x s</c>.</summary>
    ///
    /// <param name="folder">A function to update the state data when given a value from an result.</param>
    /// <param name="result">The input result.</param>
    /// <param name="state">The initial state.</param>
    ///
    /// <returns>The original state if the result is Error, otherwise it returns the updated state with the folder
    /// and the result value.</returns>
    ///
    /// <example id="foldBack-1">
    /// <code lang="fsharp">
    /// (Error 2, 0) ||> Result.foldBack (fun x accum -> accum + x * 2) // evaluates to 0
    /// (Ok 1, 0) ||> Result.foldBack (fun x accum -> accum + x * 2) // evaluates to 2
    /// (Ok 1, 10) ||> Result.foldBack (fun x accum -> accum + x * 2) // evaluates to 12
    /// </code>
    /// </example>
    [<CompiledName("FoldBack")>]
    val foldBack<'T, 'Error, 'State> :
        folder: ('T -> 'State -> 'State) -> result: Result<'T, 'Error> -> state: 'State -> 'State

    /// <summary><c>exists p inp</c> evaluates to <c>match inp with Error _ -> false | Ok x -> p x</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates to a boolean when given a value from the result type.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>False if the result is Error, otherwise it returns the result of applying the predicate
    /// to the result value.</returns>
    ///
    /// <example id="exists-1">
    /// <code lang="fsharp">
    /// Error 6 |> Result.exists (fun x -> x >= 5) // evaluates to false
    /// Ok 42 |> Result.exists (fun x -> x >= 5) // evaluates to true
    /// Ok 4 |> Result.exists (fun x -> x >= 5) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Exists")>]
    val exists: predicate: ('T -> bool) -> result: Result<'T, 'Error> -> bool

    /// <summary><c>forall p inp</c> evaluates to <c>match inp with Error _ -> true | Ok x -> p x</c>.</summary>
    ///
    /// <param name="predicate">A function that evaluates to a boolean when given a value from the result type.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>True if the result is Error, otherwise it returns the result of applying the predicate
    /// to the result value.</returns>
    ///
    /// <example id="forall-1">
    /// <code lang="fsharp">
    /// Error 1 |> Result.forall (fun x -> x >= 5) // evaluates to true
    /// Ok 42 |> Result.forall (fun x -> x >= 5) // evaluates to true
    /// Ok 4 |> Result.forall (fun x -> x >= 5) // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("ForAll")>]
    val forall: predicate: ('T -> bool) -> result: Result<'T, 'Error> -> bool

    /// <summary>Evaluates to true if <paramref name="result"/> is <c>Ok</c> and its value is equal to <paramref name="value"/>.</summary>
    ///
    /// <param name="value">The value to test for equality.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <returns>True if the result is <c>Ok</c> and contains a value equal to <paramref name="value"/>, otherwise false.</returns>
    ///
    /// <example id="contains-1">
    /// <code lang="fsharp">
    /// (99, Error 99) ||> Result.contains // evaluates to false
    /// (99, Ok 99) ||> Result.contains // evaluates to true
    /// (99, Ok 100) ||> Result.contains // evaluates to false
    /// </code>
    /// </example>
    [<CompiledName("Contains")>]
    val inline contains: value: 'T -> result: Result<'T, 'Error> -> bool when 'T: equality

    /// <summary><c>iter f inp</c> executes <c>match inp with Error _ -> () | Ok x -> f x</c>.</summary>
    ///
    /// <param name="action">A function to apply to the result value.</param>
    /// <param name="result">The input result.</param>
    ///
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// Error "Hello world" |> Result.iter (printfn "%s") // does nothing
    /// Ok "Hello world" |> Result.iter (printfn "%s") // prints "Hello world"
    /// </code>
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action: ('T -> unit) -> result: Result<'T, 'Error> -> unit

    /// <summary>Convert the result to an array of length 0 or 1.</summary>
    ///
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The result array.</returns>
    ///
    /// <example id="toArray-1">
    /// <code lang="fsharp">
    /// Error 42 |> Result.toArray // evaluates to [||]
    /// Ok 42 |> Result.toArray // evaluates to [| 42 |]
    /// </code>
    /// </example>
    [<CompiledName("ToArray")>]
    val toArray: result: Result<'T, 'Error> -> 'T[]

    /// <summary>Convert the result to a list of length 0 or 1.</summary>
    ///
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The result list.</returns>
    ///
    /// <example id="toList-1">
    /// <code lang="fsharp">
    /// Error 42 |> Result.toList // evaluates to []
    /// Ok 42 |> Result.toList // evaluates to [ 42 ]
    /// </code>
    /// </example>
    [<CompiledName("ToList")>]
    val toList: result: Result<'T, 'Error> -> List<'T>

    /// <summary>Convert the result to an Option value.</summary>
    ///
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The option value.</returns>
    ///
    /// <example id="toOption-1">
    /// <code lang="fsharp">
    /// Error 42 |> Result.toOption // evaluates to None
    /// Ok 42 |> Result.toOption // evaluates to Some 42
    /// </code>
    /// </example>
    [<CompiledName("ToOption")>]
    val toOption: result: Result<'T, 'Error> -> Option<'T>

    /// <summary>Convert the result to an Option value.</summary>
    ///
    /// <param name="result">The input result.</param>
    ///
    /// <returns>The result value.</returns>
    ///
    /// <example id="toValueOption-1">
    /// <code lang="fsharp">
    /// Error 42 |> Result.toOption // evaluates to ValueNone
    /// Ok 42 |> Result.toOption // evaluates to ValueSome 42
    /// </code>
    /// </example>
    [<CompiledName("ToValueOption")>]
    val toValueOption: result: Result<'T, 'Error> -> ValueOption<'T>
