// #Regression #Conformance #ObjectOrientedTypes #Delegates 

// Verify error when given an invalid delegate definition
//<Expects id="FS0193" status="error">Illegal definition for runtime implemented delegate method.</Expects>

type InvalidDelegateDefinition(x: int) =
    delegate of int -> int

let invalidDelegate = InvalidDelegateDefinition(fun _ -> 1)

exit 1
