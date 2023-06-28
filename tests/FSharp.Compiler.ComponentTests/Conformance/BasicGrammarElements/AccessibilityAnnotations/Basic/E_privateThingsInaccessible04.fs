// #Regression #Conformance #DeclarationElements #Accessibility 
#light
// Private modules
// Private type or module is private to its immediately enclosing module. This means the module PrivateModule in the repro 
// is indeed accessible to the rest of the implicit enclosing module. 
//<Expects id="FS0039" span="(25,17-25,30)" status="error">The value, namespace, type or module 'PrivateModule' is not defined</Expects>
//<Expects id="FS0039" span="(26,17-26,30)" status="error">The value, namespace, type or module 'PrivateModule' is not defined</Expects>
//<Expects id="FS0039" span="(28,17-28,18)" status="error">The value or constructor 'y' is not defined</Expects>
//<Expects id="FS0039" span="(29,17-29,18)" status="error">The value or constructor 'g' is not defined</Expects>

module M = 
    let private y = 42
    let private g y = y * y

    module private PrivateModule =
        let x = 1
        let f x = x * x + y

    module public PublicModule =
        let x = 1
        let f x = x * x + y
        
module Module2 =
    open M
    let test4 = PrivateModule.x           // Error
    let test5 = PrivateModule.f 2         // Error

    let test6 = y                         // Error
    let test7 = g 2                       // Error

    let test8 = PublicModule.x            // OK
    let test9 = PublicModule.f 2          // OK




