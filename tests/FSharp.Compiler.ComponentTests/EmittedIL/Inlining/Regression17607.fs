open System
open System.Diagnostics


// Will create a tail il instruction and force a tail call. This is will become
// a fast tail call on unix x64 as the caller and callee have equal stack size
let fifth() =
    let rec fifthMethodFirstCallee(iterationCount, firstArg: int) =
        if iterationCount = 0 then
            100
        else if iterationCount % 2 = 0 then
            fifthMethodSecondCallee(iterationCount - 1, firstArg)
        else
            fifthMethodFirstCallee(iterationCount - 1, firstArg)

    and fifthMethodSecondCallee(iterationCount, firstArg) =
        if iterationCount = 0 then
            101
        else if iterationCount % 2 = 0 then
            fifthMethodSecondCallee(iterationCount - 1, firstArg)
        else
            fifthMethodFirstCallee(iterationCount - 1, firstArg)



    fifthMethodFirstCallee(1000000, 158_423)


[<EntryPoint>]
let main argv =
    fifth ()