// #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:946
// comment lexing does not handle "" and \ inside @" strings
//<Expects status="success"></Expects>
#light
(* @"\" *)

(** @"\\" *)
let e1 = @"\\"       (* @"\\" *)    // @"\\"

(** @"\" *)
let e2 = @"\"        (* @"\" *)     // @"\"

(** @"\'" *)
let e3 = @"\'"       (* @"\'" *)    // @"\'"

(** @"\n" *)
let e4 = @"\n"       (* @"\n" *)    // @"\n"

(** @"\t" *)
let e5 = @"\t"       (* @"\t" *)    // @"\t"

(** @"\b" *)
let e6 = @"\b"       (* @"\b" *)    // @"\b"

(** @"\r" *)
let e7 = @"\r"       (* @"\r" *)    // @"\r"

exit (if (e1.Length = 2 && e1.[0] = '\\') &&
         (e2.Length = 1 && e2.[0] = '\\') &&
         (e3.Length = 2 && e3.[0] = '\\') &&
         (e4.Length = 2 && e4.[0] = '\\') &&
         (e5.Length = 2 && e5.[0] = '\\') &&
         (e6.Length = 2 && e6.[0] = '\\') &&
         (e7.Length = 2 && e7.[0] = '\\') then 0 else 1)


