// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Math

#if FX_NO_BIGINT
open System
open System.Diagnostics.CodeAnalysis
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Primitives.Basics

type ints = int array

[<NoEquality; NoComparison>]
type internal BigNat = 

    // Have n = sum (from i=0 to bound) a.[i] * baseN ^ i
    // Have 0 <= a.[i] < baseN.
    //------
    // Invariant: bound is least such, i.e. bound=0 or (a.[bound-1] is highest coeff).
    // Zero is {bound=0,a=...}.
    // Naturals are a normal form,
    // but not structurally so,
    // since arrays may have non-contributing cells at a.[bound] and beyond.
    //
    { mutable bound : int;  // non-zero coeff must be 0...(bound-1) 
      digits : ints         // must have at least elts   0...(bound-1),
                            // maybe more (which should be zero!).
                            // Actually, the "zero" condition may be relaxed.
                            //
    }


module internal BigNatModule = 

    //-------------------------------------------------------------------------
    // misc
    //-----------------------------------------------------------------------

    #if SELFTEST
    let check b = if not b then failwith "assertion failwith"
    #endif

    module FFT =
        let rec pow32 x n =
            if   n=0       then 1 
            elif n % 2 = 0 then pow32 (x*x) (n / 2)
            else                x* pow32 (x*x) (n / 2)

        let leastBounding2Power b =
            let rec findBounding2Power b tp i = if b<=tp then tp,i else findBounding2Power b (tp*2) (i+1) in
            findBounding2Power b 1 0

        //-------------------------------------------------------------------------
        // p = 2^k.m + 1 prime and w primitive 2^k root of 1 mod p
        //-----------------------------------------------------------------------

        // Given p = 2^k.m + 1 prime and w a primitive 2^k root of unity (mod p).
        // Required to define arithmetic ops for Fp = field modulo p.
        // The following are possible choices for p.
           
        //                            p,  k,  m,       g,  w 
        //  let p,k,m,g,w =         97L,  4,  6,       5,   8           // p is  7 bit 
        //  let p,k,m,g,w =        769L,  8,  3,       7,   7           // p is 10 bit 
        //  let p,k,m,g,w =       7681L,  8, 30,      13, 198           // p is 13 bit 
        //  let p,k,m,g,w =      12289L, 10, 12,      11,  49           // p is 14 bit 
        //  let p,k,m,g,w =  167772161L, 25,  5,  557092, 39162105      // p is 28 bit 
        //  let p,k,m,g,w =  469762049L, 26,  7, 1226571, 288772249     // p is 29 bit 
        

        let p,k,m,g,w = 2013265921L, 27, 15,      31, 440564289     // p is 31 bit 
        let primeP = p  

        let maxBitsInsideFp = 30    


        //-------------------------------------------------------------------------
        // Fp = finite field mod p - rep is uint32
        //-----------------------------------------------------------------------


        type fp = uint32
        // operations in Fp (finite field size p) 
        module Fp = 
            //module I = UInt32
            let p   = 2013265921ul : fp
            let p64 = 2013265921UL : uint64
            let toInt   (x:fp) : int = int32 x
            let ofInt32   (x:int) : fp = uint32 x

            let mzero : fp = 0ul
            let mone  : fp = 1ul
            let mtwo  : fp = 2ul
            let inline madd (x:fp) (y:fp) : fp = (x + y) % p
            let inline msub (x:fp) (y:fp) : fp = (x + p - y) % p
            let inline mmul (x:fp) (y:fp) : fp = uint32 ((uint64 x * uint64 y) % p64)

            let rec mpow x n =
                if n=0       then mone
                elif n % 2=0 then mpow (mmul x x) (n / 2)
                else              mmul x (mpow (mmul x x) (n / 2))
                    
            let rec mpowL x n =
                if   n = 0L      then mone
                elif n % 2L = 0L then mpowL (mmul x x) (n / 2L)
                else                  mmul x (mpowL (mmul x x) (n / 2L))
                    
            // Have the w is primitive 2^kth root of 1 in Zp           
            let m2PowNthRoot n =
                // Find x s.t. x is (2^n)th root of unity.
                //
                //   pow w (pow 2 k) = 1 primitively.
                // = pow w (pow 2 ((k-n)+n))
                // = pow w (pow 2 (k-n) * pow 2 n)
                // = pow (pow w (pow 2 (k-n))) (pow 2 n)
                //
                // Take wn = pow (pow w (pow 2 (k-n)))
                 
                mpow (uint32 w) (pow32 2 (k-n))
                
            let minv x = mpowL x (primeP - 2L)


        //-------------------------------------------------------------------------
        // FFT - in place low garbage
        //-----------------------------------------------------------------------

        open Fp
        let rec computeFFT lambda mu n w (u: _[])  (res: _[])  offset =
            // Given n a 2-power,
            //       w an nth root of 1 in Fp, and
            //       lambda, mu and u(x) defining
            //       poly(lambda,mu,x) = sum(i<n) u(lambda.i + mu).x^i
            //
            //       Note, "lambda.i + mu" for i=0...(n-1) defines the coefficients of the u(x) odd/even sub polys.
            // 
            // Compute res.[offset+j] = poly(lambda,mu,w^j)
            // ---
            // poly(lambda,mu,x) = sum(i<n/2) u.[lambda.2i + mu] * x^2i  + x.sum(i<n/2) u.[lambda.(2i+1) + mu] * x^2i
            //                   = poly(2.lambda,mu,x^2)                 + x.poly(2.lambda,lambda+mu,x^2)
            // ---
            // Recursively call s.t.
            // For j<n/2,
            //   res.[offset+j    ] = poly(2.lambda,mu       ,(w^2)^j)
            //   res.[offset+j+n/2] = poly(2.lambda,lambda+mu,(w^2)^j)
            // For j<n/2,
            //   even = res.[offset+j]
            //   odd  = res.[offset+j+n/2]
            //   res.[offset+j]     = even + w^j * odd
            //   res.[offset+j+n/2] = even - w^j * odd
             
            if n=1 then
                res.[offset] <- u.[mu]
            else
                let halfN       = n/2 
                let ww          = mmul w w 
                let offsetHalfN = offset + halfN 
                computeFFT (lambda*2) mu            halfN ww u res offset      
                computeFFT (lambda*2) (lambda + mu) halfN ww u res offsetHalfN 
                let mutable wj  = mone 
                for j = 0 to halfN-1 do
                    let even = res.[offset+j]      
                    let odd  = res.[offsetHalfN+j] 
                    res.[offset+j]      <- madd even (mmul wj odd);
                    res.[offsetHalfN+j] <- msub even (mmul wj odd);
                    wj <- mmul w wj

        let computFftInPlace n w u =
            // Given n a power of 2,
            //       w a primitive nth root of unity in Fp,
            //       u(x) = sum(i<n) u.[i] * x^i
            // Compute res.[j] = u(w^j) for j<n.           
            let lambda = 1 
            let mu     = 0 
            let res    = Array.create n mzero 
            let offset = 0 
            computeFFT lambda mu n w u res offset;
            res

        let computeInverseFftInPlace n w uT =
            let bigKInv = minv (uint32 n) 
            Array.map
              (mmul bigKInv)
              (computFftInPlace n (minv w) uT)

        //-------------------------------------------------------------------------
        // FFT - polynomial product
        //-----------------------------------------------------------------------

        let maxTwoPower   = 29
        let twoPowerTable = Array.init (maxTwoPower-1) (fun i -> pow32 2 i)

        let computeFftPaddedPolynomialProduct bigK k u v =
            // REQUIRES: bigK = 2^k
            // REQUIRES: Array lengths of u and v = bigK.
            // REQUIRES: degree(uv) <= bigK-1
            // ---
            // Given u,v polynomials.
            // Computes the product polynomial by FFT.
            // For correctness,
            //   require the result coeff to be in range [0,p-1], for p defining Fp above.
             
        #if SELFTEST
            check ( k <= maxTwoPower );
            check ( bigK = twoPowerTable.[k] );
            check ( u.Length = bigK );
            check ( v.Length = bigK );
        #endif
            // Find 2^k primitive root of 1 
            let w      = m2PowNthRoot k 
            // FFT 
            let n  = bigK 
            let uT = computFftInPlace n w u 
            let vT = computFftInPlace n w v 
            // Evaluate 
            let rT = Array.init n (fun i -> mmul uT.[i] vT.[i]) 
            // INV FFT 
            let r  = computeInverseFftInPlace n w rT 
            r

        let padTo n (u: _ array) =
            let uBound = u.Length 
            Array.init n (fun i -> if i<uBound then Fp.ofInt32 u.[i] else Fp.mzero)

        let computeFftPolynomialProduct degu u degv v =
            // u,v polynomials.
            // Compute the product polynomial by FFT.
            // For correctness,
            //   require the result coeff to be in range [0,p-1], for p defining Fp above.
             
            let deguv  = degu + degv 
            let bound  = deguv + 1   
            let bigK,k = leastBounding2Power bound 
            let w      = m2PowNthRoot k 
            // PAD 
            let u      = padTo bigK u 
            let v      = padTo bigK v 
            // FFT 
            let n  = bigK 
            let uT = computFftInPlace n w u 
            let vT = computFftInPlace n w v 
            // Evaluate 
            let rT = Array.init n (fun i -> mmul uT.[i] vT.[i]) 
            // INV FFT 
            let r  = computeInverseFftInPlace n w rT 
            Array.map Fp.toInt r


        //-------------------------------------------------------------------------
        // fp exports
        //-----------------------------------------------------------------------

        open Fp
        let mzero = mzero
        let mone  = mone
        let maxFp             = msub Fp.p mone

        //-------------------------------------------------------------------------
        // FFT - reference implementation
        //-----------------------------------------------------------------------
            
        #if SELFTEST
        open Fp
        let rec computeFftReference n w u =
            // Given n a 2-power,
            //       w an nth root of 1 in Fp, and
            //       u(x) = sum(i<n) u(i).x^i
            // Compute res.[j] = u(w^j)
            // ---
            // u(x) = sum(i<n/2) u.[2i] * x^i  +  x . sum(i<n/2) u.[2i+1] * x^i
            //      = ueven(x)                 +  x . uodd(x)
            // ---
            // u(w^j)         = ueven(w^2j) + w^j . uodd(w^2j)
            // u(w^(halfN+j)) = ueven(w^2j) - w^j . uodd(w^2j)
            //)
            if n=1 then
              [| u.[0];
              |]
            else
                let ueven   = Array.init (n/2) (fun i -> u.[2*i])   
                let uodd    = Array.init (n/2) (fun i -> u.[2*i+1]) 
                let uevenFT = computeFftReference (n/2) (mmul w w) ueven 
                let uoddFT  = computeFftReference (n/2) (mmul w w) uodd  
                  Array.init n
                    (fun j ->
                       if j < n/2 then
                         madd
                           (uevenFT.[j])
                           (mmul
                              (mpow w j)
                              (uoddFT.[j]))
                       else
                         let j = j - (n/2) 
                         msub
                             (uevenFT.[j])
                             (mmul
                                (mpow w j)
                                (uoddFT.[j])))
        #endif

    open FFT
    
    type n = BigNat

    let bound (n: n) = n.bound
    let setBound (n: n) (v:int32) = n.bound <- v
    let coeff (n:n) i = n.digits.[i]
    let coeff64 (n:n) i = int64 (coeff n i)
    let setCoeff (n:n) i v = n.digits.[i] <- v

    let rec pow64 x n =
        if   n=0       then 1L 
        elif n % 2 = 0 then pow64 (x * x) (n / 2)
        else                x * (pow64 (x * x) (n / 2))

    let rec pow32 x n =
        if   n=0       then 1 
        elif n % 2 = 0 then pow32 (x*x) (n / 2)
        else                x* pow32 (x*x) (n / 2)
      
    let hash(n) = 
        let mutable res = 0 
        for i = 0 to n.bound - 1 do  // could stop soon, it's "hash" 
           res <- n.digits.[i] + (res <<< 3)
        done;
        res

    //----------------------------------------------------------------------------
    // misc
    //--------------------------------------------------------------------------

