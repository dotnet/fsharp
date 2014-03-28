namespace Microsoft.FSharp.Math

open Microsoft.FSharp.Math
open System
open System.Numerics
open System.Globalization

type INumeric<'T> =
    abstract Zero: 'T
    abstract One: 'T
    abstract Add: 'T * 'T -> 'T
    abstract Subtract: 'T * 'T -> 'T
    abstract Multiply : 'T * 'T -> 'T
    abstract Compare : 'T * 'T -> int
    abstract Equals : 'T * 'T -> bool
    abstract Negate : 'T -> 'T
    abstract Sign : 'T -> int
    abstract Abs : 'T -> 'T
    abstract ToString : 'T * string * System.IFormatProvider -> string
    abstract Parse : string * System.Globalization.NumberStyles * System.IFormatProvider -> 'T

type IIntegral<'T> =
    inherit INumeric<'T>
    abstract Modulus: 'T * 'T -> 'T
    abstract Divide : 'T * 'T -> 'T
    abstract DivRem : 'T * 'T -> 'T * 'T
    abstract ToBigInt : 'T -> BigInteger
    abstract OfBigInt : BigInteger -> 'T
  
type IFractional<'T> =
    inherit INumeric<'T>
    abstract Reciprocal : 'T -> 'T
    abstract Divide : 'T * 'T -> 'T

type IFloating<'T> =
    inherit IFractional<'T>
    abstract Pi : 'T
    abstract Exp : 'T -> 'T
    abstract Log : 'T -> 'T
    abstract Sqrt : 'T -> 'T
    abstract LogN : 'T * 'T -> 'T
    abstract Sin : 'T -> 'T
    abstract Cos : 'T -> 'T
    abstract Tan : 'T -> 'T
    abstract Asin : 'T -> 'T
    abstract Acos : 'T -> 'T
    abstract Atan : 'T -> 'T
    abstract Atan2 : 'T * 'T -> 'T
    abstract Sinh : 'T -> 'T
    abstract Cosh : 'T -> 'T
    abstract Tanh : 'T -> 'T

type IIEEE<'T> =
    inherit IFloating<'T>
    abstract PositiveInfinity : 'T
    abstract NegativeInfinity : 'T
    abstract NaN              : 'T
    abstract EpsilonOne          : 'T
    abstract IsNaN: 'T -> bool 
    abstract IsInfinite : 'T -> bool 

type INormFloat<'T> =
    abstract Norm : 'T -> float
 
