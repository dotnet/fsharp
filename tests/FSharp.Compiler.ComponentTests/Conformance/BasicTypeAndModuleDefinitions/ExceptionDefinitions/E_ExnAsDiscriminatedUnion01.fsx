// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception definition define new discriminated union cases
// Verify that we cannot use "sig-spec" when defining an exception
//<Expects id="FS0010" span="(11,14-11,15)" status="error">Unexpected symbol ':' in implementation file</Expects>
#light

// This is the corresponding case for DU (deprecated, but ok)
// type T = | T1 : a:int -> T

// The same/similar construct is not supported for exceptions
exception E1 : a:int -> exn

