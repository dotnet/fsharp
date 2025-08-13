// Expected: Warning for module inside class with constructor
module Module

type MyClass(x: int) =
    let mutable value = x
    member _.Value = value
    module InternalModule =
        let helper() = 42
