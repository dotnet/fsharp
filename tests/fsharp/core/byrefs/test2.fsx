#if TESTS_AS_APP
module Core_byrefs
#endif

let failures = ref false
let report_failure (s) = 
  stderr.WriteLine ("NO: " + s); failures := true
let test s b = if b then () else report_failure(s) 

(* TEST SUITE FOR Int32 *)

let out r (s:string) = r := !r @ [s]

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else report_failure (sprintf "%s: FAILED, expected %A, got %A" s expected actual)

let check2 s expected actual = check s actual expected

// POST INFERENCE CHECKS
#if NEGATIVE
module NegativeTests =

    let test1 doIt =
        let mutable x = 42
        let r =
            if doIt then
                let mutable y = 1
                &y // not allowed
            else
                &x

        let c = 
            if doIt then
                let mutable z = 2
                &z // not allowed
            else
                &x

        x + r + c

    let test2 () =
        let x =
            let mutable x = 1
            &x // not allowed

        let y =
            let mutable y = 2
            &y // not allowed

        x + y

    let test3 doIt =
        let mutable x = 1
        if doIt then
            &x // not allowed
        else
            let mutable y = 1
            &y // not allowed

    let test4 doIt =
        let mutable x = 1
        let y =
            if doIt then
                &x
            else
                let mutable z = 1
                &z // not allowed
        &y // not allowed

    type Coolio() =

        static member Cool(x: inref<int>) = &x

    let test5 () =

        let y =
            let x = 1
            &Coolio.Cool(&x) // not allowed

        () 

    let test6 () =

        let y =
            let mutable x = 1
            &Coolio.Cool(&x) // not allowed

        () 

    let test7 () =
        let mutable x = 1
        let f = fun () -> &x // not allowed
        
        ()
        
    type ByRefInterface =

        abstract Test : byref<int> * byref<int> -> byref<int>

    type Test() =

        member __.Beef() =
            let mutable a = Unchecked.defaultof<ByRefInterface>
            let obj = { new ByRefInterface with

                member __.Test(x,y) =
                    let mutable x = 1
                    let obj2 =
                        { new ByRefInterface with

                            member __.Test(_x,y) = &x } // is not allowed
                    a <- obj2
                    &y
            }
            let mutable x = 500
            let mutable y = 500
            obj.Test(&x, &y) |> ignore
            a
            
    type Beef = delegate of unit-> byref<int>
    let testBeef () =
        let mutable x = 1
        let f = Beef(fun () -> &x) // is not allowed
        ()
#endif

module Tests =

    let test1 () =
        let x = 1
        let f = fun () ->
            let y = &x // is allowed
            ()

        let g = fun () ->
            let y = &x // is allowed
            ()
        ()

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test2.ok","ok"); 
        exit 0)