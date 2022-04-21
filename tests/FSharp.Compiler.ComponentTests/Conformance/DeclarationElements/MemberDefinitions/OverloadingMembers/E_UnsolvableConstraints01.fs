// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression for FSHARP1.0:6164
//<Expects ID="FS0465" status="error" span="(9,21-9,22)">Type inference problem too complicated \(maximum iteration depth reached\)\. Consider adding further type annotations\.</Expects>

type 'a D =
  static member inline (+)(_:^b D, _:^b) : ^b when ^b : (static member (+) : ^b * ^b -> ^b) = failwith "Not implemented"
  static member inline (+)(_:^b, _:^b D) : ^b when ^b : (static member (+) : ^b * ^b -> ^b) = failwith "Not implemented"
 
let f (x:int D) = x + 1
