// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:6157 - Units of measure containing "/" get rejected in arguments to method invocation

[<Measure>] type m
[<Measure>] type s

let sqr<[<Measure>] 'u>(x:float<'u>) = x*x

type T() =
  member this.sqr<[<Measure>] 'u>(x:float<'u>) = x*x


let t = new T()
let a = sqr<m>(5.0<m>)
let b = sqr<m/s>(2.0<m/s>)
let c = sqr<m s^-1>(3.0<m s^-1>)
let d = t.sqr<m>(5.0<m>)
let e = t.sqr<m/s>(2.0<m/s>)
let f = t.sqr<m s^-1>(3.0<m s^-1>)
