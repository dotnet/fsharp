type Method = ReturnFrom | ReturnFromFinal

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
    member b.For(e,f) = (fun () -> for x in e do f x ())
    member b.While(gd,x) = (fun () -> while gd() do x())
    member b.Run(x) = x()

let shouldEqual x y =
    printfn "%A" x
    if x <> y then failwithf "Expected %A but got %A" y x

let expectReturnFrom = shouldEqual ReturnFrom
let expectReturnFromFinal = shouldEqual ReturnFromFinal
let expectNone _ = failwith "Should not be called"

do 
    let sync = SyncBuilder expectReturnFromFinal

    sync {
        printf "expectReturnFromFinal: "
        return! sync.Return 1 } |> ignore

do
   let sync = SyncBuilder expectReturnFrom

   sync {
       printf "expectReturnFrom: "
       try 
           return! sync.Return 1
       finally ()
   } |> ignore

do 
   let sync = SyncBuilder expectNone

   sync {
       printf "expectNone: "
       let! a = sync.Return 1
       let! b = sync.Return 2
       return a + b
   } |> ignore