// Expected: No warning - keywords in comments are ignored
module Module

type MyType() =
    // This comment mentions module and type
    member _.Method() = 1
    
    (* This multi-line comment also contains
       module MyModule
       type MyType
       exception MyException
       open System
    *)
    member _.AnotherMethod() = 2
    
    /// XML doc comment with module, type, exception keywords
    member _.DocumentedMethod() = 3
