// Regression: an inline SRTP-using function dispatches to different 'open type'
// extension operator overloads based on argument type, where each overload is
// declared on a *separate* holder type. Exercises SelectExtensionMethInfosForTrait
// pulling candidates from the eOpenedTypeOperators bucket of multiple holders,
// with overload resolution selecting the applicable one per call site.

module OpenTypeOperatorSRTPDispatch

[<AbstractClass; Sealed>]
type IntOps =
    static member inline (+!) (a: int, b: int) = a + b + 1

[<AbstractClass; Sealed>]
type StringOps =
    static member inline (+!) (a: string, b: string) = a + b + "!"

open type IntOps
open type StringOps

let inline combine (a: ^T) (b: ^T) = a +! b

let r1 : int = combine 10 20
if r1 <> 31 then failwith $"Expected 31, got {r1}"

let r2 : string = combine "hi" "world"
if r2 <> "hiworld!" then failwith $"Expected 'hiworld!', got '{r2}'"

// Direct call sites as well
let r3 : int = 1 +! 2
if r3 <> 4 then failwith $"Expected 4, got {r3}"

let r4 : string = "a" +! "b"
if r4 <> "ab!" then failwith $"Expected 'ab!', got '{r4}'"
