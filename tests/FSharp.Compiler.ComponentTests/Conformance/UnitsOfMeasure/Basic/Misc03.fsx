// #Conformance #UnitsOfMeasure 
let recip1 x = 1.0/x
let recip2 (x:float<_>) = 1.0/x

let zero1 = 0.0
let zero2 = 0.0<_>

let halvelist0 xs = List.map ( (/) 2.0 : float -> float ) xs
let halvelist1 xs = List.map ( (/) 2.0 : float<_> -> float<_> ) xs
let halvelist2 (xs:float<_> list) = List.map ( (/) 2.0) xs
let halvelist3 xs = List.map ( (/) 2.0) xs

let pr x y = printf "%f %s" x y
let pr2 (x:float<_>) = printf "%f" x

// From thesis
let abs x = if x < 0.0<_> then 0.0<_> - x else x
let sqr (x:float<_>) = x*x
let cube x = x*sqr x
let powers x y z = sqr x + cube y + sqr z * cube z

// Now let's test some explicit types
let sqr2< [<Measure>]  'u>(x:float<'u>) = x*x
let cube2<[<Measure>] 'v>(x) = sqr2<'v> x * x
let rec reccube1(x) = if x < 0.0<_> then x else - reccube1(x)
