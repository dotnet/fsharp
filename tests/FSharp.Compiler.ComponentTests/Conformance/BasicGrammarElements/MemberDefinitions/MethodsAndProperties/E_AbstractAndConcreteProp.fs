// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify error when you have a property with both an abstract and concrete component
// Regression from FSB 4496

//<Expects id="FS0435" status="error" span="(10,14-10,19)">The property 'State' of type 'X' has a getter and a setter that do not match\. If one is abstract then the other must be as well\.$</Expects>

[<AbstractClass>]
type X() = 
    let mutable state = 0
    member x.State with get() = state
    abstract State : int  with set
