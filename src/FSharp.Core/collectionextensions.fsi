// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open Microsoft.FSharp.Core
open System.ComponentModel

/// <summary>A set of extra operators and functions. This module is automatically opened in all F# code.</summary>
///
/// <category>Basic Operators</category>
[<AutoOpen>]
module CollectionExtensions =

    /// <summary>Builds a set from a sequence of objects. The objects are indexed using generic comparison.</summary>
    ///
    /// <param name="elements">The input sequence of elements.</param>
    ///
    /// <returns>The created set.</returns>
    ///
    /// <example id="set-1">
    /// <code lang="fsharp">
    /// let values = set [ 1; 2; 3; 5; 7; 11 ]
    /// </code>
    /// Evaluates to a set containing the given numbers.
    /// </example>
    [<CompiledName("CreateSet")>]
    val inline set: elements: seq<'T> -> Set<'T>

    /// <summary>Builds a read-only lookup table from a sequence of key/value pairs. The key objects are indexed using generic hashing and equality.</summary>
    /// 
    /// <example id="dict-1">
    /// <code lang="fsharp">
    /// let table = dict [ (1, 100); (2, 200) ]
    ///
    /// table[1]
    /// </code>
    /// Evaluates to <c>100</c>.
    /// </example>
    /// 
    /// <example id="dict-2">
    /// <code lang="fsharp">
    /// let table = dict [ (1, 100); (2, 200) ]
    ///
    /// table[3]
    /// </code>
    /// Throws <c>System.Collections.Generic.KeyNotFoundException</c>.
    /// </example>
    [<CompiledName("CreateDictionary")>]
    val dict: keyValuePairs: seq<'Key * 'Value> -> System.Collections.Generic.IDictionary<'Key,'Value> when 'Key : equality

    /// <summary>Builds a read-only lookup table from a sequence of key/value pairs. The key objects are indexed using generic hashing and equality.</summary>
    /// 
    /// <example id="readonlydict-1">
    /// <code lang="fsharp">
    /// let table = readOnlyDict [ (1, 100); (2, 200) ]
    ///
    /// table[1]
    /// </code>
    /// Evaluates to <c>100</c>.
    /// </example>
    /// 
    /// <example id="readonlydict-2">
    /// <code lang="fsharp">
    /// let table = readOnlyDict [ (1, 100); (2, 200) ]
    ///
    /// table[3]
    /// </code>
    /// Throws <c>System.Collections.Generic.KeyNotFoundException</c>.
    /// </example>
    [<CompiledName("CreateReadOnlyDictionary")>]
    val readOnlyDict: keyValuePairs: seq<'Key * 'Value> -> System.Collections.Generic.IReadOnlyDictionary<'Key,'Value> when 'Key : equality

    /// <summary>Builds a 2D array from a sequence of sequences of elements.</summary>
    /// 
    /// <example id="array2d-1">
    /// <code lang="fsharp">
    /// array2D [ [ 1.0; 2.0 ]; [ 3.0; 4.0 ] ]
    /// </code>
    /// Evaluates to a 2x2 zero-based array with contents <c>[[1.0; 2.0]; [3.0; 4.0]]</c>
    /// </example>
    [<CompiledName("CreateArray2D")>]
    val array2D: rows: seq<#seq<'T>> -> 'T[,]
