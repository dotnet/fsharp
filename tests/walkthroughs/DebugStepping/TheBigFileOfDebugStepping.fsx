// Instructions: 
//    artifacts\bin\fsc\Debug\net472\fsc.exe --debug+ --tailcalls- --optimize- --langversion:preview tests\walkthroughs\DebugStepping\TheBigFileOfDebugStepping.fsx
//    devenv /debugexe TheBigFileOfDebugStepping.exe
//
//   Repeat the above with
//      - Just My Code on/off
//      - 32-bit or 64-bit
//      - .NET Core and .NET Framework
//      - Linux and Windows
//      - Different visual debuggers

open System
open System.Threading
open System.Threading.Tasks

type U2 = U2 of int * int


let (!) (r: 'T ref)  = r.Value
let (:=) (r: 'T ref) (v: 'T)  = r.Value <- v
let incr (r: int ref)  = r.Value <- r.Value + 1
let decr (r: int ref)  = r.Value <- r.Value - 1

let InnerRecursiveFunction (str: string) =
    let rec even n = if n = 0 then str else odd (n-1)
    and odd n = even (n-1)

    even 6

let rec TailcallRecursionTest1 n = 
    if n = 0 then 
        5
    else 
        TailcallRecursionTest1 (n-1) // check the 'n' updates correctly

let rec TailcallRecursionTest2 (U2(a,b)) = 
    if a = 0 then 
        5
    else 
        TailcallRecursionTest2 (U2(a-1,b-1)) // check the 'a' and 'b' update correctly

let AsyncExpressionSteppingTest1 () = 
    async { Console.WriteLine "hello"
            Console.WriteLine "stuck in the middle"
            Console.WriteLine "goodbye"}

let AsyncExpressionSteppingTest2 () = 
    let x = ref 0 
    async { while !x < 4 do 
                incr x
                Console.WriteLine "hello" }
                   

let AsyncExpressionSteppingTest3 () = 
    async { let x = ref 0 
            incr x
            let y = ref 0 
            incr y
            let z = !x + !y
            return z }

let AsyncExpressionSteppingTest4 () = 
    async { let x = ref 0 
            try 
                let y = ref 0 
                incr y
                let z = !x + !y
                return z 
            finally 
                incr x
                Console.WriteLine "done" }


let es = [3;4;5]
let AsyncExpressionSteppingTest5 () = 
    async { for x in es do
                Console.WriteLine "hello"
                Console.WriteLine "hello 2"
            for x in es do
                Console.WriteLine "goodbye"
                Console.WriteLine "goodbye 2" }


let f2 () = 
    async { let x = ref 0 
            incr x
            let y = ref 0 
            incr y
            let z = !x + !y
            return z }

let AsyncExpressionSteppingTest6b () = 
    async { let! x1 = f2()
            let! x2 = f2()
            let! x3 = f2()
            let y = ref 0 
            incr y
            let! x4 = f2()
            let z = x1 + !y + x4
            return z }

let AsyncBreakpoints1 () = 
    async { let! res1 = 
                f2()
            let! res2 = 
                match 1 with 
                | 1 -> f2()
                | _ -> f2()
            let! res3 = 
                match 1 with 
                | 1 -> 
                    let x = 
                        match 4 with 
                        | 2 -> f2()
                        | _ -> f2()
                    f2()
                | _ -> 
                    let x = 
                        match 4 with 
                        | 2 -> f2()
                        | _ -> f2()
                    f2()
            return () }


let ListExpressionSteppingTest1 () = 
    [ yield 1 ]

let ListExpressionSteppingTest2 () = 
    [ Console.WriteLine "hello"
      yield 1
      Console.WriteLine "goodbye"
      yield 2]


let ListExpressionSteppingTest3 () = 
    let x = ref 0 
    [ while !x < 4 do 
            incr x
            Console.WriteLine "hello"
            yield x ]


let ListExpressionSteppingTest4 () = 
    [ let x = ref 0 
      incr x
      let y = ref 0 
      incr y
      yield !x
      let z = !x + !y
      yield z ]


let ListExpressionSteppingTest5 () = 
    [ let x = ref 0 
      try 
            let y = ref 0 
            incr y
            yield !x
            let z = !x + !y
            yield z 
      finally 
            incr x
            Console.WriteLine "done" ]


let ListExpressionSteppingTest6 () = 
    [ for x in es do
            Console.WriteLine "hello"
            yield x 
      for x in es do
            Console.WriteLine "goodbye"
            yield x ]

let ListExpressionSteppingTest7 () = 
    [ for x in 1..4 do
            printfn "hello"
            yield x ]

let ListExpressionSteppingTest8 () = 
    [ for x in 1..4 do
        match x with 
        | 1 -> 
            printfn "hello"
            yield x 
        | 2 -> 
            printfn "hello"
            yield x 
        | _ -> 
            yield x 
        ]

let SeqExpressionSteppingTest1 () = 
    seq { yield 1 }


let SeqExpressionSteppingTest2 () = 
    seq { Console.WriteLine "hello"
          yield 1
          Console.WriteLine "goodbye"
          yield 2 }



let SeqExpressionSteppingTest3 () = 
    let x = ref 0 
    seq { while !x < 4 do 
                incr x
                Console.WriteLine "hello"
                yield x }


let SeqExpressionSteppingTest4 () = 
    seq { let x = ref 0 
          incr x
          let y = ref 0 
          incr y
          yield !x
          let z = !x + !y
          yield z }



let SeqExpressionSteppingTest5 () = 
    seq { let x = ref 0 
          try 
                let y = ref 0 
                incr y
                yield !x
                let z = !x + !y
                yield z 
          finally 
                incr x
                Console.WriteLine "done" }


let SeqExpressionSteppingTest6 () = 
    seq { for x in es do
                Console.WriteLine "hello"
                yield x 
          for x in es do
                Console.WriteLine "goodbye"
                yield x }



let SeqExpressionSteppingTest7() =
    // "Stepping into sequence expression pops up a dialog trying to located an unknown file"
    let r = ref 0
    let f () = [ if (incr r; true) then yield! failwith "" ]
    (try f () with Failure _ -> [!r])

let rec rwalk x = seq { yield x; yield! rwalk (x+1) }

let SeqExpressionTailCalls01() = rwalk 3 |> Seq.truncate 3 |> Seq.length

// Same as SeqExpressionTailCalls01.fs, but with MUTUALLY RECURSIVE PAIR OF SEQUENCES
let rec rwalk1 x = seq { yield x; yield! rwalk2 (x+1) }
and rwalk2 x = seq { yield x; yield! rwalk1 (x+1) }

let SeqExpressionTailCalls02() = rwalk2 3 |> Seq.truncate 3 |> Seq.length

SeqExpressionSteppingTest7()
SeqExpressionTailCalls01()
SeqExpressionTailCalls02()

let SteppingMatch01 (n) =   
        match n with                                    
        | Choice2Of2 _ ->
            Console.WriteLine("A")
        | Choice1Of2 _ ->            
            Console.WriteLine("B")
    
let SteppingMatch03 (n) =   
        match n with                                    
        | Choice1Of2 _ ->            
            Console.WriteLine("B")
        | Choice2Of2 _ ->
            Console.WriteLine("A")

let SteppingMatch03b (n) =   
        match n with                                    
        | Choice1Of3 _ ->            
            Console.WriteLine("A")
        | Choice2Of3 _ ->
            Console.WriteLine("B")
        | Choice3Of3 _ ->
            Console.WriteLine("C")


let SteppingMatch04 (n) =   
        match n with                                    
        | Choice2Of3 _ ->
            Console.WriteLine("B")
        | Choice3Of3 _ ->
            Console.WriteLine("C")
        | Choice1Of3 _ ->            
            Console.WriteLine("A")

let SteppingMatch05 (n) =   
        match n with                                    
        | Choice3Of3 _ ->
            Console.WriteLine("C")
        | Choice2Of3 _ ->
            Console.WriteLine("B")
        | Choice1Of3 _ ->            
            Console.WriteLine("A")

type Discr = CaseA | CaseB
let SteppingMatch06 (n) =
        match n with                                    
        | CaseB ->            
            Console.WriteLine("B")
        | CaseA->
            Console.WriteLine("A")

let SteppingMatch07 (n) =
        match n with                                    
        | CaseA->
            Console.WriteLine("A")
        | CaseB ->            
            Console.WriteLine("B")

let SteppingMatch08 (x) =
    let b = 
        match x with
        | 0 -> 2
        | _   -> 0

    System.Diagnostics.Debug.Write(b)
    System.Diagnostics.Debug.Write(b)

let SteppingMatch09 n =   
        match n with                                    
        | 1  ->
            Some(10)  // debug range should cover all of "Some(10)"
        | 2  ->            
            None
        | _ ->
                   Some(   22   )  // debug range should cover all of "Some(   22   )"

// Test case from https://github.com/Microsoft/visualfsharp/issues/105
let OuterWithGenericInner list =
  let GenericInner (list: 'T list) = 
     match list with 
     | [] -> 1 
     | _ -> 2

  GenericInner list

// Test case from https://github.com/Microsoft/visualfsharp/issues/105
let OuterWithNonGenericInner list =
  let NonGenericInner (list: int list) = 
     match list with 
     | [] -> 1 
     | _ -> 2

  NonGenericInner list

// Test case from https://github.com/Microsoft/visualfsharp/issues/105
let OuterWithNonGenericInnerWithCapture x list =
  let NonGenericInnerWithCapture (list: int list) = 
     match list with 
     | [] -> 1 
     | _ -> x

  NonGenericInnerWithCapture list


let TestFunction1() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4


let TestFunction10(p) =
    let (x,y) = p 
    x+y


let TestFunction11(p) =
    [p; p + p; p + p + p ]


let TestFunction12(p) =
    { contents = p+p }


let TestFunction13(x) =
    [x;x+x], [] 

              
let TestFunction14() =
    List.map (fun f -> f 2) [(fun x -> x + 1)] 

              
let TestFunction15(inp) =
    let x = inp+1
    [1;2;3] |> List.map (fun x -> x + 1)

              
type U = U of int * int

let TestFunction16(inp) =
    let x = U(inp,inp)
    x,x


type R = { x:int; y:int }

let TestFunction17(inp) =
    let x = {x=3;y=inp}
    x,x


let TestFunction18(inp) =
    System.Console.WriteLine("hello")


type C(x:int,y:int) = 
    member this.X = x
    member this.Y = y
    
let TestFunction19(inp) =
    let c1 = C(inp,inp)
    let c2 = C(inp,inp)
    Console.WriteLine $"c1 = {c1}, c2 = {c2}"

let TestFunction1_0() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction2() =
    let x = TestFunction1_0()
    Console.WriteLine "Hello";
    Console.WriteLine "World"    
    

type D(x:int,y:int) = 
    let z = x + y
    let f a = x + a
    let w = f z + z
    member this.X = x
    member this.Y = y
    
let TestFunction20(inp) =
    let d1 = D(inp,inp)
    let d2 = D(inp,inp)
    Console.WriteLine $"done d1 = {d1}, d2 = {d2}"


let TestFunction21(U2(a,b)) =
    Console.WriteLine $"a = {a}, b = {b}"

let TestFunction1_1() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction3() =
    try 
       let x = TestFunction1_1()
       Console.WriteLine "Hello";
    with _ -> 
       Console.WriteLine "World"        

let TestFunction1_2() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction3b() =
    try 
       let x = TestFunction1_2()
       failwith "hello"
    with Failure _ -> 
       Console.WriteLine "World"  

let TestFunction1_3() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction3c() =
    try 
       let x = TestFunction1_3()
       failwith "hello"
    with Failure msg when msg = "hello" -> 
       Console.WriteLine "World"        


let TestFunction1_4() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction4() =
    try 
       let x = TestFunction1_4()
       Console.WriteLine "Hello";
    finally 
       Console.WriteLine "World"

let TestFunction1_5() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction5() =
    let x = 
       let y = TestFunction1_5()
       Console.WriteLine "Hello";
       y + y
    x + x


let TestFunction1_6() =
    Console.WriteLine "Hello";
    Console.WriteLine "World";
    3+4
    
let TestFunction6() =
    let f() = 
       let y = TestFunction1_6()
       Console.WriteLine "Hello";
       y + y
    f() + f()


let TestFunction7() =
    let mutable r = 0
    while r < 3 do
       r <- r + 1

let TestFunction8(x) =
    if x > 3 then 
        x+4 
    else x-4


let TestFunction9(x) =
    match x with 
    | 3 -> "three"
    | 4 -> "four"
    | _ -> "five"

let TestFunction9b(x) =
    match x with 
    | [1;2] -> "three"
    | [3;4] -> "seven"
    | [a;b] when a+b = 4 -> "four"
    | _ -> "big"

let TestFunction22() =
    let x1 = if DateTime.Now.Day = 0 then DateTime.Now.Day else DateTime.MinValue.Day
    let x2 = if DateTime.Now.Day = 1 then DateTime.Now.Day else DateTime.MinValue.Day
    let x3 = if DateTime.Now.Day > 1 then DateTime.Now.Day else DateTime.MinValue.Day
    (x1,x2,x3)

let InnerFunctionDefinitionHadTwoBreakpoints (str: string) =
    let isVowel (ch: char) =
        let c = Char.ToUpper(ch)
        c = 'A' || c = 'E' || c = 'I' || c = 'O' || c = 'U'

    let firstChar = str.[0]

    if isVowel firstChar then
        str + "way"
    else
        str.[1..str.Length-1] + string(firstChar) + "ay"

let InnerRecursiveFunctionDefinitionHadTwoBreakpoints (str: string) =
    let firstChar = str.[0]

    let rec isVowel (ch: char) =
        let c = Char.ToUpper(ch)
        c = 'A' || c = 'E' || c = 'I' || c = 'O' || c = 'U'

    let firstChar = str.[0]

    if isVowel firstChar then
        str + "way"
    else
        str.[1..str.Length-1] + string(firstChar) + "ay"

let LocalValueShadowsArgument1 x =
    let x = // quick watch 1
        if isNull(x) then 
            printf "value is null" // quick watch 2
            2
        else 
            printf "value is not null" // breakpoint 1
            3
    ()

let LocalValueShadowsArgument2 x =
    let x = // quick watch 3
        if isNull(x) then 
            null // quick watch 4
        else 
            null // breakpoint 2
    ()

let TaskExpressionSteppingTest0 () = 
    task  { Console.WriteLine "hello"
            Console.WriteLine "stuck in the middle"
            Console.WriteLine "goodbye"}

let TaskExpressionSteppingTest1 () = 
    task  { Console.WriteLine "hello"
            Console.WriteLine "stuck in the middle"
            do! Task.Delay 100
            Console.WriteLine "goodbye"}

let TaskExpressionSteppingTest2 () = 
    let x = ref 0 
    task  { while !x < 4 do 
                incr x
                Console.WriteLine "hello" }

let TaskExpressionSteppingTest3 () = 
    task  { let x = ref 0 
            incr x
            let y = ref 0 
            incr y
            let z = !x + !y
            return z }

let TaskExpressionSteppingTest4 () = 
    task  { let x = ref 0 
            try 
                let y = ref 0 
                incr y
                let z = !x + !y
                return z 
            finally 
                incr x
                Console.WriteLine "done" }

let TaskExpressionSteppingTest5 () = 
    task  { for x in es do
                Console.WriteLine "hello"
                Console.WriteLine "hello 2"
            for x in es do
                Console.WriteLine "goodbye"
                Console.WriteLine "goodbye 2" }

let tf2 () = 
    task  { let x = ref 0 
            incr x
            let y = ref 0 
            incr y
            let z = !x + !y
            return z }

let TaskExpressionSteppingTest6b () = 
    task  { let! x1 = tf2()
            let!    x2 = tf2()
            let! x3 = tf2()
            let y = ref 0 
            incr y
            let! x4 = tf2()
            let z = x1 + !y + x4
            return z }

let TaskExpressionSteppingTest7a () = 
    task  { let x = ref 0 
            try 
                let y = ref 0 
                incr y
                let z = !x + !y
                return z 
            with exn ->
                incr x
                Console.WriteLine "done"
                return 0 }

let TaskExpressionSteppingTest7b () = 
    task  { let x = ref 0 
            try 
                let y = ref 0 
                incr y
                failwith "fail"
                let z = !x + !y
                return z 
            with exn ->
                incr x
                Console.WriteLine "done"
                return 0 }

let TaskBreakpoints1 () = 
    task  { let! res1 = 
                tf2()
            let! res2 = 
                match 1 with 
                | 1 -> tf2()
                | _ -> tf2()
            let! res3 = 
                match 1 with 
                | 1 -> 
                    let x = 
                        match 4 with 
                        | 2 -> tf2()
                        | _ -> tf2()
                    tf2()
                | _ -> 
                    let x = 
                        match 4 with 
                        | 2 -> tf2()
                        | _ -> tf2()
                    tf2()
            return () }

module InlinedCode =
    // NOTE: you can't place breakpoints in this method and hit them in either Debug and Release code
    let inline bodyRunner z body =
        let x = 1 + z
        printfn "running"
        body x
        body x

    let test() =
        let bodyWrapper =
            let x = 1 + System.Random().Next()
            bodyRunner 3 (fun n ->
                // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                printfn "line1, x = %d" x
                // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                printfn "line2, n = %d" n)

        let bodyWrapper2 =
            // TEST: check you can place breakpoint here and hit it in both Debug and Release code
            let x = 1 + System.Random().Next()
            bodyRunner 3 <| (fun n ->
                // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                printfn "line1, x = %d" x
                // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                printfn "line2, n = %d" n)
        ()

module Pipelined =
    let testListPipeline() =
        let data = [ 1 .. 5 ]
    
        // MANUAL TEST: check stepping through this looks ok
        let newData =
            data
            |> List.filter (fun x -> 
                  // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                  x > 3)
            |> List.map (fun x -> 
                  // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                  x * x)
        
        printfn "%A" newData

    let testArrayPipeline() =
            let data = [| 1 .. 5 |]
    
            let newData =
                data
                |> Array.filter (fun x -> 
                    // MANUAL TEST: check you can place breakpoint here and hit it in both Debug and Release code
                    x > 3)
                |> Array.map (fun x -> 
                    // MANUAL TEST: check you can place breakpoint here and hit it in both Debug code
                    // TODO: surprisingly no breakpoint hit here in release code
                    x * x)
        
            printfn "%A" newData

TailcallRecursionTest1 2
TailcallRecursionTest2 (U2(2,3))
SteppingMatch01 (Choice2Of2 3)
SteppingMatch01 (Choice1Of2 3)
SteppingMatch03 (Choice1Of2 3)
SteppingMatch03 (Choice2Of2 "3")
SteppingMatch03b (Choice1Of3 3)
SteppingMatch03b (Choice2Of3 "3")
SteppingMatch03b (Choice3Of3 5.0)
SteppingMatch04 (Choice2Of3 "3")
SteppingMatch04 (Choice3Of3 5.0)
SteppingMatch04 (Choice1Of3 3)
SteppingMatch05 (Choice3Of3 5.0)
SteppingMatch05 (Choice2Of3 "3")
SteppingMatch05 (Choice1Of3 3)
SteppingMatch06 CaseB
SteppingMatch06 CaseA
SteppingMatch07 CaseA
SteppingMatch07 CaseB
SteppingMatch08 0
SteppingMatch08 1
SteppingMatch09 1
SteppingMatch09 2
SteppingMatch09 3

OuterWithGenericInner [1;2;3;4;5;6]
OuterWithNonGenericInner [1;2;3;4;5;6]
OuterWithNonGenericInnerWithCapture 5 [1;2;3;4;5;6]
TestFunction1()
TestFunction2()
TestFunction3()
TestFunction4()
TestFunction5()
TestFunction6()
TestFunction7()
TestFunction8 3
TestFunction8 4
TestFunction9 3
TestFunction9 4
TestFunction9 5
TestFunction10 (5,4)
TestFunction11 5
TestFunction12 5
TestFunction13 5
TestFunction14 ()
TestFunction15 3
TestFunction16 3
TestFunction17 3
TestFunction18 3
TestFunction19 3
TestFunction21(U2(3,4))
TestFunction22()
TestFunction9b [1;2]
TestFunction9b [3;4]
TestFunction9b [3;1]
TestFunction9b [3]

AsyncExpressionSteppingTest1() |> Async.RunSynchronously
AsyncExpressionSteppingTest2() |> Async.RunSynchronously
AsyncExpressionSteppingTest3() |> Async.RunSynchronously
AsyncExpressionSteppingTest4() |> Async.RunSynchronously
AsyncExpressionSteppingTest5() |> Async.RunSynchronously
AsyncExpressionSteppingTest6b() |> Async.RunSynchronously
AsyncBreakpoints1() |> Async.RunSynchronously
ListExpressionSteppingTest1()
ListExpressionSteppingTest2()
ListExpressionSteppingTest3()
ListExpressionSteppingTest4()
ListExpressionSteppingTest5()
ListExpressionSteppingTest6()
ListExpressionSteppingTest7()
ListExpressionSteppingTest8()
SeqExpressionSteppingTest1()|> Seq.length
SeqExpressionSteppingTest2()|> Seq.length
SeqExpressionSteppingTest3()|> Seq.length
SeqExpressionSteppingTest4()|> Seq.length
SeqExpressionSteppingTest5()|> Seq.length
SeqExpressionSteppingTest6() |> Seq.length
InnerRecursiveFunction "cajcek" |> ignore
InnerFunctionDefinitionHadTwoBreakpoints "aaaa" |> ignore
InnerRecursiveFunctionDefinitionHadTwoBreakpoints "aaaa" |> ignore
LocalValueShadowsArgument1 "123"
LocalValueShadowsArgument2 "123"

module Task =
    let RunSynchronously (t: System.Threading.Tasks.Task<_>) = t.Wait(); t.Result

TaskExpressionSteppingTest0() |> Task.RunSynchronously
TaskExpressionSteppingTest1() |> Task.RunSynchronously
TaskExpressionSteppingTest2() |> Task.RunSynchronously
TaskExpressionSteppingTest3() |> Task.RunSynchronously
TaskExpressionSteppingTest4() |> Task.RunSynchronously
TaskExpressionSteppingTest5() |> Task.RunSynchronously
TaskExpressionSteppingTest6b() |> Task.RunSynchronously
TaskExpressionSteppingTest7a() |> Task.RunSynchronously
TaskExpressionSteppingTest7b() |> Task.RunSynchronously
TaskBreakpoints1() |> Task.RunSynchronously
InlinedCode.test()
Pipelined.testListPipeline()
Pipelined.testArrayPipeline()

module BooleanLogic =

    let testFunctionWithAnd x y =
        x && y

    let testFunctionWithOr x y =
        x || y

    let testFunctionWithMultipleAnd x y z =
        x && y && z

    let testFunctionWithMultipleOr x y z =
        x || y || z

    let testFunctionWithIfOfAnd x y =
        if x && y then
            1
        else
            2

    let testFunctionWithIfOfOr x y =
        if x || y then
            1
        else
            2

    testFunctionWithAnd true false
    testFunctionWithMultipleAnd true true false
    testFunctionWithMultipleOr false false true
    testFunctionWithOr true false
    testFunctionWithIfOfAnd true false
    testFunctionWithIfOfOr false false

// See https://github.com/dotnet/fsharp/issues/11977
module FalseSteppingBug =
    type U = 
       | A1 of int
       | A2 of int
       | A3 of int
       | A4 of int

    let testFunc f u = 
        match u with 
        | A1 n -> f n
        | A2 n when n > 4 -> f n // this was falsely hit
        | A2 n -> f n
        | A3 n -> f n
        | A4 n -> f n

    testFunc id (A3 4)



// https://github.com/dotnet/fsharp/pull/11981
module MissingFirstTry =
    let TestFunction3() =
        try 
           let x = 1+1
           System.Console.WriteLine "Hello";
        with _ -> 
           System.Console.WriteLine "World"      

    TestFunction3()


// https://github.com/dotnet/fsharp/issues/11979
//
// Check debug points exist for 'when'
module DebuggingSteppingForMatchWithWhen1 =

    let TestMatchWithWhen x y =
        match x with
        | [_] when y > 4 -> 5
        | [_] when y < 4 -> -5
        | _ -> 2


    TestMatchWithWhen [1] 3
    TestMatchWithWhen [1] 4
    TestMatchWithWhen [1] 5
    TestMatchWithWhen [1] 6


// https://github.com/dotnet/fsharp/issues/11979
//
// Check debug points exist for 'when'
module DebuggingSteppingForMatchWithWhenWithVariableBinding =

    let TestMatchWithWhen x  =
        match x with
        | [x] when x > 4 -> 5
        | [x] when x < 4 -> -5
        | _ -> 2


    TestMatchWithWhen [4]
    TestMatchWithWhen [5]
    TestMatchWithWhen [6]

// https://github.com/dotnet/fsharp/issues/11979
//
// Check debug points exist for 'when'
module DebuggingSteppingForMatchWithWhenWithUnionClauses=

    let TestMatchWithWhen x  =
        match x with
        | [_;x] 
        | [x] when x < 4 -> -5
        | _ -> 2


    TestMatchWithWhen [4;5]
    TestMatchWithWhen [5;4]
    TestMatchWithWhen [6]

module NestedScopesWithShadowing =

    let f2 (a, b) =
        let v1 = 1
        if a then
            let v2 = 1.4
            if b then
               let v1 = "3"
               let v2 = 5
               v1
            else
               let v1 = "3"
               let v2 = 5
               v1
        else
            let v2 = 1.4
            if b then
               let v1 = "3"
               let v2 = 5
               v1
            else
               let v1 = "3"
               let v2 = 5
               v1


    f2 (true, true)
    f2 (true, false)
    f2 (false, true)
    f2 (false, false)

module ForLoopRegularCode =
    let testSimpleForEachSeqLoopWithOneStatement inp =
        for x in inp do
            printfn $"hello, x = {x}"

    let testSimpleForEachSeqLoopWithTwoStatements inp =
        for x in inp do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachArrayLoopWithOneStatement (inp:int array) =
        for x in inp do
            printfn $"hello, x = {x}"

    let testSimpleForEachArrayLoopWithTwoStatements (inp:int array) =
        for x in inp do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachListLoopWithOneStatement (inp:int list) =
        for x in inp do
            printfn $"hello, x = {x}"

    let testSimpleForEachListLoopWithTwoStatements (inp:int list) =
        for x in inp do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachStringLoopWithOneStatement (inp:string) =
        for x in inp do
            printfn $"hello, x = {x}"

    let testSimpleForEachStringLoopWithTwoStatements (inp:string) =
        for x in inp do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachIntRangeLoopWithOneStatement (start, stop) =
        for x in start .. stop do
            printfn $"hello, x = {x}"

    let testSimpleForEachIntRangeLoopWithTwoStatements (start, stop) =
        for x in start .. stop do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachIntRangeLoopDownWithOneStatement (start, stop) =
        for x in stop .. -1 .. start do
            printfn $"hello, x = {x}"

    let testSimpleForEachIntRangeLoopDownWithTwoStatements (start, stop) =
        for x in stop .. -1 .. start do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachIntLoopWithOneStatement (start, stop) =
        for x = start to stop do
            printfn $"hello, x = {x}"

    let testSimpleForEachIntLoopWithTwoStatements (start, stop) =
        for x = start to stop do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    let testSimpleForEachIntLoopDownWithOneStatement (start, stop) =
        for x = stop downto start do
            printfn $"hello, x = {x}"

    let testSimpleForEachIntLoopDownWithTwoStatements (start, stop) =
        for x = stop downto start do
            printfn $"hello, x = {x}"
            printfn $"hello, x = {x}"

    testSimpleForEachSeqLoopWithOneStatement [1..3]
    testSimpleForEachSeqLoopWithTwoStatements [1..3]
    testSimpleForEachArrayLoopWithOneStatement [|1..3|]
    testSimpleForEachArrayLoopWithTwoStatements [|1..3|]
    testSimpleForEachListLoopWithOneStatement [1..3]
    testSimpleForEachListLoopWithTwoStatements [1..3]
    testSimpleForEachStringLoopWithOneStatement "123"
    testSimpleForEachStringLoopWithTwoStatements "123"
    testSimpleForEachIntRangeLoopWithOneStatement (1, 3)
    testSimpleForEachIntRangeLoopWithTwoStatements (1, 3)
    testSimpleForEachIntRangeLoopDownWithOneStatement (1, 3)
    testSimpleForEachIntRangeLoopDownWithTwoStatements (1, 3)
    testSimpleForEachIntLoopWithOneStatement (1, 3)
    testSimpleForEachIntLoopWithTwoStatements (1, 3)
    testSimpleForEachIntLoopDownWithOneStatement (1, 3)
    testSimpleForEachIntLoopDownWithTwoStatements (1, 3)

#if NETCOREAPP
    let testSimpleForEachReadOnlySpanLoopWithOneStatement (inp: System.ReadOnlySpan<char>) =
        [ for x in inp do
            printfn $"hello, x = {x}" ]

    let testSimpleForEachSpanLoopWithOneStatement (inp: System.Span<char>) =
        for x in inp do
            printfn $"hello, x = {x}"

    testSimpleForEachReadOnlySpanLoopWithOneStatement ("abc".AsSpan())
    testSimpleForEachSpanLoopWithOneStatement ([|'a'..'c'|].AsSpan())
#endif

module ForLoopInGeneratedList =
    let testSimpleListEachSeqLoopWithOneStatement inp =
        [ for x in inp do
             yield x+1 ]

    let testSimpleListEachSeqLoopWithTwoStatements inp =
        [ for x in inp do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachArrayLoopWithOneStatement (inp:int array) =
        [ for x in inp do
            yield x + 1 ]

    let testSimpleListEachArrayLoopWithTwoStatements (inp:int array) =
        [ for x in inp do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachListLoopWithOneStatement (inp:int list) =
        [ for x in inp do
            yield x + 1 ]

    let testSimpleListEachListLoopWithTwoStatements (inp:int list) =
        [ for x in inp do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachStringLoopWithOneStatement (inp:string) =
        [ for x in inp do
            yield x ]

    let testSimpleListEachStringLoopWithTwoStatements (inp:string) =
        [ for x in inp do
            printfn $"hello, x = {x}"
            yield x ]

    let testSimpleListEachIntRangeLoopWithOneStatement (start, stop) =
        [ for x in start .. stop do
            yield x + 1 ]

    let testSimpleListEachIntRangeLoopWithTwoStatements (start, stop) =
        [ for x in start .. stop do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachIntRangeLoopDownWithOneStatement (start, stop) =
        [ for x in stop .. -1 .. start do
            yield x + 1 ]

    let testSimpleListEachIntRangeLoopDownWithTwoStatements (start, stop) =
        [ for x in stop .. -1 .. stop do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachIntLoopWithOneStatement (start, stop) =
        [ for x = start to stop do
            yield x + 1 ]

    let testSimpleListEachIntLoopWithTwoStatements (start, stop) =
        [ for x = start to stop do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachIntLoopDownWithOneStatement (start, stop) =
        [ for x = stop downto start do
            yield x + 1 ]

    let testSimpleListEachIntLoopDownWithTwoStatements (start, stop) =
        [ for x = stop downto start do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    testSimpleListEachSeqLoopWithOneStatement [1..3]
    testSimpleListEachSeqLoopWithTwoStatements [1..3]
    testSimpleListEachArrayLoopWithOneStatement [|1..3|]
    testSimpleListEachArrayLoopWithTwoStatements [|1..3|]
    testSimpleListEachListLoopWithOneStatement [1..3]
    testSimpleListEachListLoopWithTwoStatements [1..3]
    testSimpleListEachStringLoopWithOneStatement "123"
    testSimpleListEachStringLoopWithTwoStatements "123"
    testSimpleListEachIntRangeLoopWithOneStatement (1, 3)
    testSimpleListEachIntRangeLoopWithTwoStatements (1, 3)
    testSimpleListEachIntRangeLoopDownWithOneStatement (1, 3)
    testSimpleListEachIntRangeLoopDownWithTwoStatements (1, 3)
    testSimpleListEachIntLoopWithOneStatement (1, 3)
    testSimpleListEachIntLoopWithTwoStatements (1, 3)
    testSimpleListEachIntLoopDownWithOneStatement (1, 3)
    testSimpleListEachIntLoopDownWithTwoStatements (1, 3)

#if NETCOREAPP
    let testSimpleListEachReadOnlySpanLoopWithOneStatement (inp: System.ReadOnlySpan<char>) =
        [ for x in inp do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    let testSimpleListEachSpanLoopWithOneStatement (inp: System.Span<char>) =
        [ for x in inp do
            printfn $"hello, x = {x}"
            yield x + 1 ]

    testSimpleListEachReadOnlySpanLoopWithOneStatement ("abc".AsSpan())
    testSimpleListEachSpanLoopWithOneStatement ([|'a'..'c'|].AsSpan())
#endif

module ForLoopTaskCode =
    let testSimpleTaskEachSeqLoopWithOneStatement inp =
        task {
            for x in inp do
                printfn $"hello, x = {x}" 
            return 1
        }

    let testSimpleTaskEachSeqLoopWithTwoStatements inp =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }


    let testSimpleTaskEachArrayLoopWithOneStatement (inp:int array) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachArrayLoopWithTwoStatements (inp:int array) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachListLoopWithOneStatement (inp:int list) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachListLoopWithTwoStatements (inp:int list) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachStringLoopWithOneStatement (inp:string) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachStringLoopWithTwoStatements (inp:string) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntRangeLoopWithOneStatement (start, stop) =
        task {
            for x in start .. stop do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntRangeLoopWithTwoStatements (start, stop) =
        task {
            for x in start .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntRangeLoopDownWithOneStatement (start, stop) =
        task {
            for x in stop .. -1 .. start do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntRangeLoopDownWithTwoStatements (start, stop) =
        task {
            for x in stop .. -1 .. start do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntLoopWithOneStatement (start, stop) =
        task {
            for x = start to stop do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntLoopWithTwoStatements (start, stop) =
        task {
            for x = start to stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntLoopDownWithOneStatement (start, stop) =
        task {
            for x = stop downto start do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachIntLoopDownWithTwoStatements (start, stop) =
        task {
            for x = stop downto start do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    testSimpleTaskEachSeqLoopWithOneStatement [1..3] |> fun t -> t.Result
    testSimpleTaskEachSeqLoopWithTwoStatements [1..3] |> fun t -> t.Result
    testSimpleTaskEachArrayLoopWithOneStatement [|1..3|] |> fun t -> t.Result
    testSimpleTaskEachArrayLoopWithTwoStatements [|1..3|] |> fun t -> t.Result
    testSimpleTaskEachListLoopWithOneStatement [1..3] |> fun t -> t.Result
    testSimpleTaskEachListLoopWithTwoStatements [1..3] |> fun t -> t.Result
    testSimpleTaskEachStringLoopWithOneStatement "123" |> fun t -> t.Result
    testSimpleTaskEachStringLoopWithTwoStatements "123" |> fun t -> t.Result
    testSimpleTaskEachIntRangeLoopWithOneStatement (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntRangeLoopWithTwoStatements (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntRangeLoopDownWithOneStatement (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntRangeLoopDownWithTwoStatements (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntLoopWithOneStatement (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntLoopWithTwoStatements (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntLoopDownWithOneStatement (1, 3) |> fun t -> t.Result
    testSimpleTaskEachIntLoopDownWithTwoStatements (1, 3) |> fun t -> t.Result

#if NETCOREAPP
    let testSimpleTaskEachReadOnlySpanLoopWithOneStatement (inp: System.ReadOnlySpan<char>) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleTaskEachSpanLoopWithOneStatement (inp: System.Span<char>) =
        task {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    testSimpleTaskEachReadOnlySpanLoopWithOneStatement ("abc".AsSpan()) |> fun t -> t.Result
    testSimpleTaskEachSpanLoopWithOneStatement ([|'a'..'c'|].AsSpan()) |> fun t -> t.Result
#endif


module ForLoopSeqCode =
    let testSimpleSeqEachSeqLoopWithOneStatement inp =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachSeqLoopWithTwoStatements inp =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachArrayLoopWithOneStatement (inp:int array) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachArrayLoopWithTwoStatements (inp:int array) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachListLoopWithOneStatement (inp:int list) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachListLoopWithTwoStatements (inp:int list) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachStringLoopWithOneStatement (inp:string) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachStringLoopWithTwoStatements (inp:string) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntRangeLoopWithOneStatement (start, stop) =
        seq {
            for x in start .. stop do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntRangeLoopWithTwoStatements (start, stop) =
        seq {
            for x in start .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntRangeLoopDownWithOneStatement (start, stop) =
        seq {
            for x in stop .. -1 .. start do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntRangeLoopDownWithTwoStatements (start, stop) =
        seq {
            for x in stop .. -1 .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntLoopWithOneStatement (start, stop) =
        seq {
            for x = start to stop do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntLoopWithTwoStatements (start, stop) =
        seq {
            for x = start to stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntLoopDownWithOneStatement (start, stop) =
        seq {
            for x = stop downto start do
                printfn $"hello, x = {x}"
            yield 1
        }

    let testSimpleSeqEachIntLoopDownWithTwoStatements (start, stop) =
        seq {
            for x = stop downto start do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            yield 1
        }

    testSimpleSeqEachSeqLoopWithOneStatement [1..3] |> Seq.toList
    testSimpleSeqEachSeqLoopWithTwoStatements [1..3] |> Seq.toList
    testSimpleSeqEachArrayLoopWithOneStatement [|1..3|] |> Seq.toList
    testSimpleSeqEachArrayLoopWithTwoStatements [|1..3|] |> Seq.toList
    testSimpleSeqEachListLoopWithOneStatement [1..3] |> Seq.toList
    testSimpleSeqEachListLoopWithTwoStatements [1..3] |> Seq.toList
    testSimpleSeqEachStringLoopWithOneStatement "123" |> Seq.toList
    testSimpleSeqEachStringLoopWithTwoStatements "123" |> Seq.toList
    testSimpleSeqEachIntRangeLoopWithOneStatement (1, 3) |> Seq.toList
    testSimpleSeqEachIntRangeLoopWithTwoStatements (1, 3) |> Seq.toList
    testSimpleSeqEachIntRangeLoopDownWithOneStatement (1, 3) |> Seq.toList
    testSimpleSeqEachIntRangeLoopDownWithTwoStatements (1, 3) |> Seq.toList
    testSimpleSeqEachIntLoopWithOneStatement (1, 3) |> Seq.toList
    testSimpleSeqEachIntLoopWithTwoStatements (1, 3) |> Seq.toList
    testSimpleSeqEachIntLoopDownWithOneStatement (1, 3) |> Seq.toList
    testSimpleSeqEachIntLoopDownWithTwoStatements (1, 3) |> Seq.toList

#if NETCOREAPP
    let testSimpleSeqEachReadOnlySpanLoopWithOneStatement (inp: System.ReadOnlySpan<char>) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
                yield 1
        }

    let testSimpleSeqEachSpanLoopWithOneStatement (inp: System.Span<char>) =
        seq {
            for x in inp do
                printfn $"hello, x = {x}"
                yield 1
        }

    testSimpleSeqEachReadOnlySpanLoopWithOneStatement ("abc".AsSpan())
    testSimpleSeqEachSpanLoopWithOneStatement ([|'a'..'c'|].AsSpan())
#endif

module ForLoopAsyncCode =
    let testSimpleAsyncEachSeqLoopWithOneStatement inp =
        async {
            for x in inp do
                printfn $"hello, x = {x}" 
            return 1
        }

    let testSimpleAsyncEachSeqLoopWithTwoStatements inp =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }


    let testSimpleAsyncEachArrayLoopWithOneStatement (inp:int array) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachArrayLoopWithTwoStatements (inp:int array) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachListLoopWithOneStatement (inp:int list) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachListLoopWithTwoStatements (inp:int list) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachStringLoopWithOneStatement (inp:string) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachStringLoopWithTwoStatements (inp:string) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntRangeLoopWithOneStatement (start, stop) =
        async {
            for x in start .. stop do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntRangeLoopWithTwoStatements (start, stop) =
        async {
            for x in start .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntRangeLoopDownWithOneStatement (start, stop) =
        async {
            for x in stop .. -1 .. start do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntRangeLoopDownWithTwoStatements (start, stop) =
        async {
            for x in stop .. -1 .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntLoopWithOneStatement (start, stop) =
        async {
            for x = start to stop do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntLoopWithTwoStatements (start, stop) =
        async {
            for x = start to stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntLoopDownWithOneStatement (start, stop) =
        async {
            for x = stop downto start do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachIntLoopDownWithTwoStatements (start, stop) =
        async {
            for x = stop downto start do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    testSimpleAsyncEachSeqLoopWithOneStatement [1..3] |> Async.RunSynchronously
    testSimpleAsyncEachSeqLoopWithTwoStatements [1..3] |> Async.RunSynchronously
    testSimpleAsyncEachArrayLoopWithOneStatement [|1..3|] |> Async.RunSynchronously
    testSimpleAsyncEachArrayLoopWithTwoStatements [|1..3|] |> Async.RunSynchronously
    testSimpleAsyncEachListLoopWithOneStatement [1..3] |> Async.RunSynchronously
    testSimpleAsyncEachListLoopWithTwoStatements [1..3] |> Async.RunSynchronously
    testSimpleAsyncEachStringLoopWithOneStatement "123" |> Async.RunSynchronously
    testSimpleAsyncEachStringLoopWithTwoStatements "123" |> Async.RunSynchronously
    testSimpleAsyncEachIntRangeLoopWithOneStatement (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntRangeLoopWithTwoStatements (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntRangeLoopDownWithOneStatement (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntRangeLoopDownWithTwoStatements (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntLoopWithOneStatement (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntLoopWithTwoStatements (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntLoopDownWithOneStatement (1, 3) |> Async.RunSynchronously
    testSimpleAsyncEachIntLoopDownWithTwoStatements (1, 3) |> Async.RunSynchronously

#if NETCOREAPP
    let testSimpleAsyncEachReadOnlySpanLoopWithOneStatement (inp: System.ReadOnlySpan<char>) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleAsyncEachSpanLoopWithOneStatement (inp: System.Span<char>) =
        async {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    testSimpleAsyncEachReadOnlySpanLoopWithOneStatement ("abc".AsSpan()) |> Async.RunSynchronously
    testSimpleAsyncEachSpanLoopWithOneStatement ([|'a'..'c'|].AsSpan()) |> Async.RunSynchronously
#endif

//-------------------------------------------------------------


[<RequireQualifiedAccess; Struct>] 
type ValueOrCancelled<'TResult> =
    | Value of result: 'TResult
    | Cancelled of ``exception``: OperationCanceledException

/// Represents a cancellable computation with explicit representation of a cancelled result.
///
/// A cancellable computation is passed may be cancelled via a CancellationToken, which is propagated implicitly.  
/// If cancellation occurs, it is propagated as data rather than by raising an OperationCanceledException.  
//[<Struct>]
type Cancellable<'TResult> = (*Cancellable of*) (CancellationToken -> ValueOrCancelled<'TResult>)

module Cancellable = 

    /// Run a cancellable computation using the given cancellation token
    let run (ct: CancellationToken) (comp : Cancellable<_>) = 
        if ct.IsCancellationRequested then 
            ValueOrCancelled.Cancelled (OperationCanceledException ct) 
        else
            comp ct 

(*
    /// Bind the result of a cancellable computation
    let inline bind f comp1 : Cancellable<_> = 
       (fun ct -> 
            match run ct comp1 with 
            | ValueOrCancelled.Value v1 -> run ct (f v1) 
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)
*)

    /// Return a simple value as the result of a cancellable computation
    let inline ret x : Cancellable<_> = (fun _ -> ValueOrCancelled.Value x)

    let one = ret 1

    /// Run the computation in a mode where it may not be cancelled. The computation never results in a 
    /// ValueOrCancelled.Cancelled.
    let runWithoutCancellation comp = 
        let res = run CancellationToken.None comp 
        match res with 
        | ValueOrCancelled.Cancelled _ -> failwith "unexpected cancellation" 
        | ValueOrCancelled.Value r -> r

    /// Bind the cancellation token associated with the computation
    let token () : Cancellable<_> =  (fun ct -> ValueOrCancelled.Value ct)

    /// Represents a canceled computation
    let canceled() : Cancellable<_> = (fun ct -> ValueOrCancelled.Cancelled (OperationCanceledException ct))

(*
    /// Catch exceptions in a computation
    let inline catch e : Cancellable<_> = 
        (fun ct -> 
            try 
                match e ct with 
                | ValueOrCancelled.Value r -> ValueOrCancelled.Value (Choice1Of2 r) 
                | ValueOrCancelled.Cancelled e -> ValueOrCancelled.Cancelled e 
            with err -> 
                ValueOrCancelled.Value (Choice2Of2 err))

    /// Implement try/finally for a cancellable computation
    let inline tryFinally e compensation : Cancellable<_> =
        catch e |> bind (fun res ->
            compensation()
            match res with Choice1Of2 r -> ret r | Choice2Of2 err -> raise err)

    /// Implement try/with for a cancellable computation
    let inline tryWith e handler : Cancellable<_> = 
        catch e |> bind (fun res ->
            match res with Choice1Of2 r -> ret r | Choice2Of2 err -> handler err)
*)

type CancellableBuilder() = 

    member inline _.Delay ([<InlineIfLambda>] f: unit -> Cancellable<'T>) : Cancellable<'T> = 
        (fun ct ->
            let g = f()
            g ct)

    member inline _.BindReturn([<InlineIfLambda>] comp: Cancellable<'T>, [<InlineIfLambda>] f: ('T -> 'U)) : Cancellable<'U> = 
        (fun ct -> 
            if ct.IsCancellationRequested then 
                ValueOrCancelled.Cancelled (OperationCanceledException ct) 
            else
                match comp ct with
                | ValueOrCancelled.Value res -> ValueOrCancelled.Value (f res)
                | ValueOrCancelled.Cancelled err -> ValueOrCancelled.Cancelled err)

    member inline _.Bind([<InlineIfLambda>] comp: Cancellable<'T>, [<InlineIfLambda>] f: ('T -> Cancellable<'U>)) : Cancellable<'U> =
       (fun ct -> 
            if ct.IsCancellationRequested then 
                ValueOrCancelled.Cancelled (OperationCanceledException ct) 
            else
                match comp ct with
                | ValueOrCancelled.Value res ->
                    let comp2 = f res
                    comp2 ct
                | ValueOrCancelled.Cancelled err -> ValueOrCancelled.Cancelled err)

    member inline _.Return v =
         (fun _ -> ValueOrCancelled.Value v)

    member inline _.ReturnFrom ([<InlineIfLambda>] v: Cancellable<'T>) : Cancellable<'T> =
         (fun ct -> v ct)

    member inline _.Combine([<InlineIfLambda>] comp1: Cancellable<unit>, [<InlineIfLambda>] comp2: Cancellable<'T>) : Cancellable<'T> = 
        (fun ct -> 
            if ct.IsCancellationRequested then 
                ValueOrCancelled.Cancelled (OperationCanceledException ct) 
            else
                match comp1 ct with
                | ValueOrCancelled.Value () -> comp2 ct
                | ValueOrCancelled.Cancelled err -> ValueOrCancelled.Cancelled err)

    member inline _.For(sequence: seq<'T>, [<InlineIfLambda>] (f: 'T -> Cancellable<_>)) : Cancellable<unit> = 
        (fun ct ->
            use ie = sequence.GetEnumerator()
            let mutable fin = false
            let mutable res = Unchecked.defaultof<_>
            while not fin do
                if ie.MoveNext() then
                    let step = f ie.Current
                    match step ct with
                    | ValueOrCancelled.Value () -> ()
                    | ValueOrCancelled.Cancelled err ->
                        fin <- true
                        res <- ValueOrCancelled.Cancelled err
                else
                    fin <- true
                    res <- ValueOrCancelled.Value ()
            res)

(*
    member (* inline *) _.TryWith(e, handler) = Cancellable.tryWith e handler

    member (* inline *) _.Using(resource, e) = Cancellable.tryFinally (e resource) (fun () -> (resource :> IDisposable).Dispose())

    member (* inline *) _.TryFinally(e, compensation) =  Cancellable.tryFinally e compensation

*)
    member inline _.Zero() : Cancellable<unit> =
         (fun _ -> ValueOrCancelled.Value ())

[<AutoOpen>]
module CancellableAutoOpens =
    let cancellable = CancellableBuilder()

// 1. Internal delayed type must be a function to flatten the code with InlineIfLambda
// 1. Bind and BindReturn methods must be inlined to flatten the code

module CancellableBasicTests =
    let test0() = 
        cancellable.Delay (fun () ->
            cancellable.Return(0xABBA))

    test0() |> Cancellable.runWithoutCancellation

    let test1() = 
        cancellable {
             return 12345
        }

    test1() |> Cancellable.runWithoutCancellation

    let test2() = 
        cancellable {
             printfn "hello"
             return 12345
        }

    test2() |> Cancellable.runWithoutCancellation

    let testLetBinding() = 
        cancellable {
             let x = 1
             return 12345 + x
        }

    testLetBinding() |> Cancellable.runWithoutCancellation

    let testLetFunctionBinding() = 
        cancellable {
             let f () = 1
             return 12345 + f()
        }

    testLetFunctionBinding() |> Cancellable.runWithoutCancellation

    let testBindReturn1() = 
        cancellable {
             let! one = Cancellable.one
             return 12345 + one
        }

    testBindReturn1() |> Cancellable.runWithoutCancellation

    let testBindReturn2() = 
        cancellable {
             System.Console.WriteLine()
             let! one = Cancellable.one
             return 12345 + one
        }

    testBindReturn2() |> Cancellable.runWithoutCancellation

    let testBindReturn3() = 
        cancellable {
             let! one = Cancellable.one
             System.Console.WriteLine()
             return 12345 + one
        }

    testBindReturn3() |> Cancellable.runWithoutCancellation

    let testBind1() = 
        cancellable {
             let! one = Cancellable.one
             let! one = Cancellable.one
             System.Console.WriteLine()
             return 12345 + one + one
        }

    testBind1() |> Cancellable.runWithoutCancellation


module ForLoopCancellableCode =
    let testSimpleCancellableEachSeqLoopWithOneStatement inp =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}" 
            return 1
        }

    let testSimpleCancellableEachSeqLoopWithTwoStatements inp =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }


    let testSimpleCancellableEachArrayLoopWithOneStatement (inp:int array) =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachArrayLoopWithTwoStatements (inp:int array) =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachListLoopWithOneStatement (inp:int list) =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachListLoopWithTwoStatements (inp:int list) =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachStringLoopWithOneStatement (inp:string) =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachStringLoopWithTwoStatements (inp:string) =
        cancellable {
            for x in inp do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntRangeLoopWithOneStatement (start, stop) =
        cancellable {
            for x in start .. stop do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntRangeLoopWithTwoStatements (start, stop) =
        cancellable {
            for x in start .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntRangeLoopDownWithOneStatement (start, stop) =
        cancellable {
            for x in stop .. -1 .. start do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntRangeLoopDownWithTwoStatements (start, stop) =
        cancellable {
            for x in stop .. -1 .. stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntLoopWithOneStatement (start, stop) =
        cancellable {
            for x = start to stop do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntLoopWithTwoStatements (start, stop) =
        cancellable {
            for x = start to stop do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntLoopDownWithOneStatement (start, stop) =
        cancellable {
            for x = stop downto start do
                printfn $"hello, x = {x}"
            return 1
        }

    let testSimpleCancellableEachIntLoopDownWithTwoStatements (start, stop) =
        cancellable {
            for x = stop downto start do
                printfn $"hello, x = {x}"
                printfn $"hello, x = {x}"
            return 1
        }

    testSimpleCancellableEachSeqLoopWithOneStatement [1..3] |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachSeqLoopWithTwoStatements [1..3] |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachArrayLoopWithOneStatement [|1..3|] |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachArrayLoopWithTwoStatements [|1..3|] |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachListLoopWithOneStatement [1..3] |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachListLoopWithTwoStatements [1..3] |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachStringLoopWithOneStatement "123" |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachStringLoopWithTwoStatements "123" |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntRangeLoopWithOneStatement (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntRangeLoopWithTwoStatements (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntRangeLoopDownWithOneStatement (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntRangeLoopDownWithTwoStatements (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntLoopWithOneStatement (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntLoopWithTwoStatements (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntLoopDownWithOneStatement (1, 3) |> Cancellable.runWithoutCancellation
    testSimpleCancellableEachIntLoopDownWithTwoStatements (1, 3) |> Cancellable.runWithoutCancellation

