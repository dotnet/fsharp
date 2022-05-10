// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators

/// <summary>Contains operations for working with rank 3 arrays.</summary>
///
/// <remarks>
///  See also <a href="https://docs.microsoft.com/dotnet/fsharp/language-reference/arrays">F# Language Guide - Arrays</a>.
/// </remarks>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Array3D =

    /// <summary>Creates an array whose elements are all initially the given value.</summary>
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="initial">The value of the array elements.</param>
    ///
    /// <returns>The created array.</returns>
    ///
    /// <example id="create-1">
    /// <code lang="fsharp">
    /// Array3D.create 2 2 3 1
    /// </code>
    /// Evaluates to a 2x3 array with contents <c>[[[1; 1; 1]; [1; 1; 1]]; [[1; 1; 1]; [1; 1; 1]]]</c>
    /// </example>
    ///
    [<CompiledName("Create")>]
    val create: length1: int -> length2: int -> length3: int -> initial: 'T -> 'T [,,]

    /// <summary>Creates an array given the dimensions and a generator function to compute the elements.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="initializer">The function to create an initial value at each index into the array.</param>
    ///
    /// <returns>The created array.</returns>
    ///
    /// <example id="init-1">
    /// <code lang="fsharp">
    /// Array3D.init 2 2 3 (fun i j k -> 100*i + 10*j + k)
    /// </code>
    /// Evaluates to a 2x2x3 array with contents <c>[[[0; 1; 2]; [10; 11; 12]]; [[100; 101; 102]; [110; 111; 112]]]</c>
    /// </example>
    [<CompiledName("Initialize")>]
    val init: length1: int -> length2: int -> length3: int -> initializer: (int -> int -> int -> 'T) -> 'T [,,]

    /// <summary>Fetches an element from a 3D array. You can also use the syntax 'array.[index1,index2,index3]'</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="index3">The index along the third dimension.</param>
    ///
    /// <returns>The value at the given index.</returns>
    ///
    /// <remarks>
    /// Indexer syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let array = Array3D.init 2 3 3 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array[0,2,1]
    /// </code>
    /// Evaluates to <c>11</c>.
    /// </remarks>
    ///
    /// <example id="set-1">
    /// <code lang="fsharp">
    /// let array = Array3D.init 2 3 3 (fun i j k -> 100*i + 10*j + k)
    ///
    /// Array3D.get array 0 2 1
    /// </code>
    /// Evaluates to <c>21</c>.
    /// </example>
    [<CompiledName("Get")>]
    val get: array: 'T [,,] -> index1: int -> index2: int -> index3: int -> 'T

    /// <summary>Applies the given function to each element of the array.</summary>
    ///
    /// <param name="action">The function to apply to each element of the array.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// let inputs = Array3D.init 2 2 3 (fun i j k -> 100*i + 10*j + k)
    ///
    /// inputs |> Array3D.iter (fun v -> printfn $"value = {v}")
    /// </code>
    /// Evaluates to <c>unit</c> and prints
    /// <code>
    /// value = 0
    /// value = 1
    /// value = 2
    /// value = 10
    /// value = 11
    /// value = 12
    /// value = 100
    /// value = 101
    /// value = 102
    /// value = 110
    /// value = 111
    /// value = 112
    /// </code>
    /// in the console.
    /// </example>
    [<CompiledName("Iterate")>]
    val iter: action: ('T -> unit) -> array: 'T [,,] -> unit

    /// <summary>Applies the given function to each element of the array. The integer indices passed to the
    /// function indicates the index of element.</summary>
    ///
    /// <param name="action">The function to apply to each element of the array.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <example id="iter-1">
    /// <code lang="fsharp">
    /// let inputs = Array3D.init 2 2 3 (fun i j k -> 100*i + 10*j + k)
    ///
    /// inputs |> Array3D.iteri (fun i j k v -> printfn $"value at ({i},{j},{k}) = {v}")
    /// </code>
    /// Evaluates to <c>unit</c> and prints
    /// <code>
    /// value at (0,0,0) = 0
    /// value at (0,0,1) = 1
    /// value at (0,0,2) = 2
    /// value at (0,1,0) = 10
    /// value at (0,1,1) = 11
    /// value at (0,1,2) = 12
    /// value at (1,0,0) = 100
    /// value at (1,0,1) = 101
    /// value at (1,0,2) = 102
    /// value at (1,1,0) = 110
    /// value at (1,1,1) = 111
    /// value at (1,1,2) = 112
    /// </code>
    /// in the console.
    /// </example>
    [<CompiledName("IterateIndexed")>]
    val iteri: action: (int -> int -> int -> 'T -> unit) -> array: 'T [,,] -> unit

    /// <summary>Returns the length of an array in the first dimension  </summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the first dimension.</returns>
    ///
    /// <example id="length1-1">
    /// <code>
    /// let array = Array3D.init 2 3 4 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array3D.length1
    /// </code>
    /// Evaluates to <c>2</c>.
    /// </example>
    [<CompiledName("Length1")>]
    val length1: array: 'T [,,] -> int

    /// <summary>Returns the length of an array in the second dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the second dimension.</returns>
    ///
    /// <example id="length2-1">
    /// <code>
    /// let array = Array3D.init 2 3 4 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array3D.length2
    /// </code>
    /// Evaluates to <c>3</c>.
    /// </example>
    [<CompiledName("Length2")>]
    val length2: array: 'T [,,] -> int

    /// <summary>Returns the length of an array in the third dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the third dimension.</returns>
    ///
    /// <example id="length3-1">
    /// <code>
    /// let array = Array3D.init 2 3 4 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array3D.length3
    /// </code>
    /// Evaluates to <c>4</c>.
    /// </example>
    [<CompiledName("Length3")>]
    val length3: array: 'T [,,] -> int

    /// <summary>Builds a new array whose elements are the results of applying the given function
    /// to each of the elements of the array.</summary>
    ///
    /// <remarks>For non-zero-based arrays the basing on an input array will be propagated to the output
    /// array.</remarks>
    /// <param name="mapping">The function to transform each element of the array.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The array created from the transformed elements.</returns>
    ///
    /// <example id="map-1">
    /// <code lang="fsharp">
    /// let inputs = Array3D.init 2 3 3 (fun i j k -> 100*i + 10*j + k)
    ///
    /// inputs |> Array3D.map (fun v -> 2 * v)
    /// </code>
    /// Evaluates to a 2x3x3 array with contents <c> <c>[[[0; 2; 4]; [20; 22; 24]]; [[200; 202; 204]; [220; 222; 224]]]</c></c>
    /// </example>
    [<CompiledName("Map")>]
    val map: mapping: ('T -> 'U) -> array: 'T [,,] -> 'U [,,]

    /// <summary>Builds a new array whose elements are the results of applying the given function
    /// to each of the elements of the array. The integer indices passed to the
    /// function indicates the element being transformed.</summary>
    ///
    /// <remarks>For non-zero-based arrays the basing on an input array will be propagated to the output
    /// array.</remarks>
    /// <param name="mapping">The function to transform the elements at each index in the array.</param>
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The array created from the transformed elements.</returns>
    ///
    /// <example id="mapi-1">
    /// <code lang="fsharp">
    /// let inputs = Array3D.zeroCreate 2 3 3
    ///
    /// inputs |> Array3D.mapi (fun i j k v -> 100*i + 10*j + k)
    /// </code>
    /// Evaluates to a 2x3x3 array with contents <c>[[[0; 2; 4]; [20; 22; 24]]; [[200; 202; 204]; [220; 222; 224]]]</c>
    /// </example>
    [<CompiledName("MapIndexed")>]
    val mapi: mapping: (int -> int -> int -> 'T -> 'U) -> array: 'T [,,] -> 'U [,,]

    /// <summary>Sets the value of an element in an array. You can also
    /// use the syntax 'array.[index1,index2,index3] &lt;- value'.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="index3">The index along the third dimension.</param>
    /// <param name="value">The value to set at the given index.</param>
    ///
    /// <remarks>
    /// Indexer syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let array = Array3D.zeroCreate 2 3 3
    ///
    /// array[0,2,1] &lt; 4.0
    /// </code>
    /// Evaluates to <c>11</c>.
    /// </remarks>
    ///
    /// <example id="set-1">
    /// <code lang="fsharp">
    /// let array = Array3D.zeroCreate 2 3 3
    ///
    /// Array3D.set array 0 2 1 4.0
    /// </code>
    /// After evaluation <c>array</c> is a 2x3x3 array with contents <c>[[[0.0; 0.0; 0.0]; [0.0; 4.0; 0.0]]; [[0.0; 0.0; 0.0]; [0.0; 0.0; 0.0]]]</c>
    /// </example>
    [<CompiledName("Set")>]
    val set: array: 'T [,,] -> index1: int -> index2: int -> index3: int -> value: 'T -> unit

    /// <summary>Creates an array where the entries are initially the "default" value.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    ///
    /// <returns>The created array.</returns>
    ///
    /// <example id="zerocreate-1">
    /// <code lang="fsharp">
    /// let array : float[,,] = Array3D.zeroCreate 2 3 3
    /// </code>
    /// After evaluation <c>array</c> is a 2x3x3 array with contents all zero.
    /// </example>
    [<CompiledName("ZeroCreate")>]
    val zeroCreate: length1: int -> length2: int -> length3: int -> 'T [,,]

/// <summary>Contains operations for working with rank 4 arrays. </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Array4D =

    /// <summary>Creates an array whose elements are all initially the given value</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="length4">The length of the fourth dimension.</param>
    /// <param name="initial">The initial value for each element of the array.</param>
    ///
    /// <returns>The created array.</returns>
    ///
    /// <example id="create-1">
    /// <code lang="fsharp">
    /// Array4D.create 2 2 2 2 1
    /// </code>
    /// Evaluates to a 2x2x2x2 array with all entries <c>1</c>
    /// </example>
    ///
    [<CompiledName("Create")>]
    val create: length1: int -> length2: int -> length3: int -> length4: int -> initial: 'T -> 'T [,,,]

    /// <summary>Creates an array given the dimensions and a generator function to compute the elements.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="length4">The length of the fourth dimension.</param>
    /// <param name="initializer">The function to create an initial value at each index in the array.</param>
    ///
    /// <returns>The created array.</returns>
    ///
    /// <example id="init-1">
    /// <code lang="fsharp">
    /// Array4D.init 2 2 2 2 (fun i j k l -> i*1000+j*100+k*10+l)
    /// </code>
    /// Evaluates to a 2x2x2x2 array with contents <c>[[[[0; 1]; [10; 11]]; [[100; 101]; [110; 111]]];[[[1000; 1]; [1010; 1011]]; [[1100; 1101]; [1110; 1111]]]]</c>
    /// </example>
    ///
    [<CompiledName("Initialize")>]
    val init:
        length1: int ->
        length2: int ->
        length3: int ->
        length4: int ->
        initializer: (int -> int -> int -> int -> 'T) ->
            'T [,,,]

    /// <summary>Returns the length of an array in the first dimension  </summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the first dimension.</returns>
    ///
    /// <example id="length1-1">
    /// <code>
    /// let array = Array4D.init 2 3 4 5 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array4D.length1
    /// </code>
    /// Evaluates to <c>2</c>.
    /// </example>
    [<CompiledName("Length1")>]
    val length1: array: 'T [,,,] -> int

    /// <summary>Returns the length of an array in the second dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the second dimension.</returns>
    ///
    /// <example id="length2-1">
    /// <code>
    /// let array = Array4D.init 2 3 4 5 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array4D.length2
    /// </code>
    /// Evaluates to <c>3</c>.
    /// </example>
    [<CompiledName("Length2")>]
    val length2: array: 'T [,,,] -> int

    /// <summary>Returns the length of an array in the third dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the third dimension.</returns>
    ///
    /// <example id="length3-1">
    /// <code>
    /// let array = Array4D.init 2 3 4 5 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array4D.length3
    /// </code>
    /// Evaluates to <c>4</c>.
    /// </example>
    [<CompiledName("Length3")>]
    val length3: array: 'T [,,,] -> int

    /// <summary>Returns the length of an array in the fourth dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the fourth dimension.</returns>
    ///
    /// <example id="length4-1">
    /// <code>
    /// let array = Array4D.init 2 3 4 5 (fun i j k -> 100*i + 10*j + k)
    ///
    /// array |> Array4D.length4
    /// </code>
    /// Evaluates to <c>5</c>.
    /// </example>
    [<CompiledName("Length4")>]
    val length4: array: 'T [,,,] -> int

    /// <summary>Creates an array where the entries are initially the "default" value.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="length4">The length of the fourth dimension.</param>
    ///
    /// <returns>The created array.</returns>
    ///
    /// <example id="zerocreate-1">
    /// <code lang="fsharp">
    /// let array : float[,,,] = Array4D.zeroCreate 2 3 3 5
    /// </code>
    /// After evaluation <c>array</c> is a 2x3x3x5 array with contents all zero.
    /// </example>
    [<CompiledName("ZeroCreate")>]
    val zeroCreate: length1: int -> length2: int -> length3: int -> length4: int -> 'T [,,,]

    /// <summary>Fetches an element from a 4D array. You can also use the syntax 'array.[index1,index2,index3,index4]'</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="index3">The index along the third dimension.</param>
    /// <param name="index4">The index along the fourth dimension.</param>
    ///
    /// <returns>The value at the given index.</returns>
    ///
    /// <remarks>
    /// Indexer syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let array: float[,,,] = Array4D.zeroCreate 2 3 4 5
    ///
    /// array[0,2,1,3]
    /// </code>
    /// </remarks>
    ///
    /// <example id="get-1">
    /// <code lang="fsharp">
    /// let array = Array4D.zeroCreate 2 3 4 5
    ///
    /// Array4D.get array 0 2 1 3
    /// </code>
    /// </example>
    [<CompiledName("Get")>]
    val get: array: 'T [,,,] -> index1: int -> index2: int -> index3: int -> index4: int -> 'T

    /// <summary>Sets the value of an element in an array. You can also
    /// use the syntax 'array.[index1,index2,index3,index4] &lt;- value'.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="index3">The index along the third dimension.</param>
    /// <param name="index4">The index along the fourth dimension.</param>
    /// <param name="value">The value to set.</param>
    ///
    /// <remarks>
    /// Indexer syntax is generally preferred, e.g.
    /// <code lang="fsharp">
    /// let array: float[,,,] = Array4D.zeroCreate 2 3 4 5
    ///
    /// array[0,2,1,3] &lt;- 5.0
    /// </code>
    /// </remarks>
    ///
    /// <example id="get-1">
    /// <code lang="fsharp">
    /// let array = Array4D.zeroCreate 2 3 4 5
    ///
    /// Array4D.2et array 0 2 1 3 5.0
    /// </code>
    /// </example>
    [<CompiledName("Set")>]
    val set: array: 'T [,,,] -> index1: int -> index2: int -> index3: int -> index4: int -> value: 'T -> unit
