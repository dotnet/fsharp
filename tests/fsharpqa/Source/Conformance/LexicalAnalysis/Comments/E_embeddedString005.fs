// #Regression #Conformance #LexicalAnalysis 
// string embedded in a comment: malformed string
// which yield to a compilation error
//<Expects status="error" id="FS0517" span="(9,1-9,3)>End of file in string embedded in comment begun at or before here</Expects>




(* Remove the "\\" " which is mandatory in OCaml regex. *)
exit 1