#if CHECKED
    let check b str = if not b then failwith ("check failed: " + str)
#endif
    let maxInt a b = if a<b then b else (a:int)  
    let minInt a b = if a<b then a else (b:int)  

//----------------------------------------------------------------------------
// n = big nat
//--------------------------------------------------------------------------

    // Nats stored as bit patterns, split into "digits" of baseBits each.
    // Intending to use 32 bits with unsigned ints on .NET.
    // The FFT algorithm wants to split out a fixed number of bits, say, 8 bits,
    // so using a multiple of that here.
    //
    let baseBits    = 24
    let baseN       = 0x1000000
    let baseMask    = 0xffffff
    let baseNi64    = 0x1000000L
    let baseMaski64 = 0xffffffL
    let baseMaskU   = 0xffffffUL

    // Masks and shifts to generate a uint32      
    let baseMask32A  = 0xffffff
    let baseMask32B  = 0xff
    let baseShift32B = 24

    // Masks and shifts to generate a uint64
    let baseMask64A  = 0xffffff
    let baseMask64B  = 0xffffff
    let baseMask64C  = 0xffff
    let baseShift64B = 24
    let baseShift64C = 48
      
      
#if CHECKED
    let _ = check (baseN    = pow32 2 baseBits) "baseN"
    let _ = check (baseNi64 = int64 baseN)   "baseNi64"
    let _ = check (baseMaski64 + 1L = baseNi64)     "baseMask"
#endif
     
    let inline mod64base  (x:int64) = (x &&& baseMaski64) |> int32
    let inline div64base  (x:int64) = int64 (uint64 x >>> baseBits)

    let divbase x = int32 (uint32 x >>> baseBits)
    let modbase x = (x &&& baseMask)
      
    let inline index z i = if i < z.bound then z.digits.[i] else 0

    let createN b = { bound = b; 
                      digits = Array.zeroCreate b }
    let copyN   x = { bound = x.bound; 
                      digits = Array.copy x.digits } // could copy just enough... 

    let normN n =
        // normalises bound 
        let rec findLeastBound (na:ints) i = if i = -1 || na.[i]<>0 then i+1 else findLeastBound na (i-1)
        let bound = findLeastBound n.digits (n.bound-1)
        n.bound <- bound;
        n

    let boundInt    = 2 // int  will fit with bound=2 
    let boundInt64  = 3 // int64  will fit with bound=3 
    let boundBase   = 1 // base will fit with bound=1 - obviously! 

