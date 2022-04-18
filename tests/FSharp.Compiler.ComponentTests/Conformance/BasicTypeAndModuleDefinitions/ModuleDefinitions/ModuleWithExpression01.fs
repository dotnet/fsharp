// #Regression #Conformance #TypesAndModules #Modules 
// Regression test for FSHARP1.0:2644 (a module may start with an expression)
// Verify that we can compile a module with an expression in it
//<Expects status="success"></Expects>
#light

module M = 
    printfn "hello"
    let x = 1

