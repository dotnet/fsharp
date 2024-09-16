// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:4265 - Disallow an abstract setter with concrete getter

// See also FSHARP1.0:3661 and 4981
[<AbstractClass>]
type X() = 
    let mutable state = 0
    member x.State with get() = state
    abstract State : int  with set
