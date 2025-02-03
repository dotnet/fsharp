// #Conformance #Constants #Recursion #LetBindings #MemberDefinitions #Mutable 
module Core_byrefs

let test s b = if b then () else failwith s 

(* TEST SUITE FOR Int32 *)

let out r (s:string) = r := !r @ [s]

let check s actual expected = 
    if actual = expected then printfn "%s: OK" s
    else failwithf "%s: FAILED, expected %A, got %A" s expected actual

let check2 s expected actual = check s actual expected

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

    type TestPositiveOverloading() =

        static member TestMethod(dt: byref<int>) = ()

        static member TestMethod(dt: inref<float32>) = ()

        static member TestMethod(dt: outref<float>) = ()

    type PositiveInterface =

        abstract Test : byref<int> * byref<int> -> byref<int>

    // This looks like it should fail, but its sig is 'val test2 : x: byref<int> -> y: byref<int> -> unit' 
    //     unless a signature tells it otherwise, e.g. 'val test2 : (byref<int> -> byref<int>) -> unit'
    let test2 (x: byref<int>) =
        fun (y: byref<int>) -> ()

    type StaticTest private () =

        static member Test (x: byref<int>, y: int) = ()

        static member Test2 (x: inref<int>, y: int) = ()

        // This passes because tup becomes 'int ref * int', which is valid and produces valid code.
        // We include this to test current behavior with inference and byrefs.
        static member PositiveTest(tup) =
            StaticTest.Test(tup)

    let test3 () =
        StaticTest.Test2 // is passing, but probably shouldn't be

printf "TEST PASSED OK" 
