// #Conformance #Interop 

module StaticTest2

let mutable trigger = 1

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)

module CheckStatic2 = 
    type B() = 
        static let x = 1
        static let x2 = B.P1 + 1 // should be ok 
        static let x3 = B.P1 + 2 // should be ok 
        static member P1 = x
        static member P2 = x2
        static member P3 = x3

    let check2() = 
        check "cwknecw021b" B.P1 1
        check "cwknecw021c" B.P2 2
        check "cwknecw021d" B.P3 3   



let checkAll() = 
    CheckStatic2.check2()
    

