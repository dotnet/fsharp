// Regression: a holder-type operator whose CLR emission is renamed via
// [<CompiledName("...")>] must still be resolvable through 'open type' at
// the F# source level. The LogicalName (op_PlusPlus etc.) is what feeds
// into eOpenedTypeOperators keying and IsLogicalOpName filtering, so
// CompiledName must not interfere with either.

module OpenTypeOperatorCompiledName

[<AbstractClass; Sealed>]
type Ops =
    [<CompiledName("CustomAdd")>]
    static member inline (++) (a: int, b: int) = a + b + 100

open type Ops

let r1 : int = 1 ++ 2
if r1 <> 103 then failwith $"Expected 103, got {r1}"

let inline bump (a: ^T) (b: ^T) = a ++ b
let r2 : int = bump 5 6
if r2 <> 111 then failwith $"Expected 111, got {r2}"
