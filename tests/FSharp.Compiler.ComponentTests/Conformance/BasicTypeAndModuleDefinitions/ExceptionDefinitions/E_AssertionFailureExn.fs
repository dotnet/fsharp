// #Regression #Conformance #TypesAndModules #Exceptions 


// Regression test for FSharp1.0:3583 - Remove AssertionFailureException
//<Expects id="FS0039" span="(11,36-11,61)" status="error">The type 'AssertionFailureException' is not defined</Expects>

let foo =
    try
        assert(false)
    with
        | :? Microsoft.FSharp.Core.AssertionFailureException -> printfn "Exception Caught!"
