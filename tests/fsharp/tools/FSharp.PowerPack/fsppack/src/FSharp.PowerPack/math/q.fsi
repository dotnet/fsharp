namespace Microsoft.FSharp.Math

    open System
    open System.Numerics
      
    /// The type of arbitrary-sized rational numbers
    [<Sealed>]
    type BigNum =
        /// Return the sum of two rational numbers
        static member ( + ) : BigNum * BigNum -> BigNum
        /// Return the product of two rational numbers
        static member ( * ) : BigNum * BigNum -> BigNum
        /// Return the difference of two rational numbers
        static member ( - ) : BigNum * BigNum -> BigNum
        /// Return the ratio of two rational numbers
        static member ( / ) : BigNum * BigNum -> BigNum
        /// Return the negation of a rational number
        static member ( ~- ): BigNum          -> BigNum
        /// Return the given rational number
        static member ( ~+ ): BigNum          -> BigNum

        override ToString: unit -> string
        override GetHashCode: unit -> int
        interface System.IComparable

        /// Get zero as a rational number
        static member Zero : BigNum  
        /// Get one as a rational number
        static member One : BigNum  
        /// This operator is for use from other .NET languages
        static member op_Equality : BigNum * BigNum -> bool
        /// This operator is for use from other .NET languages
        static member op_Inequality : BigNum * BigNum -> bool
        /// This operator is for use from other .NET languages
        static member op_LessThan: BigNum * BigNum -> bool 
        /// This operator is for use from other .NET languages
        static member op_GreaterThan: BigNum * BigNum -> bool 
        /// This operator is for use from other .NET languages
        static member op_LessThanOrEqual: BigNum * BigNum -> bool 
        /// This operator is for use from other .NET languages
        static member op_GreaterThanOrEqual: BigNum * BigNum -> bool
        
        /// Return a boolean indicating if this rational number is strictly negative
        member IsNegative: bool 
        /// Return a boolean indicating if this rational number is strictly positive
        member IsPositive: bool 

        /// Return the numerator of the normalized rational number
        member Numerator: BigInteger
        /// Return the denominator of the normalized rational number
        member Denominator: BigInteger

        /// Return the absolute value of a rational number 
        static member Abs : BigNum -> BigNum
        /// Return the sign of a rational number; 0, +1 or -1
        member Sign : int 
        /// Return the result of raising the given rational number to the given power
        static member PowN : BigNum * int -> BigNum
        /// Return the result of converting the given integer to a rational number
        static member FromInt : int         -> BigNum  
        /// Return the result of converting the given big integer to a rational number
        static member FromBigInt : BigInteger      -> BigNum  
        /// Return the result of converting the given rational number to a floating point number
        static member ToDouble: BigNum -> float 
        /// Return the result of converting the given rational number to a big integer
        static member ToBigInt: BigNum -> BigInteger
        /// Return the result of converting the given rational number to an integer
        static member ToInt32 : BigNum -> int
        /// Return the result of converting the given rational number to a floating point number
        static member op_Explicit : BigNum -> float 
        /// Return the result of converting the given rational number to a big integer
        static member op_Explicit : BigNum -> BigInteger
        /// Return the result of converting the given rational number to an integer
        static member op_Explicit : BigNum -> int
        /// Return the result of converting the string to a rational number 
        static member Parse: string -> BigNum

    type BigRational = BigNum

    type bignum = BigNum

namespace Microsoft.FSharp.Core

    type bignum = Microsoft.FSharp.Math.BigNum
    [<RequireQualifiedAccess>]
    module NumericLiteralN = 
        val FromZero : unit -> bignum
        val FromOne : unit -> bignum
        val FromInt32 : int32 -> bignum
        val FromInt64 : int64 -> bignum
        val FromString : string -> bignum