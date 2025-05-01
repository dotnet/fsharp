// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
//<Expects status="error" span="(6,12)" id="FS0673">This instance member needs a parameter to represent the object being invoked\. Make the member static or use the notation 'member x\.Member\(args\) = \.\.\.'\.$</Expects>

type Foo() =
    let m_val = 0
    member DoStuff() = m_val
