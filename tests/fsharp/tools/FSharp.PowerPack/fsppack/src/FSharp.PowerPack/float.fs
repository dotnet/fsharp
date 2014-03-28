namespace Microsoft.FSharp.Compatibility

/// ML-like operations on 64-bit System.Double floating point numbers.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Float = 

    let add (x:float) (y:float) = x + y
    let div (x:float) (y:float) = x / y
    let mul (x:float) (y:float) = x * y
    let sub (x:float) (y:float) = x - y
    let neg (x:float)   = - x

    let compare (x:float) y = compare x y

    let of_int (x:int) = float x
    let ceil      (x:float)           = System.Math.Ceiling(x)
    let floor     (x:float)           = System.Math.Floor(x)
    let to_int (x:float) = 
#if FX_NO_TRUNCATE
         System.Convert.ToInt32(x) // REVIEW: possible implications of not calling Trancate?
#else
         System.Convert.ToInt32(System.Math.Truncate(x))
#endif

    let of_int64 (x:int64) = float x
    let to_int64 (x:float) = int64 x

    let of_int32 (x:int32) = float x
    let to_int32 (x:float) = int32 x

    let of_float32 (x:float32) = float x
    let to_float32 (x:float) = float32 x

    let to_string (x:float) = (box x).ToString()
    let of_string (s:string) = 
      (* Note System.Double.Parse doesn't handle -0.0 correctly (it returns +0.0) *)
      let s = s.Trim()  
      let l = s.Length 
      let p = 0 
      let p,sign = if (l >= p + 1 && s.[p] = '-') then 1,false else 0,true 
      let n = 
        try 
          if p >= l then raise (new System.FormatException()) 
          System.Double.Parse(s.[p..],System.Globalization.CultureInfo.InvariantCulture)
        with :? System.FormatException -> failwith "Float.of_string"
      if sign then n else -n

#if FX_NO_DOUBLE_BIT_CONVERTER
#else
    let to_bits (x:float) = System.BitConverter.DoubleToInt64Bits(x)
    let of_bits (x:int64) = System.BitConverter.Int64BitsToDouble(x)
#endif

