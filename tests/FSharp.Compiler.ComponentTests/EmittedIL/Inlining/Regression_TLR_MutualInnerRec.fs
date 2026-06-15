module Regression_TLR_MutualInnerRec

let fifth() =
    let rec fifthMethodFirstCallee(iterationCount, firstArg: int) =
        if iterationCount = 0 then 100
        else if iterationCount % 2 = 0 then fifthMethodSecondCallee(iterationCount - 1, firstArg)
        else fifthMethodFirstCallee(iterationCount - 1, firstArg)
    and fifthMethodSecondCallee(iterationCount, firstArg) =
        if iterationCount = 0 then 101
        else if iterationCount % 2 = 0 then fifthMethodSecondCallee(iterationCount - 1, firstArg)
        else fifthMethodFirstCallee(iterationCount - 1, firstArg)
    fifthMethodFirstCallee(1000000, 158_423)

[<EntryPoint>]
let main _argv = fifth ()