//----------------------------------------------------------------------------
// base, coefficients, poly
//--------------------------------------------------------------------------

    let embed x =
        let x = if x<0 then 0 else x // no -ve naturals 
        if x < baseN then
            let r = createN 1
            r.digits.[0] <- x;
            normN r
        else 
            let r = createN boundInt
            for i = 0 to boundInt - 1 do
              r.digits.[i] <- (x / pow32 baseN i) % baseN
            done;
            normN r

    let embed64 x =
        let x = if x<0L then 0L else x // no -ve naturals 
        let r = createN boundInt64
        for i = 0 to boundInt64-1 do
            r.digits.[i] <- int32 ( (x / pow64 baseNi64 i) % baseNi64)
        done;
        normN r

    let eval n = 
      if n.bound = 1 
      then n.digits.[0] 
      else
          let mutable acc = 0
          for i = n.bound-1 downto 0 do
            acc <- n.digits.[i] + baseN * acc
          done;
          acc

    let eval64 n =
      if n.bound = 1 
      then int64 n.digits.[0] 
      else 
          let mutable acc = 0L
          for i = n.bound-1 downto 0 do
            acc <- int64 (n.digits.[i]) + baseNi64 * acc
          done;
          acc

    let one  = embed 1  
    let zero = embed 0

    let restrictTo d n =
       { bound = minInt d n.bound; digits = n.digits}

    let shiftUp d n =
      let m = createN (n.bound+d)
      for i = 0 to n.bound-1 do
        m.digits.[i+d] <- n.digits.[i]
      done;
      m

    let shiftDown d n =
      if n.bound-d<=0 then
        zero
      else
        let m = createN (n.bound-d)
        for i = 0 to m.bound-1 do
          m.digits.[i] <- n.digits.[i+d]
        done;
        m

    let degree n = n.bound-1


//----------------------------------------------------------------------------
// add, sub
//--------------------------------------------------------------------------

    // addition       
    let rec addP i n c p q r = // p+q + c 
        if i<n then
            let x = index p i + index q i + c
            r.digits.[i] <- modbase x;
            let c = divbase x
            // if p (or q) exhausted and c zero could switch to copying mode 
            addP (i+1) n c p q r

    let add p q =
        let rbound = 1 + maxInt p.bound q.bound
        let r = createN rbound
        let carry = 0
        addP 0 rbound carry p q r;
        normN r

    // subtraction 
    let rec subP i n c p q r = // p-q + c 
        if i<n then
            let x = index p i - index q i + c
            if x>0 then 
                r.digits.[i] <- modbase x;
                let c = divbase x
                // if p (or q) exhausted and c zero could switch to copying mode 
                subP (i+1) n c p q r
            else 
                let x = x + baseN      // add baseN 
                r.digits.[i] <- modbase x;
                let c = divbase x - 1  // sub baseN 
                // if p (or q) exhausted and c zero could switch to copying mode 
                subP (i+1) n c p q r
        else
            let underflow = c<>0
            underflow

    let sub p q =
        // NOTE: x-y=0 when x<=y, it is natural subtraction 
        let rbound = maxInt p.bound q.bound
        let r = createN rbound
        let carry = 0
        let underflow = subP 0 rbound carry p q r
        if underflow then
            embed 0
        else
            normN r


//----------------------------------------------------------------------------
// isZero, equal, ordering, sign, min, max
//--------------------------------------------------------------------------

    let isZero p = p.bound=0
    let IsZero p = isZero p
    let isOne  p = p.bound=1 && p.digits.[0] = 1

    let equal p q =
      (p.bound = q.bound) &&
      (let rec check (pa:ints) (qa:ints) i =
         // HAVE: pa.[j] = qa.[j] for i < j < p.bound 
            (i = -1) || (pa.[i]=qa.[i] && check pa qa (i-1))
      
       check p.digits q.digits (p.bound-1))
        
    let shiftCompare p pn q qn  =
        if   p.bound + pn < q.bound + qn then -1 
        elif p.bound + pn > q.bound + pn then  1 
        else
            let rec check (pa:ints) (qa:ints) i =
              // HAVE: pa.[j-pn] = qa.[j-qn] for i < j < p.bound 
              // Looking for most significant differing coeffs to determine ordering 
              if i = -1 then
                0
              else
                let pai = if i < pn then 0 else pa.[i-pn]
                let qai = if i < qn then 0 else qa.[i-qn]
                if   pai = qai then check pa qa (i-1)
                elif pai < qai then -1
                else                1
           
            check p.digits q.digits (p.bound + pn - 1)

    let compare p q =
        if p.bound < q.bound then -1 
        elif p.bound > q.bound then  1 
        else
            let rec check (pa:ints) (qa:ints) i =
                // HAVE: pa.[j] = qa.[j] for i < j < p.bound 
                // Looking for most significant differing coeffs to determine ordering 
                if i = -1 then 0
                elif pa.[i]=qa.[i] then check pa qa (i-1)
                elif pa.[i]<qa.[i] then -1
                else                    1
            check p.digits q.digits (p.bound-1)

    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]
    let lt    p q = compare p q =  -1
    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]
    let gt    p q = compare p q =   1
    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]
    let lte   p q = compare p q <>  1
    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]
    let gte   p q = compare p q <> -1

    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]
    let min a b = if lt a b then a else b
    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]
    let max a b = if lt a b then b else a


//----------------------------------------------------------------------------
// scale
//--------------------------------------------------------------------------

    // REQUIRE: baseN + baseN.2^32 < Int64.maxInt 
    let rec contributeArr (a:ints) i (c:int64) =
        // Given c and require c < baseN.2^32
        // Compute: r <- r + c . B^i
        // via r.digits.[i] <- r.digits.[i] + c and normalised
        let x = int64 a.[i] + c
        // HAVE: x < baseN + baseN.2^32 
        let c = div64base x
        let x = mod64base x
        // HAVE: c < 1 + 2^32 < baseN.2^32, recursive call ok 
        // HAVE: x < baseN 
        a.[i] <- x;  // store residue x 
        if c>0L then
            contributeArr a (i+1) c // contribute carry next position 

    let inline contribute r i c = contributeArr r.digits i c

    // REQUIRE: maxInt < 2^32
    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]    
    let rec scale (k:int) (p:n) =
      // Given k and p and require k < 2^32
      // Computes "scalar" product k.p
      //
      let rbound = p.bound + boundInt
      let r = createN rbound
      let k = int64 k
      for i = 0 to p.bound-1 do
        let kpi = k * int64 p.digits.[i]
        // HAVE: kpi < 2^32 * baseN which meets "contribute" requirement 
        contribute r i kpi
      done;
      normN r


//----------------------------------------------------------------------------
// mulSchoolBook
//--------------------------------------------------------------------------

    // multiplication: naively O(n^2) 
