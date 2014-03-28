namespace Microsoft.FSharp.Compatibility

/// UInt64: basic operations on 64-bit System.UInt64 numbers.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Obsolete("Consider using operators such as 'int32' and 'uint64' to convert numbers")>]
module UInt64 = 

    val zero: uint64
    val one: uint64
    val pred: uint64 -> uint64
    val succ: uint64 -> uint64
    val max_int: uint64
    val min_int: uint64

    val add: uint64 -> uint64 -> uint64
    val div: uint64 -> uint64 -> uint64
    val mul: uint64 -> uint64 -> uint64
    val rem: uint64 -> uint64 -> uint64
    val sub: uint64 -> uint64 -> uint64

    val compare: uint64 -> uint64 -> int

    val logand: uint64 -> uint64 -> uint64
    val lognot: uint64 -> uint64
    val logor: uint64 -> uint64 -> uint64
    val logxor: uint64 -> uint64 -> uint64
    val shift_left: uint64 -> int -> uint64
    val shift_right: uint64 -> int -> uint64

    val of_float: float -> uint64
    val to_float: uint64 -> float

    val of_float32: float32 -> uint64
    val to_float32: uint64 -> float32

    val of_int: int -> uint64
    val to_int: uint64 -> int

    val of_uint32: uint32 -> uint64
    val to_uint32: uint64 -> uint32

    val of_int64: int64 -> uint64
    val to_int64: uint64 -> int64

    val of_unativeint: unativeint -> uint64
    val to_unativeint: uint64 -> unativeint

    val of_string: string -> uint64
    val to_string: uint64 -> string

    val float_of_bits: uint64 -> float
    val bits_of_float: float -> uint64

