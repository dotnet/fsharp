#nowarn "52" // defensive copy of structs warning

namespace Microsoft.FSharp.Math

    open Microsoft.FSharp.Math
    open System
    open System.Globalization

    [<Struct>]
    [<CustomEquality; CustomComparison>]
    type Complex(real: float, imaginary: float) =
        //new() = new Complex(0.0,0.0)
        member x.r = real
        member x.i = imaginary
        override x.ToString() = x.ToString("g")
        member x.ToString(fmt) = x.ToString(fmt,CultureInfo.InvariantCulture)
        member x.ToString(fmt,fmtprovider:IFormatProvider) = 
               x.r.ToString(fmt,fmtprovider)+"r"+(if x.i < 0.0 then "-" else "+")+(System.Math.Abs x.i).ToString(fmt,fmtprovider)+"i"
        interface IComparable with 
            member x.CompareTo(obj) = 
                match obj with 
                | :? Complex as y -> 
                     let c = compare x.r y.r
                     if c <> 0 then c else compare x.i y.i
                | _ -> invalidArg "obj" "not a Complex number"
        override x.Equals(obj) = 
                match obj with 
                | :? Complex as y -> x.r = y.r && x.i = y.i
                | _ -> false
        override x.GetHashCode() = 
                (hash x.r >>> 5) ^^^  (hash x.r <<< 3) ^^^  (((hash x.i >>> 4) ^^^  (hash x.i <<< 4)) + 0x9e3779b9)

                
    type complex = Complex

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Complex = 
      let mkRect(a,b) = new Complex(a,b)
      let conjugate (c:complex) = mkRect (c.r, -c.i)
      let mkPolar(a,b) = mkRect (a * Math.Cos(b), a * Math.Sin(b))
      let cis b = mkPolar(1.0,b)
      let zero = mkRect(0.,0.)
      let one = mkRect(1.,0.) 
      let onei = mkRect(0.,1.) 
      let magnitude (c:complex) = sqrt(c.r*c.r + c.i*c.i)
      let phase (c:complex) = Math.Atan2(c.i,c.r)
      let realPart (c:complex) = c.r
      let imagPart (c:complex) = c.i  
      let abs (a:complex) = sqrt (a.r**2.0 + a.i**2.0)
      let add (a:complex) (b:complex) = mkRect(a.r + b.r, a.i+b.i)
      let sub (a:complex) (b:complex) = mkRect(a.r - b.r, a.i-b.i)
      let mul (a:complex) (b:complex) = mkRect(a.r * b.r - a.i * b.i, a.i*b.r + b.i*a.r)
      let div (x:complex) (y:complex) = 
          let a = x.r in let b = x.i in 
          let c = y.r in let d = y.i in 
          //(a+ib)/(c+id)=(ac+bd+i(bc-ad))/(c2+d2) 
          let q = c*c + d*d in 
          mkRect((a*c+b*d)/q, (b*c - a*d)/q)
      let neg (a:complex) = mkRect(-a.r,-a.i)
      let smul (a:float)(b:complex) = mkRect(a * b.r, a*b.i)
      let muls (a:complex) (b:float) = mkRect(a.r *b, a.i*b)
      let fmt_of_string numstyle fmtprovider (s:string) =
        mkRect (System.Double.Parse(s,numstyle,fmtprovider),0.0) 
      let of_string s = fmt_of_string NumberStyles.Any CultureInfo.InvariantCulture s

      // ik.(r + i.th) = -k.th + i.k.r 
      let iscale k (x:complex) = mkRect (-k * x.i , k * x.r)

      // LogN : 'a * 'a -> 'a
      // Asin : 'a -> 'a
      // Acos : 'a -> 'a
      // Atan : 'a -> 'a
      // Atan2 : 'a * 'a -> 'a
      // Sinh : 'a -> 'a
      // Cosh : 'a -> 'a
      // Tanh : 'a -> 'a

      let pi    = mkRect (Math.PI,0.0)

      // exp(r+it) = exp(r).(cos(t)+i.sin(t)) - De Moivre Theorem 
      let exp (x:complex) = smul (exp(x.r)) (mkRect(cos(x.i), sin(x.i)))
      // x = mag.e^(i.th) = e^ln(mag).e^(i.th) = e^(ln(mag) + i.th) 
      let log x = mkRect (log(magnitude(x)),phase(x))

      let sqrt x = mkPolar (sqrt(magnitude x),phase x / 2.0)

      // cos(x) = (exp(i.x) + exp(-i.x))/2 
      let cos x = smul 0.5 (add (exp(iscale 1.0 x)) (exp(iscale -1.0 x)))
      // sin(x) = (exp(i.x) - exp(-i.x))/2 . (-i) 
      let sin x = smul 0.5 (sub (exp(iscale 1.0 x)) (exp(iscale -1.0 x))) |> iscale (-1.0)
      // tan(x) = (exp(i.x) - exp(-i.x)) . (-i) / (exp(i.x) + exp(-i.x)) 
      //        = (exp(2i.x) - 1.0)      . (-i) / (exp(2i.x) + 1.0)      
      let tan x = let exp2ix = exp(iscale 2.0 x) in
                  (div (sub exp2ix one) (add exp2ix one)) |> iscale -1.0


    type Complex with 
        static member Create(a,b) = Complex.mkRect (a,b)
        static member CreatePolar(a,b) = Complex.mkPolar (a,b)
        member x.Magnitude = Complex.magnitude x
        member x.Phase = Complex.phase x
        member x.RealPart = x.r
        member x.ImaginaryPart = x.i
        member x.Conjugate = Complex.conjugate x

        static member Sin(x) = Complex.sin(x)
        static member Cos(x) = Complex.cos(x)
        static member Abs(x) = Complex.abs(x)
        static member Tan(x) = Complex.tan(x)
        static member Log(x) = Complex.log(x)
        static member Exp(x) = Complex.exp(x)
        static member Sqrt(x) = Complex.sqrt(x)
        
        static member Zero = Complex.zero
        static member One = Complex.one 
        static member OneI = Complex.onei 
        static member ( +  ) (a,b) = Complex.add a b
        static member ( -  ) (a,b) = Complex.sub a b
        static member ( *  ) (a,b) = Complex.mul a b
        static member ( /  ) (a,b) = Complex.div a b
        static member ( ~- ) a = Complex.neg a
        static member ( * ) (a,b) = Complex.smul a b
        static member ( * ) (a,b) = Complex.muls a b


    module ComplexTopLevelOperators = 
        let complex x y = Complex.mkRect (x,y)

