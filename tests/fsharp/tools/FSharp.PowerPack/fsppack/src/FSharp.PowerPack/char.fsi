namespace Microsoft.FSharp.Compatibility

/// Unicode characters, i.e. the <c>System.Char</c> type.  see also the operations
/// in <c>System.Char</c> and the <c>System.Text.Encoding</c> interfaces if necessary.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
[<CompilerMessage("This module is for ML compatibility. Consider using the F# overloaded operators such as 'char' and 'int' to convert basic types and System.Char.ToLower and System.Char.ToUpper to change case. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Char = 

    /// Converts the value of the specified 32-bit signed integer to its equivalent Unicode character
    val chr: i:int -> char
    /// Converts the value of the specified Unicode character to the equivalent 32-bit signed integer
    val code: c:char -> int
    /// Compares a and b and returns 1 if a > b, -1 if b < a and 0 if a = b
    val compare: a:char -> b:char -> int
    /// Converts the value of a Unicode character to its lowercase equivalent
    val lowercase: char -> char
    /// Converts the value of a Unicode character to its uppercase equivalent
    val uppercase: char -> char 
