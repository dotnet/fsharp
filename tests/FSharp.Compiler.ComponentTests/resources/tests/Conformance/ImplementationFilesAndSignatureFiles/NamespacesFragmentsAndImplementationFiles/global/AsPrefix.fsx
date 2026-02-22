// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937
// Usage of 'global'
//<Expects status="success"></Expects>
module M =
    let p  =        Microsoft.FSharp.Core.Choice1Of2(1)  // ok
    let p' = global.Microsoft.FSharp.Core.Choice1Of2(1)  // ok
    
    (if p = p' then 0 else 1) |> exit
    
