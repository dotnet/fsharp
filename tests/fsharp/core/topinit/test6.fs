// #Conformance #Interop 

module GenericStaticTest6

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)


module CheckStatic6 = 
    type B<'T>() = 
        static let x = 1 // should be ok
        static let x2 = C<'T>.P2 + 1 // should fail
        static member P1 = x
        static member P2 = x2

    and C<'T>() = 
        static let x3 = B<'T>.P1 + 2 // should be ok 
        static member P2 = B<'T>.P2
        static member P3 = x3

    let check6() = 
        // This is static initialization in a generic type, and the first access happens here
        check "GenericStaticTest6.cwknecDw021e1TryA" (try B<int>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        // NOTE NOTE NOTE: the rest of this test may be flakey under 
        //    - NGEN of code
        //    - Different CLRs

        check "GenericStaticTest6.cwknecw021e1TryA" B<int>.P1 1
        check "GenericStaticTest6.cwknecDw021e3TryB" (try B<int>.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "GenericStaticTest6.cwknecDw021e1TryC" (try B<int>.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        // Note, initialization of the generic types is independent - initialization of C succeeded
        check "GenericStaticTest6.cwknecDw021e2TryD" C<int>.P3 3
        check "GenericStaticTest6.cwknecDw021e3TryE" C<int>.P3 3


        // This is static initialization in a generic type, and the first access happens here
        check "GenericStaticTest6.cwknecDw021e1TryA" (try B<string>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        // NOTE NOTE NOTE: the rest of this test may be flakey under 
        //    - NGEN of code
        //    - Different CLRs

#if OMITTED
        //check "GenericStaticTest6.cwknecDw021e1TryA" (try B<string>.P1 |> ignore; false  with :? System.TypeInitializationException -> true) true
        //check "GenericStaticTest6.cwknecDw021e3TryB" (try B<string>.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        //check "GenericStaticTest6.cwknecDw021e1TryC" (try B<string>.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        // Note, initialization of the generic types is independent - initialization of C succeeded
        //check "GenericStaticTest6.cwknecDw021e2TryD" C<string>.P3 3
        //check "GenericStaticTest6.cwknecDw021e3TryE" C<string>.P3 3
#endif

let checkAll() = 
    //CheckStatic6.check6() // BUG: FSHARP1.0:5705
    ()
    