(*
    let mulSchoolBook' p q =
      let rbound = p.bound + q.bound + boundBase*2
      let r = createN rbound
      let pa = p.digits
      let qa = q.digits
      for i = 0 to p.bound-1 do
        for j = 0 to q.bound-1 do
          contribute r (i+j) (int64 pa.[i] * int64 qa.[j])
        done
      done;
      normN r
*)

    let mulSchoolBookBothSmall p q =
        let r = createN 2
        let rak = int64 p * int64 q
        setCoeff r 0 (mod64base rak);
        setCoeff r 1 (int32 (div64base rak))
        normN r

    let rec mulSchoolBookCarry r c k =
        if ( c > 0L ) then
            // ToAdd = c.B^k 
            let rak = (coeff64 r k) + c
            setCoeff r k (mod64base rak);
            mulSchoolBookCarry r (div64base rak) (k + 1)

    let mulSchoolBookOneSmall p q =
        let bp = bound(p)
        let rbound = bp + 1
        let r = createN rbound
        let q = int64 q
        let mutable c = 0L
        for i = 0 to bp-1 do
            let rak = c + (coeff64 r i) + (coeff64 p i) * q
            setCoeff r i (mod64base rak);
            c <- div64base rak;
        mulSchoolBookCarry r c bp
        normN r


    // multiplication: naively O(n^2) -- this version - unchecked - is faster 
    let mulSchoolBookNeitherSmall p q =
        let rbound = p.bound + q.bound 
        let r = createN rbound
        let ra = r.digits
        let pa = p.digits
        let qa = q.digits
        // ToAdd p*q 
        for i = 0 to p.bound-1 do
            // ToAdd p.[i] * q * B^i 
            let pai = int64 pa.[i]
            let mutable c = 0L
            let mutable k = i  // k = i + j 
            // ToAdd = pi.qj.B^(i+j) for j = 0,j+1... 
            for j = 0 to q.bound-1 do
                // ToAdd = c.B^k + pi.qj.B^(i+j) for j = j,j+1... and k = i+j      
                let qaj = int64 qa.[j]
                let rak = int64 ra.[k] + c + pai * qaj
                ra.[k] <- int32 (mod64base rak);
                c <- div64base rak;
                k <- k + 1;
            mulSchoolBookCarry r c k
        normN r

    let mulSchoolBook p q =
        let pSmall = (bound(p) = 1)
        let qSmall = (bound(q) = 1)
        if (pSmall && qSmall) then mulSchoolBookBothSmall (coeff p 0) (coeff q 0)
        elif pSmall           then mulSchoolBookOneSmall q (coeff p 0) 
        elif qSmall           then mulSchoolBookOneSmall p (coeff q 0) 
        else mulSchoolBookNeitherSmall p q

       
//----------------------------------------------------------------------------
// quickMulUsingFft
//--------------------------------------------------------------------------

    // The FFT polynomial multiplier requires the result coeffs fit inside Fp.
    //
    // OVERVIEW:
    //   The numbers are recoded as polynomials to be evaluated at (x=2^bigL).
    //   The polynomials are FFT multiplied, requiring result coeff to fit Fp.
    //   The result product is recovered by evaluating the poly at (x=2^bigL).
    //
    // REF:
    //   QuickMul: Practical FFT-base Integer Multiplication,
    //   Chee Yap and Chen Yi.
    //
    // There is choice of how to encode the nats polynomials.
    // The choice is the (2^bigL) base to use.
    // For bigL=1, the FFT will cater for a product of upto 256M bits.
    // Larger bigL have less reach, but compute faster.
    // So plan to choose bigL depending on the number of bits product.
    //  
    // DETERMINING THE K,L BOUNDS.
    //
    // Given representing using K-vectors, K a power of 2, K=2^k, and
    // If choosing inputs to have L-bit coefficients.
    //
    // The result coeff are:
    //
    //   res(i) = sum (j<i)   x(i) * y(i)
    // 
    // So "bits(res(i)) < bits(K) + 2L".
    // Require "bits(res(i)) <= maxBitsInsideFp",
    // So choose "bits(K) + 2L <= maxBitsInsideFp".
    //
    // An under-estimate of the bits the result is K*L.
    // K*L is an under-estimate of the maximum result which can be computed ok.
    //
    // The bigL options (restricted to bigL divides baseBits (=24)) are:
    //

    [<NoEquality; NoComparison>]
    type encoding =
        { bigL      : int;       // bits per input coeff 
          twoToBigL : int;       // 2^bigL 
          k         : int; 
          bigK      : int;       // bigK = 2^k, number of terms polynomials 
          bigN      : int;       // bits result (under-estimate of limit) 
          split     : int;       // baseBits / bigL 
          splits    : int array;
        }

#if CHECKED
    let _ = check (baseBits=24) "24bit"
#endif
      // Requiring baseN mod 2^bigL = 0 gave quick encoding, but...
      // also a terrible drop performance when the bigK jumped by more than needed!
      // Below, it choose a minimal bigK to hold the product.

    let mkEncoding (bigL,k,bigK,bigN) =
#if CHECKED
      check (bigK = pow32 2 k) "bigK";
      check (bigN = bigK * bigL) "bigN";
      check (2 * bigL + k <= maxBitsInsideFp) "constraint";
