// #Regression #Conformance #DeclarationElements #Accessibility 
#light
// Private modules
// Private type or module is private to its immediately enclosing module. This means the module PrivateModule in the repro 
// is indeed accessible to the rest of the implicit enclosing module. 







module M = 
    let private y = 42
    let private g y = y * y

    module private PrivateModule =
        let x = 1
        let f x = x * x + y

    module public PublicModule =
        let x = 1
        let f x = x * x + y
        
module Module1 =
    let test4 = M.PrivateModule.x           // Error
    let test5 = M.PrivateModule.f 2         // Error

    let test6 = M.y                         // Error
    let test7 = M.g 2                       // Error

    let test8 = M.PublicModule.x            // OK
    let test9 = M.PublicModule.f 2          // OK

