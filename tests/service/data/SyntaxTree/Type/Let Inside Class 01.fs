// Expected: No warning - let bindings are valid in classes
module Module

type MyClass() =
    let mutable privateField = 10
    let helper x = x * 2
    
    member _.GetValue() = helper privateField
    member _.SetValue(v) = privateField <- v