module Instances = 
  let Int32Numerics = 
    { new IIntegral<int32> with 
         member __.Zero = 0
         member __.One = 1
         member __.Add(a,b) = a + b
         member __.Subtract(a,b) = a - b
         member __.Multiply(a,b) = a * b
         member __.Equals(a,b) = (a = b)
         member __.Compare(a,b) = compare a b
         member __.Negate(a) = - a 
         member __.Abs(a) = a
         member __.ToBigInt(a) = new BigInteger(a)
         member __.OfBigInt(a) = int32 a
         member __.Sign(a) = Math.Sign(a)
         member __.Modulus(a,b) = a % b
         member __.Divide(a,b) = a / b
         member __.DivRem(a,b) = (a / b, a % b)
         member __.ToString((x:int32),fmt,fmtprovider) = 
                x.ToString(fmt,fmtprovider) 
         member __.Parse(s,numstyle,fmtprovider) = 
                System.Int32.Parse(s,numstyle,fmtprovider)
      interface INormFloat<int32> with  
         member __.Norm(x) = float (abs x)
    }
  let Int64Numerics = 
    { new IIntegral<int64> with 
         member __.Zero =0L
         member __.One = 1L
         member __.Add(a,b) = a + b
         member __.Subtract(a,b) = a - b
         member __.Multiply(a,b) = a * b
         member __.Negate(a) = - a 
         member __.Abs(a) = Math.Abs(a)
         member __.ToBigInt(a) = new BigInteger(a)
         member __.OfBigInt(a) = int64 a
         member __.Sign(a) = Math.Sign(a)
         member __.Modulus(a,b) = a % b
         member __.Equals(a,b) = (a = b)
         member __.Compare(a,b) = compare a b
         member __.Divide(a,b) = a / b
         member __.DivRem(a,b) = (a / b, a % b)
         member __.ToString((x:int64),fmt,fmtprovider) = x.ToString(fmt,fmtprovider) 
         member __.Parse(s,numstyle,fmtprovider) = System.Int64.Parse(s,numstyle,fmtprovider)
      interface INormFloat<int64> with
         member __.Norm(x) = float (Math.Abs x)
    }
  let FloatNumerics = 
    { new IIEEE<float> with 
         member __.Zero = 0.0
         member __.One =  1.0
         member __.Add(a,b) =  a + b
         member __.Subtract(a,b) = a - b
         member __.Multiply(a,b) = a * b
         member __.Equals(a,b) = (a = b)
         member __.Compare(a,b) = compare a b
         member __.PositiveInfinity = Double.PositiveInfinity
         member __.NegativeInfinity = Double.NegativeInfinity
         member __.NaN = Double.NaN
         member __.EpsilonOne = 0x3CB0000000000000LF
         member __.IsInfinite(a) = Double.IsInfinity(a)
         member __.IsNaN(a) = Double.IsNaN(a)
         member __.Pi = Math.PI
         member __.Reciprocal(a) = 1.0/a
         member __.Abs(a) = Math.Abs(a)
         member __.Sign(a) = Math.Sign(a)
         member __.Asin(a) = Math.Asin(a)
         member __.Acos(a) = Math.Acos(a)
         member __.Atan(a) = Math.Atan(a)
         member __.Atan2(a,b) = Math.Atan2(a,b)
         member __.Tanh(a) = Math.Tanh(a)
         member __.Tan(a) = Math.Tan(a)
         member __.Sqrt(a) = Math.Sqrt(a)
         member __.Sinh(a) = Math.Sinh(a)
         member __.Cosh(a) = Math.Cosh(a)
         member __.Sin(a) = Math.Sin(a)
         member __.Cos(a) = Math.Cos(a)
         member __.LogN(a,n) = 
#if FX_NO_LOGN
             raise (System.NotSupportedException("this operation is not supported on this platform"))
#else
             Math.Log(a,n)
#endif
         member __.Log(a) = Math.Log(a)
         member __.Exp(a) = Math.Exp(a)
         member __.Negate(a) = -a 
         member __.Divide(a,b) = a / b
         member __.ToString((x:float),fmt,fmtprovider) = x.ToString(fmt,fmtprovider) 
         member __.Parse(s,numstyle,fmtprovider) = System.Double.Parse(s,numstyle,fmtprovider)
      interface INormFloat<float> with
          member __.Norm(x) = float (Math.Abs x)
    }
  let Float32Numerics = 
    { new IFractional<float32> with
           member __.Zero = 0.0f
           member __.One =  1.0f
           member __.Add(a,b) = a + b
           member __.Subtract(a,b) = a - b
           member __.Multiply(a,b) = a * b
           member __.Equals(a,b) = (a = b)
           member __.Compare(a,b) = compare a b
           member __.Negate(a) = -a 
           member __.Reciprocal(a) = 1.0f/a
           member __.Sign(a) = Math.Sign(a)
           member __.Abs(a) = Math.Abs(a)
           member __.Divide(a,b) = a / b
           member __.ToString((x:float32),fmt,fmtprovider) = x.ToString(fmt,fmtprovider) 
           member __.Parse(s,numstyle,fmtprovider) = System.Single.Parse(s,numstyle,fmtprovider)
       interface INormFloat<float32> with  
           member __.Norm(x) = float (Math.Abs x)
    }

  let BigNumNumerics = 
    { new IFractional<bignum> with 
         member __.Zero = BigNum.Zero
         member __.One = BigNum.One
         member __.Add(a,b)      = a + b
         member __.Subtract(a,b) = a - b
         member __.Multiply(a,b) = a * b
         member __.Equals(a,b) = (a = b)
         member __.Compare(a,b) = compare a b
         member __.Divide(a,b)   = a / b
         member __.Abs(a) = BigNum.Abs a
         member __.Sign(a) = a.Sign
         member __.Negate(a) = - a 
         member __.Reciprocal(a) = BigNum.One / a 
         // Note, this ignores fmt, fmtprovider
         member __.ToString((x:bignum),fmt,fmtprovider) = x.ToString()
         // Note, this ignroes numstyle, fmtprovider
         member __.Parse(s,numstyle,fmtprovider) = BigNum.Parse(s)

      interface INormFloat<bignum> with
         member __.Norm(x) = float (BigNum.Abs x)
    }       

  let BigIntNumerics = 
    let ZeroI = new BigInteger(0)
    { new IIntegral<_> with 
         member __.Zero = BigInteger.Zero
         member __.One =  BigInteger.One
         member __.Add(a,b) = a + b
         member __.Subtract(a,b) = a - b
         member __.Multiply(a,b) = a * b
         member __.Equals(a,b) = (a = b)
         member __.Compare(a,b) = compare a b
         member __.Divide(a,b) = a / b
         member __.Negate(a) = -a 
         member __.Modulus(a,b) = a % b
         member __.DivRem(a,b) = 
            let mutable r = new BigInteger(0)
            (BigInteger.DivRem (a,b,&r),r)
         member __.Sign(a) = a.Sign
         member __.Abs(a) = abs a
         member __.ToBigInt(a) = a 
         member __.OfBigInt(a) = a 
         
         member __.ToString(x,fmt,fmtprovider) = 
#if FX_ATLEAST_40
             x.ToString(fmt,fmtprovider) 
#else
             // Note: this ignores fmt and fmtprovider
             x.ToString() 
#endif
         // Note: this ignores fmt and fmtprovider
         member __.Parse(s,numstyle,fmtprovider) = 
#if FX_ATLEAST_40
             BigInteger.Parse(s,numstyle,fmtprovider)
#else
             BigInteger.Parse(s)
#endif

      interface INormFloat<BigInteger> with  
         member __.Norm(x) = float (abs x)
    }       

