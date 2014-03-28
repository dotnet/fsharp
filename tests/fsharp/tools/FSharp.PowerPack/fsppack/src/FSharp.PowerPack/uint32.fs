namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module UInt32 = 

    let compare (x:uint32) y = compare x y

    let zero = 0ul
    let one = 1ul
    let add (x:uint32) (y:uint32) = x + y
    let sub (x:uint32) (y:uint32) = x - y
    let mul (x:uint32) (y:uint32) = x * y
    let div (x:uint32) (y:uint32) = x / y
    let rem (x:uint32) (y:uint32) = x % y
    let succ (x:uint32) = x + 1ul
    let pred (x:uint32) = x - 1ul
    let max_int = 0xFFFFFFFFul
    let min_int = 0ul 
    let logand (x:uint32) (y:uint32) = x &&& y
    let logor (x:uint32) (y:uint32) = x ||| y
    let logxor (x:uint32) (y:uint32) = x ^^^ y
    let lognot (x:uint32) = ~~~x
    let shift_left (x:uint32) (n:int) =  x <<< n
    let shift_right (x:uint32) (n:int) =  x >>> n
    let of_int (x:int) =  uint32 x
    let to_int (x:uint32) = int x
    let of_int32 (x:int32) =  uint32 x
    let to_int32 (x:uint32) = int32 x

    let of_float (x:float) =  uint32 x
    let to_float (x:uint32) =  float x

    let of_string (s:string) = try uint32 s with _ -> failwith "UInt32.of_string"
    let to_string (x:uint32) = (box x).ToString()

    let bits_of_float32 (x:float32) = 
        System.BitConverter.ToUInt32(System.BitConverter.GetBytes(x),0)
          
    let float32_of_bits (x:uint32) = 
        System.BitConverter.ToSingle(System.BitConverter.GetBytes(x),0)

    let float32_to_float (x:float32) = float x
    let float_to_float32 (x:float) = float32 x

    let float_of_bits x = float32_to_float (float32_of_bits x)
    let bits_of_float x = bits_of_float32 (float_to_float32 x)

    let of_unativeint (x:unativeint) =  uint32 x
    let to_unativeint (x:uint32) =  unativeint x

    let of_uint64 (x:uint64) =  uint32 x
    let to_uint64 (x:uint32) =  uint64 x



    let of_float32 (x:float32) =  uint32 x
    let to_float32 (x:uint32) =  float32 x
