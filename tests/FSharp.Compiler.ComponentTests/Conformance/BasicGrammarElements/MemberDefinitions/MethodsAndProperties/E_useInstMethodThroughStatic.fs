// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
//<Expects status="error" id="FS3214" span="(10,14)">Method or object constructor 'DoStuff' is not static$</Expects>

type Foo() =
    let m_val = 0
    member this.DoStuff() = m_val
    static member StaticMethod() = 42
    
let x = new Foo()
let result = Foo.DoStuff()

