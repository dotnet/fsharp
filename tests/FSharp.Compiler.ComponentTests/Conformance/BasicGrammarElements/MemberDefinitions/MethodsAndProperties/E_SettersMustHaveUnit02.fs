// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify error message when property setter doesn't return unit.
//<Expects id="FS0001" status="error" span="(9,53-9,71)">This expression was expected to have type.*'unit'.*but here has type.*'string'</Expects>

type Foo() = 
    let mutable m_value = 0
    
    member this.Property with get() = m_value 
                         and  set x = m_value <- x; "Returns a string"
    
let foo = new Foo()
let result = (foo.Property <- 40)
