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
    /// <example-tbd></example-tbd>
    [<CompiledName("Base1")>]
    val base1: array:'T[,] -> int

    /// <summary>Fetches the base-index for the second dimension of the array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The base-index of the second dimension of the array.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Base2")>]
    val base2: array:'T[,] -> int

    /// <summary>Builds a new array whose elements are the same as the input array.</summary>
    ///
    /// <remarks>For non-zero-based arrays the basing on an input array will be propagated to the output
    /// array.</remarks>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>A copy of the input array.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Copy")>]
    val copy: array:'T[,] -> 'T[,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("CopyTo")>]
    val blit: source:'T[,] -> sourceIndex1:int -> sourceIndex2:int -> target:'T[,] -> targetIndex1:int -> targetIndex2:int -> length1:int -> length2:int -> unit

    /// <summary>Creates an array given the dimensions and a generator function to compute the elements.</summary>
    ///
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    /// <param name="initializer">A function to produce elements of the array given the two indices.</param>
    ///
    /// <returns>The generated array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when either of the lengths is negative.</exception>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Initialize")>]
    val init: length1:int -> length2:int -> initializer:(int -> int -> 'T) -> 'T[,]

    /// <summary>Creates an array whose elements are all initially the given value.</summary>
    ///
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    /// <param name="value">The value to populate the new array.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when length1 or length2 is negative.</exception>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Create")>]
    val create: length1:int -> length2:int -> value:'T -> 'T[,]

    /// <summary>Creates an array where the entries are initially Unchecked.defaultof&lt;'T&gt;.</summary>
    ///
    /// <param name="length1">The length of the first dimension of the array.</param>
    /// <param name="length2">The length of the second dimension of the array.</param>
    ///
    /// <returns>The created array.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when length1 or length2 is negative.</exception>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ZeroCreate")>]
    val zeroCreate : length1:int -> length2:int -> 'T[,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("InitializeBased")>]
    val initBased: base1:int -> base2:int -> length1:int -> length2:int -> initializer:(int -> int -> 'T) -> 'T[,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("CreateBased")>]
    val createBased: base1:int -> base2:int -> length1:int -> length2:int -> initial: 'T -> 'T[,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("ZeroCreateBased")>]
    val zeroCreateBased : base1:int -> base2:int -> length1:int -> length2:int -> 'T[,]

    /// <summary>Applies the given function to each element of the array.</summary>
    ///
    /// <param name="action">A function to apply to each element of the array.</param>
    /// <param name="array">The input array.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Iterate")>]
    val iter: action:('T -> unit) -> array:'T[,] -> unit

    /// <summary>Applies the given function to each element of the array.  The integer indices passed to the
    /// function indicates the index of element.</summary>
    ///
    /// <param name="action">A function to apply to each element of the array with the indices available as an argument.</param>
    /// <param name="array">The input array.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IterateIndexed")>]
    val iteri: action:(int -> int -> 'T -> unit) -> array:'T[,] -> unit

    /// <summary>Returns the length of an array in the first dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the first dimension.</returns>  
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length1")>]
    val length1: array:'T[,] -> int

    /// <summary>Returns the length of an array in the second dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the second dimension.</returns>  
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length2")>]
    val length2: array:'T[,] -> int

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
    /// <example-tbd></example-tbd>
    [<CompiledName("Map")>]
    val map: mapping:('T -> 'U) -> array:'T[,] -> 'U[,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("MapIndexed")>]
    val mapi: mapping:(int -> int -> 'T -> 'U) -> array:'T[,] -> 'U[,]


    /// <summary>Builds a new array whose elements are the same as the input array but
    /// where a non-zero-based input array generates a corresponding zero-based 
    /// output array.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The zero-based output array.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Rebase")>]
    val rebase: array:'T[,] -> 'T[,]

    /// <summary>Sets the value of an element in an array. You can also use the syntax <c>array.[index1,index2] &lt;- value</c>.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="value">The value to set in the array.</param>
    /// <exception cref="T:System.ArgumentException">Thrown when the indices are negative or exceed the bounds of the array.</exception> 
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Set")>]
    val set: array:'T[,] -> index1:int -> index2:int -> value:'T -> unit

    /// <summary>Fetches an element from a 2D array. You can also use the syntax <c>array.[index1,index2]</c>.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    ///
    /// <returns>The value of the array at the given index.</returns>
    /// <exception cref="T:System.ArgumentException">Thrown when the indices are negative or exceed the bounds of the array.</exception>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Get")>]
    val get: array:'T[,] -> index1:int -> index2:int -> 'T
