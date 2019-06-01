// #Conformance #LexicalAnalysis 
// string embedded in a comment: invalid escape sequence
// \s is not an escaped char, so it's the same as \\s
//<Expects status="success"></Expects>

#light

(* Remove the "\s" which is mandatory in OCaml regex. *)
exit 0
