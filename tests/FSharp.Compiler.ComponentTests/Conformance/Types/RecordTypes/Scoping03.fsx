// #Conformance #TypesAndModules #Records 
// Field labels have module scope
//<Expects status="success"></Expects>
#light

    type T1 = { a : decimal }
    
    module M0 =
        type T1 = { a : int;}

    let x = { a = 10M }
