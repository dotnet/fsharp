// #Regression #Conformance #LexicalAnalysis 
#light

//<Expects id="FS0516" status="error">End of file in comment begun at or before here</Expects>

// Verify correct error if nested comments don't terminate

(*
1
(*
2
(*
3
(*
4
(*
5
(*
6
(*
7
(*
8
(*
9
(*
10
*)

Error: Comment levels
9, 8, 7, 6, 5, 4, 3, 2, 1 not closed
