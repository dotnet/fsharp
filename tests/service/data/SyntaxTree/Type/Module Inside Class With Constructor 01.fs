// Testing: Module inside class with constructor
module Module

type MyClass(x: int) =
    module InternalModule =
        let helper = 42
