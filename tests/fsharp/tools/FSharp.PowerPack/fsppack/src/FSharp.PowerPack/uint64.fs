namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module UInt64 = 

    let compare (x:uint64) y = compare x y

    let zero = 0UL
    let one = 1UL
    let add (x:uint64) (y:uint64) = x + y
    let sub (x:uint64) (y:uint64) = x - y
    let mul (x:uint64) (y:uint64) = x * y
    let div (x:uint64) (y:uint64) = x / y
    let rem (x:uint64) (y:uint64) = x % y
    let succ (x:uint64) = x + 1UL
    let pred (x:uint64) = x - 1UL
    let max_int = 0xFFFFFFFFFFFFFFFFUL
    let min_int = 0x0UL
    let logand (x:uint64) (y:uint64)   = x &&& y
    let logor  (x:uint64) (y:uint64)   = x ||| y
    let logxor (x:uint64) (y:uint64)   = x ^^^ y
    let lognot (x:uint64)              = ~~~x
    let shift_left  (x:uint64) (n:int) =  x <<< n
    let shift_right (x:uint64) (n:int) =  x >>> n
    let of_int        (n:int)             = uint64 n
    let to_int        (x:uint64)          = int x
    let of_uint32     (n:uint32)          = uint64 n
    let to_uint32     (x:uint64)          = uint32 x
    let of_int64      (n:int64)           = uint64 n
    let to_int64      (x:uint64)          = int64 x
    let of_unativeint (n:unativeint)      = uint64 n
    let to_unativeint (x:uint64)          = unativeint x
    let of_float      (f:float)           = uint64 f
    let to_float      (x:uint64)          = float x

    let of_string (s:string) = try uint64 s with _ -> failwith "UInt64.of_string"
    let to_string (x:uint64) = (box x).ToString()
    let bits_of_float (x:float) = System.BitConverter.ToUInt64(System.BitConverter.GetBytes(x),0)
    let float_of_bits (x:uint64) = System.BitConverter.ToDouble(System.BitConverter.GetBytes(x),0)

    let of_float32 (f:float32) =  uint64 f
    let to_float32 (x:uint64) =  float32 x
