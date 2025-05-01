// #Regression #Conformance #TypeConstraints  #ReqNOMT 
// Regression test for FSHARP1.0:5091
// "CLR4 Tuple.Create<_>(x) leads to error 'type x is not compatible with type x'"
// "System.Tuple.Create<_>(1) should return a tuple type, not an int"

// This is supposed to:
// 1) Compile just fine when targeting 4.0
// 2) Return a tuple
let x1 : System.Tuple<int> = System.Tuple.Create<_>(1)
let x2 = System.Tuple.Create(1)

(if x1 = x2 then 0 else 1) |> exit
