// #Conformance #PatternMatching #PatternMatchingGuards 

open System

let x: Result<unit, exn> = Error (NullReferenceException())

match x with
| Error (_: exn & :? NullReferenceException) -> printfn "NullRef"
| _ -> ()

match x with
| Error (_: exn & (:? NullReferenceException)) -> printfn "NullRef"
| _ -> ()

match x with
| Error ((_: exn) & :? NullReferenceException) -> printfn "NullRef"
| _ -> ()

match x with
| Error ((_: exn) & (:? NullReferenceException)) -> printfn "NullRef"
| _ -> ()
