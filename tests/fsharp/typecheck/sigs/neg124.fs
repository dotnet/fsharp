module Neg124

// Variation on test case mentioned in https://github.com/dotnet/fsharp/pull/6805#issuecomment-580368303
module Negative_SelectOverloadedWitnessBasedOnInputAndReturnTypeWithoutOutputTypeSelector =
    type witnesses = 
      static member inline unsigned_witness (x : sbyte) = uint8 x
      static member inline unsigned_witness (x : byte) = x
      static member inline unsigned_witness (x : int16) = uint16 x
      static member inline unsigned_witness (x : uint16) = x
      static member inline unsigned_witness (x : int32) = uint32 x
      static member inline unsigned_witness (x : uint32) = x
      static member inline unsigned_witness (x : int64) = uint64 x
      static member inline unsigned_witness (x : uint64) = x

    // Note, this doesn't try to use the output to select
    let inline call_unsigned_witness< ^witnesses, ^input, ^output when (^witnesses or ^input) : (static member unsigned_witness : ^input -> ^output)> (x : ^input) =
      ((^witnesses or ^input) : (static member unsigned_witness : ^input -> ^output) x)

    // unsigned: ^a -> ^b
    let inline unsigned num = call_unsigned_witness<witnesses, _, _> num

    // Positive cases
    let v1 = unsigned 0y
    let v2 = unsigned 0s
    let v3 = unsigned 0
    let v4 = unsigned 0L

    let f1 : int8 -> uint8 = unsigned
    let f2 : int16 -> uint16 = unsigned
    let f3 : int32 -> uint32 = unsigned
    let f4 : int64 -> uint64 = unsigned

    let g1 : int8 -> _ = unsigned
    let g2 : int16 -> _ = unsigned
    let g3 : int32 -> _ = unsigned
    let g4 : int64 -> _ = unsigned

    // Negative case - not enough information here
    let h1 : _ -> uint8 = unsigned
    