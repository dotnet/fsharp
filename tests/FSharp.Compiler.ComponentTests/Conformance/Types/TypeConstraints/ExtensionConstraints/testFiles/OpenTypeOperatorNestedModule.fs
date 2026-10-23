// Regression: 'open type' inside a nested module correctly scopes the
// extension operator only to that module, and a sibling scope sees neither
// the operator nor the holder.

module OpenTypeOperatorNestedModule

[<AbstractClass; Sealed>]
type Ops =
    static member inline (+.?) (a: int, b: int) = a * b + 1

module Inner =
    open type Ops
    let useIt () : int = 3 +.? 4

let inner = Inner.useIt()
if inner <> 13 then failwith $"Expected 13, got {inner}"
