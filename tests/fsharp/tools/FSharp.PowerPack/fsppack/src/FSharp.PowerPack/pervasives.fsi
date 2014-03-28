//==========================================================================
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match. The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================
///Pervasives: Additional OCaml-compatible bindings 
[<CompilerMessage("This module is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Pervasives

#nowarn "62" // compatibility warnings
#nowarn "35"  // 'deprecated' warning about redefining '<' etc.
#nowarn "86"  // 'deprecated' warning about redefining '<' etc.


open System
open System.IO
open System.Collections.Generic

#if COMPILER
#endif
 
//--------------------------------------------------------------------------
//Pointer (physical) equality and hashing.

///Reference/physical equality. 
///True if boxed versions of the inputs are reference-equal, OR if
///both are value types and the implementation of Object.Equals for the type
///of the first argument returns true on the boxed versions of the inputs. 
///
///In normal use on reference types or non-mutable value types this function 
///has the following properties:
///   - returns 'true' for two F# values where mutation of data
///     in mutable fields of one affects mutation of data in the other
///   - will return 'true' if (=) returns true
///   - hashq will return equal hashes if (==) returns 'true'
///
///The use on mutable value types is not recommended.
[<CompilerMessage("This construct is for ML compatibility. Using the physical equality operator '==' is not recommended except in cross-compiled code. Consider using generic structural equality 'x = y' or 'LanguagePrimitives.PhysicalEquality x y'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (==): 'T -> 'T -> bool when 'T : not struct

/// Negation of the '==' operator, see also Obj.eq
[<CompilerMessage("This construct is for ML compatibility. Using the physical inequality operator '!=' is not recommended except in cross-compiled code. Consider using generic structual inequality 'x <> y' or 'not(LanguagePrimitives.PhysicalEquality x y)'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (!=): 'T -> 'T -> bool when 'T : not struct

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x % y' instead of 'x mod y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (mod): int -> int -> int 

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x &&& y' instead of 'x land y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]

val inline (land): int -> int -> int 

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x ||| y' instead of 'x lor y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (lor) : int -> int -> int 

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x ^^^ y' instead of 'x lxor y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (lxor): int -> int -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator '~~~x' instead of 'lnot x'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline lnot  : int -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x <<< y' instead of 'x lsl y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (lsl): int -> int -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x >>> y' on an unsigned type instead of 'x lsr y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (lsr): int -> int -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x >>> y' instead of 'x asr y'. The precedence of these operators differs, so you may need to add parentheses. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline (asr): int -> int -> int

/// 1D Array element get-accessor ('getter')
[<CompilerMessage("This construct is for ML compatibility. Consider using 'arr.[idx]' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline ( .() ) : 'T array -> int -> 'T

/// 1D Array element set-accessor ('setter')
[<CompilerMessage("This construct is for ML compatibility. Consider using 'arr.[idx] <- v' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val inline ( .()<- ) : 'T array -> int -> 'T -> unit

//--------------------------------------------------------------------------
//Integer-specific arithmetic

/// n-1 (no overflow checking)
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val pred: int -> int

/// n+1 (no overflow checking)
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val succ: int -> int

/// The lowest representable value in the 'int' type
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Int32.MinValue' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val min_int : int

/// The highest representable value in the 'int' type
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Int32.MaxValue' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val max_int : int

/// Negation on integers of the 'int' type
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator '-x' instead of 'int_neg x'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val int_neg : int -> int

//--------------------------------------------------------------------------
//Exceptions

[<CompilerMessage("This construct is for ML compatibility. Consider using System.IO.EndOfStreamException instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception End_of_file = System.IO.EndOfStreamException

[<CompilerMessage("This construct is for ML compatibility. Consider using System.OutOfMemoryException instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Out_of_memory = System.OutOfMemoryException

[<CompilerMessage("This construct is for ML compatibility. Consider using System.DivideByZeroException instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Division_by_zero = System.DivideByZeroException

[<CompilerMessage("This construct is for ML compatibility. Consider using System.StackOverflowException instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Stack_overflow = System.StackOverflowException 

[<CompilerMessage("This construct is for ML compatibility. This is a synonym for 'System.Collections.Generic.KeyNotFoundException'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val Not_found<'T> : exn

[<CompilerMessage("This construct is for ML compatibility. This is a synonym for 'System.Collections.Generic.KeyNotFoundException'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val (|Not_found|_|) : exn -> unit option

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Exit 

///  Non-exhaustive match failures will raise Match failures
/// A future release of F# may map this exception to a corresponding .NET exception.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'MatchFailureException' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Match_failure = Microsoft.FSharp.Core.MatchFailureException

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Undefined 

/// The exception thrown by 'assert' failures.
/// A future release of F# may map this exception to a corresponding .NET exception.
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
exception Assert_failure of string * int * int 

/// The exception thrown by <c>invalid_arg</c> and misues of F# library functions
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.ArgumentException' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val Invalid_argument : string -> exn

[<CompilerMessage("This construct is for ML compatibility. Consider matching against 'System.ArgumentException' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val (|Invalid_argument|_|) : exn -> string option

//--------------------------------------------------------------------------
//Floating point.
//
//The following operators only manipulate 'float64' numbers. The operators  '+' etc. may also be used.

/// This value is present primarily for compatibility with other versions of ML
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x * y' instead of 'x *. y'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ( *. ): float -> float -> float

/// This value is present primarily for compatibility with other versions of ML. In F#
/// the overloaded operators may be used.
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x + y' instead of 'x +. y'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ( +. ): float -> float -> float

/// This value is present primarily for compatibility with other versions of ML. In F#
/// the overloaded operators may be used.
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x - y' instead of 'x -. y'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ( -. ): float -> float -> float

/// This value is present primarily for compatibility with other versions of ML. In F#
/// the overloaded operators may be used.
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator '-x' instead of '-. x'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ( ~-. ): float -> float

/// This value is present primarily for compatibility with other versions of ML. In F#
/// the overloaded operators may be used.
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator '+x' instead of '+. x'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ( ~+. ): float -> float

/// This value is present primarily for compatibility with other versions of ML. In F#
/// the overloaded operators may be used.
[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'x / y' instead of 'x /. y'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ( /. ): float -> float -> float

[<CompilerMessage("This construct is for ML compatibility. Consider using the overloaded F# library function 'abs' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val abs_float: float -> float

/// This value is present primarily for compatibility with other versions of ML
/// The highest representable positive value in the 'float' type
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Double.MaxValue' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val max_float: float

/// This value is present primarily for compatibility with other versions of ML
/// The lowest non-denormalized positive IEEE64 float
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val min_float: float

/// This value is present primarily for compatibility with other versions of ML
/// The smallest value that when added to 1.0 gives a different value to 1.0
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val epsilon_float: float

/// This value is present primarily for compatibility with other versions of ML
[<CompilerMessage("This construct is for ML compatibility. Consider using the '%' operator instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val mod_float: float -> float -> float

/// This value is present primarily for compatibility with other versions of ML
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val modf: float -> float * float

/// This value is present primarily for compatibility with other versions of ML
[<CompilerMessage("This construct is for ML compatibility. Consider using '-infinity' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val neg_infinity: float

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val ldexp: float -> int -> float

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type fpclass = 
  | FP_normal
  | FP_zero
  | FP_infinite
  | FP_nan

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val classify_float: float -> fpclass

//--------------------------------------------------------------------------
//Common conversions. See also conversions such as
//Float32.to_int etc.

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val bool_of_string: string -> bool

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'char' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val char_of_int: int -> char

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'int' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val int_of_char: char -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'int' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val int_of_string: string -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'int' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val int_of_float: float -> int

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'string' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val string_of_bool: bool -> string

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'string' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val string_of_float: float -> string

[<CompilerMessage("This construct is for ML compatibility. Consider using the operator 'string' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val string_of_int: int -> string

[<CompilerMessage("This construct is for ML compatibility. Consider using the overloaded conversion function 'float' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val float_of_int: int -> float

[<CompilerMessage("This construct is for ML compatibility. Consider using the overloaded conversion function 'float' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val float_of_string: string -> float


//--------------------------------------------------------------------------
//I/O
//
//Caveat: These functions do not have precisely the same behaviour as 
//corresponding functions in other ML implementations, e.g. OCaml. 
//For example they may raise .NET exceptions rather than Sys_error.

  
/// This type is present primarily for compatibility with other versions of ML. When
/// not cross-compiling we recommend using the .NET I/O libraries
[<CompilerMessage("This construct is for ML compatibility. For advanced I/O consider using the System.IO namespace. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type open_flag = 
  | Open_rdonly
  | Open_wronly
  | Open_append
  | Open_creat
  | Open_trunc
  | Open_excl
  | Open_binary
  | Open_text
#if FX_NO_NONBLOCK_IO
#else
  | Open_nonblock
#endif
  | Open_encoding of System.Text.Encoding

//--------------------------------------------------------------------------


/// A pseudo-abstraction over binary and textual input channels.
/// OCaml-compatible channels conflate binary and text IO, and for this reasons their
/// use from F# is somewhat deprecated (direct use of System.IO StreamReader, TextReader and 
/// BinaryReader objects is preferred, e.g. see System.IO.File.OpenText). 
/// Well-written OCaml-compatible code that simply opens either a channel in text or binary 
/// mode and then does text or binary I/O using the OCaml-compatible functions below
/// will work, though care must be taken with regard to end-of-line characters (see 
/// input_char below).
///
/// This library pretends that an in_channel is just a System.IO.TextReader. Channel values
/// created using open_in_bin maintain a private System.IO.BinaryReader, which will be used whenever
/// you do I/O using this channel. 
///
/// InChannel.of_BinaryReader and InChannel.of_StreamReader allow you to build input 
/// channels out of the corresponding .NET abstractions.
[<CompilerMessage("This construct is for ML compatibility. Consider using one of the types System.IO.TextReader, System.IO.BinaryReader or System.IO.StreamReader instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type in_channel = System.IO.TextReader
    

/// Open the given file to read. 
///
///In the absence of an explicit encoding (e.g. using Open_encoding) open_in
///uses the default text encoding (System.Text.Encoding.Default). If you want to read a file
///regardless of encoding then you should use binary modes. Note that .NET's 
///"new StreamReader" function defaults to use a utf8 encoding, and also attempts
///to determine an automatic encoding by looking for "byteorder-marks" at the head
///of a text file. This function does not do this.
///
/// No CR-LF translation is done on input.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.IO.File.OpenText(path)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val open_in: path:string -> in_channel

/// Open the given file to read in binary-mode 
[<CompilerMessage("This construct is for ML compatibility. Consider using 'new System.IO.BinaryReader(System.IO.File.OpenRead(path))' and changing your type to be a BinaryReader instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val open_in_bin: path:string -> in_channel

/// Open the given file in the mode specified by the given flags
[<CompilerMessage("This construct is for ML compatibility. For advanced I/O consider using the System.IO namespace instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val open_in_gen: flags: open_flag list -> int -> path:string -> in_channel

/// Close the channel
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Close()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val close_in: channel:in_channel -> unit

/// Return the length of the input channel
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.BaseStream.Length' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val in_channel_length: channel:in_channel -> int

/// Attempt to input the given number of bytes from the channel, writing them into the
/// buffer at the given start position. Does not block if the bytes are not available.
///
/// The use of this function with a channel performing byte-to-character translation (e.g. one
/// created with open_in, open_in_utf8 or open_in_encoded, or one 
/// or built from a StreamReader or TextReader) is not recommended.
/// Instead, open the channel using open_in_bin or InChannel.of_BinaryReader.
///
/// If used with a StreamReader channel, i.e. one created using 
/// open_in, open_in_utf8 or open_in_encoded, or one 
/// or built from a StreamReader, this function reads bytes directly from the underlying
/// BaseStream. This may not be appropriate if any other input techniques are being
/// used on the channel.
///
/// If used with a TextReader channel (e.g. stdin), this function reads characters from the
/// stream and then fills some of the byte array with the decoding of these into 
/// bytes, where the decoding is performed using the System.Text.Encoding.Default encoding
///
/// Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Read(buffer,index,count)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input: channel:in_channel -> buffer:byte[] -> index:int -> count:int -> int

/// Attempt to input characters from a channel. Does not block if inpout is not available.
/// Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
///
/// No CRLF translation is done on input, even in text mode. That is, if an input file
/// has '\r\n' (CRLF) line terminators both characters will be seen in the input.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Read(buffer,index,count)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input_chars: channel:in_channel -> buffer:char[] -> index:int -> count:int -> int

/// Input a binary integer from a binary channel. Compatible with output_binary_int.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.ReadInt32()' on a BinaryReader instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input_binary_int: channel:in_channel -> int

/// Input a single byte. 
/// For text channels this only accepts characters with a UTF16 encoding that fits in a byte, e.g. ASCII.
/// Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. Consider using the 'Read()' method on a 'BinaryReader' instead, which returns -1 if no byte is available. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input_byte: channel:in_channel -> int

/// Input a single character. Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. Consider using the 'channel.Read()' method instead, which returns -1 if no character is available. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input_char: channel:in_channel -> char

/// Input a single line. Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. Consider using the 'channel.ReadLine()' method instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input_line: channel:in_channel -> string

#if FX_NO_BINARY_SERIALIZATION
#else
/// Input a single serialized value from a binary stream. Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. Consider deserializing using an object of type 'System.Runtime.Serialization.Formatters.Binary.BinaryFormatter' method instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val input_value: channel:in_channel -> 'T
#endif
/// Report the current position in the input channel
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.BaseStream.Position' property instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val pos_in: channel:in_channel -> int

/// Reads bytes from the channel. Blocks if the bytes are not available.
/// See 'input' for treatment of text channels.
/// Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val really_input: channel:in_channel -> buffer:byte[] -> index:int -> count:int -> unit

/// Reads bytes from the channel. Blocks if the bytes are not available.
/// For text channels this only accepts UTF-16 bytes with an encoding less than 256.
/// Raise End_of_file (= System.IO.EndOfStreamException) if end of file reached.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.BaseStream.Seek' method instead, or using a 'System.IO.BinaryReader' and related types for binary I/O. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val seek_in: channel:in_channel -> int -> unit

/// Set the binary mode to true or false. If the binary mode is changed from "true" to 
/// "false" then a StreamReader is created to read the binary stream. The StreamReader uses 
/// the default text encoding System.Text.Encoding.Default
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.IO.BinaryReader' and related types for binary I/O. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val set_binary_mode_in: channel:in_channel -> bool -> unit

[<Obsolete("For F# code unsafe_really_input is identical to really_input")>]
val unsafe_really_input: channel:in_channel -> byte[] -> int -> int -> unit

//--------------------------------------------------------------------------
//Output channels (out_channel). 

/// An pseudo-abstraction over binary and textual output channels.
/// OCaml-compatible channels conflate binary and text IO, and for this reasons their
/// use from F# is somewhat deprecated The direct use of System.IO StreamWriter, TextWriter and 
/// BinaryWriter objects is preferred, e.g. see System.IO.File.CreateText). Well-written OCaml code 
/// that simply opens either a channel in text or binary mode and then does text 
/// or binary I/O using the OCaml functions will work, though care must 
/// be taken with regard to end-of-line characters (see output_char below).
///
/// This library pretends that an out_channel is just a System.IO.TextWriter. Channels
/// created using open_out_bin maintain a private System.IO.BinaryWriter, which will be used whenever
/// do I/O using this channel. 
[<CompilerMessage("This construct is for ML compatibility. Consider using one of the types 'System.IO.TextWriter', 'System.IO.StreamWriter' or 'System.IO.BinaryWriter' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type out_channel  = System.IO.TextWriter

/// Open the given file to write in text-mode using the
/// System.Text.Encoding.Default encoding
///
/// See output_char for a description of CR-LF translation
/// done on output.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.IO.File.CreateText(path)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val open_out: path:string -> out_channel

/// Open the given file to write in binary-mode 
[<CompilerMessage("This construct is for ML compatibility. Consider using 'new System.IO.BinaryWriter(System.IO.File.Create(path))' and changing your type to be a BinaryWriter instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val open_out_bin: path:string -> out_channel

/// Open the given file to write in the mode according to the specified flags
[<CompilerMessage("This construct is for ML compatibility. For advanced I/O consider using the System.IO namespace. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val open_out_gen: open_flag list -> int -> path:string -> out_channel

/// Close the given output channel
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Close()' instead, or create the channel via a 'use' binding to ensure automatic cleanup. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val close_out: channel:out_channel -> unit

/// Return the length of the output channel. 
/// Raise an exception if not an app
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.BaseStream.Length' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val out_channel_length: channel:out_channel -> int

/// Write the given range of bytes to the output channel. 
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Write(buffer,index,count)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val output: channel:out_channel -> bytes:byte[] -> index:int -> count:int -> unit

/// Write the given integer to the output channel in binary format.
/// Only valid on binary channels.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Write(int)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val output_binary_int: channel:out_channel -> int:int -> unit

/// Write the given byte to the output channel. No CRLF translation is
/// performed.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Write(byte)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val output_byte: channel:out_channel -> byte:int -> unit

/// Write the given Unicode character to the output channel. 
///
/// If the output channel is a binary stream and the UTF-16 value of the Unicode character is greater
/// than 255 then ArgumentException is thrown.
///
/// No CRLF translation is done on output. That is, if the output character is
/// '\n' (LF) characters they will not be written as '\r\n' (CRLF) characters, regardless
/// of whether the underlying operating system or output stream uses CRLF as the default
/// line-feed character.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Write(char)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val output_char: channel:out_channel -> char -> unit

/// Write the given Unicode string to the output channel. See output_char for the treatment of
/// '\n' characters within the string.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Write(string)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val output_string: channel:out_channel -> string -> unit

#if FX_NO_BINARY_SERIALIZATION
#else
/// Serialize the given value to the output channel.
[<CompilerMessage("This construct is for ML compatibility. Consider serializing using an object of type 'System.Runtime.Serialization.Formatters.Binary.BinaryFormatter' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val output_value: channel:out_channel -> 'T -> unit
#endif
/// Return the current position in the output channel, measured from the
/// start of the channel. Not valid on all channels.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.BaseStream.Position' on a TextWriter or '.Position' on a Stream instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val pos_out: channel:out_channel -> int

/// Set the current position in the output channel, measured from the
/// start of the channel.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.BaseStream.Seek' on a TextReader or 'channel.Seek' on a Stream instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val seek_out: channel:out_channel -> int -> unit

/// Set the binary mode. If the binary mode is changed from "true" to 
/// "false" then a StreamWriter is created to write the binary stream. The StreamWriter uses 
/// the default text encoding System.Text.Encoding.Default.
[<CompilerMessage("This construct is for ML compatibility. For advanced I/O consider using the System.IO namespace. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val set_binary_mode_out: channel:out_channel -> bool -> unit

/// Flush all pending output on the channel to the physical
/// output device.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'channel.Flush()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val flush: channel:out_channel -> unit

//--------------------------------------------------------------------------
//Printing data to stdout/stderr


/// Print a character to the stderr stream
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Error.Write(char)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val prerr_char: char -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Error.WriteLine(string)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val prerr_endline: string -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Error.Write(double)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val prerr_float: float -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Error.Write(int)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val prerr_int: int -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Error.WriteLine()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val prerr_newline: unit -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Error.Write(string)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val prerr_string: string -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Write(char)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val print_char: char -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.WriteLine(string)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val print_endline: string -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Write(double)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val print_float: float -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Write(int)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val print_int: int -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.WriteLine()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val print_newline: unit -> unit

[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.Write(string)' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val print_string: string -> unit

//--------------------------------------------------------------------------
//Reading data from the console.


///Read a floating point number from the console.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.ReadLine() |> float' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val read_float: unit -> float

///Read an integer from the console.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.ReadLine() |> int' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val read_int: unit -> int

///Read a line from the console, without the end-of-line character.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'System.Console.ReadLine()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val read_line: unit -> string


[<CompilerMessage("This construct is for ML compatibility. Consider using Microsoft.FSharp.Core.Format<_,_,_,_> instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type ('a,'b,'c,'d) format4 = Microsoft.FSharp.Core.Format<'a,'b,'c,'d>

[<CompilerMessage("This construct is for ML compatibility. Consider using Microsoft.FSharp.Core.Format<_,_,_,_> instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type ('a,'b,'c) format = Microsoft.FSharp.Core.Format<'a,'b,'c,'c>

/// Throw an ArgumentException
[<CompilerMessage("This construct is for ML compatibility. Consider using 'invalidArg' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val invalid_arg: string -> 'T

/// Throw an <c>KeyNotFoundException</c> exception
[<CompilerMessage("This construct is for ML compatibility. Consider using 'raise (KeyNotFoundException(message))' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val not_found : unit -> 'T 

[<Obsolete("Use methods and properties on the corresponding type instead")>]
module OutChannel =
    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_BinaryWriter: System.IO.BinaryWriter -> out_channel
    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_StreamWriter: System.IO.StreamWriter -> out_channel
    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_TextWriter: System.IO.TextWriter -> out_channel

    /// Wrap a stream by creating a StreamWriter for the 
    /// stream and then wrapping is as an output channel.
    /// A text encoding must be given, e.g. System.Text.Encoding.UTF8
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_Stream: System.Text.Encoding -> System.IO.Stream -> out_channel

    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_Stream: out_channel -> System.IO.Stream
    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_TextWriter: out_channel -> System.IO.TextWriter
    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_StreamWriter: out_channel -> System.IO.StreamWriter
    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_BinaryWriter: out_channel -> System.IO.BinaryWriter

[<Obsolete("Use methods and properties on the corresponding type instead")>]
module InChannel =
    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_Stream: in_channel -> System.IO.Stream

    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_TextReader: in_channel -> System.IO.TextReader

    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_StreamReader: in_channel -> System.IO.StreamReader
    /// Access the underlying stream-based objects for the channel
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val to_BinaryReader: in_channel -> System.IO.BinaryReader

    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_BinaryReader: System.IO.BinaryReader -> in_channel
    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_StreamReader: System.IO.StreamReader -> in_channel
    ///Link .NET IO with the out_channel/in_channel model
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_TextReader: System.IO.TextReader -> in_channel
    /// Wrap a stream by creating a StreamReader for the 
    /// stream and then wrapping is as an input channel.
    /// A text encoding must be given, e.g. System.Text.Encoding.UTF8
    [<Obsolete("Use methods and properties on the corresponding type instead")>]
    val of_Stream:  System.Text.Encoding -> System.IO.Stream -> in_channel

//--------------------------------------------------------------------------
// OCaml path-lookup compatibility. All these constructs are in scope already
// for F# from Microsoft.FSharp.Operators and elsewhere. This module 
// is Microsoft.FSharp.Compatibility.OCaml.Pervasives.Pervasives and is only included 
// to resolve references in OCaml code written "compare" etc.
// We hide these away in the sub-module called "Pervasives" because we don't
// particularly want normal references such as "compare" to resolve to the 
// values in Pervasives.


[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Pervasives = 
    //--------------------------------------------------------------------------
    // Comparison based on F# term structure and/or calls to System.IComparable

    ///Structural less-than comparison
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (<): 'T -> 'T -> bool when 'T : comparison

    ///Structural less-than-or-equal comparison
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (<=): 'T -> 'T -> bool when 'T : comparison

    ///Structural inequality
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (<>): 'T -> 'T -> bool when 'T : equality

    ///Structural equality
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (=): 'T -> 'T -> bool when 'T : equality

    ///Structural greater-than
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (>): 'T -> 'T -> bool when 'T : comparison

    ///Structural greater-than-or-equal
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (>=): 'T -> 'T -> bool when 'T : comparison

    ///Structural comparison
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val compare: 'T -> 'T -> int when 'T : comparison

    ///Maximum based on structural comparison
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val max: 'T -> 'T -> 'T when 'T : comparison

    ///Minimum based on structural comparison
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val min: 'T -> 'T -> 'T when 'T : comparison

    ///The "hash" function is a structural hash function. It is 
    ///designed to return equal hash values for items that are 
    ///equal according to the polymorphic equality 
    ///function Pervasives.(=) (i.e. the standard "=" operator).
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val hash: 'T -> int

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (+)  : int -> int -> int

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (-)  : int -> int -> int

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val ( * ): int -> int -> int

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (/)  : int -> int -> int

    ///Absolute value of the given integer
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val abs : int -> int

    ///Dereference a mutable reference cell
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (!) : 'T ref -> 'T

    ///Assign to a mutable reference cell
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (:=): 'T ref -> 'T -> unit

    ///Create a mutable reference cell
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val ref : 'T -> 'T ref

    /// Throw a 'Failure' exception
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val failwith: string -> 'T

    /// Throw an exception
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val raise: exn -> 'T

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val fst: ('T1 * 'T2) -> 'T1

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val snd: ('T1 * 'T2) -> 'T2

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val ignore: 'T -> unit

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val not: bool -> bool

    ///Decrement a mutable reference cell containing an integer
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val decr: int ref -> unit

    ///Increment a mutable reference cell containing an integer
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val incr: int ref -> unit
    
#if FX_NO_EXIT
#else
    ///Exit the current hardware isolated process, if security settings permit,
    ///otherwise raise an exception. Calls System.Environment.Exit.
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val exit: int -> 'T   
#endif
    /// Concatenate two strings. The overlaoded operator '+' may also be used.
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (^): string -> string -> string

    /// Concatenate two lists.
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val (@): 'T list -> 'T list -> 'T list

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val float: int -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val acos: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val asin: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val atan: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val atan2: float -> float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val ceil: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val exp: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val floor: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val log: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val log10: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val sqrt: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val cos: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val cosh: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val sin: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val sinh: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val tan: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val tanh: float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val truncate: float -> int

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val ( **  ): float -> float -> float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val nan: float

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val infinity: float

    ///The type of pointers to mutable reference cells
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    type 'T ref = Microsoft.FSharp.Core.Ref<'T>

    ///The type of None/Some options
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    type 'T option = Microsoft.FSharp.Core.Option<'T>

    ///The type of simple immutable lists 
    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    type 'T list = Microsoft.FSharp.Collections.List<'T>

    [<CompilerMessage("This construct is for ML compatibility. Consider replacing uses of the functions accessible via Pervasives.* with their F# equivalents, usually by deleting 'Pervasives.'. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    type exn = System.Exception