#endif
      { bigL      = bigL;
        twoToBigL = pow32 2 bigL;
        k         = k;
        bigK      = bigK;
        bigN      = bigN;
        split     = baseBits/bigL;  // should divide exactly 
        splits    = Array.init (baseBits/bigL) (fun i -> pow32 2 (bigL*i))
      }

    let table =
      [|           // bigL , k  , bigK      , bigN       //
        mkEncoding ( 1     , 28 , 268435456 , 268435456    ) ;
        mkEncoding ( 2     , 26 , 67108864  , 134217728    ) ;
        mkEncoding ( 3     , 24 , 16777216  , 50331648     ) ;
        mkEncoding ( 4     , 22 , 4194304   , 16777216     ) ;
        mkEncoding ( 5     , 20 , 1048576   , 5242880      ) ;
        mkEncoding ( 6     , 18 , 262144    , 1572864      ) ;
        mkEncoding ( 7     , 16 , 65536     , 458752       ) ;
        mkEncoding ( 8     , 14 , 16384     , 131072       ) ;
        mkEncoding ( 9     , 12 , 4096      , 36864        ) ;
        mkEncoding ( 10    , 10 , 1024      , 10240        ) ;
        mkEncoding ( 11    , 8  , 256       , 2816         ) ;
        mkEncoding ( 12    , 6  , 64        , 768          ) ;
        mkEncoding ( 13    , 4  , 16        , 208          ) ;
      |]

    let calculateTableTow bigL = 
      // Given L.
      // Have L via "log2 K <= maxBitsInsideFp - 2L".
      // Have N via "N = K.L"
      // 
      let k    = maxBitsInsideFp - 2*bigL
      let bigK = pow64 2L k
      let N    = bigK * int64 bigL
      bigL,k,bigK,N

    let encodingGivenResultBits bitsRes =
      // choose maximum bigL s.t. bitsRes < bigN 
      // EXCEPTION: fails is bitsRes exceeds 2^28 (largest bigN table) 
      let rec selectFrom i =
        if i+1 < table.Length && bitsRes < table.[i+1].bigN then
          selectFrom (i+1)
        else
          table.[i]
     
      if bitsRes >= table.[0].bigN then
        failwith "Product is huge, around 268435456 bits, beyond quickmul"
      else
        selectFrom 0

    let bitmask      = Array.init baseBits (fun i -> (pow32 2  i - 1))
    let twopowers    = Array.init baseBits (fun i -> (pow32 2  i))
    let twopowersI64 = Array.init baseBits (fun i -> (pow64 2L i))
    // bitmask(k)   = 2^k - 1 
    // twopowers(k) = 2^k    //    

    let wordBits word =
      let rec hi k =
        if k=0 then 0 
        elif (word &&& twopowers.[k-1]) <> 0 then k 
        else hi (k-1)
     
      hi baseBits
      
    let bits u =
      if u.bound=0 then 0 
      else degree u * baseBits + wordBits u.digits.[degree u]
      
    let extractBits n enc bi =
      let bj  = bi + enc.bigL - 1  // the last bit (inclusive) 
      let biw = bi / baseBits     // first bit is this index pos 
      let bjw = bj / baseBits     // last  bit is this index pos 
      if biw <> bjw then
        // two words 
        let x = index n biw          
        let y = index n bjw           // bjw = biw+1 
        let xbit   = bi % baseBits                // start bit x 
        let nxbits = baseBits - xbit              // number of bitsin x 
        let x = x >>> xbit          // shift down x so bit0 is first 
        let y = y <<< nxbits        // shift up   y so it starts where x finished 
        let x = x ||| y                   // combine them 
        let x = x &&& bitmask.[enc.bigL] // mask out (high y bits) to get required bits 
        x
      else
        // one word 
        let x = index n biw
        let xbit = bi % baseBits                  // start bit x 
        let x = x >>> xbit         
        let x = x &&& bitmask.[enc.bigL]
        x

    let encodePoly enc n =
      // Find poly s.t. n = poly evaluated at x=2^bigL
      // with 0 <= pi < 2^bigL.
      //
      let poly = Array.create enc.bigK (Fp.ofInt32 0)
      let biMax = n.bound * baseBits
      let rec encoder i bi =
        // bi = i * bigL 
        if i=enc.bigK || bi > biMax then
          () // done 
        else
          ( let pi = extractBits n enc bi
            poly.[i] <- Fp.ofInt32 pi;
            let i     = i + 1
            let bi    = bi + enc.bigL
            encoder i bi
          )
     
      encoder 0 0;
      poly

    let decodeResultBits enc (poly : fp array) =
      // Decoding evaluates poly(x) (coeff Fp) at X = 2^bigL.
      // A bound on number of result bits is "enc.bigN + boundInt", but that takes HUGE STEPS.
      // Garbage has a cost, so we minimize it by working out a tight bound.
      //
      // poly(X) = sum i=0..n coeff_i * X^i    where n is highest non-zero coeff.
      //        <= 2^maxBitsInsideFp * (1 + X + ... X^n)
      //        <= 2^maxBitsInsideFp * (X^(n+1) - 1) / (X - 1)
      //        <= 2^maxBitsInsideFp * X^(n+1)       / (X - 1)
      //        <= 2^maxBitsInsideFp * X^(n+1)       / (X/2)     provided X/2 <= X-1
      //        <= 2^maxBitsInsideFp * X^n * 2
      //        <= 2^maxBitsInsideFp * (2^bigL)^n * 2
      //        <= 2^(maxBitsInsideFp + bigL.n + 1)
      //
      let mutable n = 0
      for i = 0 to poly.Length-1 do 
        if poly.[i] <> mzero then n <- i 
      done;
      let rbits = maxBitsInsideFp + enc.bigL * n + 1
      rbits + 1 // +1 since 2^1 requires 2 bits not 1 

    // REQUIRE: bigL <= baseBits 
    let decodePoly enc poly =
      // Find n = poly evaluated at x=2^bigL
      // Note, 0 <= pi < maxFp.
      //
      let rbound = (decodeResultBits enc poly) / baseBits + 1
      let r = createN rbound
      let rec evaluate i j d =
        // HAVE: bigL.i = j * baseBits + d   and  d<baseBits 
        if i=enc.bigK then
          () // done 
        else
          // Consider ith term:
          //     poly.[i] . 2 ^ (bigL.i)
          //   = poly.[i] . 2 ^ d . 2 ^ (j.baseBits)
          //   = x                . 2 ^ (j.baseBits)
          // So contribute "x = poly.[i] . 2 ^ d" to r.[j].
          //
          ( if j >= rbound then 
#if CHECKED
              check (poly.[i] = mzero) "decodePoly";
#endif
              ()
            else (
              let x = int64 (Fp.toInt poly.[i]) * twopowersI64.[d]
              // HAVE: x < 2^32 . 2^baseBits = 2^32.baseN 
              contribute r j x
            );
            let i   = i + 1
            let d   = d + enc.bigL
            let j,d = if d >= baseBits then j+1 , d-baseBits else j,d
            // HAVE: d < baseBits, note: bigL<baseBits 
            evaluate i j d
          )
     
      evaluate 0 0 0;
      normN r

    let quickMulUsingFft u v =
      // Given u,v : n, compute product, uv, via FFT polynomial multiplication.
      //------
      // First, estimate an upper bound on the number of bits uv.
      // This determines (bigL,k,bigK) defining the encoding into polynomials.
      //
      let bitsRes  = bits u + bits v // upper estimate on result bits 
      let enc   = encodingGivenResultBits bitsRes
      // Represent u and v as polynomials with L-bit coeff evaluated at 2^bigL 
      let upoly = encodePoly enc u
      let vpoly = encodePoly enc v
      // Compute polynomial product via FFT 
      let rpoly = computeFftPaddedPolynomialProduct enc.bigK enc.k upoly vpoly
      // Obtain uv by evaluating product polynomial at 2^bigL 
      let r = decodePoly enc rpoly
      normN r


//----------------------------------------------------------------------------
// mulKaratsuba
//--------------------------------------------------------------------------

    let minDigitsKaratsuba = 16 // useful for tuning recMulKaratsuba 

    let recMulKaratsuba mul p q =
       let bp = p.bound
       let bq = q.bound
       let bmax = maxInt bp bq
       if bmax > minDigitsKaratsuba then
         let k  = bmax / 2
         let a0 = restrictTo k p
         let a1 = shiftDown  k p
         let b0 = restrictTo k q
         let b1 = shiftDown  k q
         let q0 = mul a0 b0
         let q1 = mul (add a0 a1) (add b0 b1)
         let q2 = mul a1 b1
         let p0 = q0
         let p1 = sub q1 (add q0 q2)
         let p2 = q2
         let r  = add p0 (shiftUp k (add p1 (shiftUp k p2)))
         r
       else
         mulSchoolBook p q

    let rec mulKaratsuba x y = recMulKaratsuba mulKaratsuba x y


