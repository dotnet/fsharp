namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Obsolete("Consider using operators such as 'int' and 'int64' to convert numbers")>]
module Int16 = 

    let compare (x:int16) y = compare x y

    let zero = 0s
    let one = 1s
    let minus_one = - 1s
    let neg (x:int16) =  - x
    let add (x:int16) (y:int16) = x + y
    let sub (x:int16) (y:int16) = x - y
    let mul (x:int16) (y:int16) = x * y 
    let div (x:int16) (y:int16) = x / y
    let rem (x:int16) (y:int16) = x % y
    let succ (x:int16) = x + 1s
    let pred (x:int16) = x - 1s
    let abs (x:int16) = if x < zero then neg x else x
    let max_int = 0x7FFFs
    let min_int = 0x8000s
    let logand (x:int16) (y:int16) = x &&& y
    let logor (x:int16) (y:int16) = x ||| y
    let logxor (x:int16) (y:int16) = x ^^^ y
    let lognot (x:int16) = ~~~ x
    let shift_left (x:int16) (n:int) =  x <<< n
    let shift_right (x:int16) (n:int) =  x >>> n

    let of_int8 (n:int8)   = int16 n
    let to_int8 (x:int16) = sbyte x

    let of_int (n:int) =  int16 n
    let to_int (x:int16) = int x

    let of_int32 (n:int32) =  int16 n
    let to_int32 (x:int16) = int32 x
