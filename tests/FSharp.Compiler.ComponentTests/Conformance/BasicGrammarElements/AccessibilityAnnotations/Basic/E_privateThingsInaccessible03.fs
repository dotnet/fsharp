// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:2738



module M = 
    module private PrivateModule =
        let x = 1
        
module Module1 =
    let _ = M.PrivateModule.x

module Module2 =
    open M
    let _ = PrivateModule.x
