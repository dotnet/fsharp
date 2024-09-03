// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception definition define new discriminated union cases
// Verify that we cannot use "sig-spec" when defining an exception

#light

// This is the corresponding case for DU (deprecated, but ok)
// type T = | T1 : a:int -> T

// The same/similar construct is not supported for exceptions
exception E1 : a:int -> exn

