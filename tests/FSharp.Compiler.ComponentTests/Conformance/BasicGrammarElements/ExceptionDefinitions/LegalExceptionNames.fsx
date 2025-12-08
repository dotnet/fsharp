// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception types
// Legal exception names should compile successfully



exception MyException
exception MyOtherException
exception Exception123
exception ExceptionWithLongName
exception A

let test() =
    try
        raise (MyException)
    with
    | MyException -> printfn "Caught MyException"
    | _ -> printfn "Caught other"

let () = test()