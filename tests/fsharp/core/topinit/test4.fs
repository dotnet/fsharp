// #Conformance #Interop 

module GenericStaticTest4

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)


module CheckStatic4 = 
    type B<'T>() = 
        static do printfn "Running initializer for %A (part 1)" typeof<B<'T>>
        static let x = 1 // should be ok
        static do printfn "Running initializer for %A (part 2)" typeof<B<'T>>
        static let x2 = B<'T>.P2 + 1 // should fail
        static do printfn "Running initializer for %A (part 3)" typeof<B<'T>>
        static let x3 = B<'T>.P1 + 2 // should be ok 
        static do printfn "Done initializer for %A" typeof<B<'T>>
        static member P1 = x
        static member P2 = x2
        static member P3 = x3

    let check4() = 
        // This is static initialization in a generic type, and the first access happens here
        check "cwknecw021e1TryA" (try B<int>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true



        // NOTE NOTE NOTE: the rest of this test may be flakey under 
        //    - NGEN of code
        //    - Different CLRs

        // For generic types, it looks like the CLR implements a semantics where subsequent failure raise an exception
        check "cwknecw021e1TryA" (try B<int>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e1TryA" (try B<int>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e3TryB" (try B<int>.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021e1TryC" (try B<int>.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021e2TryD" (try B<int>.P3 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e3TryE" (try B<int>.P3 |> ignore; false  with :? System.TypeInitializationException -> true) true

        // This is static initialization in a generic type, and the first access happens here
        check "cwknecw021e1TryA11" (try B<string>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e1TryA" (try B<string>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e1TryA" (try B<string>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e3TryA33" (try B<string>.P2 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e1TryA44" (try B<string>.P2 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e2TryA55" (try B<string>.P3 |> ignore; false  with :? System.TypeInitializationException -> true) true
        check "cwknecw021e3TryA66" (try B<string>.P3 |> ignore; false  with :? System.TypeInitializationException -> true) true

let checkAll() = 
    //CheckStatic4.check4() // BUG: FSHARP1.0:5705
    ()
    

