// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression for FSHARP1.0:6164


type 'a D =
  static member inline (+)(_:^b D, _:^b) : ^b when ^b : (static member (+) : ^b * ^b -> ^b) = failwith "Not implemented"
  static member inline (+)(_:^b, _:^b D) : ^b when ^b : (static member (+) : ^b * ^b -> ^b) = failwith "Not implemented"
 
let f (x:int D) = x + 1
