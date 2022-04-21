// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
//<Expects status="error" span="(10,14)" id="FS0493">StaticMethod is not an instance method$</Expects>

type Foo() =
    let m_val = 0
    member this.DoStuff() = m_val
    static member StaticMethod() = 42
    
let x = new Foo()
let result = x.StaticMethod()
