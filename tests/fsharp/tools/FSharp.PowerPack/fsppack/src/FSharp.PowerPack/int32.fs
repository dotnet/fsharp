namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Int32 = 

    let compare (x:int32) y = compare x y

    let zero = 0
    let one = 1
    let minus_one = (-1)
    let neg (x:int32) =  -x
    let add (x:int32) (y:int32) = x + y
    let sub (x:int32) (y:int32) = x - y
    let mul (x:int32) (y:int32) = x * y
    let div (x:int32) (y:int32) = x / y
    let rem (x:int32) (y:int32) = x % y
    let succ (x:int32) = x + 1
    let pred (x:int32) = x - 1
    let abs (x:int32) = if x < zero then neg x else x
    let max_int = 0x7FFFFFFF
    let min_int = 0x80000000
    let logand (x:int32) (y:int32) = x &&& y
    let logor (x:int32) (y:int32) = x ||| y
    let logxor (x:int32) (y:int32) = x ^^^ y
    let lognot (x:int32) = ~~~ x
    let shift_left (x:int32) (n:int) =  x <<< n
    let shift_right (x:int32) (n:int) =  x >>> n

    let of_uint32 (x:uint32) =  int32 x
    let to_uint32 (x:int32) =  uint32 x

    let shift_right_logical (x:int32) (n:int) =  of_uint32 (to_uint32 x >>> n)
    let of_int (n:int) =  n
    let to_int (x:int32) = x
    let of_float (x:float) =  int32 x
    let to_float (x:int32) =  float x

    let of_float32 (x:float32) =  int32 x
    let to_float32 (x:int32) =  float32 x

    let of_int64 (x:int64) =  int32 x
    let to_int64 (x:int32) =  int64 x

    let of_nativeint (x:nativeint) =  int32 x 
    let to_nativeint (x:int32) =  nativeint x


    let of_string (s:string) = try int32 s with _ -> failwith "Int32.of_string"
    let to_string (x:int32) = (box x).ToString()

    let bits_of_float32 (x:float32) = System.BitConverter.ToInt32(System.BitConverter.GetBytes(x),0)
    let float32_of_bits (x:int32) = System.BitConverter.ToSingle(System.BitConverter.GetBytes(x),0)

    let float32_to_float (x:float32) = float x
    let float_to_float32 (x:float) = float32 x

    let float_of_bits x = float32_to_float (float32_of_bits x)
    let bits_of_float x = bits_of_float32 (float_to_float32 x)
