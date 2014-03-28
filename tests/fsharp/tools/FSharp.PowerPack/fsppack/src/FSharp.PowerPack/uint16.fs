namespace Microsoft.FSharp.Compatibility

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Obsolete("Consider using the F# overloaded operators such as 'int' and 'float' to convert numbers")>]
module UInt16 = 

    let compare (x:uint16) y = compare x y

    let zero = 0us
    let one = 1us
    let add (x:uint16) (y:uint16) = x + y
    let sub (x:uint16) (y:uint16) = x - y
    let mul (x:uint16) (y:uint16) = x * y
    let div (x:uint16) (y:uint16) = x / y
    let rem (x:uint16) (y:uint16) = x % y
    let succ (x:uint16) = x + 1us
    let pred (x:uint16) = x - 1us
    let max_int = 0xFFFFus
    let min_int = 0us
    let logand (x:uint16) (y:uint16) = x &&& y
    let logor (x:uint16) (y:uint16) = x ||| y
    let logxor (x:uint16) (y:uint16) = x ^^^ y
    let lognot (x:uint16) = ~~~ x
    let shift_left (x:uint16) (n:int) =  x <<< n
    let shift_right (x:uint16) (n:int) =  x >>> n

    let of_int (n:int)    = uint16 n
    let to_int (x:uint16) = int x

    let of_int16 (n:int16)  = uint16 n
    let to_int16 (x:uint16) = int16 x

    let of_int32 (n:int32)  = uint16 n
    let to_int32 (x:uint16) = int32 x

    let of_uint32 (n:uint32) = uint16 n
    let to_uint32 (x:uint16) = uint32 x

    let of_uint8 (n:uint8) = uint16 n
    let to_uint8 (x:uint16) = byte x
