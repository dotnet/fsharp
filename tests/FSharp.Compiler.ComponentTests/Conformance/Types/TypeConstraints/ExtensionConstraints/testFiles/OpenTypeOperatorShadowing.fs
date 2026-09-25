// Regression: a local 'let' binding of an operator shadows an 'open type'-provided
// extension operator at the point of definition. Exercises correct precedence
// between the eOpenedTypeOperators bucket and the standard unqualified-value lookup.

module OpenTypeOperatorShadowing

[<AbstractClass; Sealed>]
type Ops =
    static member inline (+%) (a: int, b: int) = a + b + 1

open type Ops

// Before shadowing: the extension wins.
let before : int = 10 +% 20
if before <> 31 then failwith $"Expected 31 before shadow, got {before}"

// Local shadow.
let inline (+%) (a: int) (b: int) : int = a - b

let after : int = 10 +% 20
if after <> -10 then failwith $"Expected -10 after shadow, got {after}"
