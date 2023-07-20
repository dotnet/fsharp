// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:2738
//<Expects id="FS1092" span="(11,15-11,28)" status="error">The type 'PrivateModule' is not accessible from this code location$</Expects>
//<Expects id="FS1094" span="(11,13-11,30)" status="error">The value 'x' is not accessible from this code location$</Expects>
//<Expects id="FS0039" span="(15,13-15,26)" status="error">The value, namespace, type or module 'PrivateModule' is not defined</Expects>
module M = 
    module private PrivateModule =
        let x = 1
        
module Module1 =
    let _ = M.PrivateModule.x

module Module2 =
    open M
    let _ = PrivateModule.x
