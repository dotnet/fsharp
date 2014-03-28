// #Regression #Libraries #Operators 
// Regression test for FSHARP1.0:4487
// Feature request: loosen Pow operator constraints

type T(x : float, y : float) =
  static let mutable m = false
  static member Pow (g: T, e: float) = m <- true
                                       g
  static member Check() = m

let t = new T(1.,2.)

let c = t ** 3.     // OK!

exit <| if T.Check() then 0 else 1

