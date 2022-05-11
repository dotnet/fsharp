// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core

/// <summary>Contains operations for working with 2-dimensional arrays.</summary>
///
/// <remarks>
///  <para>See also <a href="https://docs.microsoft.com/dotnet/fsharp/language-reference/arrays">F# Language Guide - Arrays</a>.</para>
///
/// <para>F# and CLI multi-dimensional arrays are typically zero-based.
/// However, CLI multi-dimensional arrays used in conjunction with external
/// libraries (e.g. libraries associated with Visual Basic) be
/// non-zero based, using a potentially different base for each dimension.
/// The operations in this module will accept such arrays, and
/// the basing on an input array will be propagated to a matching output
/// array on the <c>Array2D.map</c> and <c>Array2D.mapi</c> operations.
/// Non-zero-based arrays can also be created using <c>Array2D.zeroCreateBased</c>,
/// <c>Array2D.createBased</c> and <c>Array2D.initBased</c>.</para>
/// </remarks>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Array2D =

    /// <summary>Fetches the base-index for the first dimension of the array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The base-index of the first dimension of the array.</returns>
    ///
    /// <example id="base1-1">Create a 10x10 1-based array:
    /// <code>
    /// open System
    ///
    /// let array = Array.CreateInstance(typeof&lt;int&gt;, [| 10; 10 |], [| 1; 1 |]) :?> int[,]
    ///
    /// array |> Array2D.base1
    /// </code>
    /// Evaluates to <c>1</c>.
    /// </example>
    [<CompiledName("Base1")>]
    val base1: array: 'T [,] -> int

    /// <summary>Fetches the base-index for the second dimension of the array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The base-index of the second dimension of the array.</returns>
    ///
    /// <example id="base2-1">Create a 10x10 1-based array:
    /// <code>
    /// open System
    ///
    /// let array = Array.CreateInstance(typeof&lt;int&gt;, [| 10; 10 |], [| 1; 1 |]) :?> int[,]
    ///
    /// array |> Array2D.base2
    /// </code>
    /// Evaluates to <c>1</c>.
    /// </example>
    [<CompiledName("Base2")>]
    val base2: array: 'T [,] -> int

    /// <summary>Builds a new array whose elements are the same as the input array.</summary>
    ///
    /// <remarks>For non-zero-based arrays the basing on an input array will be propagated to the output
    /// array.</remarks>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>A copy of the input array.</returns>
    ///
    /// <example id="copy-1">
    /// <code>
    /// open System
    ///
    /// let array = Array2D.zeroCreate&lt;int&gt; 10 10
    ///
    /// array |> Array2D.copy
    /// </code>
    /// Evaluates to a new copy of the 10x10 array.
    /// </example>
    [<CompiledName("Copy")>]
    val copy: array: 'T [,] -> 'T [,]

    /// <summary>Reads a range of elements from the first array and write them into the second.</summary>
    ///
    /// <param name="source">The source array.</param>
    /// <param name="sourceIndex1">The first-dimension index to begin copying from in the source array.</param>
    /// <param name="sourceIndex2">The second-dimension index to begin copying from in the source array.</param>
    /// <param name="target">The target array.</param>
    /// <param name="targetIndex1">The first-dimension index to begin copying into in the target array.</param>
    /// <param name="targetIndex2">The second-dimension index to begin copying into in the target array.</param>
    /// <param name="length1">The number of elements to copy across the first dimension of the arrays.</param>
    /// <param name="length2">The number of elements to copy across the second dimension of the arrays.</param>
    /// <exception cref="T:System.ArgumentException">Thrown when any of the indices are negative or if either of
    /// the counts are larger than the dimensions of the array allow.</exception>
    ///
    /// <remarks>
    /// Slicing syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let source = array2D [ [ 3; 4 ]; [ 13; 14 ] ]
    /// let target = array2D [ [ 2; 2; 2 ]; [ 12; 12; 12 ] ]
    /// target[0..1,1..2] &lt;- source
    /// </code>
    /// </remarks>
    ///
    /// <example id="blit-1">
    /// <code lang="fsharp">
    /// let source = array2D [ [ 3; 4 ]; [ 13; 14 ] ]
    /// let target = array2D [ [ 2; 2; 2 ]; [ 12; 12; 12 ] ]
    ///
    /// Array2D.blit source 0 0 target 0 1 2 2
    /// </code>
    /// After evaluation <c>target</c> contains <c>[ [ 2; 3; 4 ]; [ 12; 13; 14 ] ]</c>.
    /// </example>
    [<CompiledName("CopyTo")>]
    val blit:
        source: 'T [,] ->
        sourceIndex1: int ->
        sourceIndex2: int ->
        target: 'T [,] ->
        targetIndex1: int ->
        targetIndex2: int ->
        length1: int ->
        length2: int ->
            unit

    /// <summary>Creates an array given the dimensions and a generator function to compute the elements.</summary>
    ///
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    /// <param name="initializer">A function to produce elements of the array given the two indices.</param>
    ///
    /// <returns>The generated array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when either of the lengths is negative.</exception>
    ///
    /// <example id="init-1">
    /// <code lang="fsharp">
    /// Array2D.init 2 3 (fun i j -> i + j)
    /// </code>
    /// Evaluates to a 2x3 array with contents <c>[[0; 1; 2]; [1; 2; 3]]</c>
    /// </example>
    ///
    [<CompiledName("Initialize")>]
    val init: length1: int -> length2: int -> initializer: (int -> int -> 'T) -> 'T [,]

    /// <summary>Creates an array whose elements are all initially the given value.</summary>
    ///
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    /// <param name="value">The value to populate the new array.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when length1 or length2 is negative.</exception>
    ///
    /// <example id="create-1">
    /// <code lang="fsharp">
    /// Array2D.create 2 3 1
    /// </code>
    /// Evaluates to a 2x3 array with contents <c>[[1; 1; 1]; [1; 1; 1]]</c>
    /// </example>
    ///
    [<CompiledName("Create")>]
    val create: length1: int -> length2: int -> value: 'T -> 'T [,]

    /// <summary>Creates an array where the entries are initially Unchecked.defaultof&lt;'T&gt;.</summary>
    ///
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when length1 or length2 is negative.</exception>
    ///
    /// <example id="zeroCreate-1">
    /// <code lang="fsharp">
    /// Array2D.zeroCreate 2 3
    /// </code>
    /// Evaluates to a 2x3 array with contents <c>[[0; 0; 0]; [0; 0; 0]]</c>
    /// </example>
    ///
    [<CompiledName("ZeroCreate")>]
    val zeroCreate: length1: int -> length2: int -> 'T [,]

    /// <summary>Creates a based array given the dimensions and a generator function to compute the elements.</summary>
    ///
    /// <param name="base1">The base for the first dimension of the array.</param>
    /// <param name="base2">The base for the second dimension of the array.</param>
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    /// <param name="initializer">A function to produce elements of the array given the two indices.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when base1, base2, length1, or length2 is negative.</exception>
    ///
    /// <example id="initbased-1">
    /// <code lang="fsharp">
    /// Array2D.initBased 1 1 2 3 (fun i j -> i + j)
    /// </code>
    /// Evaluates to a 2x3 1-based array with contents <c>[[2; 3; 4]; [3; 4; 5]]</c>
    /// </example>
    ///
    [<CompiledName("InitializeBased")>]
    val initBased: base1: int -> base2: int -> length1: int -> length2: int -> initializer: (int -> int -> 'T) -> 'T [,]

    /// <summary>Creates a based array whose elements are all initially the given value.</summary>
    ///
    /// <param name="base1">The base for the first dimension of the array.</param>
    /// <param name="base2">The base for the second dimension of the array.</param>
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    /// <param name="initial">The value to populate the new array.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when base1, base2, length1, or length2 is negative.</exception>
    ///
    /// <example id="createbased-1">
    /// <code lang="fsharp">
    /// Array2D.createBased 1 1 2 3 1
    /// </code>
    /// Evaluates to a 2x3 1-based array with contents <c>[[1; 1; 1]; [1; 1; 1]]</c>
    /// </example>
    ///
    [<CompiledName("CreateBased")>]
    val createBased: base1: int -> base2: int -> length1: int -> length2: int -> initial: 'T -> 'T [,]

    /// <summary>Creates a based array where the entries are initially Unchecked.defaultof&lt;'T&gt;.</summary>
    ///
    /// <param name="base1">The base for the first dimension of the array.</param>
    /// <param name="base2">The base for the second dimension of the array.</param>
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when base1, base2, length1, or length2 is negative.</exception>
    ///
    /// <example id="zerocreatebased-1">
    /// <code lang="fsharp">
    /// Array2D.zeroCreateBased 1 1 2 3
    /// </code>
    /// Evaluates to a 2x3 1-based array with contents <c>[[0; 0; 0]; [0; 0; 0]]</c>
    /// </example>
    ///
    [<CompiledName("ZeroCreateBased")>]
    val zeroCreateBased: base1: int -> base2: int -> length1: int -> length2: int -> 'T [,]

    /// <summary>Applies the given function to each element of the array.</summary>
    ///
    /// <param name="action">A function to apply to each element of the array.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// let inputs = array2D [ [ 3; 4 ]; [ 13; 14 ] ]
    ///
    /// inputs |> Array2D.iter (fun v -> printfn $"value = {v}")
    /// </code>
    /// Evaluates to <c>unit</c> and prints
    /// <code>
    /// value = 3
    /// value = 4
    /// value = 13
    /// value = 14
    /// </code>
    /// in the console.
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action: ('T -> unit) -> array: 'T [,] -> unit

    /// <summary>Applies the given function to each element of the array.  The integer indices passed to the
    /// function indicates the index of element.</summary>
    ///
    /// <param name="action">A function to apply to each element of the array with the indices available as an argument.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <example id="iteri-1">
    /// <code lang="fsharp">
    /// let inputs = array2D [ [ 3; 4 ]; [ 13; 14 ] ]
    ///
    /// inputs |> Array2D.iteri (fun i j v -> printfn $"value at ({i},{j}) = {v}")
    /// </code>
    /// Evaluates to <c>unit</c> and prints
    /// <code>
    /// value at (0,0) = 3
    /// value at (0,1) = 4
    /// value at (1,0) = 13
    /// value at (1,1) = 14
    /// </code>
    /// in the console.
    /// </example>
    [<CompiledName("IterateIndexed")>]
    val iteri: action: (int -> int -> 'T -> unit) -> array: 'T [,] -> unit

    /// <summary>Returns the length of an array in the first dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the first dimension.</returns>
    ///
    /// <example id="length1-1">
    /// <code>
    /// let array = array2D [ [ 3; 4; 5 ]; [ 13; 14; 15 ] ]
    ///
    /// array |> Array2D.length1
    /// </code>
    /// Evaluates to <c>2</c>.
    /// </example>
    [<CompiledName("Length1")>]
    val length1: array: 'T [,] -> int

    /// <summary>Returns the length of an array in the second dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the second dimension.</returns>
    ///
    /// <example id="length2-1">
    /// <code>
    /// let array = array2D [ [ 3; 4; 5 ]; [ 13; 14; 15 ] ]
    ///
    /// array |> Array2D.length2
    /// </code>
    /// Evaluates to <c>3</c>.
    /// </example>
    [<CompiledName("Length2")>]
    val length2: array: 'T [,] -> int

    /// <summary>Builds a new array whose elements are the results of applying the given function
    /// to each of the elements of the array.</summary>
    ///
    /// <remarks>For non-zero-based arrays the basing on an input array will be propagated to the output
    /// array.</remarks>
    ///
    /// <param name="mapping">A function that is applied to transform each item of the input array.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <returns>An array whose elements have been transformed by the given mapping.</returns>
    ///
    /// <example id="map-1">
    /// <code lang="fsharp">
    /// let inputs = array2D [ [ 3; 4 ]; [ 13; 14 ] ]
    ///
    /// inputs |> Array2D.map (fun v -> 2 * v)
    /// </code>
    /// Evaluates to a 2x2 array with contents <c>[[6; 8;]; [26; 28]]</c>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> array: 'T [,] -> 'U [,]

    /// <summary>Builds a new array whose elements are the results of applying the given function
    /// to each of the elements of the array. The integer indices passed to the
    /// function indicates the element being transformed.</summary>
    ///
    /// <remarks>For non-zero-based arrays the basing on an input array will be propagated to the output
    /// array.</remarks>
    ///
    /// <param name="mapping">A function that is applied to transform each element of the array.  The two integers
    /// provide the index of the element.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <returns>An array whose elements have been transformed by the given mapping.</returns>
    ///
    /// <example id="mapi-1">
    /// <code lang="fsharp">
    /// let inputs = array2D [ [ 3; 4 ]; [ 13; 14 ] ]
    ///
    /// inputs |> Array2D.mapi (fun i j v -> i + j + v)
    /// </code>
    /// Evaluates to a 2x2 array with contents <c>[[3; 5;]; [14; 16]]</c>
    /// </example>
    [<CompiledName("MapIndexed")>]
    val mapi: mapping: (int -> int -> 'T -> 'U) -> array: 'T [,] -> 'U [,]

    /// <summary>Builds a new array whose elements are the same as the input array but
    /// where a non-zero-based input array generates a corresponding zero-based
    /// output array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The zero-based output array.</returns>
    ///
    /// <example id="rebase-1">
    /// <code lang="fsharp">
    /// let inputs = Array2D.createBased 1 1 2 3 1
    ///
    /// inputs |> Array2D.rebase
    /// </code>
    /// Evaluates to a 2x2 zero-based array with contents <c>[[1; 1]; [1; 1]]</c>
    /// </example>
    [<CompiledName("Rebase")>]
    val rebase: array: 'T [,] -> 'T [,]

    /// <summary>Sets the value of an element in an array. You can also use the syntax <c>array.[index1,index2] &lt;- value</c>.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="value">The value to set in the array.</param>
    ///
    /// <exception cref="T:System.ArgumentException">Thrown when the indices are negative or exceed the bounds of the array.</exception>
    ///
    /// <remarks>
    /// Indexer syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let array = Array2D.zeroCreate 2 2
    ///
    /// array[0,1] &lt;- 4.0
    /// </code>
    /// </remarks>
    ///
    /// <example id="set-1">
    /// <code lang="fsharp">
    /// let array = Array2D.zeroCreate 2 2
    ///
    /// Array2D.set array 0 1 4.0
    /// </code>
    /// After evaluation <c>array</c> is a 2x2 array with contents <c>[[0.0; 4.0]; [0.0; 0.0]]</c>
    /// </example>
    [<CompiledName("Set")>]
    val set: array: 'T [,] -> index1: int -> index2: int -> value: 'T -> unit

    /// <summary>Fetches an element from a 2D array. You can also use the syntax <c>array.[index1,index2]</c>.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    ///
    /// <returns>The value of the array at the given index.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when the indices are negative or exceed the bounds of the array.</exception>
    ///
    /// <remarks>
    /// Indexer syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let array = array2D [ [ 1.0; 2.0 ]; [ 3.0; 4.0 ] ]
    ///
    /// array[0,1]
    /// </code>
    /// Evaluates to <c>2.0</c>.
    /// </remarks>
    ///
    /// <example id="set-1">
    /// <code lang="fsharp">
    /// let array = array2D [ [ 1.0; 2.0 ]; [ 3.0; 4.0 ] ]
    ///
    /// Array2D.get array 0 1
    /// </code>
    /// Evaluates to <c>2.0</c>.
    /// </example>
    [<CompiledName("Get")>]
    val get: array: 'T [,] -> index1: int -> index2: int -> 'T
