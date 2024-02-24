// #Conformance #TypesAndModules #Unions 
#light

// Verify additional checks when using a dynamic type cast to ensure
// Discriminated Unions cannot be null

type DUType =
    | A of int
    | B of string
    | C

// Works
let test1 = (box C :?> DUType)

// Dynamic type test should throw a null ref exception
let testPassed = 
    try
        let _ = (box null :?> DUType)
        false
    with
    :? System.NullReferenceException
        -> true


if not testPassed then failwith "Failed: 1"
