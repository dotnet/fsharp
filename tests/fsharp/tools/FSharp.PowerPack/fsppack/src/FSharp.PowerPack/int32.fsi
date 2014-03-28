namespace Microsoft.FSharp.Compatibility

/// Basic operations on 32-bit integers. The type int32 is identical to <c>System.Int32</c>. 
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<CompilerMessage("This module is for ML compatibility. Consider using the F# overloaded operators such as 'int' and 'float' to convert numbers. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Int32 = 

    /// The value zero as a System.Int32
    val zero: int32
    /// The value one as a System.Int32
    val one: int32
    /// The value minus one as a System.Int32
    val minus_one: int32
    /// Returns the predeccessor of the argument 
    val pred: int32 -> int32
    /// Returns the largest 32-bit signed integer
    val max_int: int32
    /// Returns the smallest 32-bit signed integer
    val min_int: int32
    /// Returns the successor of the argument 
    val succ: int32 -> int32

    /// Returns the absolute value of the argument
    val abs: int32 -> int32
    /// Returns the sum of a and b
    val add: a:int32 -> b:int32 -> int32
    /// Returns a divided by b 
    val div: a:int32 -> b:int32 -> int32
    /// Returns a multiplied by b 
    val mul: a:int32 -> b:int32 -> int32    
    /// Returns -a
    val neg: a:int32 -> int32
    /// Returns the remainder of a divided by b
    val rem: a:int32 -> b:int32 -> int32
    /// Returns a minus b 
    val sub: a:int32 -> b:int32 -> int32

    /// Compares a and b and returns 1 if a > b, -1 if b < a and 0 if a = b
    val compare: a:int32 -> b:int32 -> int

    /// Combines the binary representation of a and b by bitwise and
    val logand: a:int32 -> b:int32 -> int32
    /// Returns the bitwise logical negation of a
    val lognot: a:int32 -> int32
    /// Combines the binary representation of a and b by bitwise or
    val logor: a:int32 -> b:int32 -> int32
    /// Combines the binary representation of a and b by bitwise xor
    val logxor: int32 -> int32 -> int32
    /// Shifts the binary representation a by n bits to the left
    val shift_left: a:int32 -> n:int -> int32
    /// Shifts the binary representation a by n bits to the right; high-order empty bits are set to the sign bit
    val shift_right: a:int32 -> n:int -> int32
    /// Shifts the binary representation a by n bits to the right; high-order bits are zero-filled
    val shift_right_logical: a:int32 -> n:int -> int32

    /// Converts a 64-bit float to a 32-bit integer
    val of_float: float -> int32
    /// Converts a 32-bit integer to a 64-bit float 
    val to_float: int32 -> float

    /// Converts a 32-bit float to a 32-bit integer
    val of_float32: float32 -> int32
    /// Converts a 32-bit integer to a 32-bit float 
    val to_float32: int32 -> float32

    (* In F#, type int32 is identical to int.  These operations are *)
    (* included mostly for compatibility with other versions of ML *)
    /// Converts a 32-bit integer to a 32-bit integer (included for ML compatability)
    val of_int: int -> int32
    /// Converts a 32-bit integer to a 32-bit integer (included for ML compatability)
    val to_int: int32 -> int

    (* Conversions to integers are to either the same size or same sign *)
    /// Converts a 32-bit unsigned integer to a 32-bit integer 
    val of_uint32: uint32 -> int32
    /// Converts a 32-bit integer to a 32-bit unsigned integer 
    val to_uint32: int32 -> uint32

    /// Converts a 64-bit unsigned integer to a 32-bit integer 
    val of_int64: int64 -> int32
    /// Converts a 32-bit unsigned integer to a 64-bit integer 
    val to_int64: int32 -> int64

    /// Converts a 32-bit unsigned integer to a 32-bit integer 
    val of_nativeint: nativeint -> int32
    /// Converts a 32-bit unsigned integer to a 32-bit integer 
    val to_nativeint: int32 -> nativeint

    /// Converts a string to a 32-bit integer 
    val of_string: string -> int32
    /// Converts a 32-bit integer to a string 
    val to_string: int32 -> string

    /// Converts a raw 32-bit representation to a 64-bit float
    val float_of_bits: int32 -> float
    /// Converts a 64-bit float to a raw 32-bit representation 
    val bits_of_float: float -> int32

    /// Converts a raw 32-bit representation to a 32-bit float
    val float32_of_bits: int32 -> float32
    /// Converts a 32-bit float to a raw 32-bit representation 
    val bits_of_float32: float32 -> int32

