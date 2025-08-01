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
