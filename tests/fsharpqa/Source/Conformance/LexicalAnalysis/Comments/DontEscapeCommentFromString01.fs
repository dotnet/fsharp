// #Conformance #LexicalAnalysis 
#light

// Verify that '*)' doesn't end a comment if within a string literal

(*
let x = "..... *) ....."
exit 1
*)

exit 0
