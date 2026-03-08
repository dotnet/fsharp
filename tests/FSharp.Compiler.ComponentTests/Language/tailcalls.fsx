type Method = ReturnFrom | ReturnFromFinal | YieldFrom | YieldFromFinal

type Sync<'a> = (unit -> 'a)
type SyncBuilder<'a>(signal) = 
    member b.Bind(x,f) = f (x())
    member b.Using(x,f) = (fun () -> use r = x in f r ())
    member b.TryFinally(x,f) = (fun () -> try x() finally f())
    member b.TryWith(x, f) = (fun () -> try x() with e -> f e)
    member b.Combine(f1,g) = (fun () -> f1(); g()())
    member b.Delay(f) = (fun () -> f()())
    member b.Zero() = (fun () -> ())
    member b.Return (x: 'a) = (fun () -> x)
    member b.ReturnFrom (x: Sync<_>) = signal ReturnFrom; x
    member b.ReturnFromFinal x = signal ReturnFromFinal; x
    member b.YieldFrom x = (fun () -> signal YieldFrom; x)
    member b.YieldFromFinal x = (fun () -> signal YieldFromFinal; x)
    member b.For(e,f) = (fun () -> for x in e do f x ())
    member b.While(gd,x) = (fun () -> while gd() do x())

let shouldEqual x y =
    printfn "%A" x
    if x <> y then failwithf "Expected %A but got %A" y x

let expect = shouldEqual

let expectNone _ = failwith "Should not be called"

let run f = f () |> ignore

do 
    let sync = SyncBuilder (expect ReturnFromFinal)

    sync {
        printf "expect ReturnFromFinal: "
        return! sync { return 1 }
    } |> run


do 
    let sync = SyncBuilder (expect ReturnFromFinal)

    sync {
        printf "expect ReturnFromFinal: "
        do! sync { printfn "inner" }
    } |> run

do
   let sync = SyncBuilder<int> (expect ReturnFrom)

   sync { return 1 } |> run

   sync {
       printf "expect ReturnFrom: "
       try 
           return! sync { return 1 }
       finally ()
   } |> run

do 
    let sync = SyncBuilder (expect YieldFromFinal)

    sync {
        printf "expect YieldFromFinal: "
        yield! sync { return 1 }
    } |> run

do
   let sync = SyncBuilder<int> (expect YieldFrom)

   sync {
       printf "expect YieldFrom: "
       try
           yield! sync { return 1 }
       with _ -> return 0
   } |> run

do 
   let sync = SyncBuilder expectNone

   sync {
       printf "expectNone: "
       let! a = sync { return 1 }
       let! b = sync { return 2 }
       return a + b
   } |> run

// A realistic seq-like list builder for comprehensive *Final testing.
// Uses list<'T> throughout; all methods compose correctly so we can
// verify both the produced result AND which method variant was called.
type ListBuilder() =
    let mutable yieldFromCount = 0
    let mutable yieldFromFinalCount = 0
    member _.YieldFromCount = yieldFromCount
    member _.YieldFromFinalCount = yieldFromFinalCount
    member _.Reset() = yieldFromCount <- 0; yieldFromFinalCount <- 0
    member _.Yield(x) = [x]
    member _.YieldFrom(xs: list<_>) = yieldFromCount <- yieldFromCount + 1; xs
    member _.YieldFromFinal(xs: list<_>) = yieldFromFinalCount <- yieldFromFinalCount + 1; xs
    member _.Return(x) = [x]
    member _.ReturnFrom(xs: list<_>) = xs
    member _.Combine(a: list<_>, b: unit -> list<_>) = a @ b()
    member _.Delay(f: unit -> list<_>) = f
    member _.Run(f: unit -> list<_>) = f()
    member _.Zero() : list<_> = []
    member _.For(source: list<_>, body: _ -> list<_>) : list<_> = source |> List.collect body
    member _.While(guard: unit -> bool, body: unit -> list<_>) : list<_> =
        let mutable acc = []
        while guard() do acc <- acc @ body()
        acc
    member _.TryWith(body: unit -> list<_>, handler: exn -> list<_>) : list<_> =
        try body() with e -> handler e
    member _.TryFinally(body: unit -> list<_>, compensation: unit -> unit) : list<_> =
        try body() finally compensation()
    member _.Using(resource: #System.IDisposable, body: _ -> list<_>) : list<_> =
        try body resource finally (resource :> System.IDisposable).Dispose()
    member _.Bind(m: list<_>, f: _ -> list<_>) : list<_> = m |> List.collect f
    member _.MergeSources(a: list<_>, b: list<_>) : list<_ * _> = [ for x in a do for y in b do yield (x, y) ]

// Regression test for https://github.com/dotnet/fsharp/issues/19402
// yield! inside a for-loop body should call YieldFrom, not YieldFromFinal.
do
    let b = ListBuilder()
    let result =
        b {
            for i in [ 1; 2; 3 ] do
                yield! [ i; i * 10 ]
        }
    shouldEqual result [ 1; 10; 2; 20; 3; 30 ]
    shouldEqual b.YieldFromCount 3
    shouldEqual b.YieldFromFinalCount 0

// yield! in genuine tail position → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            yield 0
            yield! [ 1; 2; 3 ]
        }
    shouldEqual result [ 0; 1; 2; 3 ]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 1

// yield! in try/with body → YieldFrom (not tail), result correct
do
    let b = ListBuilder()
    let result =
        b {
            try
                yield! [1; 2]
            with _ -> yield 0
        }
    shouldEqual result [1; 2]
    shouldEqual b.YieldFromCount 1
    shouldEqual b.YieldFromFinalCount 0

// yield! in try/finally body → YieldFrom (not tail), result correct
do
    let b = ListBuilder()
    let mutable finalized = false
    let result =
        b {
            try
                yield! [1; 2]
            finally finalized <- true
        }
    shouldEqual result [1; 2]
    shouldEqual finalized true
    shouldEqual b.YieldFromCount 1
    shouldEqual b.YieldFromFinalCount 0

// yield! in try/with handler (tail position) → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            try
                failwith "err"
                yield 0
            with _ -> yield! [1; 2; 3]
        }
    shouldEqual result [1; 2; 3]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 1

