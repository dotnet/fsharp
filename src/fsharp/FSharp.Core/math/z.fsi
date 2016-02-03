// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace System.Numerics
#if FX_NO_BIGINT

    open System
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Core
    
    /// The type of arbitrary-sized integers
    [<Struct>]
    [<CustomEquality; CustomComparison>]
    type BigInteger =
        /// Return the sum of two big integers
        static member ( + )      : x:BigInteger * y:BigInteger -> BigInteger
        /// Return the modulus of big integers
        static member ( % )      : x:BigInteger * y:BigInteger -> BigInteger
        /// Return the product of big integers
        static member ( * )      : x:BigInteger * y:BigInteger -> BigInteger
        /// Return the difference of two big integers
        static member ( - )      : x:BigInteger * y:BigInteger -> BigInteger
        /// Return the ratio of two big integers
        static member ( / )      : x:BigInteger * y:BigInteger -> BigInteger
        /// Return the negation of a big integer
        static member (~-)       : x:BigInteger -> BigInteger
        /// Return the given big integer
        static member (~+)       : x:BigInteger -> BigInteger
        /// Convert a big integer to a floating point number
        static member op_Explicit : x:BigInteger -> float
        /// Convert a big integer to a 64-bit signed integer
        static member op_Explicit : x:BigInteger -> int64
        /// Convert a big integer to a 32-bit signed integer
        static member op_Explicit : x:BigInteger -> int32
        /// Parse a big integer from a string format
        static member Parse    : text:string -> BigInteger
        /// Return the sign of a big integer: 0, +1 or -1
        member Sign    : int
        /// Compute the ratio and remainder of two big integers
        static member DivRem : x:BigInteger * y:BigInteger * [<System.Runtime.InteropServices.Out>]rem:BigInteger byref -> BigInteger

        /// This operator is for consistency when this type be used from other CLI languages
        static member op_LessThan           : x:BigInteger * y:BigInteger -> bool
        /// This operator is for consistency when this type be used from other CLI languages
        static member op_LessThanOrEqual    : x:BigInteger * y:BigInteger -> bool
        /// This operator is for consistency when this type be used from other CLI languages
        static member op_GreaterThan        : x:BigInteger * y:BigInteger -> bool
        /// This operator is for consistency when this type be used from other CLI languages
        static member op_GreaterThanOrEqual : x:BigInteger * y:BigInteger -> bool
        /// This operator is for consistency when this type be used from other CLI languages
        static member op_Equality             : x:BigInteger * y:BigInteger -> bool
        /// This operator is for consistency when this type be used from other CLI languages
        static member op_Inequality           : x:BigInteger * y:BigInteger -> bool

        /// Return the greatest common divisor of two big integers
        static member GreatestCommonDivisor : x:BigInteger * y:BigInteger -> BigInteger
        /// Return n^m for two big integers
        static member Pow    : x:BigInteger * y:int32 -> BigInteger
        /// Compute the absolute value of a big integer 
        static member Abs    : x:BigInteger -> BigInteger
        /// Get the big integer for zero
        static member Zero    : BigInteger 
        /// Get the big integer for one
        static member One     : BigInteger 

        /// Return true if a big integer is 'zero'
        member IsZero : bool
        /// Return true if a big integer is 'one'
        member IsOne : bool
        interface System.IComparable
        override Equals : obj -> bool
        override GetHashCode : unit -> int
        override ToString : unit -> string

        /// Construct a BigInteger value for the given integer
        new : x:int -> BigInteger
        /// Construct a BigInteger value for the given 64-bit integer
        new : x:int64 -> BigInteger
#endif


namespace Microsoft.FSharp.Core

    type bigint = System.Numerics.BigInteger

    [<AutoOpen>]
    /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
    module NumericLiterals =

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
        module NumericLiteralI = 
            open System.Numerics

            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromZero : value:unit -> 'T
            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromOne : value:unit -> 'T
            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromInt32 : value:int32 -> 'T
            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromInt64 : value:int64 -> 'T
            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromString : text:string -> 'T
            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromInt64Dynamic : value:int64 -> obj
            /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' 
            val FromStringDynamic : text:string -> obj

        
