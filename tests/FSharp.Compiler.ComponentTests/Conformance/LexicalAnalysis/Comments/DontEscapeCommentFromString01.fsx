// #Conformance #LexicalAnalysis 
#light

// Verify that '*)' doesn't end a comment if within a string literal

(*
let x = "..... *) ....."
ignore 1
*)

ignore 0
