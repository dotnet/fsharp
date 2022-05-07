// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest7 // Regression test for FSHARP1.0:4454
// "Stepping into sequence expression pops up a dialog trying to located an unknown file"
let mutable r = 0
let f () = [ if (r <- r + 1; true) then yield! failwith "" ]
printfn "res = %A" (try f () with Failure _ -> [r])


let testSimpleForEachSeqLoopWithOneStatement inp =
    for x in inp do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachSeqLoopWithTwoStatements inp =
    for x in inp do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let testSimpleForEachArrayLoopWithOneStatement (inp:int array) =
    for x in inp do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachArrayLoopWithTwoStatements (inp:int array) =
    for x in inp do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let testSimpleForEachListLoopWithOneStatement (inp:int list) =
    for x in inp do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachListLoopWithTwoStatements (inp:int list) =
    for x in inp do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntRangeLoopWithOneStatement (start, stop) =
    for x in start .. stop do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntRangeLoopWithTwoStatements (start, stop) =
    for x in start .. stop do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntRangeLoopDownWithOneStatement (start, stop) =
    for x in start .. -1 .. stop do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntRangeLoopDownWithTwoStatements (start, stop) =
    for x in start .. -1 .. stop do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntLoopWithOneStatement (start, stop) =
    for x = start to stop do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntLoopWithTwoStatements (start, stop) =
    for x = start to stop do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntLoopDownWithOneStatement (start, stop) =
    for x = stop downto start do
       System.Console.WriteLine("{0}", x)

let testSimpleForEachIntLoopDownWithTwoStatements (start, stop) =
    for x = stop downto start do
       System.Console.WriteLine("{0}", x)
       System.Console.WriteLine("{0}", x)

let ListExpressionSteppingTest7 () = 
    [ for x in 1..4 do
            printfn "hello"
            yield x ]
