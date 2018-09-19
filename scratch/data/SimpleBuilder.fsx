
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
        yield! None
        yield! baz
        yield! None
        yield! bar
    }

printfn "quux: %+A" quux 

(*
type TraceBuilder() =

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

    member __.Delay(f) = 
        printfn "Delay"
        f

    member __.Combine(xOpt, yOpt) =
        let res =
            match xOpt with
            | Some _ ->
                xOpt
            | None ->
                yOpt
        printfn "Combining %+A with %+A to get %+A" xOpt yOpt res
        res

    member __.ReturnFrom(x) = x

    member __.TryWith(body, handler) =
        try __.ReturnFrom(body())
        with e -> handler e

    member __.TryFinally(body, compensation) =
        try __.ReturnFrom(body())
        finally compensation() 

    member __.Using(disposable:#System.IDisposable, body) =
        let body' = fun () -> body disposable
        __.TryFinally(body', fun () -> 
            match disposable with 
                | null ->
                    printfn "Not disposing: Value is null"
                    () 
                | disp ->
                    printfn "Disposing %O" disp
                    disp.Dispose())
*)