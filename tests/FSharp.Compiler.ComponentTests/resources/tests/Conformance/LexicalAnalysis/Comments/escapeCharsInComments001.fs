// #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:945
// comment lexing does not handle \" inside quoted strings
//<Expects status="success"></Expects>
#light

(* simple escape chars (escape-char in the grammar) *)

(** "\\" *)
let e1 = "\\"       (* "\\" *)    // "\\"

(** "\"" *)
let e2 = "\""       (* "\"" *)    // "\""

(** "\'" *)
let e3 = "\'"       (* "\'" *)    // "\'"

(** "\n" *)
let e4 = "\n"       (* "\n" *)    // "\n"

(** "\t" *)
let e5 = "\t"       (* "\t" *)    // "\t"

(** "\b" *)
let e6 = "\b"       (* "\b" *)    // "\b"

(** "\r" *)
let e7 = "\r"       (* "\r" *)    // "\r"

exit (if (e1.Length = 1 && e1.[0] = '\\') &&
         (e2.Length = 1 && e2.[0] = '\"') &&
         (e3.Length = 1 && e3.[0] = '\'') &&
         (e4.Length = 1 && e4.[0] = '\n') &&
         (e5.Length = 1 && e5.[0] = '\t') &&
         (e6.Length = 1 && e6.[0] = '\b') &&
         (e7.Length = 1 && e7.[0] = '\r') then 0 else 1)

