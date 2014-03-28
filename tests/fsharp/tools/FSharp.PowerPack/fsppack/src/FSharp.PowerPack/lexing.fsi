//==========================================================================
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.  The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================

/// Lexing: ML-like lexing support
///
/// This file maintains rough compatibility for lexbuffers used by some ML
/// laxer generators.  The lexbuf carries an associated pair of positions.
/// Beware that only the "cnum" (absolute character number) field is automatically 
/// updated as each lexeme is matched.  Upon each successful match the prior end
/// position is transferred to be the start position and a new start position
/// is allocated with an updated pos_cnum field.
//[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
[<CompilerMessage("This module is for ML compatibility. Consider using the Microsoft.FSharp.Text.Lexing namespace directly", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Lexing
open Microsoft.FSharp.Text.Lexing

     
type position = Position

/// ASCII LexBuffers 
///
/// The type "lexbuf" is opaque, but has an internal position information field 
/// that can be updated by setting "lexbuf.EndPos", for example if you wish 
/// to update the other fields in that position data before or during 
/// lexing.  You will need to do this if you wish to maintain accurate 
/// line-count information.  If you do this and wish to maintain strict 
/// cross-compiling compatibility with OCamlLex and other tools you may need code 
/// to conditionally use lexbuf_set_curr_p when compiling F# code.
type lexbuf = LexBuffer<byte>

/// Remove all input, though don't discard the  except the current lexeme 
val flush_input: lexbuf -> unit

/// Fuel a lexer using the given in_channel.  The bytes are read using Pervasives.input.
/// If the in_channel is a textual channel the bytes are 
/// presented to the lexer by decoding the characters using System.Text.Encoding.ASCII.
val from_channel: System.IO.TextReader -> lexbuf

/// Fuel a lexer using the given TextReader or StreamReader.
/// The characters read are decoded to bytes using the given encoding (e.g. System.Text.Encoding.ASCII)
/// and the bytes presented to the lexer.  The encoding used to decode the characters
/// is associated with the expectations of the lexer (e.g. a lexer may be constructed to accept only 
/// ASCII or pseudo-UTF8 bytes) and will typically be different to 
/// the encoding used to decode the file.
val from_text_reader: System.Text.Encoding -> System.IO.TextReader -> lexbuf

/// Fuel a lexer using the given BinaryReader.  
val from_binary_reader: System.IO.BinaryReader -> lexbuf

/// Fuel a lexer from a string, converted to ascii using <c>System.Text.Encoding.ASCII.GetBytes</c>
val from_string: string -> lexbuf

/// Fuel a lexer from an array of bytes
val from_bytearray: byte[] -> lexbuf

/// Fuel a lexer from function that fills an array of bytes up to the given length, returning the
/// number of bytes filled.
val from_function: (byte[] -> int -> int) -> lexbuf

#if FX_NO_ASCII_ENCODING
#else
/// Return the matched string 
val lexeme: lexbuf -> string
#endif

/// Return the matched string interpreting the bytes using the given Unicode text encoding
val lexeme_utf8: lexbuf -> string

/// Return the bytes for the matched string 
val lexeme_bytes:  lexbuf -> byte array

/// Return a character from the matched string, innterpreting the bytes using an ASCII encoding
val lexeme_char: lexbuf -> int -> char

/// Return the positions stored in the lexbuf for the matched string 
val lexeme_start_p: lexbuf -> position
/// Return the positions stored in the lexbuf for the matched string 
val lexeme_end_p: lexbuf -> position


/// Return absolute positions into the entire stream of characters
val lexeme_start: lexbuf -> int
/// Return absolute positions into the entire stream of characters
val lexeme_end: lexbuf -> int

/// same as lexeme_end_p 
[<System.Obsolete("Get the EndPos property in the lexbuf directly, e.g. 'lexbuf.EndPos'")>]
val lexbuf_curr_p: lexbuf -> position 
[<System.Obsolete("Set the EndPos property in the lexbuf directly, e.g. 'lexbuf.EndPos <- pos'")>]
val lexbuf_set_curr_p: lexbuf -> position -> unit
[<System.Obsolete("Set the StartPos property in the lexbuf directly, e.g. 'lexbuf.StartPos <- pos'")>]
val lexbuf_set_start_p: lexbuf -> position -> unit
