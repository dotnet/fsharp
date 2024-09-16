// #Regression #Conformance #TypesAndModules #Records 
// Field labels have module scope

type T1 = { a : decimal }

module M0 =
    type T1 = { a : int;}

let x = { a = 10 }              // error - 'expecting decimal' (which means we are not seeing M0.T1)

