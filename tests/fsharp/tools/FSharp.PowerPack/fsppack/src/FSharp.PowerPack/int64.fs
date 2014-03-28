namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Int64 = 

    let compare (x:int64) y = compare x y

    let zero = 0L
    let one = 1L
    let minus_one = -1L
    let neg (x:int64) =  -x
    let add (x:int64) (y:int64) = x + y
    let sub (x:int64) (y:int64) = x - y
    let mul (x:int64) (y:int64) = x * y
    let div (x:int64) (y:int64) = x / y
    let rem (x:int64) (y:int64) = x % y
    let succ (x:int64) = x + 1L
    let pred (x:int64) = x - 1L
    let abs (x:int64) = if x < zero then neg x else x
    let max_int = 0x7FFFFFFFFFFFFFFFL
    let min_int = 0x8000000000000000L
    let logand (x:int64) (y:int64) = x &&& y
    let logor (x:int64) (y:int64) = x ||| y
    let logxor (x:int64) (y:int64) = x ^^^ y
    let lognot (x:int64) = ~~~x
    let shift_left (x:int64) (n:int) =  x <<< n
    let shift_right (x:int64) (n:int) =  x >>> n
    let of_int (n:int) =  int64 n
    let to_int (x:int64) = int x
    let of_int32 (n:int32) =  int64 n
    let to_int32 (x:int64) = int32 x
    let of_uint64 (n:uint64) =  int64 n
    let to_uint64 (x:int64) = uint64 x
    let shift_right_logical (x:int64) (n:int) =  of_uint64 (to_uint64 x >>> n)

    let of_nativeint (n:nativeint) =  int64 n
    let to_nativeint (x:int64) = nativeint x
    let of_float (x:float) =  int64 x
    let to_float (x:int64) =  float x

    let of_string (s:string) = try int64 s with _ -> failwith "Int64.of_string"
    let to_string (x:int64) = (box x).ToString()

#if FX_NO_DOUBLE_BIT_CONVERTER
#else
    let bits_of_float (x:float) = System.BitConverter.DoubleToInt64Bits(x)
    let float_of_bits (x:int64) = System.BitConverter.Int64BitsToDouble(x)
#endif


    let of_float32 (x:float32) =  int64 x
    let to_float32 (x:int64) =  float32 x