// yield! in try/with handler (non-tail, more code follows) → YieldFrom, result correct
do
    let b = ListBuilder()
    let result =
        b {
            try
                failwith "err"
                yield 0
            with _ -> yield! [1; 2]
            yield! [3]
        }
    shouldEqual result [1; 2; 3]
    shouldEqual b.YieldFromCount 1
    shouldEqual b.YieldFromFinalCount 1

// yield! in use body → YieldFrom (not tail due to Using wrapper), result correct
do
    let b = ListBuilder()
    let mutable disposed = false
    let result =
        b {
            use _x = { new System.IDisposable with member _.Dispose() = disposed <- true }
            yield! [10; 20]
        }
    shouldEqual result [10; 20]
    shouldEqual disposed true
    shouldEqual b.YieldFromCount 1
    shouldEqual b.YieldFromFinalCount 0

// yield! in while body → YieldFrom (not tail), result correct
do
    let b = ListBuilder()
    let mutable count = 2
    let result =
        b {
            while count > 0 do
                count <- count - 1
                yield! [count]
        }
    shouldEqual result [1; 0]
    shouldEqual b.YieldFromCount 2
    shouldEqual b.YieldFromFinalCount 0

// yield! in match branch (tail position) → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            match 42 with
            | 42 -> yield! [1; 2]
            | _ -> yield! [3; 4]
        }
    shouldEqual result [1; 2]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 1

// yield! in if/else (tail position) → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            if true then
                yield! [10]
            else
                yield! [20]
        }
    shouldEqual result [10]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 1

// yield! in let! continuation (tail position) → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            let! x = [1; 2]
            yield! [x * 10]
        }
    shouldEqual result [10; 20]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 2

// yield! in sequential (non-tail) + tail → only last is YieldFromFinal
do
    let b = ListBuilder()
    let result =
        b {
            yield! [1]
            yield! [2; 3]
        }
    shouldEqual result [1; 2; 3]
    shouldEqual b.YieldFromCount 1
    shouldEqual b.YieldFromFinalCount 1

// yield! in match! clause body (tail position) → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            match! [1; 2] with
            | 1 -> yield! [10]
            | x -> yield! [x * 100]
        }
    shouldEqual result [10; 200]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 2

// yield! in and! continuation (MergeSources+Bind, tail position) → YieldFromFinal, result correct
do
    let b = ListBuilder()
    let result =
        b {
            let! x = [1; 2]
            and! y = [10; 20]
            yield! [x + y]
        }
    shouldEqual result [11; 21; 12; 22]
    shouldEqual b.YieldFromCount 0
    shouldEqual b.YieldFromFinalCount 4

// yield! in and! continuation (non-tail, more code follows) → YieldFrom, result correct
do
    let b = ListBuilder()
    let result =
        b {
            let! x = [1]
            and! y = [10]
            yield! [x + y]
            yield 99
        }
    shouldEqual result [11; 99]
    shouldEqual b.YieldFromCount 1
    shouldEqual b.YieldFromFinalCount 0
