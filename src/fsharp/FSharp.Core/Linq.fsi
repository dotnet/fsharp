// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq

#if FX_ATLEAST_LINQ

    open System
    open System.Linq.Expressions
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections

    /// Operators for working with nullable values
    [<AutoOpen>]
    module NullableOperators =
        open System
        /// The '>=' operator where a nullable value appears on the left
        val ( ?>= )   : Nullable<'T> ->           'T -> bool when 'T : comparison
        /// The '>' operator where a nullable value appears on the left
        val ( ?> )    : Nullable<'T> ->           'T -> bool when 'T : comparison
        /// The '<=' operator where a nullable value appears on the left
        val ( ?<= )   : Nullable<'T> ->           'T -> bool when 'T : comparison
        /// The '<' operator where a nullable value appears on the left
        val ( ?< )    : Nullable<'T> ->           'T -> bool when 'T : comparison
        /// The '=' operator where a nullable value appears on the left
        val ( ?= )    : Nullable<'T> ->           'T -> bool when 'T : equality
        /// The '<>' operator where a nullable value appears on the left
        val ( ?<> )   : Nullable<'T> ->           'T -> bool when 'T : equality

        /// The '>=' operator where a nullable value appears on the right
        val ( >=? )   :           'T -> Nullable<'T> -> bool when 'T : comparison
        /// The '>' operator where a nullable value appears on the right
        val ( >? )    :           'T -> Nullable<'T> -> bool when 'T : comparison
        /// The '<=' operator where a nullable value appears on the right
        val ( <=? )   :           'T -> Nullable<'T> -> bool when 'T : comparison
        /// The '<' operator where a nullable value appears on the right
        val ( <? )    :           'T -> Nullable<'T> -> bool when 'T : comparison
        /// The '=' operator where a nullable value appears on the right
        val ( =? )    :           'T -> Nullable<'T> -> bool when 'T : equality
        /// The '<>' operator where a nullable value appears on the right
        val ( <>? )   :           'T -> Nullable<'T> -> bool when 'T : equality

        /// The '>=' operator where a nullable value appears on both left and right sides
        val ( ?>=? )  : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison
        /// The '>' operator where a nullable value appears on both left and right sides
        val ( ?>? )   : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison
        /// The '<=' operator where a nullable value appears on both left and right sides
        val ( ?<=? )  : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison
        /// The '<' operator where a nullable value appears on both left and right sides
        val ( ?<? )   : Nullable<'T> -> Nullable<'T> -> bool when 'T : comparison
        /// The '=' operator where a nullable value appears on both left and right sides
        val ( ?=? )   : Nullable<'T> -> Nullable<'T> -> bool when 'T : equality
        /// The '<>' operator where a nullable value appears on both left and right sides
        val ( ?<>? )  : Nullable<'T> -> Nullable<'T> -> bool when 'T : equality


        /// The addition operator where a nullable value appears on the left
        val inline ( ?+ )   : Nullable< ^T1 > ->           ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The addition operator where a nullable value appears on the right
        val inline ( +? )   : ^T1  ->           Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The addition operator where a nullable value appears on both left and right sides
        val inline ( ?+? )  : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( + ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

        /// The subtraction operator where a nullable value appears on the left
        val inline ( ?- )   : Nullable< ^T1 > ->           ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The subtraction operator where a nullable value appears on the right
        val inline ( -? )   : ^T1  ->           Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The subtraction operator where a nullable value appears on both left and right sides
        val inline ( ?-? )  : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( - ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

        /// The multiplication operator where a nullable value appears on the left
        val inline ( ?* )   : Nullable< ^T1 > ->           ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The multiplication operator where a nullable value appears on the right
        val inline ( *? )   : ^T1  ->           Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The multiplication operator where a nullable value appears on both left and right sides
        val inline ( ?*? )  : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( * ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

        /// The modulus operator where a nullable value appears on the left
        val inline ( ?% )   : Nullable< ^T1 > ->           ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The modulus operator where a nullable value appears on the right
        val inline ( %? )   : ^T1  ->           Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The modulus operator where a nullable value appears on both left and right sides
        val inline ( ?%? )  : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( % ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

        /// The division operator where a nullable value appears on the left
        val inline ( ?/ )   : Nullable< ^T1 > ->           ^T2 -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The division operator where a nullable value appears on the right
        val inline ( /? )   : ^T1  ->           Nullable< ^T2 > -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int
        /// The division operator where a nullable value appears on both left and right sides
        val inline ( ?/? )  : Nullable< ^T1 > -> Nullable< ^T2 >  -> Nullable< ^T3 > when (^T1 or ^T2) : (static member ( / ) : ^T1 * ^T2    -> ^T3) and default ^T2 : ^T3 and default ^T3 : ^T1 and default ^T3 : ^T2 and default ^T1 : ^T3 and default ^T1 : ^T2 and default ^T1 : int

    /// Functions for converting nullable values
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Nullable =

        /// <summary>Converts the argument to byte. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted byte</returns>
        [<CompiledName("ToByte")>]
        val inline byte       : value:Nullable< ^T > -> Nullable<byte>       when ^T : (static member op_Explicit : ^T -> byte)       and default ^T : int        
        
        /// <summary>Converts the argument to byte. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted byte</returns>
        [<CompiledName("ToUInt8")>]
        val inline uint8       : value:Nullable< ^T > -> Nullable<uint8>       when ^T : (static member op_Explicit : ^T -> uint8)       and default ^T : int        
        
        /// <summary>Converts the argument to signed byte. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted sbyte</returns>
        [<CompiledName("ToSByte")>]
        val inline sbyte      : value:Nullable< ^T > -> Nullable<sbyte>      when ^T : (static member op_Explicit : ^T -> sbyte)      and default ^T : int
        
        
        /// <summary>Converts the argument to signed byte. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted sbyte</returns>
        [<CompiledName("ToInt8")>]
        val inline int8      : value:Nullable< ^T > -> Nullable<int8>      when ^T : (static member op_Explicit : ^T -> int8)      and default ^T : int
        
        /// <summary>Converts the argument to signed 16-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted int16</returns>
        [<CompiledName("ToInt16")>]
        val inline int16      : value:Nullable< ^T > -> Nullable<int16>      when ^T : (static member op_Explicit : ^T -> int16)      and default ^T : int
        
        /// <summary>Converts the argument to unsigned 16-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted uint16</returns>
        [<CompiledName("ToUInt16")>]
        val inline uint16     : value:Nullable< ^T > -> Nullable<uint16>     when ^T : (static member op_Explicit : ^T -> uint16)     and default ^T : int
        
        /// <summary>Converts the argument to signed 32-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted int</returns>
        [<CompiledName("ToInt")>]
        val inline int        : value:Nullable< ^T > -> Nullable<int>        when ^T : (static member op_Explicit : ^T -> int)        and default ^T : int
        
        /// <summary>Converts the argument to a particular enum type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted enum type.</returns>
        [<CompiledName("ToEnum")>]
        val inline enum       : value:Nullable< int32 > -> Nullable< ^U >        when ^U : enum<int32> 

        /// <summary>Converts the argument to signed 32-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted int32</returns>
        [<CompiledName("ToInt32")>]
        val inline int32      : value:Nullable< ^T > -> Nullable<int32>       when ^T : (static member op_Explicit : ^T -> int32)      and default ^T : int

        /// <summary>Converts the argument to unsigned 32-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted uint32</returns>
        [<CompiledName("ToUInt32")>]
        val inline uint32     : value:Nullable< ^T  > -> Nullable<uint32>     when ^T : (static member op_Explicit : ^T -> uint32)     and default ^T : int

        /// <summary>Converts the argument to signed 64-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted int64</returns>
        [<CompiledName("ToInt64")>]
        val inline int64      : value:Nullable< ^T > -> Nullable<int64>      when ^T : (static member op_Explicit : ^T -> int64)      and default ^T : int

        /// <summary>Converts the argument to unsigned 64-bit integer. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted uint64</returns>
        [<CompiledName("ToUInt64")>]
        val inline uint64     : value:Nullable< ^T > -> Nullable<uint64>     when ^T : (static member op_Explicit : ^T -> uint64)     and default ^T : int

        /// <summary>Converts the argument to 32-bit float. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted float32</returns>
        [<CompiledName("ToFloat32")>]
        val inline float32    : value:Nullable< ^T > -> Nullable<float32>    when ^T : (static member op_Explicit : ^T -> float32)    and default ^T : int

        /// <summary>Converts the argument to 64-bit float. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted float</returns>
        [<CompiledName("ToFloat")>]
        val inline float      : value:Nullable< ^T > -> Nullable<float>      when ^T : (static member op_Explicit : ^T -> float)      and default ^T : int

        /// <summary>Converts the argument to 32-bit float. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted float32</returns>
        [<CompiledName("ToSingle")>]
        val inline single    : value:Nullable< ^T > -> Nullable<single>    when ^T : (static member op_Explicit : ^T -> single)    and default ^T : int

        /// <summary>Converts the argument to 64-bit float. This is a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted float</returns>
        [<CompiledName("ToDouble")>]
        val inline double      : value:Nullable< ^T > -> Nullable<double>      when ^T : (static member op_Explicit : ^T -> double)      and default ^T : int

        /// <summary>Converts the argument to signed native integer. This is a direct conversion for all 
        /// primitive numeric types. Otherwise the operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted nativeint</returns>
        [<CompiledName("ToIntPtr")>]
        val inline nativeint  : value:Nullable< ^T > -> Nullable<nativeint>  when ^T : (static member op_Explicit : ^T -> nativeint)  and default ^T : int

        /// <summary>Converts the argument to unsigned native integer using a direct conversion for all 
        /// primitive numeric types. Otherwise the operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted unativeint</returns>
        [<CompiledName("ToUIntPtr")>]
        val inline unativeint : value:Nullable< ^T > -> Nullable<unativeint> when ^T : (static member op_Explicit : ^T -> unativeint) and default ^T : int
        
        /// <summary>Converts the argument to System.Decimal using a direct conversion for all 
        /// primitive numeric types. The operation requires an appropriate
        /// static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted decimal.</returns>
        [<CompiledName("ToDecimal")>]
        val inline decimal : value:Nullable< ^T > -> Nullable<decimal> when ^T : (static member op_Explicit : ^T -> decimal) and default ^T : int

        /// <summary>Converts the argument to character. Numeric inputs are converted according to the UTF-16 
        /// encoding for characters. The operation requires an appropriate static conversion method on the input type.</summary>
        /// <param name="value">The input value.</param>
        /// <returns>The converted char.</returns>
        [<CompiledName("ToChar")>]
        val inline char        : value:Nullable< ^T > -> Nullable<char>      when ^T : (static member op_Explicit : ^T -> char)        and default ^T : int

namespace Microsoft.FSharp.Linq.RuntimeHelpers

    open System
    open System.Linq.Expressions
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections

    module LeafExpressionConverter =
        open Microsoft.FSharp.Quotations

        /// When used in a quotation, this function indicates a specific conversion
        /// should be performed when converting the quotation to a LINQ expression. 
        ///
        /// This function should not be called directly. 
        //
        // NOTE: when an F# expression tree is converted to a Linq expression tree using ToLinqExpression 
        // the transformation of <c>LinqExpressionHelper(e)</c> is the same as the transformation of
        // 'e'. This allows ImplicitExpressionConversionHelper to be used as a marker to satisfy the C# design where 
        // certain expression trees are constructed using methods with a signature that expects an
        // expression tree of type <c>Expression<T></c> but are passed an expression tree of type T.
        //[<System.Obsolete("This type is for use by the quotation to LINQ expression tree converter and is not for direct use from user code")>]
        val ImplicitExpressionConversionHelper : 'T -> Expression<'T>

        /// When used in a quotation, this function indicates a specific conversion
        /// should be performed when converting the quotation to a LINQ expression. 
        ///
        /// This function should not be called directly. 
        //[<System.Obsolete("This type is for use by the quotation to LINQ expression tree converter and is not for direct use from user code")>]
        val MemberInitializationHelper : 'T ->  'T

        /// When used in a quotation, this function indicates a specific conversion
        /// should be performed when converting the quotation to a LINQ expression. 
        ///
        /// This function should not be called directly. 
        //[<System.Obsolete("This type is for use by the quotation to LINQ expression tree converter and is not for direct use from user code")>]
        val NewAnonymousObjectHelper : 'T ->  'T

        /// Converts a subset of F# quotations to a LINQ expression, for the subset of LINQ expressions represented by the
        /// expression syntax in the C# language.
        val QuotationToExpression : Expr -> Expression

        /// Converts a subset of F# quotations to a LINQ expression, for the subset of LINQ expressions represented by the
        /// expression syntax in the C# language. 
        val QuotationToLambdaExpression : Expr<'T> -> Expression<'T>

        /// Evaluates a subset of F# quotations by first converting to a LINQ expression, for the subset of LINQ expressions represented by the
        /// expression syntax in the C# language.
        val EvaluateQuotation : Expr -> obj

        /// A runtime helper used to evaluate nested quotation literals.
        val SubstHelper : Expr * Var[] * obj[] -> Expr<'T>

        /// A runtime helper used to evaluate nested quotation literals.
        val SubstHelperRaw : Expr * Var[] * obj[] -> Expr

        val internal (|SpecificCallToMethod|_|) : System.RuntimeMethodHandle -> (Expr -> (Expr option * Type list * Expr list) option)
#endif