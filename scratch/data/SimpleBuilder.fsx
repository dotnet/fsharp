
[<Struct>]
type OptionalBuilder =

    member __.Apply(fOpt : ('a -> 'b) option, xOpt : 'a option) : 'b option =
        match fOpt, xOpt with
        | Some f, Some x -> Some (f x)
        | _ -> None

    member __.Return(x : 'a) : 'a option = Some x

    // Below is only needed to make yield keyword work

    member __.YieldFrom(x : 'a) : 'a = x

    member __.Delay(f : unit -> 'a) : 'a = f () // If you type `Delay : 'a -> 'a` explicitly, it builds but things get weird. Fast.

    member __.Zero() : unit option = Some ()

    member __.Combine(xOpt : 'a option, yOpt : 'a option) : 'a option =
        match xOpt with
        | Some _ -> xOpt
        | None -> yOpt

let opt = OptionalBuilder()

let fOpt = Some (sprintf "f (x = %d) (y = %s) (z = %f)")
let xOpt = Some 1
let yOpt = Some "A"
let zOpt = Some 3.0

let superSimpleExampleDesugared : bool option =
    opt.Apply(
        opt.Apply(
            opt.Return(
                (fun x ->
                    (fun y ->
                        x || y
                    )
                )
            ),
            Some true), 
        Some false)

printfn "Super simple example desugared: \"%+A\"" superSimpleExampleDesugared

let superSimpleExample : bool option =
    opt {
        let! (x : bool) = Some true
        and! (y : bool) = Some false
        return x || y
    }

printfn "Super simple example: \"%+A\"" superSimpleExample 

let foo : string option =
    opt {
        let! f = fOpt
        and! x = xOpt
        and! y = yOpt
        and! z = zOpt
        return (f x y z)
    }

printfn "foo: \"%+A\"" foo 

let bar : int option =
    opt {
        let! x = None
        and! y = Some 1
        and! z = Some 2
        return x + y + z + 1
    }

printfn "bar: %+A" bar 

type 'a SingleCaseDu = SingleCaseDu of 'a

let baz : int option =
    opt {
        let! x                = Some 5
        and! (SingleCaseDu y) = Some (SingleCaseDu 40)
        and! (z,_)            = Some (300, "whatever")
        return (let w = 10000 in w + x + y + z + 2000)
    }

printfn "baz: %+A" baz 

let quux : int option =
    opt {
        let! a =
            opt { // Takes the first yield!'d value that is not None, i.e. takes the second block in this case
                  // You caan view yield! (in this builder) to mean a prioritied list of values, where the highest priority Some wins
                yield! 
                    opt {
                        let! x = Some 11
                        and! y = None
                        and! z = Some 2
                        return x + y + z + 1 // Bails out because y is None
                    }
                yield! 
                    opt {
                        let! x = Some -3
                        and! y = Some 1
                        and! z = Some 2
                        return x + y + z + 4 // Computes result because all args are Some
                    }
            }
        and! b =
            opt {
                let! x = Some 9
                and! _ = Some "IGNORED" // Inner value goes unused, but we'd still bail out if this value were None
                and! z = Some 4
                return x + z - 10
            }
        and! c =
            opt {
                let! x = Some 0
                and! y = Some 1
                and! z = Some 2
                return (x + y + z + 1)
            }
        and! d = baz // Makes use of an optional value from outside the computation expression (bailing out with None if it is None, as with any other value)
        return a * b * c * d // Computes this value because all args are some
        // Another way to view this is let! ... and! ... is a bit like a conjunction, and yield! ... yield! ... is a bit like a disjunction
    }

printfn "quux: %+A" quux 

type TraceOptBuilder() =

    member __.Apply(fOpt : ('a -> 'b) option, xOpt : 'a option) : 'b option =
        match fOpt, xOpt with
        | Some f, Some x ->
            printfn "Applying %+A to %+A" f x
            Some (f x)
        | _ ->
            printfn "Applying with None. Exiting."
            None

    member __.Return(x) = Some x

    member __.Yield(x) = Some x

    member __.Zero() =
        printfn "Zero"
        Some ()

    member __.Combine(xOpt, yOpt) =
        let res =
            match xOpt with
            | Some _ ->
                xOpt
            | None ->
                yOpt
        printfn "Combining %+A with %+A to get %+A" xOpt yOpt res
        res

    member __.MapTryFinally(body, compensation) =
        try body()
        finally compensation() 

    member __.MapUsing(disposable:#System.IDisposable, body) =
        printfn "Using disposable %O" disposable
        let body' = fun () -> body disposable
        __.MapTryFinally(body', fun () -> 
            printfn "Disposing %O" disposable
            disposable.Dispose())

let traceOpt = TraceOptBuilder()

type FakeDisposable =
    FakeDisposable of int
    with
    interface System.IDisposable with
        member __.Dispose() =
            let (FakeDisposable x) = __
            printf "\"Disposed\" %+A" x
            ()

let simpleFooUsing : string option =
    traceOpt {
        use! xUsing    = Some (FakeDisposable 1)
        anduse! yUsing = Some (FakeDisposable 2)
        return (sprintf "xUsing = %+A, yUsing = %+A" xUsing yUsing)
    }

printfn "simplefooUsing: \"%+A\"" simpleFooUsing

let fooUsing : string option =
    traceOpt {
        use! xUsing    = Some (FakeDisposable 1)
        anduse! yUsing = Some (FakeDisposable 2)
        anduse! zUsing = Some (FakeDisposable 3)
        return (let (FakeDisposable x) = xUsing in sprintf "Unwrapped x = %d" x)
    }

printfn "fooUsing: \"%+A\"" fooUsing

(* INVALID: Not a simple name on the LHS
let fooUsing2 : int option =
    traceOpt {
        use! (FakeDisposable x)    = Some (FakeDisposable 1)
        anduse! (FakeDisposable y) = Some (FakeDisposable 2)
        anduse! (FakeDisposable z) = Some (FakeDisposable 3)
        return x * y + z
    }

printfn "fooUsing2: \"%+A\"" fooUsing
*)