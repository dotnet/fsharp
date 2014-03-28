namespace Microsoft.FSharp.Math

    open System
      
    /// The type of complex numbers stored as pairs of 64-bit floating point numbers in rectangular coordinates
    [<Struct>]
    [<CustomEquality; CustomComparison>]
    type Complex = 
        /// The real part of a complex number
        member r: float
        /// The imaginary part of a complex number
        member i: float
        /// The polar-coordinate magnitude of a complex number
        member Magnitude: float
        /// The polar-coordinate phase of a complex number
        member Phase: float
        /// The real part of a complex number
        member RealPart: float
        /// The imaginary part of a complex number
        member ImaginaryPart: float
        /// The conjugate of a complex number, i.e. x-yi
        member Conjugate: Complex
        /// Create a complex number x+ij using rectangular coordinates
        static member Create      : float * float -> Complex
        /// Create a complex number using magnitude/phase polar coordinates
        static member CreatePolar : float * float -> Complex
        /// The complex number 0+0i
        static member Zero   : Complex
        /// The complex number 1+0i
        static member One    : Complex
        /// The complex number 0+1i
        static member OneI   : Complex
        /// Add two complex numbers
        static member ( +  ) : Complex * Complex -> Complex
        /// Subtract one complex number from another
        static member ( -  ) : Complex * Complex -> Complex
        /// Multiply two complex numbers
        static member ( *  ) : Complex * Complex -> Complex
        /// Complex division of two complex numbers
        static member ( /  ) : Complex * Complex -> Complex
        /// Unary negation of a complex number
        static member ( ~- ) : Complex           -> Complex
        /// Multiply a scalar by a complex number 
        static member ( * ) : float   * Complex -> Complex
        /// Multiply a complex number by a scalar
        static member ( * ) : Complex * float   -> Complex

        static member Sin : Complex -> Complex
        static member Cos : Complex -> Complex
        
        /// Computes the absolute value of a complex number: e.g. Abs x+iy = sqrt(x**2.0 + y**2.0.)
        /// Note: Complex.Abs(z) is the same as z.Magnitude
        static member Abs : Complex -> float
        static member Tan : Complex -> Complex
        static member Log : Complex -> Complex
        static member Exp : Complex -> Complex
        static member Sqrt : Complex -> Complex
        
        override ToString : unit -> string
        override Equals : obj -> bool
        interface System.IComparable
        member ToString : format:string -> string
        member ToString : format:string * provider:System.IFormatProvider -> string

    /// The type of complex numbers 
    type complex = Complex


    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Complex =

        val mkRect: float * float -> complex

          /// The polar-coordinate magnitude of a complex number
        val magnitude: complex -> float
          /// The polar-coordinate phase of a complex number
        val phase     : complex -> float
          /// The real part of a complex number
        val realPart  : complex -> float
          /// The imaginary part of a complex number
        val imagPart  : complex -> float
          /// Create a complex number using magnitude/phase polar coordinates
        val mkPolar : float * float -> complex
        /// A complex of magnitude 1 and the given phase and , i.e. cis x = mkPolar 1.0 x
        val cis     : float -> complex
        
          /// The conjugate of a complex number, i.e. x-yi
        val conjugate : complex -> complex

          /// The complex number 0+0i
        val zero    : complex
          /// The complex number 1+0i
        val one     : complex
          /// The complex number 0+1i
        val onei    : complex
          /// Add two complex numbers
        val add     : complex -> complex -> complex
          /// Subtract one complex number from another
        val sub     : complex -> complex -> complex
          /// Multiply two complex numbers
        val mul     : complex -> complex -> complex
          /// Complex division of two complex numbers
        val div     : complex -> complex -> complex
          /// Unary negation of a complex number
        val neg     : complex -> complex
          /// Multiply a scalar by a complex number 
        val smul    : float -> complex -> complex
          /// Multiply a complex number by a scalar
        val muls    : complex -> float -> complex

          /// pi
        val pi  : Complex
          /// exp(x) = e^x
        val exp : Complex -> Complex
          /// log(x) is natural log (base e)
        val log : Complex -> Complex
          /// sqrt(x) and 0 <= phase(x) < pi
        val sqrt : Complex -> Complex
          /// Sine
        val sin : Complex -> Complex    
          /// Cosine
        val cos : Complex -> Complex
          /// Tagent
        val tan : Complex -> Complex
        

    [<AutoOpen>]
    module ComplexTopLevelOperators = 
        /// Constructs a complex number from both the real and imaginary part.
        val complex : float -> float -> complex



