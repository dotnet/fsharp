// #Regression #Conformance #ApplicationExpressions 
#light

// Before check-in 11472, assert(false) had a special treatment (used to throw an AssertionFailure exception)
// Now, it is just a normal expression and it is subject to conditional compilation rules.

let _ =
    try
        assert(false)
        exit 0
    with
    //| Microsoft.FSharp.Core.AssertionFailure(_,_,_) -> exit 0
    | _                                             -> exit 1
