type Method = ReturnFrom | ReturnFromFinal | YieldFrom | YieldFromFinal

type Sync<'a> = (unit -> 'a)
type SyncBuilder(signal) = 
    member b.Bind(x,f) = f (x())
    member b.Using(x,f) = (fun () -> use r = x in f r ())
    member b.TryFinally(x,f) = (fun () -> try x() finally f())
    member b.TryWith(x,f) = (fun () -> try x() with e -> f e ())
    member b.Combine(f1,g) = (fun () -> f1(); g()())
    member b.Delay(f) = (fun () -> f()())
    member b.Zero() = (fun () -> ())
    member b.Return x = (fun () -> x)
    member b.ReturnFrom x = signal ReturnFrom; x
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
        return! sync.Return 1
    } |> run


do 
    let sync = SyncBuilder (expect ReturnFromFinal)

    sync {
        printf "expect ReturnFromFinal: "
        do! sync { printfn "inner" }
    } |> run

do
   let sync = SyncBuilder (expect ReturnFrom)

   sync {
       printf "expect ReturnFrom: "
       try 
           return! sync.Return 1
       finally ()
   } |> run

do 
    let sync = SyncBuilder (expect YieldFromFinal)

    sync {
        printf "expect YieldFromFinal: "
        yield! sync.Return 1
    } |> run

do
   let sync = SyncBuilder (expect YieldFrom)

   sync {
       printf "expect YieldFrom: "
       try 
           yield! sync.Return 1
       finally ()
   } |> run

do 
   let sync = SyncBuilder expectNone

   sync {
       printf "expectNone: "
       let! a = sync.Return 1
       let! b = sync.Return 2
       return a + b
   } |> run