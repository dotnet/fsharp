// Regression: a single 'open type' holder declares two homograph overloads
// that differ only in one parameter type (int*int vs int*float). Overload
// resolution at the call site must pick the correct overload based on the
// *second* argument's type. Exercises multi-entry bucket + downstream
// ResolveOverloading disambiguation.

module OpenTypeOperatorOverloadByParam

[<AbstractClass; Sealed>]
type Ops =
    static member inline (+?) (a: int, b: int) = a + b + 1
    static member inline (+?) (a: int, b: float) = float a + b + 0.5

open type Ops

let r1 : int = 10 +? 20
if r1 <> 31 then failwith $"Expected 31, got {r1}"

let r2 : float = 10 +? 2.0
if r2 <> 12.5 then failwith $"Expected 12.5, got {r2}"
