// #Regression #Conformance #TypeInference #TypeConstraints 
#light

// Verify that type functions need to be declared explicitly
//<Expects id="FS0030" status="error">Value restriction</Expects>

let someTypeFunc<'a> = printfn "side effect"; ref ([] : 'a list);;

// This should fail since the type param needs to be declared explicitly.
let shouldNotAllow = someTypeFunc

exit 1
