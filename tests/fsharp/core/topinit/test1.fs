// #Conformance #Interop 


module StaticTest1

let mutable trigger = 1

let check s b1 b2 = 
    if b1 = b2 then printfn "%s OK" s
    else (printfn "FAIL %s: expected %A, got %A" s b2 b1; exit 1)

module CheckStatic1 = 
    type B() = 
        static let x = B.P
        static member P = x

    let check1() = 
        check "cwknecw021a1Try" (try B.P |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021a2Try" (try B.P |> ignore; false  with :? System.InvalidOperationException -> true) true
        check "cwknecw021a3Try" (try B.P |> ignore; false  with :? System.InvalidOperationException -> true) true

let checkAll() = 
    CheckStatic1.check1()
    
