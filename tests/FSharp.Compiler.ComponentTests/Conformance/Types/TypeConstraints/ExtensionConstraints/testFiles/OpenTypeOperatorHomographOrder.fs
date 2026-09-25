// Regression: multiple homograph operators declared on a single 'open type' holder
// must all be discoverable via SRTP. Exercises the >=2 entries-per-bucket path of
// eOpenedTypeOperators (NameMultiMap<MethInfo>), where within-call source order
// is preserved by List.foldBack at insertion.

module OpenTypeOperatorHomographOrder

[<AbstractClass; Sealed>]
type Ops =
    // Three homograph (++!) overloads on int * int / float * float / string * string
    static member inline (++!) (a: int, b: int) = a + b + 1
    static member inline (++!) (a: float, b: float) = a + b + 1.0
    static member inline (++!) (a: string, b: string) = a + b + "!"

open type Ops

let r1 : int = 1 ++! 2
if r1 <> 4 then failwith $"Expected 4, got {r1}"

let r2 : float = 1.5 ++! 2.5
if r2 <> 5.0 then failwith $"Expected 5.0, got {r2}"

let r3 : string = "hi" ++! "world"
if r3 <> "hiworld!" then failwith $"Expected 'hiworld!', got '{r3}'"
