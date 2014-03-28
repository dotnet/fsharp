namespace Microsoft.FSharp.Compatibility

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Obsolete("Consider using operators such as 'int' and 'uint32' and 'uint64' to convert numbers")>]
/// Byte (8-bit) operations.
module Byte = 

    /// The value zero as a System.Byte
    val zero: byte
    /// The value one as a System.Byte
    val one: byte
    /// Returns the successor of the argument wrapped around 255uy
    val succ: byte -> byte
    /// Returns the predeccessor of the argument wrapped around 0uy
    val pred: byte -> byte

    /// Returns the sum of a and b
    val add: a:byte -> b:byte -> byte
    /// Returns a divided by b 
    val div: a:byte -> b:byte -> byte
    /// Returns a multiplied by b 
    val mul: a:byte -> b:byte -> byte
    /// Returns a minus b 
    val sub: a:byte -> b:byte -> byte
    /// Returns the remainder of a divided by b
    val rem: a:byte -> b:byte -> byte

    /// Compares a and b and returns 1 if a > b, -1 if b < a and 0 if a = b
    val compare: a:byte -> b:byte -> int

    /// Combines the binary representation of a and b by bitwise and
    val logand: a:byte -> b:byte -> byte
    /// Returns the bitwise logical negation of a
    val lognot: a:byte -> byte
    /// Combines the binary representation of a and b by bitwise or
    val logor: a:byte -> b:byte -> byte
    /// Combines the binary representation of a and b by bitwise xor
    val logxor: a:byte -> b:byte -> byte
    /// Shifts the binary representation a by n bits to the left
    val shift_left: a:byte -> n:int -> byte
    /// Shifts the binary representation a by n bits to the right
    val shift_right: a:byte -> n:int -> byte

    /// Converts a char to a byte
    val of_char: c:char -> byte
    /// Converts a byte to a char 
    val to_char: b:byte -> char

    /// Converts a 32-bit integer to a byte
    val of_int: i:int -> byte
    /// Converts a byte to a 32-bit integer 
    val to_int: b:byte -> int

    /// Converts a 32-bit integer to a byte
    val of_int32: i:int32 -> byte
    /// Converts a byte to a 32-bit integer 
    val to_int32: b:byte -> int32

    /// Converts a 16-bit integer to a byte
    val of_uint16: ui:uint16 -> byte
    /// Converts a byte to a 16-bit integer 
    val to_uint16: b:byte -> uint16

    /// Converts an unsigned 32-bit integer to a byte
    val of_uint32: ui:uint32 -> byte
    /// Converts a byte to an unsigned 32-bit integer 
    val to_uint32: b:byte -> uint32

    /// Converts a string to a byte
    val of_string: s:string -> byte
    /// Converts a byte to a string
    val to_string: b:byte -> string
