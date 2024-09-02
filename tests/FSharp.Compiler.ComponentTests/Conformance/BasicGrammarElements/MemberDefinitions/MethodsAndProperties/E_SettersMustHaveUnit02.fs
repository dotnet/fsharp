// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify error message when property setter doesn't return unit.


type Foo() = 
    let mutable m_value = 0
    
    member this.Property with get() = m_value 
                         and  set x = m_value <- x; "Returns a string"
    
let foo = new Foo()
let result = (foo.Property <- 40)