//----------------------------------------------------------------------------
// mul - composite
//--------------------------------------------------------------------------

    let productDigitsUpperSchoolBook = (64000 / baseBits)
       // When is it worth switching away from SchoolBook?
       // SchoolBook overhead is low, so although it's O(n^2) it remains competitive.
       //
       // 28/3/2006:
       // The FFT can take over from SchoolBook at around 64000 bits.
       // Note, FFT performance is stepwise, according to enc from table.
       // The steps are big steps (meaning sudden jumps/drops perf).
       //

    let singleDigitForceSchoolBook = (32000 / baseBits)
       // If either argument is "small" then stay with SchoolBook.
       //

    let productDigitsUpperFft  = (table.[0].bigN / baseBits)
       // QuickMul is good upto a finite (but huge) limit:
       // Limit 268,435,456 bits product.
       // 
       // From the code:
       //   let bitsRes = bits u + bits v
       //   fails when bitsRes >= table.[0].bigN
       // So, not applicable when:
       //   P1: table.[0].bigN <= bits(u) + bits(v)
       //   P2: table.[0].bigN <= .. <= baseBits * (u.bound + v.bound)
       //   P3: table.[0].bigN <= .. <= baseBits * (u.bound + v.bound)
       //   P4: table.[0].bigN / baseBits <= u.bound + v.bound
       //

    // Summary of mul algorithm choice:
    // 0                 <= uv_bound < upper_school_book - Schoolbook
    // upper_school_book <= uv_bound < upper_fft         - QuickMul
    // upper_fft         <= uv_bound < ...               - Karatsuba
    //
    // NOTE:
    // - Karatsuba current implementation has high garbage cost.
    // - However, a linear space cost is possible...
    // - Meantime, switch to Karatsuba only beyond FFT range.
    //

    let rec mul p q =
        let pqBound = p.bound + q.bound
        if pqBound < productDigitsUpperSchoolBook ||
           p.bound  < singleDigitForceSchoolBook   ||
           q.bound  < singleDigitForceSchoolBook
        then
            // Within school-book initial range: 
            mulSchoolBook p q
        else
          if pqBound < productDigitsUpperFft then
            // Inside QuickMul FFT range: 
            quickMulUsingFft p q
          else
            // Beyond QuickMul FFT range, or maybe between Schoolbook and QuickMul (no!):
            // Use karatsuba method, with "mul" as recursive multiplier,
            //   so will reduce sizes of products on recursive calls,
            //   and QuickMul will take over if they fall within it's range.
            //
            recMulKaratsuba mul p q


//----------------------------------------------------------------------------
// division - scaleSubInPlace
//--------------------------------------------------------------------------

    let scaleSubInPlace x f a n =
      // Have x = sumR 0 xd (\i.xi.B^i)   where xd = degree x
      //      a = sumR 0 ad (\i.digitsi.B^i)   where ad = degree a
      //      f < B
      //      n < xd
      // Assumes "f.digits.B^n < x".
      // Required to remove f.digits.B^n from x place.
      //------
      // Result = x_initial - f.digits.B^n
      //        = x_initial - f.[sumR 0 ad (\i.digitsi.B^(i+n))]
      // State: j = 0
      //        z = f * a0
      // Invariant(x,z,j,n):
      //    P1: x_result = x - z.B^(j+n) - f.[sumR (j+1) ad (\i.digitsi.B^i+n)]
      //    P2: z < B^2 - 1, and so has form z = zHi.B + zLo for zHi,zLo < B.
      // Base: Invariant holds initially.
      // Step: (a) Remove zLo from x:
      //           If zLo <= x_(j+n)    then zLo     <- 0
      //                                     x_(j+n) <- x_(j+n) - zLo
      //                                else zLo     <- 0
      //                                     x_(j+n) <- x_(j+n) + (B - zLo)
      //                                              = x_(j+n) - zLo + B
      //                                     zHi     <- zHi + 1
      //           Here, invariant P1 still holds, P2 may break.
      //       (b) Advance j:
      //           Have z = zHi.B since zLo = 0.
      //           j <- j + 1
      //           z <- zHi + f * a_(j+1)
      //           P2 holds:
      //             Have z <= B + (B-1)*(B-1) = B + B^2 - 2B + 1 = B^2 - B + 1
      //             Have z <= B^2 - 1 when B >= 2 which is required for B being a base.
      //           P1 holds,
      //             moved f.digits_(j+1).B^(j+1+n) factor over.
      // 
      // Once j+1 exceeds ad, summation is zero and it contributes no more terms (b).
      // Continue until z = 0, which happens since z decreases towards 0.
      // Done.
      //
      let invariant (_,_,_) = ()     
    #if CHECKED
      let x_initial = copyN x
      let x_result  = sub x_initial (shiftUp n (scale f a))
      let invariant (z,j,n) =
        let P1 =
          equal
            x_result
            (sub x (add (shiftUp (j+n) (embed64 z))
                      (mul (embed f)
                         (shiftUp (j+1+n) (shiftDown (j+1) a)))))
        let P2 = z < baseNi64 * baseNi64 - 1L
        check P1 "P1";
        check P2 "P2"
     
    #endif
      let xres = x
      let x,xd = x.digits,degree x
      let a,ad = a.digits,degree a
      let f = int64 f
      let mutable j = 0
      let mutable z = f * int64 a.[0]
      while( z > 0L || j < ad ) do
        if j > xd then failwith "scaleSubInPlace: pre-condition did not apply, result would be -ve";
        invariant(z,j,n); // P1,P2 hold 
        let mutable zLo = mod64base z |> int32
        let mutable zHi = div64base z
        if zLo <= x.[j+n] then
          x.[j+n] <- x.[j+n] - zLo
        else (
          x.[j+n] <- x.[j+n] + (baseN - zLo);
          zHi <- zHi + 1L
        );
        // P1 holds 
        if j < ad then
          z <- zHi + f * int64 a.[j+1]
        else
          z <- zHi;
        j <- j + 1;
        // P1,P2 hold 
      done;
      ignore (normN xres)

    //    
    let scaleSub x f a n =
      let freshx = add x zero
      scaleSubInPlace freshx f a n;
      normN freshx
(*

    let scaleSub2 x f a n = sub x (shiftUp n (mul (embed f) a))
      
    let x = (mul (embed 234234234) (pow (embed 10) (embed 20)))
    let f = 2
    let a = (embed 1231231231)
    let n = 2
    let res  = scaleSub  x f a n 
    let res2 = scaleSub2 x f a n
     
    let x, xd, f, a, ad, n = freshx.digits, freshx.bound, f, a.digits, a.bound, n
   *)


//----------------------------------------------------------------------------
// division - scaleAddInPlace
//--------------------------------------------------------------------------

    let scaleAddInPlace x f a n =
      // Have x = sumR 0 xd (\i.xi.B^i)
      //      a = sumR 0 ad (\i.digitsi.B^i)
      //      f < B
      //      n < xd
      // Required to add f.digits.B^n to x place.
      // Assumes result will fit with x (0...xd).
      //------
      // Result = x_initial + f.digits.B^n
      //        = x_initial + f.[sumR 0 ad (\i.digitsi.B^i+n)]
      // State: j = 0
      //        z = f * a0
      // Invariant(x,z,j,n):
      //    P1: x_result = x + z.B^(j+n) + f.[sumR (j+1) ad (\i.digitsi.B^i+n)]
      //    P2: z < B^2 - 1, and so has form z = zHi.B + zLo for zHi,zLo < B.
      // Base: Invariant holds initially.
      // Step: (a) Add zLo to x:
      //           If zLo < B - x_(j+n) then zLo     <- 0
      //                                     x_(j+n) <- x_(j+n) + zLo
      //                                else zLo     <- 0
      //                                     x_(j+n) <- zLo - (B - x_(j+n))
      //                                              = x_(j+n) + zLo - B
      //                                     zHi     <- zHi + 1
      //           Here, invariant P1 still holds, P2 may break.
      //       (b) Advance j:
      //           Have z = zHi.B since zLo = 0.
      //           j <- j + 1
      //           z <- zHi + f * a_(j+1)
      //           P2 holds:
      //             Have z <= B + (B-1)*(B-1) = B + B^2 - 2B + 1 = B^2 - B + 1
      //             Have z <= B^2 - 1 when B >= 2 which is required for B being a base.
      //           P1 holds,
      //             moved f.digits_(j+1).B^(j+1+n) factor over.
      // 
      // Once j+1 exceeds ad, summation is zero and it contributes no more terms (b).
      // Continue until z = 0, which happens since z decreases towards 0.
      // Done.
      //
      let invariant (_,_,_) = ()     
