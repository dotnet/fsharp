// #Conformance #UnitsOfMeasure 
#light

let diff (h:float<_>) (f :_ -> float<_>) = fun x -> (f (x+h) - f (x-h)) / (2.0*h)
  
let integrate (f:float<_> -> float<_>) (a:float<_>) (b:float<_>) n =
  let h = (b-a) / (float n)
  let rec iter x i =
    if i=0 then 0.0<_> 
    else f x + iter (x+h) (i-1)
  h * (f a / 2.0 + iter (a+h) (n-1) + f b / 2.0)

let rec newton (f:float<_> -> float<_>) f' x xacc =
  let dx = f x / f' x
  let x' = x - dx
  if abs dx / x' < xacc
  then x' 
  else newton f f' x' xacc

// Non-regular datatype: a list of derivatives of a function
type derivs<[<Measure>] 'u, [<Measure>] 'v> =
  Nil
| Cons of (float<'u> -> float<'v>) * derivs<'u,'v/'u>

// Essential use of polymorphic recursion!
// Given a step h, function f, and integer n, compute a list of n derivatives
//   [f; f'; f''; ...]
// where f is the function itself, f' is the first derivative, f'' is the second, etc.

let rec makeDerivs<[<Measure>] 'u, [<Measure>] 'v> (h:float<'u>, f:float<'u> -> float<'v>, n:int) : derivs<'u,'v> = 
  if n=0 then Nil else Cons(f, makeDerivs(h, diff h f, n-1))
