// Testing: Let bindings are valid in classes
module Module

type MyClass() =
    let privateField = 10
    member _.GetValue() = privateField
