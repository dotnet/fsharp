// #Conformance #LexicalAnalysis 
// string embedded in a comment: legitimate string containing the 
// block-comment-end token
//<Expects status="success"></Expects>

#light

(* Remove the "*)" which is mandatory in OCaml regex. *)
exit 0
