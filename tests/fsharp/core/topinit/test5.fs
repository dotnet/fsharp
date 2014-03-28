// #Conformance #Interop 

module StaticTest5

let mutable trigger = 1

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)

module CheckStatic5 = 
    type B() = 
        static let x = 1 // should be ok
        static let x2 = C.P2 + 1 // should fail
        static member P1 = x
        static member P2 = x2

    and C() = 
        static let x3 = B.P1 + 2 // should be ok 
        static member P2 = B.P2
        static member P3 = x3

    let check5() = 
        check "StaticTest5.cwknecDw021e2" B.P1 1 // subsequent accesses succeed!
        check "StaticTest5.cwknecDw021e2" B.P1 1 // subsequent accesses succeed!
        check "StaticTest5.cwknecDw021e3TryB" (try C.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "StaticTest5.cwknecDw021e1TryC" (try C.P2 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "StaticTest5.cwknecDw021e2TryD" (try C.P3 |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "StaticTest5.cwknecDw021e3TryE" (try C.P3 |> ignore; false  with :? System.InvalidOperationException -> true) true


let checkAll() = 
    CheckStatic5.check5()

    

