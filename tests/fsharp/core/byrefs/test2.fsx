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
module NegativeScoping =

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

    let test3 () =
        let x = &1 // not allowed
        let y = &2 // not allowed
        x + y

    let test4 doIt =
        let mutable x = 1
        if doIt then
            &x // not allowed
        else
            &1 // not allowed

    let test5 doIt =
        let mutable x = 1
        let y =
            if doIt then
                &x
            else
                &1 // not allowed
        &y // not allowed 
#endif

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test2.ok","ok"); 
        exit 0)