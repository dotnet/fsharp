module Neg128

module Negative_SelectOverloadedWitnessBasedOnReturnTypeByPassingDummyArgumentGenericOutputSelector = 
    open System
    open System.Numerics
    let _uint8max = bigint (uint32 Byte.MaxValue)
    let _uint16max = bigint (uint32 UInt16.MaxValue)
    let _uint32max = bigint UInt32.MaxValue
    let _uint64max = bigint UInt64.MaxValue
    type witnesses = 
      static member inline convert_witness (x : bigint, _output : int32) = int (uint32 (x &&& _uint32max))
      static member inline convert_witness (x : bigint, _output : int64) = int64 (uint64 (x &&& _uint64max))
      static member inline convert_witness (x : bigint, _output : bigint) = x
      static member inline convert_witness (x : bigint, _output : float) = float x
      static member inline convert_witness (x : bigint, _output : sbyte) = sbyte (byte (x &&& _uint8max))
      static member inline convert_witness (x : bigint, _output : int16) = int16 (uint16 (x &&& _uint16max))
      static member inline convert_witness (x : bigint, _output : byte) = byte (x &&& _uint8max)
      static member inline convert_witness (x : bigint, _output : uint16) = uint16 (x &&& _uint16max)
      static member inline convert_witness (x : bigint, _output : uint32) = uint32 (x &&& _uint32max)
      static member inline convert_witness (x : bigint, _output : uint64) = uint64 (x &&& _uint64max)
      static member inline convert_witness (x : bigint, _output : float32) = float32 x
      static member inline convert_witness (x : bigint, _output : decimal) = decimal x
      static member inline convert_witness (x : bigint, _output : Complex) = Complex(float x, 0.0)

    let inline call_convert_witness< ^witnesses, ^input, 'output when (^witnesses or ^input) : (static member convert_witness : ^input * 'output -> 'output)> (b : ^input, c : 'output) =
        ((^witnesses or ^input) : (static member convert_witness : ^input * 'output -> 'output) (b, c))

    let inline convert num : 'output =
      call_convert_witness<witnesses, _, _> (num, Unchecked.defaultof<'output>)

    // These solve ok
    let v1 : int32 = convert 777I
    let v2 : int64 = convert 777I

    // Pre-FS1043 this gives an error, because solving kicks in once all selector types are known. Post-FS1043 it should compile
    let inline inst (num: bigint) : 'output = convert num
