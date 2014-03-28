namespace Microsoft.FSharp.Compatibility

/// UInt32: ML-like operations on 32-bit System.UInt32 numbers.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Obsolete("Consider using operators such as 'int32' and 'uint32' to convert numbers")>]
module UInt32 = 

    val zero: uint32
    val one: uint32
    val pred: uint32 -> uint32
    val max_int: uint32
    val min_int: uint32
    val succ: uint32 -> uint32

    val add: uint32 -> uint32 -> uint32
    val div: uint32 -> uint32 -> uint32
    val mul: uint32 -> uint32 -> uint32
    val rem: uint32 -> uint32 -> uint32
    val sub: uint32 -> uint32 -> uint32

    val compare: uint32 -> uint32 -> int

    val logand: uint32 -> uint32 -> uint32
    val lognot: uint32 -> uint32
    val logor: uint32 -> uint32 -> uint32
    val logxor: uint32 -> uint32 -> uint32
    val shift_left: uint32 -> int -> uint32
    val shift_right: uint32 -> int -> uint32

    val of_float: float -> uint32
    val to_float: uint32 -> float

    val of_float32: float32 -> uint32
    val to_float32: uint32 -> float32

    (* Conversions to int are included because int is the most convenient *)
    (* integer type to use from F#.  Otherwise conversions are either to *)
    (* integers of the same size or same sign *)
    val of_int: int -> uint32
    val to_int: uint32 -> int

    val of_int32: int32 -> uint32
    val to_int32: uint32 -> int32

    val of_uint64: uint64 -> uint32
    val to_uint64: uint32 -> uint64

    val of_unativeint: unativeint -> uint32
    val to_unativeint: uint32 -> unativeint

    val of_string: string -> uint32
    val to_string: uint32 -> string

    val float_of_bits: uint32 -> float
    val bits_of_float: float -> uint32

    val float32_of_bits: uint32 -> float32
    val bits_of_float32: float32 -> uint32

