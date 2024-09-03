// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify error when you have a property with both an abstract and concrete component
// Regression from FSB 4496



[<AbstractClass>]
type X() = 
    let mutable state = 0
    member x.State with get() = state
    abstract State : int  with set
