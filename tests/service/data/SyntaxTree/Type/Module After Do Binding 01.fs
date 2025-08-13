// Expected: Warning for module after do binding
module Module

type MyClass() =
    do printfn "Initializing"
    module M = 
        let helper = 42
