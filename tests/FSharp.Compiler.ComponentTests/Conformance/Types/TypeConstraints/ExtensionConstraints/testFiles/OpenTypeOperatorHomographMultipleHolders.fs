// Regression: homograph operators declared across TWO separate holder types
// each opened via 'open type'. Exercises cross-call bucket accumulation of
// eOpenedTypeOperators (NameMultiMap<MethInfo>) — each 'open type' is a
// separate AddStaticContentOfTypeToNameEnv call; both must contribute to the
// same bucket keyed by LogicalName.

module OpenTypeOperatorHomographMultipleHolders

[<AbstractClass; Sealed>]
type OpsA =
    static member inline (+!) (a: int, b: int) = a + b + 10

[<AbstractClass; Sealed>]
type OpsB =
    static member inline (+!) (a: float, b: float) = a + b + 100.0
    static member inline (+!) (a: string, b: string) = a + b + "_B"

open type OpsA
open type OpsB

let r1 : int = 1 +! 2
if r1 <> 13 then failwith $"Expected 13, got {r1}"

let r2 : float = 1.5 +! 2.5
if r2 <> 104.0 then failwith $"Expected 104.0, got {r2}"

let r3 : string = "hi" +! "world"
if r3 <> "hiworld_B" then failwith $"Expected 'hiworld_B', got '{r3}'"
