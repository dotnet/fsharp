//==========================================================================
// Buffer: StringBuilder operations for ML compatibility
//
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.  The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================

/// Imperative buffers for building strings, a shallow interface to <c>System.Text.StringBuilder</c>
[<CompilerMessage("This module is for ML compatibility. The Buffer module is a thin wrapper over the type System.Text.StringBuilder. Consider using that type directly. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Buffer
open Microsoft.FSharp.Compatibility.OCaml.Pervasives

#nowarn "62" // use of ocaml compat types from Pervasives

type t = System.Text.StringBuilder

/// Add second buffer to the first.
val add_buffer: t -> t -> unit

/// Add character to the buffer.
val add_char: t -> char -> unit

/// Add string to the buffer.
val add_string: t -> string -> unit

/// Given a string, start position and length add that substring to the buffer.
val add_substring: t -> string -> int -> int -> unit

/// Clears the buffer.
val clear: t -> unit

/// Gets the string built from the buffer.
val contents: t -> string

/// Create a buffer with suggested size.
val create: int -> t

/// Number of characters in the buffer.
val length: t -> int

val output_buffer: out_channel -> t -> unit

/// Clears the buffer (same as Buffer.clear).
val reset: t -> unit

#if FX_NO_ASCII_ENCODING
#else
/// Read the given number of bytes as ASCII and add the resulting string 
/// to the buffer.  Warning: this assumes an ASCII encoding for the I/O channel, i.e. it uses 
/// Pervasives.really_input and then use ascii_to_string to produce the string 
/// to add.  
val add_channel: t -> in_channel -> int -> unit
#endif
