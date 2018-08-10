#if TESTS_AS_APP
module Core_byrefs
#endif

open System
open System.Runtime.CompilerServices
open CSharpLib3

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

// Test extension members for byrefs

[<Extension>]
type Ext() =
    [<Extension>]
    static member inline Change(dt: byref<DateTime>) = ()

    [<Extension>]
    static member inline NotChange(dt: inref<DateTime>) = ()

#if NEGATIVE
module Negatives =

    let test1 () : byref<DateTime> =
        let dt = DateTime.Now
        let x = &dt.Test2() // should fail
        &x // should fail

    let test2 () =
        let dt = DateTime.Now
        dt.Change() // should fail

    let test3 () =
        let dt = DateTime.Now
        let dtr = &dt
        let _x = &dtr.Test2() // should fail
        ()

    let test4 () =
        let dt = DateTime.Now
        let dtr = &dt
        dtr.Change() // should fail

    let test5 () =
        let dt = DateTime.Now
        let x = dt.NotChange // should fail
        let y = dt.Test // should fail
        let z = dt.Change // should fail
        let w = dt.Test2 // should fail
        ()

#endif

module Positives =

    let test1 () =
        let dt = DateTime.Now
        let _x = dt.Test()
        let dtr = &dt
        dtr.Test()

    let test2 () =
        let dt = DateTime.Now
        dt.NotChange()
        let dtr = &dt
        dtr.NotChange()

    let test3 () =
        let mutable dt = DateTime.Now
        let _x = dt.Test2()
        dt.Test()
        let dtr = &dt
        let _x = dtr.Test2()
        dtr.Test()

    let test4 () =
        let mutable dt = DateTime.Now
        dt.Change()
        dt.NotChange()
        let dtr = &dt
        dtr.Change()
        dtr.NotChange()

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test3.ok","ok"); 
        exit 0)