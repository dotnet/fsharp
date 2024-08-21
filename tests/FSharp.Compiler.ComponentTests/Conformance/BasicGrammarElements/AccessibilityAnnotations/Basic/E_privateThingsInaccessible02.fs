// #Regression #Conformance #DeclarationElements #Accessibility 
#light
// Private modules
// Private type or module is private to its immediately enclosing module. This means the module PrivateModule in the repro 
// is indeed accessible to the rest of the implicit enclosing module. 
//<Expects id="FS1094" span="(26,17-26,34)" status="error">The value 'x' is not accessible from this code location</Expects>
//<Expects id="FS1094" span="(27,17-27,34)" status="error">The value 'f' is not accessible from this code location</Expects>
//<Expects id="FS1094" span="(29,17-29,20)" status="error">The value 'y' is not accessible from this code location</Expects>
//<Expects id="FS1094" span="(30,17-30,20)" status="error">The value 'g' is not accessible from this code location</Expects>
//<Expects id="FS1092" span="(27,19-27,32)" status="error">The type 'PrivateModule' is not accessible from this code location</Expects>
//<Expects id="FS1092" span="(26,19-26,32)" status="error">The type 'PrivateModule' is not accessible from this code location</Expects>

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

