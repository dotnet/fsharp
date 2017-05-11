// #Conformance #LexicalAnalysis 
#light

let x = 0 (*IF-OCAML*) + 1 (*ENDIF-OCAML*)

// should be = 0 since IF-OCAML is stripped out
exit x