#if CHECKED
      let x_initial = copyN x
      let x_result  = add x_initial (shiftUp n (scale f a))
      let invariant (z,j,n) =
        let P1 =
          equal
            x_result
            (add x (add (shiftUp (j+n) (embed64 z))
                      (mul (embed f)
                         (shiftUp (j+1+n) (shiftDown (j+1) a)))))
        let P2 = z < baseNi64 * baseNi64 - 1L
        check P1 "P1";
        check P2 "P2"
     
#endif
      let xres = x
      let x,xd = x.digits,degree x
      let a,ad = a.digits,degree a      
      let f = int64 f
      let mutable j = 0
      let mutable z = f * int64 a.[0]
      while( z > 0L || j < ad ) do
        if j > xd then failwith "scaleSubInPlace: pre-condition did not apply, result would be -ve";
        invariant(z,j,n); // P1,P2 hold 
        let mutable zLo = mod64base z |> int32
        let mutable zHi = div64base z
        if zLo < baseN - x.[j+n] then
          x.[j+n] <- x.[j+n] + zLo
        else (
          x.[j+n] <- zLo - (baseN - x.[j+n]);
          zHi <- zHi + 1L
        );
        // P1 holds 
        if j < ad then
          z <- zHi + f * int64 a.[j+1]
        else
          z <- zHi;
        j <- j + 1;    
        // P1,P2 hold 
      done;
      ignore (normN xres)

    //    
    let scaleAdd x f a n =
      let freshx = add x zero
      scaleAddInPlace freshx f a n;
      normN freshx

(*
    let scaleAdd2 x f a n = add x (shiftUp n (mul (embed f) a))
      
    let x = (mul (embed 234234234) (pow (embed 10) (embed 20)))
    let f = 2
    let a = (embed 1231231231)
    let n = 2
    let res  = scaleAdd  x f a n 
    let res2 = scaleAdd2 x f a n
     
    let x, xd, f, a, ad, n = freshx.digits, freshx.bound, f, a.digits, a.bound, n
*)
        
//----------------------------------------------------------------------------
// division - removeFactor
//--------------------------------------------------------------------------

    (*
    let removeFactorReference x a n =
      let ff = div x (shiftUp n a)
      toInt ff
   *)     

    let removeFactor x a n =
      // Assumes x < a.B^(n+1)
      // Choose f s.t.
      //  (a) f.digits.B^n <= x
      //  (b) f=0  iff  x < a.B^n
      //
      let dega,degx = degree a,degree x
      if degx < dega + n then 0 else // possible with "normalisation" 
      let aa,xa = a.digits,x.digits
      let f = 
        if dega = 0 then // a = a0 
          if degx = n then
            xa.[n] / aa.[0]
          else (
#if CHECKED
            check (degx = n+1) "removeFactor degx#1";
#endif
            let f64 = (int64 xa.[degx] * baseNi64 + int64 xa.[degx-1]) / int64 aa.[0]
            int32 f64
          )
        else // a = sumR 0 dega (\i.digitsi.B^i) 
          if degx = dega + n then
            xa.[degx] / (aa.[dega] + 1)              // +1 to bound above a 
          else (
#if CHECKED
            check (degx = dega+n+1) "removeFactor degx#2";
#endif
            let f64 = (int64 xa.[degx] * baseNi64 + int64 xa.[degx-1])
                    / (int64 aa.[dega] + 1L)        // +1 to bound above a 
            int32 f64
          )
     
      if f = 0 then
        let lte = (shiftCompare a n x 0) <> 1
        if lte then 1 else 0
      else
        f


//----------------------------------------------------------------------------
// division - divmod
//--------------------------------------------------------------------------

    let divmod b a =
      // Returns d,r where b = d.digits + r and r<a
      //------
      // state:
      //   x = b
      //   d = 0
      //   p = degree(b)
      //   m = degree(a)   -- constant
      //   n = p - m
      //------
      // Invariant(d,x,n,p)
      //   P1: b = d.digits + x
      //   P2: x < a.B^(n+1)
      //   P3: p = m+1 or p = m+n+1
      // Base: Invariant holds initially.
      // Step:
      //   (a) Choose f the remove factor s.t. f.digits.B^n < x and f=0 iff x < a.B^n
      //       x <- x - f.digits.B^n
      //       d <- d + f.B^n
      //       Invariant(d,x,n,p) holds.
      //   (b) If f=0, Have x < a.B^n.
      //       If p = m+n, Invariant(d,x,n-1,p) holds.
      //       If p = m+n+1,
      //         Have x_p = 0,
      //           otherwise,
      //             a.B^n < x_p.B^p <= x
      //           contra x < a.B^n.
      //         Invariant(d,x,n-1,p-1) holds.
      //       n <- n - 1
      // Have x and n decreasing |N.
      // Terminate when f=0 and n=0.
      // From invariant, b = d.digits + x.
      //------
      // Working space:
      //   x, x must be size of b.
      //   d, d.digits <= b so lg(d) <= lg(b) - lg(a)
      //      words(d) = words(b) - words(a) + 1
      //------
      //
      if isZero a then raise (new System.DivideByZeroException()) else    
      if degree b < degree a then
          // "b = d.digits + r" as "b = 0.digits + b" when b<a 
          zero,b
      else
          let x = copyN b
          let d = createN ((degree b - degree a + 1) + 1)
          let mutable p = degree b
          let         m = degree a
          let mutable n = p - m   
#if CHECKED
          let Invariant(d,x,n,p) =
            let P1 = equal b (add (mul d a) x)
            let P2 = lt x (shiftUp (n+1) a)
            let P3 = (p = m+n) || (p = m+n+1)
            check P1 "P1";
            check P2 "P2";
            check P3 "P3"
#else
          let Invariant(_,_,_,_) = ()           
