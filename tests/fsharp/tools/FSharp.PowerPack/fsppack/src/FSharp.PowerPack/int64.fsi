namespace Microsoft.FSharp.Compatibility

/// Basic operations on 64-bit integers. The type int64 is identical to <c>System.Int64</c>. 
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<CompilerMessage("This module is for ML compatibility. Consider using the F# overloaded operators such as 'int' and 'float' to convert numbers. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Int64 = 

    /// The value zero as a System.Int64
    val zero: int64
    /// The value one as a System.Int64
    val one: int64
    /// Returns the predeccessor of the argument 
    val pred: int64 -> int64
    /// Returns the successor of the argument 
    val succ: int64 -> int64
    /// Returns the largest 64-bit signed integer
    val max_int: int64
    /// Returns the smallest 64-bit signed integer
    val min_int: int64
    /// The value minus one as a System.Int64
    val minus_one: int64

    /// Returns the absolute value of the argument
    val abs: int64 -> int64
    /// Returns the sum of a and b
    val add: a:int64 -> b:int64 -> int64
    /// Returns a divided by b 
    val div: a:int64 -> b:int64 -> int64
    /// Returns a multiplied by b 
    val mul: a:int64 -> b:int64 -> int64
    /// Returns -a
    val neg: a:int64 -> int64
    /// Returns the remainder of a divided by b
    val rem: a:int64 -> b:int64 -> int64
    /// Returns a minus b 
    val sub: a:int64 -> b:int64 -> int64

    /// Compares a and b and returns 1 if a > b, -1 if b < a and 0 if a = b
    val compare: int64 -> int64 -> int

    /// Combines the binary representation of a and b by bitwise and
    val logand: int64 -> int64 -> int64
    /// Returns the bitwise logical negation of a
    val lognot: int64 -> int64
    /// Combines the binary representation of a and b by bitwise or
    val logor: int64 -> int64 -> int64
    /// Combines the binary representation of a and b by bitwise xor
    val logxor: int64 -> int64 -> int64
    /// Shifts the binary representation a by n bits to the left
    val shift_left: a:int64 -> n:int -> int64
    /// Shifts the binary representation a by n bits to the right; high-order empty bits are set to the sign bit
    val shift_right: a:int64 -> n:int -> int64
    /// Shifts the binary representation a by n bits to the right; high-order bits are zero-filled
    val shift_right_logical: a:int64 -> n:int -> int64

    /// Converts a 32-bit float to a 64-bit integer
    val of_float32: float32 -> int64
    /// Converts a 64-bit integer to a 32-bit float 
    val to_float32: int64 -> float32

    /// Converts a 64-bit float to a 64-bit integer
    val of_float: float -> int64
    /// Converts a 64-bit integer to a 64-bit float 
    val to_float: int64 -> float

    (* Conversions to integers are to either the same size or same sign *)
    /// Converts a 32-bit integer to a 64-bit integer
    val of_int: int -> int64
    /// Converts a 64-bit integer to a 32-bit integer
    val to_int: int64 -> int

    /// Converts a 32-bit integer to a 64-bit integer
    val of_int32: int32 -> int64
    /// Converts a 64-bit integer to a 32-bit integer
    val to_int32: int64 -> int32

    /// Converts an unsigned 64-bit integer to a 64-bit integer
    val of_uint64: uint64 -> int64
    /// Converts a 64-bit integer to an unsigned 64-bit integer
    val to_uint64: int64 -> uint64

    /// Converts a native integer to a 64-bit integer
    val of_nativeint: nativeint -> int64
    /// Converts a 64-bit integer to a native integer 
    val to_nativeint: int64 -> nativeint

    /// Converts a string to a 64-bit integer 
    val of_string: string -> int64
    /// Converts a 64-bit integer to a string
    val to_string: int64 -> string

#if FX_NO_DOUBLE_BIT_CONVERTER
#else
    /// Converts a raw 64-bit representation to a 64-bit float
    val float_of_bits: int64 -> float
    /// Converts a 64-bit float to a raw 64-bit representation 
    val bits_of_float: float -> int64
#endif
