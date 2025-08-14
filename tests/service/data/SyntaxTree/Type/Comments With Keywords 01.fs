// Testing: Keywords in comments should be ignored
module Module

type MyType =
    // This comment mentions module and type
    member _.Method() = 1
    (* module MyModule *)
    member _.AnotherMethod() = 2
