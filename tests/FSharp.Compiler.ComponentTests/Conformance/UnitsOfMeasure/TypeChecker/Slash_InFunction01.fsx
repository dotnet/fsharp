// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:6157
// Units of measure containing "/" get rejected in arguments to method invocation (works fine in a function)
[<Measure>] type m
[<Measure>] type s

let sqr<[<Measure>] 'u>(x:float<'u>) = x*x
let M<[<Measure>] 'u, [<Measure>] 'v>(x:float<'u>, y:float<'v>) = x * y

let _ = sqr<m/s> 2.0<m/s>
let _ = sqr<m/s> 2.0<m/s>
