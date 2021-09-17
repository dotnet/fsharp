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
    /// <example-tbd></example-tbd>
    [<CompiledName("Create")>]
    val create: length1:int -> length2:int -> length3:int -> initial:'T -> 'T[,,]

    /// <summary>Creates an array given the dimensions and a generator function to compute the elements.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="initializer">The function to create an initial value at each index into the array.</param>
    ///
    /// <returns>The created array.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Initialize")>]
    val init: length1:int -> length2:int -> length3:int  -> initializer:(int -> int -> int -> 'T) -> 'T[,,]

    /// <summary>Fetches an element from a 3D array. You can also use the syntax 'array.[index1,index2,index3]'</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="index3">The index along the third dimension.</param>
    ///
    /// <returns>The value at the given index.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Get")>]
    val get: array:'T[,,] -> index1:int -> index2:int -> index3:int -> 'T

    /// <summary>Applies the given function to each element of the array.</summary>
    ///
    /// <param name="action">The function to apply to each element of the array.</param>
    /// <param name="array">The input array.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Iterate")>]
    val iter: action:('T -> unit) -> array:'T[,,] -> unit

    /// <summary>Applies the given function to each element of the array. The integer indices passed to the
    /// function indicates the index of element.</summary>
    ///
    /// <param name="action">The function to apply to each element of the array.</param>
    /// <param name="array">The input array.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IterateIndexed")>]
    val iteri: action:(int -> int -> int -> 'T -> unit) -> array:'T[,,] -> unit

    /// <summary>Returns the length of an array in the first dimension  </summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the first dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length1")>]
    val length1: array:'T[,,] -> int

    /// <summary>Returns the length of an array in the second dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the second dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length2")>]
    val length2: array:'T[,,] -> int

    /// <summary>Returns the length of an array in the third dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the third dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length3")>]
    val length3: array:'T[,,] -> int

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
    /// <example-tbd></example-tbd>
    [<CompiledName("Map")>]
    val map: mapping:('T -> 'U) -> array:'T[,,] -> 'U[,,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("MapIndexed")>]
    val mapi: mapping:(int -> int -> int -> 'T -> 'U) -> array:'T[,,] -> 'U[,,]

    /// <summary>Sets the value of an element in an array. You can also 
    /// use the syntax 'array.[index1,index2,index3] &lt;- value'.</summary>
    ///
    /// <param name="array">The input array.</param>
    /// <param name="index1">The index along the first dimension.</param>
    /// <param name="index2">The index along the second dimension.</param>
    /// <param name="index3">The index along the third dimension.</param>
    /// <param name="value">The value to set at the given index.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Set")>]
    val set: array:'T[,,] -> index1:int -> index2:int -> index3:int -> value:'T -> unit

    /// <summary>Creates an array where the entries are initially the "default" value.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    ///
    /// <returns>The created array.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ZeroCreate")>]
    val zeroCreate: length1:int -> length2:int -> length3:int  -> 'T[,,]



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
    /// <example-tbd></example-tbd>
    [<CompiledName("Create")>]
    val create: length1:int -> length2:int -> length3:int -> length4:int -> initial:'T -> 'T[,,,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("Initialize")>]
    val init: length1:int -> length2:int -> length3:int  -> length4:int  -> initializer:(int -> int -> int -> int -> 'T) -> 'T[,,,]

    /// <summary>Returns the length of an array in the first dimension  </summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the first dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length1")>]
    val length1: array:'T[,,,] -> int

    /// <summary>Returns the length of an array in the second dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the second dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length2")>]
    val length2: array:'T[,,,] -> int

    /// <summary>Returns the length of an array in the third dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the third dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length3")>]
    val length3: array:'T[,,,] -> int

    /// <summary>Returns the length of an array in the fourth dimension.</summary>
    ///
    /// <param name="array">The input array.</param>
    ///
    /// <returns>The length of the array in the fourth dimension.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("Length4")>]
    val length4: array:'T[,,,] -> int

    /// <summary>Creates an array where the entries are initially the "default" value.</summary>
    ///
    /// <param name="length1">The length of the first dimension.</param>
    /// <param name="length2">The length of the second dimension.</param>
    /// <param name="length3">The length of the third dimension.</param>
    /// <param name="length4">The length of the fourth dimension.</param>
    ///
    /// <returns>The created array.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ZeroCreate")>]
    val zeroCreate: length1:int -> length2:int -> length3:int  -> length4:int  -> 'T[,,,]

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
    /// <example-tbd></example-tbd>
    [<CompiledName("Get")>]
    val get: array:'T[,,,] -> index1:int -> index2:int -> index3:int -> index4:int -> 'T

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
    /// <example-tbd></example-tbd>
    [<CompiledName("Set")>]
    val set: array:'T[,,,] -> index1:int -> index2:int -> index3:int -> index4:int -> value:'T -> unit
