#nowarn "44"  // OK to use the "compiler only" function RangeGeneric
#nowarn "52"  // The value has been copied to ensure the original is not mutated by this operation

namespace Microsoft.FSharp.Math

    open System
    open System.Numerics
    open System.Globalization

    module BigRationalLargeImpl = 
        let ZeroI = new BigInteger(0)
        let OneI = new BigInteger(1)
        let bigint (x:int) = new BigInteger(x)
        let ToDoubleI (x:BigInteger) =  double x
        let ToInt32I (x:BigInteger) = int32 x

    open BigRationalLargeImpl
        
    [<CustomEquality; CustomComparison>]
    type BigRationalLarge = 
        | Q of BigInteger * BigInteger // invariants: (p,q) in lowest form, q >= 0 

        override n.ToString() =
            let (Q(p,q)) = n 
            if q.IsOne then p.ToString() 
            else p.ToString() + "/" + q.ToString()


        static member Hash (Q(ap,aq)) = 
            // This hash code must be identical to the hash for BigInteger when the numbers coincide.
            if aq.IsOne then ap.GetHashCode() else (ap.GetHashCode() <<< 3) + aq.GetHashCode()
        

        override x.GetHashCode()            = BigRationalLarge.Hash(x)
        
        static member Equals(Q(ap,aq), Q(bp,bq)) = 
            BigInteger.(=)  (ap,bp) && BigInteger.(=) (aq,bq)   // normal form, so structural equality 
        
        static member LessThan(Q(ap,aq), Q(bp,bq)) = 
            BigInteger.(<)  (ap * bq,bp * aq)
        
        // note: performance improvement possible here
        static member Compare(p,q) = 
            if BigRationalLarge.LessThan(p,q) then -1 
            elif BigRationalLarge.LessThan(q,p)then  1 
            else 0 

        interface System.IComparable with 
            member this.CompareTo(obj:obj) = 
                match obj with 
                | :? BigRationalLarge as that -> BigRationalLarge.Compare(this,that)
                | _ -> invalidArg "obj" "the object does not have the correct type"

        override this.Equals(that:obj) = 
            match that with 
            | :? BigRationalLarge as that -> BigRationalLarge.Equals(this,that)
            | _ -> false

        member x.IsNegative = let (Q(ap,_)) = x in sign ap < 0
        member x.IsPositive = let (Q(ap,_)) = x in sign ap > 0

        member x.Numerator = let (Q(p,_)) = x in p
        member x.Denominator = let (Q(_,q)) = x in q
        member x.Sign = (let (Q(p,_)) = x in sign p)

        static member ToDouble (Q(p,q)) = 
            ToDoubleI p / ToDoubleI q

        static member Normalize (p:BigInteger,q:BigInteger) =
            if q.IsZero then
                raise (System.DivideByZeroException())  (* throw for any x/0 *)
            elif q.IsOne then
                Q(p,q)
            else
                let k = BigInteger.GreatestCommonDivisor(p,q)
                let p = p / k 
                let q = q / k 
                if sign q < 0 then Q(-p,-q) else Q(p,q)

        static member Rational  (p:int,q:int) = BigRationalLarge.Normalize (bigint p,bigint q)
        static member RationalZ (p,q) = BigRationalLarge.Normalize (p,q)
       
        static member Parse (str:string) =
          let len = str.Length 
          if len=0 then invalidArg "str" "empty string";
          let j = str.IndexOf '/' 
          if j >= 0 then 
              let p = BigInteger.Parse (str.Substring(0,j)) 
              let q = BigInteger.Parse (str.Substring(j+1,len-j-1)) 
              BigRationalLarge.RationalZ (p,q)
          else
              let p = BigInteger.Parse str 
              BigRationalLarge.RationalZ (p,OneI)
        
        static member (~-) (Q(bp,bq))    = Q(-bp,bq)          // still coprime, bq >= 0 
        static member (+) (Q(ap,aq),Q(bp,bq)) = BigRationalLarge.Normalize ((ap * bq) + (bp * aq),aq * bq)
        static member (-) (Q(ap,aq),Q(bp,bq)) = BigRationalLarge.Normalize ((ap * bq) - (bp * aq),aq * bq)
        static member (*) (Q(ap,aq),Q(bp,bq)) = BigRationalLarge.Normalize (ap * bp,aq * bq)
        static member (/) (Q(ap,aq),Q(bp,bq)) = BigRationalLarge.Normalize (ap * bq,aq * bp)
        static member ( ~+ )(n1:BigRationalLarge) = n1
        
 
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module BigRationalLarge = 
        open System.Numerics
    
        let inv    (Q(ap,aq)) = BigRationalLarge.Normalize(aq,ap)    

        let pown (Q(p,q)) (n:int) = Q(BigInteger.Pow(p,n),BigInteger.Pow  (q,n)) // p,q powers still coprime
        
        let equal (Q(ap,aq)) (Q(bp,bq)) = ap=bp && aq=bq   // normal form, so structural equality 
        let lt    a b = BigRationalLarge.LessThan(a,b)
        let gt    a b = BigRationalLarge.LessThan(b,a)
        let lte   (Q(ap,aq)) (Q(bp,bq)) = BigInteger.(<=) (ap * bq,bp * aq)
        let gte   (Q(ap,aq)) (Q(bp,bq)) = BigInteger.(>=) (ap * bq,bp * aq)

        let of_bigint   z = BigRationalLarge.RationalZ(z,OneI )
        let of_int n = BigRationalLarge.Rational(n,1)
       
        // integer part
        let integer (Q(p,q)) =
            let mutable r = BigInteger(0)
            let d = BigInteger.DivRem (p,q,&r)          // have p = d.q + r, |r| < |q| 
            if r < ZeroI
            then d - OneI                 // p = (d-1).q + (r+q) 
            else d                             // p =     d.q + r       
      
        
    //----------------------------------------------------------------------------
    // BigNum
    //--------------------------------------------------------------------------

    [<CustomEquality; CustomComparison>]
    [<StructuredFormatDisplay("{StructuredDisplayString}N")>]
    type BigNum =
        | Z of BigInteger
        | Q of BigRationalLarge

        static member ( + )(n1,n2) = 
            match n1,n2 with
            | Z z ,Z zz -> Z (z + zz)
            | Q q ,Q qq -> Q (q + qq)
            | Z z ,Q qq -> Q (BigRationalLarge.of_bigint z + qq)
            | Q q ,Z zz -> Q (q  + BigRationalLarge.of_bigint zz)

        static member ( * )(n1,n2) = 
            match n1,n2 with
            | Z z ,Z zz -> Z (z * zz)
            | Q q ,Q qq -> Q (q * qq)
            | Z z ,Q qq -> Q (BigRationalLarge.of_bigint z * qq)
            | Q q ,Z zz -> Q (q  * BigRationalLarge.of_bigint zz)

        static member ( - )(n1,n2) = 
            match n1,n2 with
            | Z z ,Z zz -> Z (z - zz)
            | Q q ,Q qq -> Q (q - qq)
            | Z z ,Q qq -> Q (BigRationalLarge.of_bigint z - qq)
            | Q q ,Z zz -> Q (q  - BigRationalLarge.of_bigint zz)

        static member ( / )(n1,n2) = 
            match n1,n2 with
            | Z z ,Z zz -> Q (BigRationalLarge.RationalZ(z,zz))
            | Q q ,Q qq -> Q (q / qq)
            | Z z ,Q qq -> Q (BigRationalLarge.of_bigint z / qq)
            | Q q ,Z zz -> Q (q  / BigRationalLarge.of_bigint zz)

        static member ( ~- )(n1) = 
            match n1 with
            | Z z -> Z (-z)
            | Q q -> Q (-q)

        static member ( ~+ )(n1:BigNum) = n1

        // nb. Q and Z hash codes must match up - see notes above
        override n.GetHashCode() = 
            match n with 
            | Z z -> z.GetHashCode()
            | Q q -> q.GetHashCode() 

        override this.Equals(obj:obj) = 
            match obj with 
            | :? BigNum as that -> BigNum.(=)(this, that)
            | _ -> false

        interface System.IComparable with 
            member n1.CompareTo(obj:obj) = 
                match obj with 
                | :? BigNum as n2 -> 
                      if BigNum.(<)(n1, n2) then -1 elif BigNum.(=)(n1, n2) then 0 else 1
                | _ -> invalidArg "obj" "the objects are not comparable"

        static member FromInt (x:int) = Z (bigint x)
        static member FromBigInt x = Z x

        static member Zero = BigNum.FromInt(0) 
        static member One = BigNum.FromInt(1) 


        static member PowN (n,i:int) =
            match n with
            | Z z -> Z (BigInteger.Pow (z,i))
            | Q q -> Q (BigRationalLarge.pown q i)

        static member op_Equality (n,nn) = 
            match n,nn with
            | Z z ,Z zz -> BigInteger.(=) (z,zz)
            | Q q ,Q qq -> (BigRationalLarge.equal q qq)
            | Z z ,Q qq -> (BigRationalLarge.equal (BigRationalLarge.of_bigint z) qq)
            | Q q ,Z zz -> (BigRationalLarge.equal q (BigRationalLarge.of_bigint zz))
        static member op_Inequality (n,nn) = not (BigNum.op_Equality(n,nn))
    
        static member op_LessThan (n,nn) = 
            match n,nn with
            | Z z ,Z zz -> BigInteger.(<) (z,zz)
            | Q q ,Q qq -> (BigRationalLarge.lt q qq)
            | Z z ,Q qq -> (BigRationalLarge.lt (BigRationalLarge.of_bigint z) qq)
            | Q q ,Z zz -> (BigRationalLarge.lt q (BigRationalLarge.of_bigint zz))
        static member op_GreaterThan (n,nn) = 
            match n,nn with
            | Z z ,Z zz -> BigInteger.(>) (z,zz)
            | Q q ,Q qq -> (BigRationalLarge.gt q qq)
            | Z z ,Q qq -> (BigRationalLarge.gt (BigRationalLarge.of_bigint z) qq)
            | Q q ,Z zz -> (BigRationalLarge.gt q (BigRationalLarge.of_bigint zz))
        static member op_LessThanOrEqual (n,nn) = 
            match n,nn with
            | Z z ,Z zz -> BigInteger.(<=) (z,zz)
            | Q q ,Q qq -> (BigRationalLarge.lte q qq)
            | Z z ,Q qq -> (BigRationalLarge.lte (BigRationalLarge.of_bigint z) qq)
            | Q q ,Z zz -> (BigRationalLarge.lte q (BigRationalLarge.of_bigint zz))
        static member op_GreaterThanOrEqual (n,nn) = 
            match n,nn with
            | Z z ,Z zz -> BigInteger.(>=) (z,zz)
            | Q q ,Q qq -> (BigRationalLarge.gte q qq)
            | Z z ,Q qq -> (BigRationalLarge.gte (BigRationalLarge.of_bigint z) qq)
            | Q q ,Z zz -> (BigRationalLarge.gte q (BigRationalLarge.of_bigint zz))
        

        member n.IsNegative = 
            match n with 
            | Z z -> sign z < 0 
            | Q q -> q.IsNegative

        member n.IsPositive = 
            match n with 
            | Z z -> sign z > 0
            | Q q -> q.IsPositive
            
        member n.Numerator = 
            match n with 
            | Z z -> z
            | Q q -> q.Numerator

        member n.Denominator = 
            match n with 
            | Z _ -> OneI
            | Q q -> q.Denominator

        member n.Sign = 
            if n.IsNegative then -1 
            elif n.IsPositive then  1 
            else 0

        static member Abs(n:BigNum) = 
            if n.IsNegative then -n else n

        static member ToDouble(n:BigNum) = 
            match n with
            | Z z -> ToDoubleI z
            | Q q -> BigRationalLarge.ToDouble q

        static member ToBigInt(n:BigNum) = 
            match n with 
            | Z z -> z
            | Q q -> BigRationalLarge.integer q 

        static member ToInt32(n:BigNum) = 
            match n with 
            | Z z -> ToInt32I(z)
            | Q q -> ToInt32I(BigRationalLarge.integer q )

        static member op_Explicit (n:BigNum) = BigNum.ToInt32 n
        static member op_Explicit (n:BigNum) = BigNum.ToDouble n
        static member op_Explicit (n:BigNum) = BigNum.ToBigInt n


        override n.ToString() = 
            match n with 
            | Z z -> z.ToString()
            | Q q -> q.ToString()

        member x.StructuredDisplayString = x.ToString()
                   
        static member Parse(s:string) = Q (BigRationalLarge.Parse s)

    type BigRational = BigNum
    type bignum = BigNum

namespace Microsoft.FSharp.Core
    open Microsoft.FSharp.Math
    open System.Numerics    
    type bignum = BigNum

    // FxCop suppressions 
    open System.Diagnostics.CodeAnalysis  
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_Addition(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_Division(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_GreaterThan(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_GreaterThanOrEqual(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_LessThan(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_LessThanOrEqual(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_Multiply(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_Subtraction(Microsoft.FSharp.Math.BigNum,Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_UnaryNegation(Microsoft.FSharp.Math.BigNum)")>]
    [<assembly: SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Scope="member", Target="Microsoft.FSharp.Math.BigNum.#op_UnaryPlus(Microsoft.FSharp.Math.BigNum)")>]
    do()

    module NumericLiteralN = 
        let FromZero () = BigNum.Zero 
        let FromOne () = BigNum.One 
        let FromInt32 i = BigNum.FromInt i
        let FromInt64 (i64:int64) = BigNum.FromBigInt (new BigInteger(i64))
        let FromString s = BigNum.Parse s
