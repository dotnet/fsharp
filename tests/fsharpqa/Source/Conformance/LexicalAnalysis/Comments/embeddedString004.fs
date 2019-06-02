// #Conformance #LexicalAnalysis 
// string embedded in a comment: legitimate escape sequence
// A backslash in in the string
//<Expects status="success"></Expects>

#light

(* Remove the "\\" which is mandatory in OCaml regex. *)
exit 0