#endif      
          let mutable finished = false
          while( not finished ) do
              //printf "-- p=%d n=%d m=%d\n" p n m;
              Invariant(d,x,n,p);
              let f = removeFactor x a n
              //printf " - x=%s a=%s n=%d f=%d\n" (toString x) (toString a) n f;
              //printf " - n=%d f=%d\n" n f;    
              if f>0 then
                  scaleSubInPlace x f a   n;
                  scaleAddInPlace d f one n;
                  Invariant(d,x,n,p)
              else 
                  finished <- f=0 && n=0;
                  if not finished then
                      if p = m+n then 
                          Invariant(d,x,n-1,p);
                          n <- n-1
                      else 
                          Invariant(d,x,n-1,p-1);
                          n <- n-1;
                          p <- p-1
          // Have: "b = d.digits + x" return d,x 
          normN d,normN x
        
    //----------------------------------------------------------------------------
    // div, mod
    //--------------------------------------------------------------------------

    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]    
    let div b a = fst (divmod b a)  
    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]                                               
    let rem b a = snd (divmod b a)
    // rem b a, for small a can do (base mod a) trick - O(N) 


    //----------------------------------------------------------------------------
    // hcf
    //--------------------------------------------------------------------------

    let hcf a b =
        // Have: 0 <= a,b since naturals 
        let rec hcfloop a b = // Require: 0 <= a <= b 
            if equal zero a then b 
            else
                // Have: 0 < a <= b 
                let _,r = divmod b a
                // Have: r < a  from divmod 
                hcfloop r a // Have: 0 <= r < a  
       
        if lt a b then hcfloop a b // Have: 0 <= a <  b 
                  else hcfloop b a // Have: 0 <= b <= a 

    //----------------------------------------------------------------------------
    // pow
    //--------------------------------------------------------------------------

    let two = embed 2
    let powi x n =
        let rec power acc x n =
            if n=0 then acc
            elif n % 2=0 then power acc         (mul x x) (n / 2)
            else              power (mul x acc) (mul x x) (n / 2)
       
        power one x n

    let pow x n =
        let rec power acc x n =
            if isZero n then acc
            else
              let ndiv2,nmod2 = divmod n two  // use: intdivmod when available 
              if isZero nmod2 then power acc         (mul x x) ndiv2
              else                 power (mul x acc) (mul x x) ndiv2
       
        power one x n

//----------------------------------------------------------------------------
// float n
//--------------------------------------------------------------------------

    let toFloat n =
        let basef = float baseN
        let rec evalFloat acc k i =
            if i = n.bound then
                acc
            else
                evalFloat (acc + k * float n.digits.[i]) (k * basef) (i+1)
        evalFloat 0.0 1.0 0

//----------------------------------------------------------------------------
// n <-> int
//--------------------------------------------------------------------------

    let ofInt32 n = embed n
    let ofInt64 n = embed64 n

    /// Convert BigNat to uint32 otherwise OverflowException.
    let toUInt32 n =
      match n.bound with
        | 0 -> 0u
        | 1 -> n.digits.[0] |> uint32
        | 2 -> let xA,xB = n.digits.[0],n.digits.[1]
               if xB > baseMask32B then raise (System.OverflowException())
               ( uint32 (xA &&& baseMask32A)) +
               ((uint32 (xB &&& baseMask32B)) <<< baseShift32B)        
        | _ -> raise (System.OverflowException())

    /// Convert BigNat to uint64 otherwise OverflowException.
    let toUInt64 n =
      match n.bound with
        | 0 -> 0UL
        | 1 -> n.digits.[0] |> uint64
        | 2 -> let xA,xB = n.digits.[0],n.digits.[1]
               ( uint64 (xA &&& baseMask64A)) +
               ((uint64 (xB &&& baseMask64B)) <<< baseShift64B)
        | 3 -> let xA,xB,xC = n.digits.[0],n.digits.[1],n.digits.[2]
               if xC > baseMask64C then raise (System.OverflowException())
               ( uint64 (xA &&& baseMask64A)) +
               ((uint64 (xB &&& baseMask64B)) <<< baseShift64B) +
               ((uint64 (xC &&& baseMask64C)) <<< baseShift64C)
        | _ -> raise (System.OverflowException())
            

//----------------------------------------------------------------------------
// n  -> string
//--------------------------------------------------------------------------

     
#if CHECKED
    let checks = false
#endif
    let toString n =
      // Much better complexity than naive_string_of_z.
      // It still does "nDigit" calls to (int)divmod,
      // but the degree on which it is called halves (not decrements) each time.
      //
      let degn = degree n
      let rec route prior k ten2k =
        if degree ten2k > degn
        then (k,ten2k) :: prior
        else route ((k,ten2k) :: prior) (k+1) (mul ten2k ten2k)
      let kten2ks = route [] 0 (embed 10)
      let rec collect isLeading digits n = function
        | [] ->
            // Have 0 <= n < 10^1, so collect a single digit (if needed) 
            let n = eval n
#if CHECKED
            if checks then check (0 <= n) "toString: digit0";
            if checks then check (n <= 9) "toString: digit9";
#endif
            if isLeading && n=0 then digits // suppress leading 0  
            else string n :: digits
        | (_,ten2k) :: prior ->
#if CHECKED
            if checks then check (lt n (mul ten2k ten2k)) "string_of_int: bound n";
#endif
            // Have 0 <= n     < (ten2k)^2 and ten2k = 10^(2^k) 
            let nH,nL  = divmod n ten2k
#if CHECKED
            if checks then check (lt nH ten2k) "string_of_int: bound nH";
            if checks then check (lt nL ten2k) "string_of_int: bound nL";
#endif
            // Have 0 <= nH,nL < (ten2k)   and ten2k = 10^(2^k) 
            if isLeading && isZero nH then
              // suppress leading 0s 
              let digits = collect isLeading digits nL prior
              digits
            else
              let digits = collect false     digits nL prior
              let digits = collect isLeading digits nH prior
              digits
     
      let prior  = kten2ks
      let digits = collect true [] n prior
      match digits with 
      | [] -> "0" 
      | _ -> digits |> Array.ofList |> System.String.Concat 
        
//----------------------------------------------------------------------------
// n <-  string
//--------------------------------------------------------------------------

    let ofString (str:string) =
        // Would it be better to split string half and combine results? 
        let len = str.Length
        if System.String.IsNullOrEmpty str then invalidArg "str" "empty string";
        let ten = embed 10
        let rec build acc i =
            if i=len then
              acc
            else
                let c = str.[i]
                let d = int c - int '0' 
                if 0 <= d && d <= 9 then
                    build (add (mul ten acc) (embed d)) (i+1)
                else
                    raise (new System.FormatException(SR.GetString(SR.badFormatString)))
       
        build (embed 0) 0

    let isSmall n = (n.bound <= 1)
    let getSmall n = index n 0

    //----------------------------------------------------------------------------
    // factorial
    //--------------------------------------------------------------------------

    let factorial n =
        //*****
        // Factorial(n) = 1.2.3.....(n-1).n
        //
        // Factorial is sometimes used as a test for multiplication.
        // The QuickMul FFT multiplier takes over only when both operands reach a given size.
        // How to compute factorial?
        //
        // (a) Factorial(n) = factorial(n-1).n
        //     This is unlikely to make use of the FFT (n never large enough).
        // (b) Factorial(n) = (1.2.3.4....k) . (k.[k+1]...(n-1).n)
        //     Applied recursively QuickMul FFT will take over on large products.
        //
        //****
        let rec productR a b =
            if equal a b then a 
            else
                let m = div (add a b) (ofInt32 2)
                mul (productR a m) (productR (add m one) b)
       
        productR one n


#endif
