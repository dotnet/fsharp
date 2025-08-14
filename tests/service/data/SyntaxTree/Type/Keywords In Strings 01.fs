// Testing: Keywords in strings should be ignored
module Module

type MyClass =
    let message = "type and module keywords"
    member _.GetMessage() = message
