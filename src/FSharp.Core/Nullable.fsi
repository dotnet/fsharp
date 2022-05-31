// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq

open System
open Microsoft.FSharp.Core

/// Operators for working with nullable values, primarily used on F# queries.
[<AutoOpen>]
module NullableOperators =
    /// <summary>The '&gt;=' operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example id="nge-1">
    /// <code lang="fsharp">
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable(3) ?>= 4 // false
    /// Nullable(4) ?>= 4 // true
    /// Nullable() ?>= 4 // false
    /// </code>
    /// </example>
    ///
    val ( ?>= ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '&gt;' operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example id="ngt-1">
    /// <code lang="fsharp">
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable(3) ?> 4 // false
    /// Nullable(5) ?> 4 // true
    /// Nullable() ?> 4 // false
    /// </code>
    /// </example>
    ///
    val ( ?> ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '&lt;=' operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example id="nlte-1">
    /// <code lang="fsharp">
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable(3) ?&lt;= 4 // true
    /// Nullable(5) ?&lt;= 4 // false
    /// Nullable() ?&lt;= 4 // false
    /// </code>
    /// </example>
    val ( ?<= ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '&lt;' operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example id="nlt-1">
    /// <code lang="fsharp">
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable(3) ?&lt; 4 // true
    /// Nullable(4) ?&lt; 4 // false
    /// Nullable() ?&lt; 4 // false
    /// </code>
    /// </example>
    val ( ?< ) : Nullable<'T> -> 'T -> bool when 'T : comparison

    /// <summary>The '=' operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example id="neq-1">
    /// <code lang="fsharp">
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable(3) ?= 4 // false
    /// Nullable(4) ?= 4 // true
    /// Nullable() ?= 4 // false
    /// </code>
    /// </example>
    ///
    val ( ?= ) : Nullable<'T> -> 'T -> bool when 'T : equality

    /// <summary>The '&lt;>' operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example id="nneq-1">
    /// <code lang="fsharp">
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable(3) ?&lt;>= 4 // true
    /// Nullable(4) ?&lt;>= 4 // false
    /// Nullable() ?&lt;> 4 // true
    /// </code>
    /// </example>
    ///
    val ( ?<> ) : Nullable<'T> -> 'T -> bool when 'T : equality

    /// <summary>The '&gt;=' operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( >=? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&gt;' operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( >? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;=' operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( <=? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;' operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( <? ) : 'T -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '=' operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( =? ) : 'T -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The '&lt;>' operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( <>? ) : 'T -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The '&gt;=' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( ?>=? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&gt;' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( ?>? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;=' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( ?<=? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '&lt;' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( ?<? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison

    /// <summary>The '=' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( ?=? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The '&lt;>' operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val ( ?<>? ) : Nullable<'T> -> Nullable<'T> -> bool when 'T : equality

    /// <summary>The addition operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?+ ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The addition operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( +? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The addition operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?+? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The subtraction operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?- ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The subtraction operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( -? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The subtraction operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?-? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
    
    /// <summary>The multiplication operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?* ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The multiplication operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( *? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The multiplication operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?*? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The modulus operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?% ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The modulus operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( %? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The modulus operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?%? ) : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The division operator where a nullable value appears on the left</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( ?/ ) : Nullable< ^T1 > -> ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The division operator where a nullable value appears on the right</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
    val inline ( /? ) : ^T1  -> Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// <summary>The division operator where a nullable value appears on both left and right sides</summary>
    /// 
    /// <remarks>This operator is primarily for use in F# queries</remarks>
    ///
    /// <example>See the other operators in this module for related examples.</example>
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
    /// <example id="byte-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.byte (Nullable&lt;int>())  // evaluates to Nullable&lt;byte>()
    /// Nullable.byte (Nullable&lt;int>(3))  // evaluates to Nullable(3uy)
    /// </code>
    /// </example>
    ///
    [<CompiledName("ToByte")>]
    val inline byte: value: Nullable< ^T > -> Nullable<byte> when ^T : (static member op_Explicit : ^T -> byte) and default ^T : int  
    
    /// <summary>Converts the argument to byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted byte</returns>
    /// 
    /// <example id="uint8-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.uint8 (Nullable&lt;int>())  // evaluates to Nullable&lt;byte>()
    /// Nullable.uint8 (Nullable&lt;int>(3))  // evaluates to Nullable(3uy)
    /// </code>
    /// </example>
    [<CompiledName("ToUInt8")>]
    val inline uint8: value: Nullable< ^T > -> Nullable<uint8> when ^T : (static member op_Explicit : ^T -> uint8) and default ^T : int  
    
    /// <summary>Converts the argument to signed byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted sbyte</returns>
    /// 
    /// <example id="sbyte-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.sbyte (Nullable&lt;int>())  // evaluates to Nullable&lt;sbyte>()
    /// Nullable.sbyte (Nullable&lt;int>(3))  // evaluates to Nullable(3y)
    /// </code>
    /// </example>
    [<CompiledName("ToSByte")>]
    val inline sbyte: value: Nullable< ^T > -> Nullable<sbyte> when ^T : (static member op_Explicit : ^T -> sbyte) and default ^T : int

    /// <summary>Converts the argument to signed byte. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted sbyte</returns>
    /// 
    /// <example id="int8-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.int8 (Nullable&lt;int>())  // evaluates to Nullable&lt;sbyte>()
    /// Nullable.int8 (Nullable&lt;int>(3))  // evaluates to Nullable(3y)
    /// </code>
    /// </example>
    [<CompiledName("ToInt8")>]
    val inline int8: value: Nullable< ^T > -> Nullable<int8> when ^T : (static member op_Explicit : ^T -> int8) and default ^T : int
    
    /// <summary>Converts the argument to signed 16-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int16</returns>
    /// 
    /// <example id="int16-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.int16 (Nullable&lt;int>())  // evaluates to Nullable&lt;int16>()
    /// Nullable.int16 (Nullable&lt;int>(3))  // evaluates to Nullable(3s)
    /// </code>
    /// </example>
    [<CompiledName("ToInt16")>]
    val inline int16: value: Nullable< ^T > -> Nullable<int16> when ^T : (static member op_Explicit : ^T -> int16) and default ^T : int
    
    /// <summary>Converts the argument to unsigned 16-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted uint16</returns>
    /// 
    /// <example id="uint16-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.uint16 (Nullable&lt;int>())  // evaluates to Nullable&lt;uint16>()
    /// Nullable.uint16 (Nullable&lt;int>(3))  // evaluates to Nullable(3us)
    /// </code>
    /// </example>
    [<CompiledName("ToUInt16")>]
    val inline uint16: value: Nullable< ^T > -> Nullable<uint16> when ^T : (static member op_Explicit : ^T -> uint16) and default ^T : int
    
    /// <summary>Converts the argument to signed 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int</returns>
    /// 
    /// <example id="int-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.int (Nullable&lt;int64>())  // evaluates to Nullable&lt;int>()
    /// Nullable.int (Nullable&lt;int64>(3))  // evaluates to Nullable(3)
    /// </code>
    /// </example>
    [<CompiledName("ToInt")>]
    val inline int: value: Nullable< ^T > -> Nullable<int> when ^T : (static member op_Explicit : ^T -> int) and default ^T : int
    
    /// <summary>Converts the argument to an unsigned 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted unsigned integer</returns>
    /// 
    /// <example id="uint-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.uint (Nullable&lt;int>())  // evaluates to Nullable&lt;uint>()
    /// Nullable.uint (Nullable&lt;int>(3))  // evaluates to Nullable(3u)
    /// </code>
    /// </example>
    [<CompiledName("ToUInt")>]
    val inline uint: value:  Nullable< ^T > -> Nullable<uint> when ^T :(static member op_Explicit: ^T -> uint) and default ^T : uint

    /// <summary>Converts the argument to a particular enum type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted enum type.</returns>
    /// 
    /// <example id="enum-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.enum&lt;DayOfWeek> (Nullable&lt;int>())  // evaluates to Nullable&lt;uint>()
    /// Nullable.enum&lt;DayOfWeek> (Nullable&lt;int>(3))  // evaluates to Nullable&lt;DayOfWeek>(Wednesday)
    /// </code>
    /// </example>
    [<CompiledName("ToEnum")>]
    val inline enum: value: Nullable< int32 > -> Nullable< ^U > when ^U : enum<int32> 

    /// <summary>Converts the argument to signed 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int32</returns>
    /// 
    /// <example id="int32-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.int32 (Nullable&lt;int64>())  // evaluates to Nullable&lt;int32>()
    /// Nullable.int32 (Nullable&lt;int64>(3))  // evaluates to Nullable(3)
    /// </code>
    /// </example>
    [<CompiledName("ToInt32")>]
    val inline int32: value: Nullable< ^T > -> Nullable<int32> when ^T : (static member op_Explicit : ^T -> int32) and default ^T : int

    /// <summary>Converts the argument to unsigned 32-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted uint32</returns>
    /// 
    /// <example id="uint32-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.uint32 (Nullable&lt;int>())  // evaluates to Nullable&lt;uint32>()
    /// Nullable.uint32 (Nullable&lt;int>(3))  // evaluates to Nullable(3u)
    /// </code>
    /// </example>
    [<CompiledName("ToUInt32")>]
    val inline uint32: value: Nullable< ^T  > -> Nullable<uint32> when ^T : (static member op_Explicit : ^T -> uint32) and default ^T : int

    /// <summary>Converts the argument to signed 64-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted int64</returns>
    /// 
    /// <example id="int64-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.int64 (Nullable&lt;int>())  // evaluates to Nullable&lt;int64>()
    /// Nullable.int64 (Nullable&lt;int>(3))  // evaluates to Nullable&lt;int64>(3L)
    /// </code>
    /// </example>
    [<CompiledName("ToInt64")>]
    val inline int64: value: Nullable< ^T > -> Nullable<int64> when ^T : (static member op_Explicit : ^T -> int64) and default ^T : int

    /// <summary>Converts the argument to unsigned 64-bit integer. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted uint64</returns>
    /// 
    /// <example id="uint64-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.uint64 (Nullable&lt;int>())  // evaluates to Nullable&lt;uint64>()
    /// Nullable.uint64 (Nullable&lt;int>(3))  // evaluates to Nullable&lt;uint64>(3UL)
    /// </code>
    /// </example>
    [<CompiledName("ToUInt64")>]
    val inline uint64: value: Nullable< ^T > -> Nullable<uint64> when ^T : (static member op_Explicit : ^T -> uint64) and default ^T : int

    /// <summary>Converts the argument to 32-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float32</returns>
    /// 
    /// <example id="float32-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.float32 (Nullable&lt;int>())  // evaluates to Nullable&lt;float32>()
    /// Nullable.float32 (Nullable&lt;int>(3))  // evaluates to Nullable&lt;float32>(3.0f)
    /// </code>
    /// </example>
    [<CompiledName("ToFloat32")>]
    val inline float32: value: Nullable< ^T > -> Nullable<float32> when ^T : (static member op_Explicit : ^T -> float32) and default ^T : int

    /// <summary>Converts the argument to 64-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float</returns>
    /// 
    /// <example id="float-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.float (Nullable&lt;int>())  // evaluates to Nullable&lt;float>()
    /// Nullable.float (Nullable&lt;int>(3))  // evaluates to Nullable&lt;float>(3.0)
    /// </code>
    /// </example>
    [<CompiledName("ToFloat")>]
    val inline float: value: Nullable< ^T > -> Nullable<float> when ^T : (static member op_Explicit : ^T -> float) and default ^T : int

    /// <summary>Converts the argument to 32-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float32</returns>
    /// 
    /// <example id="single-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.single (Nullable&lt;int>())  // evaluates to Nullable&lt;float32>()
    /// Nullable.single (Nullable&lt;int>(3))  // evaluates to Nullable&lt;float32>(3.0f)
    /// </code>
    /// </example>
    [<CompiledName("ToSingle")>]
    val inline single: value: Nullable< ^T > -> Nullable<single> when ^T : (static member op_Explicit : ^T -> single) and default ^T : int

    /// <summary>Converts the argument to 64-bit float. This is a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted float</returns>
    /// 
    /// <example id="double-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.double (Nullable&lt;int>())  // evaluates to Nullable&lt;double>()
    /// Nullable.double (Nullable&lt;int>(3))  // evaluates to Nullable&lt;double>(3.0)
    /// </code>
    /// </example>
    [<CompiledName("ToDouble")>]
    val inline double: value: Nullable< ^T > -> Nullable<double> when ^T : (static member op_Explicit : ^T -> double) and default ^T : int

    /// <summary>Converts the argument to signed native integer. This is a direct conversion for all 
    /// primitive numeric types. Otherwise the operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted nativeint</returns>
    /// 
    /// <example id="nativeint-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.nativeint (Nullable&lt;int>())  // evaluates to Nullable&lt;nativeint>()
    /// Nullable.nativeint (Nullable&lt;int>(3))  // evaluates to Nullable&lt;nativeint>(3n)
    /// </code>
    /// </example>
    [<CompiledName("ToIntPtr")>]
    val inline nativeint: value: Nullable< ^T > -> Nullable<nativeint> when ^T : (static member op_Explicit : ^T -> nativeint) and default ^T : int

    /// <summary>Converts the argument to unsigned native integer using a direct conversion for all 
    /// primitive numeric types. Otherwise the operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted unativeint</returns>
    /// 
    /// <example id="unativeint-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.unativeint (Nullable&lt;int>())  // evaluates to Nullable&lt;unativeint>()
    /// Nullable.unativeint (Nullable&lt;int>(3))  // evaluates to Nullable&lt;unativeint>(3un)
    /// </code>
    /// </example>
    [<CompiledName("ToUIntPtr")>]
    val inline unativeint: value: Nullable< ^T > -> Nullable<unativeint> when ^T : (static member op_Explicit : ^T -> unativeint) and default ^T : int
    
    /// <summary>Converts the argument to System.Decimal using a direct conversion for all 
    /// primitive numeric types. The operation requires an appropriate
    /// static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted decimal.</returns>
    /// 
    /// <example id="decimal-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.decimal (Nullable&lt;int>())  // evaluates to Nullable&lt;decimal>()
    /// Nullable.decimal (Nullable&lt;int>(3))  // evaluates to Nullable&lt;decimal>(3.0M)
    /// </code>
    /// </example>
    [<CompiledName("ToDecimal")>]
    val inline decimal: value: Nullable< ^T > -> Nullable<decimal> when ^T : (static member op_Explicit : ^T -> decimal) and default ^T : int

    /// <summary>Converts the argument to character. Numeric inputs are converted according to the UTF-16 
    /// encoding for characters. The operation requires an appropriate static conversion method on the input type.</summary>
    ///
    /// <param name="value">The input value.</param>
    ///
    /// <returns>The converted char.</returns>
    /// 
    /// <example id="char-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Linq.NullableOperators
    ///
    /// Nullable.char (Nullable&lt;int>())  // evaluates to Nullable&lt;char>()
    /// Nullable.char (Nullable&lt;int>(64))  // evaluates to Nullable&lt;char>('@')
    /// </code>
    /// </example>
    [<CompiledName("ToChar")>]
    val inline char: value: Nullable< ^T > -> Nullable<char> when ^T : (static member op_Explicit : ^T -> char) and default ^T : int
