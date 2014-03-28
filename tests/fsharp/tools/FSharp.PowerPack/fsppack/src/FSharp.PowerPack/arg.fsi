/// A simple command-line argument processor.
#if INTERNALIZED_POWER_PACK
module internal Internal.Utilities.Arg
#else
[<CompilerMessage("This module is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.Arg
#endif

/// The spec value describes the action of the argument,
/// and whether it expects a following parameter.
type spec = 
  | Clear of bool ref
  | Float of (float -> unit)
  | Int of (int -> unit)
  | Rest of (string -> unit)
  | Set of bool ref
  | String of (string -> unit)
  | Unit of (unit -> unit)

type argspec = (string * spec * string)

/// "parse specs f use" parses the arguments given by Sys.argv
/// according to the argument processing specifications "specs".
/// Arguments begin with "-". Non-arguments are passed to "f" in
/// order.  "use" is printed as part of the usage line if an error occurs.
///
/// Permitted arguments are specified using triples: (arg, action, help).
/// Actions are:
///   Unit(f): call f, no subseq. arg
///   Set(br): set ref to 'true', no subseq. arg.
///   Clear(br): set ref to 'false, no subseq. arg.
///   String(f): pass the subseq. arg to f
///   Int(f): pass the subseq. arg to f
///   Float(f): pass the subseq. arg to f
///   Rest(f): pass all subseq. args to f in order
#if FX_NO_COMMAND_LINE_ARGS
#else
val parse: argspec list -> (string -> unit) -> string -> unit
#endif
val parse_argv: int ref -> string array -> argspec list -> (string -> unit) -> string -> unit

/// "usage specs use" prints the help for each argument.
val usage: argspec list -> string -> unit
exception Bad of string
exception Help of string
