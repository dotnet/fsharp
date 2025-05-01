// #Regression #Conformance #TypesAndModules #Exceptions 


// Regression test for FSharp1.0:3583 - Remove AssertionFailureException


let foo =
    try
        assert(false)
    with
        | :? Microsoft.FSharp.Core.AssertionFailureException -> printfn "Exception Caught!"
