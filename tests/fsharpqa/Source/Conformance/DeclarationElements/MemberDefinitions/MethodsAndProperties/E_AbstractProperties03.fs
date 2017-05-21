// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:4265 - Disallow an abstract setter with concrete getter
//<Expects id="FS0435" span="(8,14-8,19)" status="error">The property 'State' of type 'X' has a getter and a setter that do not match\. If one is abstract then the other must be as well</Expects>
// See also FSHARP1.0:3661 and 4981
[<AbstractClass>]
type X() = 
    let mutable state = 0
    member x.State with get() = state
    abstract State : int  with set
