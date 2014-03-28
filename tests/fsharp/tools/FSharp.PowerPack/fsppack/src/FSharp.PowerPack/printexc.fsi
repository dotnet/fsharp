//==========================================================================
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.  The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================

/// Compatibility module to display data about exceptions.
[<CompilerMessage("This module is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Printexc

val print: mapping:('a -> 'b) -> 'a -> 'b
val to_string: exn -> string

