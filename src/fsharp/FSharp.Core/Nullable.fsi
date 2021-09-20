// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq

open System
open Microsoft.FSharp.Core

/// Operators for working with nullable values, primarily used on F# queries.
[<AutoOpen>]
module NullableOperators =
    /// <summary>The '&gt;=' operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?>= ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '&gt;' operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?> ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '&lt;=' operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?<= ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '&lt;' operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?< ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '=' operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?= ) : Nullable<'T> -> 'T -> bool when 'T : equality

    /// <summary>The '&lt;>' operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?<> ) : Nullable<'T> -> 'T -> bool when 'T : equality

    /// <summary>The '&gt;=' operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( >=? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&gt;' operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( >? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;=' operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( <=? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;' operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( <? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '=' operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( =? ) : 'T -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The '&lt;>' operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( <>? ) : 'T -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The '&gt;=' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?>=? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&gt;' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?>? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;=' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?<=? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?<? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '=' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?=? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The '&lt;>' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val ( ?<>? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The addition operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?+ ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The addition operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( +? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The addition operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?+? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The subtraction operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?- ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The subtraction operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( -? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The subtraction operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?-? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
    
    /// <summary>The multiplication operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?* ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The multiplication operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( *? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The multiplication operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?*? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The modulus operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?% ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The modulus operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( %? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The modulus operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?%? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The division operator where a nullable value appears on the left</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?/ ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The division operator where a nullable value appears on the right</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( /? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The division operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <example-tbd></example-tbd>
    val inline ( ?/? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

/// Functions for converting nullable values
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Nullable =

    /// <summary>Converts the argument to byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted byte</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToByte")>]
    val inline byte : value:Nullable< ^T > -> Nullable<byte> when ^T : (static member op_Explicit : ^T -> byte) and default ^T : int  
    
    /// <summary>Converts the argument to byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted byte</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToUInt8")>]
    val inline uint8 : value:Nullable< ^T > -> Nullable<uint8> when ^T : (static member op_Explicit : ^T -> uint8) and default ^T : int  
    
    /// <summary>Converts the argument to signed byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted sbyte</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToSByte")>]
    val inline sbyte : value:Nullable< ^T > -> Nullable<sbyte> when ^T : (static member op_Explicit : ^T -> sbyte) and default ^T : int

    /// <summary>Converts the argument to signed byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted sbyte</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToInt8")>]
    val inline int8 : value:Nullable< ^T > -> Nullable<int8> when ^T : (static member op_Explicit : ^T -> int8) and default ^T : int
    
    /// <summary>Converts the argument to signed 16-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int16</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToInt16")>]
    val inline int16 : value:Nullable< ^T > -> Nullable<int16> when ^T : (static member op_Explicit : ^T -> int16) and default ^T : int
    
    /// <summary>Converts the argument to unsigned 16-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted uint16</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToUInt16")>]
    val inline uint16 : value:Nullable< ^T > -> Nullable<uint16> when ^T : (static member op_Explicit : ^T -> uint16) and default ^T : int
    
    /// <summary>Converts the argument to signed 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToInt")>]
    val inline int : value:Nullable< ^T > -> Nullable<int> when ^T : (static member op_Explicit : ^T -> int) and default ^T : int
    
    /// <summary>Converts the argument to an unsigned 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted unsigned integer</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToUInt")>]
    val inline uint: value: Nullable< ^T > -> Nullable<uint> when ^T :(static member op_Explicit: ^T -> uint) and default ^T : uint

    /// <summary>Converts the argument to a particular enum type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted enum type.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToEnum")>]
    val inline enum : value:Nullable< int32 > -> Nullable< ^U > when ^U : enum<int32> 

    /// <summary>Converts the argument to signed 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int32</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToInt32")>]
    val inline int32 : value:Nullable< ^T > -> Nullable<int32> when ^T : (static member op_Explicit : ^T -> int32) and default ^T : int

    /// <summary>Converts the argument to unsigned 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted uint32</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToUInt32")>]
    val inline uint32 : value:Nullable< ^T  > -> Nullable<uint32> when ^T : (static member op_Explicit : ^T -> uint32) and default ^T : int

    /// <summary>Converts the argument to signed 64-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int64</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToInt64")>]
    val inline int64 : value:Nullable< ^T > -> Nullable<int64> when ^T : (static member op_Explicit : ^T -> int64) and default ^T : int

    /// <summary>Converts the argument to unsigned 64-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted uint64</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToUInt64")>]
    val inline uint64 : value:Nullable< ^T > -> Nullable<uint64> when ^T : (static member op_Explicit : ^T -> uint64) and default ^T : int

    /// <summary>Converts the argument to 32-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float32</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToFloat32")>]
    val inline float32 : value:Nullable< ^T > -> Nullable<float32> when ^T : (static member op_Explicit : ^T -> float32) and default ^T : int

    /// <summary>Converts the argument to 64-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToFloat")>]
    val inline float : value:Nullable< ^T > -> Nullable<float> when ^T : (static member op_Explicit : ^T -> float) and default ^T : int

    /// <summary>Converts the argument to 32-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float32</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToSingle")>]
    val inline single : value:Nullable< ^T > -> Nullable<single> when ^T : (static member op_Explicit : ^T -> single) and default ^T : int

    /// <summary>Converts the argument to 64-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToDouble")>]
    val inline double : value:Nullable< ^T > -> Nullable<double> when ^T : (static member op_Explicit : ^T -> double) and default ^T : int

    /// <summary>Converts the argument to signed native integer. This is a direct conversion for all 
    /// primitive numeric types. Otherwise the operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted nativeint</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToIntPtr")>]
    val inline nativeint : value:Nullable< ^T > -> Nullable<nativeint> when ^T : (static member op_Explicit : ^T -> nativeint) and default ^T : int

    /// <summary>Converts the argument to unsigned native integer using a direct conversion for all 
    /// primitive numeric types. Otherwise the operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted unativeint</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToUIntPtr")>]
    val inline unativeint : value:Nullable< ^T > -> Nullable<unativeint> when ^T : (static member op_Explicit : ^T -> unativeint) and default ^T : int
    
    /// <summary>Converts the argument to System.Decimal using a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted decimal.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToDecimal")>]
    val inline decimal : value:Nullable< ^T > -> Nullable<decimal> when ^T : (static member op_Explicit : ^T -> decimal) and default ^T : int

    /// <summary>Converts the argument to character. Numeric inputs are converted according to the UTF-16 
    /// encoding for characters. The operation requires an appropriate static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted char.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ToChar")>]
    val inline char : value:Nullable< ^T > -> Nullable<char> when ^T : (static member op_Explicit : ^T -> char) and default ^T : int
