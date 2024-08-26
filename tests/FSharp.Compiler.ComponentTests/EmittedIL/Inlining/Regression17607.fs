open System
open System.Diagnostics

// 16 byte struct
[<Struct>]
type Point2D(x: double, y: double) =
    member _.X = x
    member _.Y = y

// Will create a tail il instruction and force a tail call. This is will become
// a fast tail call on unix x64 as the caller and callee have equal stack size
let fifth() =
    let rec fifthMethodFirstCallee(iterationCount, firstArg: Point2D, secondArg: Point2D, thirdArg: Point2D, fourthArg: Point2D, fifthArg: Point2D) =
        if firstArg.X <> 10.0 then -100
        else if firstArg.Y <> 20.0 then -101
        else if secondArg.X <> 30.0 then -102
        else if secondArg.Y <> 40.0 then -103
        else if thirdArg.X <> 10.0 then -104
        else if thirdArg.Y <> 20.0 then -105
        else if fourthArg.X <> 30.0 then -106
        else if fourthArg.Y <> 40.0 then -107
        else if fifthArg.X <> 10.0 then -108
        else if fifthArg.Y <> 20.0 then -109
        else if iterationCount = 0 then
            100
        else if iterationCount % 2 = 0 then
            fifthMethodSecondCallee(iterationCount - 1, firstArg, secondArg, thirdArg, fourthArg, fifthArg)
        else
            fifthMethodFirstCallee(iterationCount - 1, firstArg, secondArg, thirdArg, fourthArg, fifthArg)

    and fifthMethodSecondCallee(iterationCount, firstArg, secondArg, thirdArg, fourthArg, fifthArg) =
        if firstArg.X <> 10.0 then -150
        else if firstArg.Y <> 20.0 then -151
        else if secondArg.X <> 30.0 then -152
        else if secondArg.Y <> 40.0 then -153
        else if thirdArg.X <> 10.0 then -154
        else if thirdArg.Y <> 20.0 then -155
        else if fourthArg.X <> 30.0 then -156
        else if fourthArg.Y <> 40.0 then -157
        else if fifthArg.X <> 10.0 then -158
        else if fifthArg.Y <> 20.0 then -159
        else if iterationCount = 0 then
            101
        else if iterationCount % 2 = 0 then
            fifthMethodSecondCallee(iterationCount - 1, firstArg, secondArg, thirdArg, fourthArg, fifthArg)
        else
            fifthMethodFirstCallee(iterationCount - 1, firstArg, secondArg, thirdArg, fourthArg, fifthArg)

    let point = Point2D(10.0, 20.0)
    let secondPoint = Point2D(30.0, 40.0)

    let retVal = fifthMethodFirstCallee(1000000, point, secondPoint, point, secondPoint, point)

    if retVal <> 100 && retVal <> 101 then
        printfn "Method -- Failed, expected result: 100 or 101, calculated: %d" retVal
        -5
    else
        0

[<EntryPoint>]
let main argv =
    let startTime = Stopwatch.StartNew()
    for i in 0..100 do
        ignore (fifth ())
    let elapsedTime = startTime.Elapsed.TotalMilliseconds
    printfn "%fms" elapsedTime
    0