// #Regression #Conformance #TypesAndModules #Records 
// Field labels have module scope

#light

    module M0 =
        type T1 = { a : int;}

    let x = { a = 10 }              // error

