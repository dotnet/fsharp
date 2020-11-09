// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:6157
// Units of measure containing "/" get rejected in arguments to method invocation
[<Measure>] type m
[<Measure>] type s

type T() = member this.sqr<[<Measure>] 'u>(x:float<'u>) = x*x
           member this.M<[<Measure>] 'u, [<Measure>] 'v>(x:float<'u>, y:float<'v>) = x * y

let t = new T()
let x = t.sqr<m/s> 2.0<m/s>
let a = t.M<m/s,s/m>(2.0<m/s>, 2.0<s/m>)
  
