// #Regression #Conformance #SyntacticSugar
//<Expects status="error" id="FS0003"></Expects>

(* Incorrect usage of infix operator *)

let x = 1 (+) 2                // this is wrong. (+) 1 2 is the right thing to do.


