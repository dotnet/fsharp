// #Conformance #Interop 

module StaticTest3

let mutable trigger = 1

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)


module CheckStatic3 = 
    type B() = 
        static let x = 1 // should be ok
        static let x2 = B.P2 + 1 // should fail
        static let x3 = B.P1 + 2 // should be ok 
        static member P1 = x
        static member P2 = x2
        static member P3 = x3

    let check3() = 
        check "cwknecw021e2" B.P1 1 // the static initialization failed, caught in main, but subsequent accesses succeed!
        check "cwknecw021e2" B.P1 1 // subsequent accesses succeed!
        check "cwknecw021e3Try" (try B.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021e1Try" (try B.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021e2Try" (try B.P3 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021e3Try" (try B.P3 |> ignore; false  with :? System.InvalidOperationException -> true) true

let checkAll() = 
    CheckStatic3.check3()
    

