module Regression_TLR_MutualInnerRec_CapturedEnv

let outer (threshold: int) (factor: int) =
    let rec a(n) =
        if n = 0 then threshold
        elif n % 2 = 0 then b(n - 1)
        else a(n - factor)
    and b(n) =
        if n = 0 then threshold + 1
        elif n % 2 = 0 then b(n - factor)
        else a(n - 1)
    a(100)

[<EntryPoint>]
let main _argv = outer 7 1
