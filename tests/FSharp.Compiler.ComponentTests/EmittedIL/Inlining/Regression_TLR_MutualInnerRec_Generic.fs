module Regression_TLR_MutualInnerRec_Generic

let outer<'T> (initial: 'T) (zero: 'T) =
    let rec a(n, v: 'T) =
        if n = 0 then v
        elif n % 2 = 0 then b(n - 1, v)
        else a(n - 1, v)
    and b(n, v: 'T) =
        if n = 0 then zero
        elif n % 2 = 0 then b(n - 1, v)
        else a(n - 1, v)
    a(1000, initial)

[<EntryPoint>]
let main _argv =
    if outer 1 0 = 1 then 0 else 1
