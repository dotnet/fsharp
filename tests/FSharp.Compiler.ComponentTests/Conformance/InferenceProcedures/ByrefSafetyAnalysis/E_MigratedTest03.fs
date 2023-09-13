#if TESTS_AS_APP
module Core_byrefs
#endif

open System
open System.Runtime.CompilerServices
open CSharpLib3

// Test extension members for byrefs

[<Extension>]
type Ext() =
    [<Extension>]
    static member inline Change(dt: byref<DateTime>) = ()

    [<Extension>]
    static member inline NotChange(dt: inref<DateTime>) = ()

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
