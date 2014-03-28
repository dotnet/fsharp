namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Byte= 

    let compare (x:byte) y = compare x y

    let zero = 0uy
    let one = 1uy
    let add (x:byte) (y:byte) = x + y
    let sub (x:byte) (y:byte) = x - y
    let mul (x:byte) (y:byte) = x * y
    let div (x:byte) (y:byte) = x / y
    let rem (x:byte) (y:byte) = x % y
    let succ (x:byte) = x + 1uy
    let pred (x:byte) = x - 1uy
    let max_int = 0xFFuy
    let min_int = 0uy
    let logand (x:byte) (y:byte) = x &&& y
    let logor (x:byte) (y:byte) = x ||| y
    let logxor (x:byte) (y:byte) = x ^^^ y
    let lognot (x:byte) = ~~~x
    let shift_left (x:byte) (n:int) =  x <<< n
    let shift_right (x:byte) (n:int) =  x >>> n
    let of_int (n:int) =  byte n
    let to_int (x:byte) = int x
    let of_char (n:char) =  byte n
    let to_char (x:byte) = char x
    let of_string (s:string) = byte (int32 s)
      
    let to_string (x:byte) =  (box x).ToString()

    let of_int32 (n:int32) = byte n
    let to_int32 (x:uint8) = int32 x

    let of_uint16 (n:uint16) = byte n
    let to_uint16 (x:uint8)  = uint16 x

    let of_uint32 (n:uint32) = byte n
    let to_uint32 (x:uint8)  = uint32 x
