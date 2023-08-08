// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
module Core_byrefs

open System
open System.Runtime.CompilerServices
open CSharpLib3

let test s b = if b then () else failwith s

(* TEST SUITE FOR Int32 *)

let out r (s:string) = r := !r @ [s]

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else failwithf "%s: FAILED, expected %A, got %A" s expected actual

let check2 s expected actual = check s actual expected

// Test extension members for byrefs

[<Extension>]
type Ext() =
    [<Extension>]
    static member inline Change(dt: byref<DateTime>) = ()

    [<Extension>]
    static member inline NotChange(dt: inref<DateTime>) = ()

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

stdout.WriteLine "Test Passed"
