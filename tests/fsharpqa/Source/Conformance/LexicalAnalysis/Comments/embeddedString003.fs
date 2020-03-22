// #Conformance #LexicalAnalysis 
// string embedded in a comment: legitimate escape sequence
//<Expects status="success"></Expects>

#light

(* Remove the "\\ " which is mandatory in OCaml regex. *)
exit 0
