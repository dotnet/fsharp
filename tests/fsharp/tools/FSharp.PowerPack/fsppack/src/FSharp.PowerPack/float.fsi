namespace Microsoft.FSharp.Compatibility

/// ML-like operations on 64-bit System.Double floating point numbers.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<CompilerMessage("This module is for ML compatibility. Consider using the F# overloaded operators such as 'int' and 'float' to convert numbers. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Float= 

    /// Returns the sum of a and b
    val add: a:float -> b:float -> float
    /// Returns a divided by b 
    val div: a:float -> b:float -> float
    /// Returns a multiplied by b 
    val mul: a:float -> b:float -> float
    /// Returns -a
    val neg: a:float -> float
    /// Returns a minus b 
    val sub: a:float -> b:float -> float
    /// Compares a and b and returns 1 if a > b, -1 if b < a and 0 if a = b
    val compare: a:float -> b:float -> int

    /// Converts a 32-bit integer to a 64-bit float
    val of_int: i:int -> float
    /// Converts a 64-bit float to a 32-bit integer
    val to_int: f:float -> int

    /// Converts a 32-bit integer to a 64-bit float
    val of_int32: i:int32 -> float
    /// Converts a 64-bit float to a 32-bit integer
    val to_int32: f:float -> int32

    /// Converts a 64-bit integer to a 64-bit float
    val of_int64: i:int64 -> float
    /// Converts a 64-bit float to a 64-bit integer 
    val to_int64: f:float -> int64

    /// Converts a 32-bit float to a 64-bit float
    val of_float32: f32:float32 -> float
    /// Converts a 64-bit float to a 32-bit float
    val to_float32: f:float -> float32

    /// Converts a string to a 64-bit float
    val of_string: s:string -> float
    /// Converts a 64-bit float to a string
    val to_string: f:float -> string
#if FX_NO_DOUBLE_BIT_CONVERTER
#else
    /// Converts a raw 64-bit representation to a 64-bit float
    val of_bits: i64:int64 -> float
    /// Converts a 64-bit float to raw 64-bit representation 
    val to_bits: f:float -> int64
#endif
